namespace TicketDesk.Web.Client.Models.Ticket
{
    using Domain.Model;

    public class NewNetworkTicketViewModel : TicketWithFilesBaseViewModel
    {
        public Ticket Ticket { get; set; }

        public CustomFieldValue Description { get; set; }

        public NewNetworkTicketViewModel()
        {
            Ticket = new Ticket();
            Description = new CustomFieldValue();
        }
    }
}