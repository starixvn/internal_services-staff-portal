namespace TicketDesk.Web.Client.Areas.Admin.Models.References
{
    using System.Collections.Generic;
    using TicketDesk.Domain.Model;

    public class ListReferencesViewModel
    {
        public IEnumerable<Reference> References { get; set; }
    }
}