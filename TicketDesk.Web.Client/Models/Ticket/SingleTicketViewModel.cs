namespace TicketDesk.Web.Client.Models.Ticket
{
    using Domain.Model;
    using System.Collections.Generic;

    public class SingleTicketViewModel
    {
        public Ticket Ticket { get; set; }

        public IEnumerable<CustomFieldValue> CustomFieldValues { get; set; }
    }
}