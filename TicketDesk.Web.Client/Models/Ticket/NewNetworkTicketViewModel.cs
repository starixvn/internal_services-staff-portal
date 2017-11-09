namespace TicketDesk.Web.Client.Models.Ticket
{
    using Domain.Model;

    public class NewNetworkTicketViewModel : TicketWithFilesBaseViewModel
    {
        public Ticket Ticket { get; set; }

        public NewNetworkTicketViewModel()
        {
            Ticket = new Ticket();
        }
    }
}