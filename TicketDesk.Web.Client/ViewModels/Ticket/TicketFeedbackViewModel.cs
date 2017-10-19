namespace TicketDesk.Web.Client.ViewModels.Ticket
{
    using Models.Ticket;
    using System;
    using System.Collections.Generic;
    using Domain.Model;

    public class TicketFeedbackViewModel
    {
        public Ticket Ticket { get; set; }

        public IList<TicketFeedbackCustomField> CustomFields { get; set; }

        public Guid tempId { get; set; }
    }
}