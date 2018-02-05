namespace TicketDesk.Web.Client.Models.Ticket
{
    using Domain.Model;

    public class NewCustomerSupportTicketViewModel : TicketWithFilesBaseViewModel
    {
        public Ticket Ticket { get; set; }

        public CustomFieldValue Description { get; set; }

        public CustomFieldValue NationalIdCardNo { get; set; }

        public CustomFieldValue TimeOfIncident { get; set; }

        public NewCustomerSupportTicketViewModel()
        {
            Ticket = new Ticket();
            Description = new CustomFieldValue();
            NationalIdCardNo = new CustomFieldValue();
            TimeOfIncident = new CustomFieldValue();
        }
    }
}