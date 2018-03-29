namespace TicketDesk.Service.Implement
{
    using System.Linq;
    using Domain.Extensions;
    using Domain.Localization;
    using Domain.Model;
    using Interface;
    using Localization.Client;

    public class TicketEventService : EntityFrameworkBaseService, ITicketEventService
    {
        private bool CreateTicketEvent(TicketEvent ticketEvent)
        {
            context.TicketEvents.Add(ticketEvent);
            var result = context.SaveChanges();

            return result > 0;
        }

        public string GetLastStatusChangeTicketEventMessage(int ticketId)
        {
            return context.TicketEvents
                .Where(x => x.TicketId == ticketId && x.ForActivity == TicketActivity.ChangeStatus)
                .OrderByDescending(x => x.EventDate)
                .FirstOrDefault()?
                .EventDescription;
        }

        public bool CreateNewTicketEvent(Ticket ticket)
        {
            TicketEvent ticketEvent = new TicketEvent
            {
                TicketId = ticket.TicketId,
                EventBy = ticket.CreatedBy,
                EventDate = ticket.CreatedDate,
                ForActivity = TicketActivity.Create,
                EventDescription = TicketTextUtility.GetTicketEventDescription(TicketActivity.Create, null, null),
            };

            return CreateTicketEvent(ticketEvent);
        }

        public bool CreateUpdateTicketStatusEvent(Ticket ticket, TicketStatus oldStatus, TicketStatus newStatus)
        {
            TicketEvent ticketEvent = new TicketEvent
            {
                TicketId = ticket.TicketId,
                EventBy = ticket.LastUpdateBy,
                EventDate = ticket.LastUpdateDate,
                ForActivity = TicketActivity.ChangeStatus,
                EventDescription = string.Format(Resources.StatusChangedMessage, oldStatus.GetDisplayName(), newStatus.GetDisplayName()),
            };

            return CreateTicketEvent(ticketEvent);
        }
    }
}
