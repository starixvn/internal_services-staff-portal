namespace TicketDesk.Web.Client.ViewModels.Ticket
{
    using Models.Ticket;
    using System.Collections.Generic;
    using System.Linq;
    using TicketDesk.Domain.Model;

    public class TicketFeedbackViewModel
    {
        public Ticket Ticket { get; set; }

        public IEnumerable<TicketCustomField> CustomFields { get; set; }
    }
}