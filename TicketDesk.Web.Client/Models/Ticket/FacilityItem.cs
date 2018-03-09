namespace TicketDesk.Web.Client.Models.Ticket
{
    using Domain.Model;

    public class FacilityItem
    {
        public string Index { get; set; }

        public CustomFieldValue ItemName { get; set; }

        public CustomFieldValue Reason { get; set; }

        public FacilityItem()
        {
            ItemName = new CustomFieldValue();
            Reason = new CustomFieldValue();
        }
    }
}