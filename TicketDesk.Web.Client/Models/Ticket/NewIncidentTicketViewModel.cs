namespace TicketDesk.Web.Client.Models.Ticket
{
    using Domain.Model;

    public class NewIncidentTicketViewModel : TicketWithFilesBaseViewModel
    {
        public Ticket Ticket { get; set; }

        public NewIncidentTicketViewModel()
        {
            Ticket = new Ticket();
        }
    }
}