namespace TicketDesk.Domain.Model
{
    using System.Collections.Generic;

    public class TicketGroup
    {
        public string Name { get; set; }

        public int TotalItems { get; set; }

        public IEnumerable<Ticket> Tickets { get; set; }
    }
}