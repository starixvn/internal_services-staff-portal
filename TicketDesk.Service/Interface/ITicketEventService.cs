namespace TicketDesk.Service.Interface
{
    using Domain.Model;

    public interface ITicketEventService
    {
        bool CreateNewTicketEvent(Ticket ticket);

        bool CreateUpdateTicketStatusEvent(Ticket ticket, TicketStatus oldStatus, TicketStatus newStatus);

        string GetLastStatusChangeTicketEventMessage(int ticketId);
    }
}
