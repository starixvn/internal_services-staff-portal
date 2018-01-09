namespace TicketDesk.Web.Client.Models.Ticket
{
    using Domain.Model;

    public class NewSoftwareTicketViewModel : TicketWithFilesBaseViewModel
    {
        public NewSoftwareTicketViewModel()
        {
            Ticket = new Ticket();
            System = new CustomFieldValue();
            Description = new CustomFieldValue();
        }

        public Ticket Ticket { get; set; }

        public CustomFieldValue System { get; set; }

        public CustomFieldValue Description { get; set; }
    }
}