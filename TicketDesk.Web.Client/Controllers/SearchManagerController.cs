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

namespace TicketDesk.Web.Client.Controllers
{
    using Domain.Infrastructure;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Domain;
    using Domain.Model;
    using Search.Common;

    [RoutePrefix("admin/search-manager")]
    [Route("{action=index}")]
    public class SearchManagerController : Controller
    {
        public SearchManagerController(TdDomainContext context)
        {
            Context = context;
        }

        private TdDomainContext Context { get; set; }

        [Route("")]
        [NapAuth(FunctionKey = FunctionKeys.SearchManagement, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [Route("remove-index")]
        [NapAuth(FunctionKey = FunctionKeys.SearchManagement, Policy = Constant.Policy.Read)]
        public async Task<ActionResult> RemoveIndex()
        {
            await TdSearchContext.Current.IndexManager.RemoveIndexAsync();

            // initialize search
            await TdSearchContext.Current.IndexManager.RunIndexMaintenanceAsync();
            IEnumerable<SearchIndexItem> searchItems = new List<SearchIndexItem>();
            await TdSearchContext.Current.IndexManager.AddItemsToIndexAsync(searchItems);

            ViewBag.IndexRemoved = true;
            return View("Index");
        }

        [Route("populate-index")]
        [NapAuth(FunctionKey = FunctionKeys.SearchManagement, Policy = Constant.Policy.Read)]
        public async Task<ActionResult> PopulateIndex()
        {
            await TdSearchContext.Current.IndexManager.RunIndexMaintenanceAsync();
            var searchItems = Context.Tickets.Include("TicketEvents").ToSeachIndexItems();
            await TdSearchContext.Current.IndexManager.AddItemsToIndexAsync(searchItems);
            ViewBag.IndexPopulated = true;
            return View("Index");
        }
    }
}