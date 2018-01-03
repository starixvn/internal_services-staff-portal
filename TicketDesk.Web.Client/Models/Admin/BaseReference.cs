using System.Collections.Generic;
using TicketDesk.Domain.Model;

namespace TicketDesk.Web.Client.Models.Admin
{
    public class BaseReference
    {
        public string ImageLink { get; set; }

        public IList<Reference> References { get; set; }
    }
}