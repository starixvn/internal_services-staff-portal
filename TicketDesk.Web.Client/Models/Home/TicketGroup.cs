namespace TicketDesk.Web.Client.Models.Home
{
    using System.Collections.Generic;
    using Domain.Model;

    public class TicketGroup
    {
        public string Name { get; set; }

        public int TotalItems { get; set; }

        public IEnumerable<Ticket> Tickets { get; set; }
    }
}