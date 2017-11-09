namespace TicketDesk.Web.Client.Models.Ticket
{
    using Domain.Model;

    public class NewCustomerSupportTicketViewModel : TicketWithFilesBaseViewModel
    {
        public Ticket Ticket { get; set; }

        public NewCustomerSupportTicketViewModel()
        {
            Ticket = new Ticket();
        }
    }
}