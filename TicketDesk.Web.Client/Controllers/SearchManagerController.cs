namespace TicketDesk.Web.Client.Controllers
{
    using Domain;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Search.Common;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [RoutePrefix("admin/search-manager")]
    [Route("{action=index}")]
    public class SearchManagerController : Controller
    {
        public SearchManagerController(TdDomainContext context)
        {
            Context = context;
        }

        private TdDomainContext Context { get; }

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
            await TdSearchContext.Current.IndexManager.RemoveIndexAsync().ConfigureAwait(false);

            // initialize search
            await TdSearchContext.Current.IndexManager.RunIndexMaintenanceAsync().ConfigureAwait(false);
            IEnumerable<SearchIndexItem> searchItems = new List<SearchIndexItem>();
            await TdSearchContext.Current.IndexManager.AddItemsToIndexAsync(searchItems).ConfigureAwait(false);

            ViewBag.IndexRemoved = true;
            return View("Index");
        }

        [Route("populate-index")]
        [NapAuth(FunctionKey = FunctionKeys.SearchManagement, Policy = Constant.Policy.Read)]
        public async Task<ActionResult> PopulateIndex()
        {
            await TdSearchContext.Current.IndexManager.RunIndexMaintenanceAsync().ConfigureAwait(false);
            var searchItems = Context.Tickets.Include("TicketEvents").ToSeachIndexItems();
            await TdSearchContext.Current.IndexManager.AddItemsToIndexAsync(searchItems).ConfigureAwait(false);
            ViewBag.IndexPopulated = true;
            return View("Index");
        }
    }
}