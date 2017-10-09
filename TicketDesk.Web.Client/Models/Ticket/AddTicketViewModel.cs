namespace TicketDesk.Web.Client.Models.Ticket
{
    using System.Collections.Generic;
    using Domain.Model;

    public class AddTicketViewModel
    {
        public Ticket Ticket { get; set; }

        public IEnumerable<CustomField> CustomFields { get; set; }
    }
}