namespace TicketDesk.Service.Interface
{
    using System;
    using System.Collections.Generic;
    using Core.Enumerations;
    using Criteria.TicketList;
    using Domain.Model;

    public interface ITicketService
    {
        Ticket GetTicket(int id);

        IList<TicketGroup> GetUserOpeningTicketsInGroups();

        ////bool UpdateTicketSubscription(int ticketId, string userId);

        bool CreateTicket(Ticket ticket, Guid tempId);

        bool UpdateTicketStatus(int id, TicketStatus status);

        IEnumerable<TicketViewModel> GetTickets(int userId, TicketStatus? ticketStatus, TicketCategoryTypes? ticketType);

        IList<TicketViewModel> GetTicketByFilter(TicketListFilterCriteria criteria);

        SingleTicketViewModel GetTicketDetail(int id);
    }
}