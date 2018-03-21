namespace TicketDesk.Service.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
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
        private readonly IMailService mailService;

        public TicketService() : this(null)
        {
        }

        public TicketService(IMailService mailService)
        {
            this.mailService = mailService ?? new MailService();
        }

        public Ticket GetTicket(int id)
        {
            return context.Tickets.Find(id);
        }

        public IList<TicketGroup> GetUserOpeningTicketsInGroups(int userId, int skip, int take)
        {
            var result = new List<TicketGroup>();
            var userIdString = userId.ToString();
            var tickets = context.Tickets
                .Where(x => x.CreatedBy == userIdString
                            && (x.TicketStatus == TicketStatus.Active
                                || x.TicketStatus == TicketStatus.MoreInfo))
                .OrderBy(x => x.CategoryId)
                .ThenByDescending(x => x.TicketId)
                .Skip(skip)
                .Take(take)
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

        public int GetUserTotalOpeningTickets(int userId)
        {
            string userIdString = userId.ToString();
            return context.Tickets.Count(x => x.CreatedBy == userIdString
                                              && (x.TicketStatus == TicketStatus.Active
                                                  || x.TicketStatus == TicketStatus.MoreInfo));
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
                        Category = (TicketCategoryTypes)t.CategoryId,
                        Status = t.TicketStatus,
                        CreatedDate = t.CreatedDate,
                        UpdatedDate = t.LastUpdateDate
                    });
        }

        public IEnumerable<TicketViewModel> GetTicketByStatus(int userId, int skip, int take,
            TicketStatus? ticketStatus)
        {
            var tickets = BasicGetTicketJoinWithShop();

            tickets = ticketStatus == null
                ? tickets.OrderByDescending(x => x.UpdatedDate)
                    .Skip(skip)
                    .Take(take)
                : tickets.Where(x => x.Status == ticketStatus)
                    .OrderByDescending(x => x.UpdatedDate)
                    .Skip(skip)
                    .Take(take);

            return tickets;
        }

        public int GetUserTotalTicketByStatus(int userId, TicketStatus? ticketStatus)
        {
            return ticketStatus == null
                ? context.Tickets.Count(x => x.CreatedBy == userId.ToString())
                : context.Tickets.Count(x => x.CreatedBy == userId.ToString() && x.TicketStatus == ticketStatus);
        }

        public IList<TicketViewModel> GetTicketByFilter(TicketListFilterCriteria criteria)
        {
            var tickets = BasicGetTicketJoinWithShop();

            if (criteria.ShopId != null)
            {
                tickets = tickets.Where(x => x.ShopId == criteria.ShopId);
            }

            if (criteria.Category != null)
            {
                tickets = tickets.Where(x => x.Category == criteria.Category);
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

            return tickets.OrderByDescending(x => x.CreatedDate).ToList();
        }

        public bool UpdateTicketSubscription(int ticketId, string userId)
        {
            var ticket = context.Tickets
                .Where(x => x.TicketId == ticketId)
                .Include(x => x.TicketSubscribers)
                .Include(t => t.TicketEvents.Select(e => e.TicketEventNotifications))
                .FirstOrDefault();

            var isSubscribed = false;

            if (ticket == null)
            {
                return false;
            }

            var subscriber = ticket.TicketSubscribers.FirstOrDefault(s => s.SubscriberId == userId);

            if (subscriber == null)
            {
                subscriber = new TicketSubscriber
                {
                    SubscriberId = userId,
                };

                ticket.TicketSubscribers.Add(subscriber);
                isSubscribed = true;
            }
            else
            {
                ticket.TicketSubscribers.Remove(subscriber);
            }

            context.SaveChanges();

            return isSubscribed;
        }

        public bool CreateTicket(Ticket ticket, Guid tempId)
        {
            ticket.CreatedBy = context.SecurityProvider.CurrentUserId;
            context.Tickets.Add(ticket);
            var result = context.SaveChanges();
            CommitPendingAttachments(ticket, tempId);

            var mailRecord = mailService.ComposeMail(ticket);
            mailService.Send(mailRecord);

            return result > 0;
        }

        public IEnumerable<string> CommitPendingAttachments(Ticket ticket, Guid tempId)
        {
            var attachments = TicketDeskFileStore.ListAttachmentInfo(tempId.ToString(), true).ToArray();

            foreach (var attachment in attachments)
            {
                TicketDeskFileStore.MoveFile(
                    attachment.Name,
                    tempId.ToString(),
                    ticket.TicketId.ToString(CultureInfo.InvariantCulture),
                    true,
                    false);
            }

            return attachments.Select(a => a.Name);
        }
    }
}