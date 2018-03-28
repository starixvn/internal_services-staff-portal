namespace TicketDesk.Web.Client.Areas.Admin.Models.References
{
    using System.Collections.Generic;
    using Core.Enumerations;
    using Domain.Model;

    public class ReferencesViewModel
    {
        public bool IsHostPermission { get; set; }

        public ReferenceType Type { get; set; }

        public IList<Reference> References { get; set; }
    }
}