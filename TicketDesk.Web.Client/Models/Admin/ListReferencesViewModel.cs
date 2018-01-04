namespace TicketDesk.Web.Client.Models.Admin
{
    using System.Collections.Generic;
    using TicketDesk.Domain.Model;

    public class ListReferencesViewModel
    {
        public IEnumerable<Reference> References { get; set; }
    }
}