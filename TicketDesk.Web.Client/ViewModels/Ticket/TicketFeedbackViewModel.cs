
namespace TicketDesk.Web.Client.ViewModels.Ticket
{
    using System.Collections.Generic;
    using System.Linq;
    using TicketDesk.Domain.Model;
    public class TicketFeedbackViewModel
    {
        public Ticket Ticket { get; set; }

        public IEnumerable<CustomField> CustomFields { get; set; }

        public IEnumerable<CustomFieldValue> CustomFieldValue { get; set; }

        public IList<string> ConvertValuesToList(string values)
        {
            return values.Split(',').ToList();
        }
    }
}