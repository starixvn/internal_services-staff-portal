namespace TicketDesk.Web.Client.ViewModels.Ticket
{
    using Models.Ticket;
    using System;
    using System.Collections.Generic;
    using TicketDesk.Domain.Model;

    public class TicketFeedbackViewModel
    {
        public Ticket Ticket { get; set; }

        public IList<TicketFeedbackCustomField> CustomFields { get; set; }

        public Guid TempId { get; set; }

        public IList<string> ConvertValuesToList(string values)
        {
            return values.Split(new string[] { "\r\n" }, StringSplitOptions.None);
        }
    }
}