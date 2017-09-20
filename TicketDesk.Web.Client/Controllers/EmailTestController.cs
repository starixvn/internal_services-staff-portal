// TicketDesk - Attribution notice
// Contributor(s):
//
//      Stephen Redd (https://github.com/stephenredd)
//
// This file is distributed under the terms of the Microsoft Public
// License (Ms-PL). See http://opensource.org/licenses/MS-PL
// for the complete terms of use.
//
// For any distribution that contains code from this file, this notice of
// attribution must remain intact, and a copy of the license must be
// provided to the recipient.

using Postal;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using TicketDesk.Domain;
using TicketDesk.Domain.Model;
using TicketDesk.Web.Client.Models;

namespace TicketDesk.Web.Client.Controllers
{
    public class EmailTestController : Controller
    {
        public EmailTestController(TdDomainContext context)
        {
            Context = context;
        }

        public TdDomainContext Context { get; set; }

        // GET: EmailTest
        public ActionResult Index(int id)
        {
            // TODO: Remove/move to admin
            var ticket = Context.Tickets.Include(t => t.TicketTags).First(t => t.TicketId == id);
            var root = Context.TicketDeskSettings.ClientSettings.GetDefaultSiteRootUrl();
            var email = new TicketEmail { Ticket = ticket, SiteRootUrl = root };

            return new EmailViewResult(email);
        }
    }
}