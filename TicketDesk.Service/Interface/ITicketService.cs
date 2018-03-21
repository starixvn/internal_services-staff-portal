namespace TicketDesk.Service.Interface
{
    using System;
    using System.Collections.Generic;
    using Criteria.TicketList;
    using Domain.Model;

    public interface ITicketService
    {
        Ticket GetTicket(int id);

        IList<TicketGroup> GetUserOpeningTicketsInGroups(int userId, int skip, int take);

        int GetUserTotalOpeningTickets(int userId);

        int GetUserTotalTicketByStatus(int userId, TicketStatus? ticketStatus);

        bool UpdateTicketSubscription(int ticketId, string userId);

        bool CreateTicket(Ticket ticket, Guid tempId);

        bool UpdateTicketStatus(int id, TicketStatus status);

        IEnumerable<string> CommitPendingAttachments(Ticket ticket, Guid tempId);

        IEnumerable<TicketViewModel> GetTicketByStatus(int userId, int skip, int take, TicketStatus? ticketStatus);

        IList<TicketViewModel> GetTicketByFilter(TicketListFilterCriteria criteria);
    }
}