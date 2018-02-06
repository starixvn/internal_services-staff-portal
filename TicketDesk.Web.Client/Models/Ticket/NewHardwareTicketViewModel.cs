namespace TicketDesk.Web.Client.Models.Ticket
{
    using Domain.Model;

    public class NewHardwareTicketViewModel : TicketWithFilesBaseViewModel
    {
        public NewHardwareTicketViewModel()
        {
            Ticket = new Ticket();
            System = new CustomFieldValue();
            Description = new CustomFieldValue();
            TimeOfIncident = new CustomFieldValue();
        }

        public Ticket Ticket { get; set; }

        public CustomFieldValue System { get; set; }

        public CustomFieldValue TimeOfIncident { get; set; }

        public CustomFieldValue Description { get; set; }
    }
}