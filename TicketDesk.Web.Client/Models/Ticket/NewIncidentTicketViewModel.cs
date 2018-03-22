namespace TicketDesk.Web.Client.Models.Ticket
{
    using System.Collections.Generic;
    using Domain.Model;

    public class NewIncidentTicketViewModel : TicketWithFilesBaseViewModel, ITicketCustomFieldsProvider
    {
        public bool AllowToChooseRecipient { get; set; }

        public Ticket Ticket { get; set; }

        public CustomFieldValue TimeOfIncident { get; set; }

        public CustomFieldValue Description { get; set; }

        public NewIncidentTicketViewModel()
        {
            Ticket = new Ticket();
            TimeOfIncident = new CustomFieldValue();
            Description = new CustomFieldValue();
        }

        public IList<CustomFieldValue> GetCustomFieldValuesToInsert(int ticketId)
        {
            var customFieldValues = new List<CustomFieldValue>();

            Description.AddDetails(null, ticketId);
            customFieldValues.Add(Description);

            TimeOfIncident.AddDetails(null, ticketId);
            customFieldValues.Add(TimeOfIncident);

            return customFieldValues;
        }
    }
}