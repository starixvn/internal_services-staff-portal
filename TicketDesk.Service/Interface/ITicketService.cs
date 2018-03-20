namespace TicketDesk.Service.Interface
{
    using System;
    using System.Collections.Generic;
    using Domain.Model;

    public interface ITicketService
    {
        Ticket GetTicket(int id);

        IList<TicketGroup> GetUserOpeningTicketsInGroups(int userId, int skip, int take);

        int GetUserTotalOpeningTickets(int userId);

        int GetUserTotalTicketByStatus(int userId, TicketStatus? ticketStatus);

        bool UpdateTicketSubscription(int ticketId, string userId);

        bool CreateTicket(Ticket ticket, Guid tempId);

        IEnumerable<string> CommitPendingAttachments(Ticket ticket, Guid tempId);

        IEnumerable<Ticket> GetTicketByStatus(int userId, int skip, int take, TicketStatus? ticketStatus);
    }
}