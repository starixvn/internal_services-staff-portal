namespace TicketDesk.Domain.Model
{
    using System.Collections.Generic;
    using IO;

    public class SingleTicketViewModel
    {
        public Ticket Ticket { get; set; }

        public string ShopName { get; set; }

        public string CreatedByUsername { get; set; }

        public string UpdatedByUsername { get; set; }

        public IEnumerable<CustomFieldValue> CustomFieldValues { get; set; }

        public IEnumerable<TicketDeskFileInfo> Attachments { get; set; }

        public string TicketEditedMessage { get; set; }
    }
}