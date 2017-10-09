namespace TicketDesk.Web.Client.Models
{
    using Postal;

    public class TicketEmail : Email
    {
        public Domain.Model.Ticket Ticket { get; set; }

        public string SiteRootUrl { get; set; }

        public bool IsMultiProject { get; set; }

        public bool ForNewTicket { get; set; }
    }
}