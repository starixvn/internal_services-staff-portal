
namespace TicketDesk.Web.Client.ViewModels.Ticket
{
    using System.Collections.Generic;
    using TicketDesk.Domain.Model;
    public class TicketFeedbackViewModel
    {
        public Ticket Ticket { get; set; }

        public IEnumerable<CustomField> CustomFields { get; set; }

    }
}