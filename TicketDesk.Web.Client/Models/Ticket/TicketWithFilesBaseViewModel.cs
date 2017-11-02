namespace TicketDesk.Web.Client.Models.Ticket
{
    using Infrastructure.Attribute;
    using System.Collections.Generic;
    using System.Web;

    public class TicketWithFilesBaseViewModel
    {
        [FileSize(MaxBytes = 33554432)]
        [FileType(new[] { ".jpg", ".jpeg", ".png", ".pdf" })]
        [NumberOfFile(Max = 20)]
        public IEnumerable<HttpPostedFileBase> Files { get; set; }
    }
}