namespace TicketDesk.Web.Client.Models.Ticket
{
    using Domain.Model;

    public class NewHardwareTicketViewModel : TicketWithFilesBaseViewModel
    {
        public Ticket Ticket { get; set; }

        public NewHardwareTicketViewModel()
        {
            Ticket = new Ticket();
        }
    }
}