namespace TicketDesk.Web.Client.Models.Ticket
{
    using Domain.Model;

    public class NewSoftwareTicketViewModel : TicketWithFilesBaseViewModel
    {
        public Ticket Ticket { get; set; }

        public NewSoftwareTicketViewModel()
        {
            Ticket = new Ticket();
        }
    }
}