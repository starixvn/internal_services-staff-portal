using System.Collections.Generic;
using TicketDesk.Domain.Model;

namespace TicketDesk.Web.Client.Models.Admin
{
    public class ReferenceViewModel
    {
        public IList<Reference> References { get; set; }
    }
}