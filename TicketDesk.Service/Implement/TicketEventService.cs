namespace TicketDesk.Service.Implement
{
    using Domain.Model;
    using Interface;

    public class TicketEventService : EntityFrameworkBaseService, ITicketEventService
    {
        public bool CreateTicketEvent(TicketEvent ticketEvent)
        {
            context.TicketEvents.Add(ticketEvent);
            var result = context.SaveChanges();

            return result > 0;
        }
    }
}
