namespace TicketDesk.Web.Client.Models.Ticket
{
    using System.Collections.Generic;
    using Domain.Model;

    public class NewCustomerSupportTicketViewModel : TicketWithFilesBaseViewModel, ITicketCustomFieldsProvider
    {
        public bool AllowToChooseRecipient { get; set; }

        public Ticket Ticket { get; set; }

        public CustomFieldValue SupportType { get; set; }

        public CustomFieldValue Description { get; set; }

        public CustomFieldValue NationalIdCardNo { get; set; }

        public CustomFieldValue TimeOfIncident { get; set; }

        public NewCustomerSupportTicketViewModel()
        {
            Ticket = new Ticket();
            Description = new CustomFieldValue();
            NationalIdCardNo = new CustomFieldValue();
            TimeOfIncident = new CustomFieldValue();
            SupportType = new CustomFieldValue();
        }

        public IList<CustomFieldValue> GetCustomFieldValuesToInsert(int ticketId)
        {
            var customFieldValues = new List<CustomFieldValue>();

            SupportType.AddDetails(null, ticketId);
            customFieldValues.Add(SupportType);

            Description.AddDetails(null, ticketId);
            customFieldValues.Add(Description);

            NationalIdCardNo.AddDetails(null, ticketId);
            customFieldValues.Add(NationalIdCardNo);

            TimeOfIncident.AddDetails(null, ticketId);
            customFieldValues.Add(TimeOfIncident);

            return customFieldValues;
        }
    }
}