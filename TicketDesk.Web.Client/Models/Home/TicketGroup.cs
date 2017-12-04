namespace TicketDesk.Web.Client.Models.Home
{
    using Domain.Model;
    using System.Collections.Generic;

    public class TicketGroup
    {
        public string Name { get; set; }

        public int TotalTickets { get; set; }

        public IEnumerable<Ticket> Tickets { get; set; }
    }
}