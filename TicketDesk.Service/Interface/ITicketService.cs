namespace TicketDesk.Service.Interface
{
    using System.Collections.Generic;
    using Domain.Model;

    public interface ITicketService
    {
        Ticket GetTicket(int id);

        IList<TicketGroup> GetUserOpeningTicketsInGroups(int userId, int skip, int take);

        int GetUserTotalOpeningTickets(int userId);
    }
}
