namespace TicketDesk.Web.Client.Models.Ticket
{
    using System.Web;

    public class TicketWithFilesBaseViewModel
    {
        public HttpPostedFileBase[] Files { get; set; }
    }
}