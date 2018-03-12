namespace TicketDesk.Service.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using Domain.Model;
    using Interface;
    using IO;

    public class TicketService : EntityFrameworkBaseService, ITicketService
    {
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
