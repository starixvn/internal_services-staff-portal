namespace TicketDesk.Web.Client.Models.Ticket
{
    using System.Collections.Generic;
    using Domain.Model;

    public class NewSoftwareTicketViewModel : TicketWithFilesBaseViewModel, ITicketCustomFieldsProvider
    {
        public NewSoftwareTicketViewModel()
        {
            Ticket = new Ticket();
            System = new CustomFieldValue();
            Description = new CustomFieldValue();
            TimeOfIncident = new CustomFieldValue();
        }

        public Ticket Ticket { get; set; }

        public CustomFieldValue System { get; set; }

        public CustomFieldValue TimeOfIncident { get; set; }

        public CustomFieldValue Description { get; set; }
        public IList<CustomFieldValue> GetCustomFieldValuesToInsert(int ticketId)
        {
            var customFieldValues = new List<CustomFieldValue>();

            System.AddDetails(null, ticketId);
            customFieldValues.Add(System);

            Description.AddDetails(null, ticketId);
            customFieldValues.Add(Description);

            TimeOfIncident.AddDetails(null, ticketId);
            customFieldValues.Add(TimeOfIncident);

            return customFieldValues;
        }
    }
}