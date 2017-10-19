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
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Domain;
    using Domain.Model;
    using Localization.Controllers;
    using Search.Common;

    [RoutePrefix("search")]
    [Route("{action=index}")]
    public class SearchController : BaseController
    {
        public SearchController(TdDomainContext context)
        {
            Context = context;
        }

        private TdDomainContext Context { get; }

        [ValidateInput(false)]
        [Route("")]
        [Route("{term}")]
        [HttpGet]
        [NapAuth(FunctionKey = FunctionKeys.QuickSearch, Policy = Constant.Policy.Read)]
        public async Task<ActionResult> Index(string term)
        {
            var projectId = await Context.UserSettingsManager.GetUserSelectedProjectIdAsync(Context).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(term))
            {
                var model = await TdSearchContext.Current.SearchAsync(Context.Tickets.Include(t => t.Project), term, projectId).ConfigureAwait(false);

                if (Context.Projects.Count() > 1)
                {
                    ViewBag.IsMultiProject = true;
                    ViewBag.SearchProjectName = (projectId == 0)
                        ? Strings.ShowingFromAllProjects
                        : string.Format(Strings.ShowingFromProject, Context.Projects.First(p => p.ProjectId == projectId).ProjectName);
                }

                return View(model);
            }

            return View(new Ticket[0]);
        }
    }
}