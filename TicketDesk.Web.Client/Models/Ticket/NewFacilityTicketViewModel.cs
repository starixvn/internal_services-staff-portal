namespace TicketDesk.Web.Client.Models.Ticket
{
    using System.Collections.Generic;
    using Domain.Model;

    public class NewFacilityTicketViewModel
    {
        public Ticket Ticket { get; set; }

        //public CustomField RequestType { get; set; }

        public CustomFieldValue RequestType { get; set; }

        public ICollection<Item> Items { get; set; }
    }

    public class Item
    {
        public string Index { get; set; }

        public CustomFieldValue ItemName { get; set; }

        public CustomFieldValue Reason { get; set; }
    }
}