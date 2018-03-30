namespace TicketDesk.Service.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using Core.Enumerations;
    using Criteria.TicketList;
    using Domain.Model;
    using Interface;
    using IO;

    public class TicketService : EntityFrameworkBaseService, ITicketService
    {
        private readonly IFileManagerService fileManagerService;
        private readonly ITicketEventService ticketEventService;
        private readonly ICustomFieldService customFieldService;

        public TicketService(IFileManagerService fileManagerService, ITicketEventService ticketEventService, ICustomFieldService customFieldService)
        {
            this.fileManagerService = fileManagerService;
            this.ticketEventService = ticketEventService;
            this.customFieldService = customFieldService;
        }

        public Ticket GetTicket(int id)
        {
            return context.Tickets.Find(id);
        }

        public IList<TicketGroup> GetUserOpeningTicketsInGroups()
        {
            var result = new List<TicketGroup>();
            var userIdString = context.SecurityProvider.CurrentUserId;
            var tickets = context.Tickets
                .Where(x => x.CreatedBy == userIdString
                            && (x.TicketStatus == TicketStatus.New
                                || x.TicketStatus == TicketStatus.InProgress
                                || x.TicketStatus == TicketStatus.Pending))
                .OrderBy(x => x.CategoryId)
                .ThenByDescending(x => x.TicketId)
                .GroupBy(x => x.CategoryId);

            // Get ticket categories
            var categories = context.TicketCategories.ToList();

            foreach (var grouping in tickets)
            {
                var languageSuffix = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

                var categoryName = languageSuffix == "en"
                    ? categories
                        .Where(x => x.CategoryId == grouping.Key)
                        .Select(x => x.Name)
                        .FirstOrDefault()
                    : categories
                        .Where(x => x.CategoryId == grouping.Key)
                        .Select(x => x.Name_vi)
                        .FirstOrDefault();

                var ticketGroup = new TicketGroup
                {
                    Name = categoryName,
                    Tickets = grouping,
                    TotalItems = grouping.Count()
                };

                result.Add(ticketGroup);
            }

            return result;
        }

        private IEnumerable<TicketViewModel> BasicGetTicketJoinWithShop()
        {
            return context.Tickets
                .Join(context.Shops,
                    t => t.ShopId,
                    s => s.ShopId,
                    (t, s) => new TicketViewModel
                    {
                        Id = t.TicketId,
                        ShopId = s.ShopId,
                        ShopName = s.ShopName,
                        Subject = t.Title,
                        TicketType = (TicketCategoryTypes)t.CategoryId,
                        Status = t.TicketStatus,
                        CreatedBy = t.CreatedBy,
                        CreatedDate = t.CreatedDate,
                        UpdatedDate = t.LastUpdateDate
                    });
        }

        public IEnumerable<TicketViewModel> GetTicketByStatus(int userId, TicketStatus? ticketStatus, TicketCategoryTypes? ticketType)
        {
            var tickets = BasicGetTicketJoinWithShop();

            tickets = tickets.Where(x => x.CreatedBy == userId.ToString());

            if (ticketStatus != null)
            {
                tickets = tickets.Where(x => x.Status == ticketStatus);
            }

            if (ticketType != null)
            {
                tickets = tickets.Where(x => x.TicketType == ticketType);
            }

            return tickets.OrderByDescending(x => x.Id).ToList();
        }

        public IList<TicketViewModel> GetTicketByFilter(TicketListFilterCriteria criteria)
        {
            var tickets = BasicGetTicketJoinWithShop();

            if (criteria.ShopId != null)
            {
                tickets = tickets.Where(x => x.ShopId == criteria.ShopId);
            }

            if (criteria.TicketType != null)
            {
                tickets = tickets.Where(x => x.TicketType == criteria.TicketType);
            }

            if (criteria.Status != null)
            {
                tickets = tickets.Where(x => x.Status == criteria.Status);
            }

            if (criteria.FromDate != null && criteria.ToDate != null)
            {
                tickets = tickets.Where(x => x.CreatedDate <= criteria.ToDate
                                             && x.CreatedDate >= criteria.FromDate);
            }

            tickets = tickets.OrderByDescending(x => x.Id).ToList();
            foreach (var ticket in tickets)
            {
                var user = context.SecurityProvider.NapContextHelper.UserService.GetById(int.Parse(ticket.CreatedBy));
                ticket.CreatedBy = user.Username;
            }

            return tickets.ToList();
        }

        public bool CreateTicket(Ticket ticket, Guid tempId)
        {
            ticket.CreatedBy = context.SecurityProvider.CurrentUserId;
            ticket.Owner = context.SecurityProvider.CurrentUserId;
            ticket.CurrentStatusSetBy = context.SecurityProvider.CurrentUserId;
            ticket.LastUpdateBy = context.SecurityProvider.CurrentUserId;

            context.Tickets.Add(ticket);
            var result = context.SaveChanges();

            fileManagerService.CommitPendingAttachments(ticket, tempId);
            ticketEventService.CreateNewTicketEvent(ticket);

            return result > 0;
        }

        public bool UpdateTicketStatus(int id, TicketStatus status)
        {
            var result = false;

            var ticket = context.Tickets.Find(id);

            if (ticket != null)
            {
                var oldStatus = ticket.TicketStatus;

                ticket.TicketStatus = status;
                ticket.LastUpdateDate = DateTime.Now;
                ticket.LastUpdateBy = context.SecurityProvider.CurrentUserId;
                result = context.SaveChanges() > 0;

                if (result)
                {
                    ticketEventService.CreateUpdateTicketStatusEvent(ticket, oldStatus, status);
                }
            }

            return result;
        }

        public SingleTicketViewModel GetTicketDetail(int id)
        {
            var ticket = GetTicket(id);
            var shopName = string.Empty;
            var userName = string.Empty;
            var singleTicket = new SingleTicketViewModel();
            if (ticket != null)
            {
                var shop = context.Shops.Find(ticket.ShopId);
                var user = context.SecurityProvider.NapContextHelper.UserService.GetById(int.Parse(ticket.CreatedBy));
                shopName = shop.ShopName;
                userName = user.Username;

                singleTicket.Ticket = ticket;
                singleTicket.ShopName = shopName;
                singleTicket.CreatedByUsername = userName;
                singleTicket.CustomFieldValues = customFieldService.GetCustomFieldValuesForTicket(ticket);
                singleTicket.Attachments = TicketDeskFileStore.ListAttachmentInfo(id.ToString(CultureInfo.InvariantCulture), false);
            }

            return singleTicket;
        }
    }
}