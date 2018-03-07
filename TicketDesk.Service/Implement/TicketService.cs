namespace TicketDesk.Service.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Domain.Model;
    using Interface;

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
    }
}
