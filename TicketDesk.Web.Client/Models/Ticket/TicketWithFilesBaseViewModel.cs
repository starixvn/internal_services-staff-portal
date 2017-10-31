namespace TicketDesk.Web.Client.Models.Ticket
{
    using Infrastructure.Attribute;
    using System.Collections.Generic;
    using System.Web;

    public class TicketWithFilesBaseViewModel
    {
        [FileType("jpg,jpeg,png,pdf")]
        public IEnumerable<HttpPostedFileBase> Files { get; set; }
    }
}