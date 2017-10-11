namespace TicketDesk.Web.Client.Models.Ticket
{
    using System.Collections.Generic;
    using Domain.Model;

    public class NewTicketViewModel
    {
        public Ticket Ticket { get; set; }

        public IEnumerable<CustomField> CustomFields { get; set; }
    }
}