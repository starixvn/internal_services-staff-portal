namespace TicketDesk.Web.Client.Controllers
{
    using Domain;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Localization.Controllers;
    using Models;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [RoutePrefix("admin")]
    [Route("{action=index}")]
    public class ProjectSettingsController : BaseController
    {
        public ProjectSettingsController(TdDomainContext context)
        {
            Context = context;
        }

        private TdDomainContext Context { get; }

        [Route("projects")]
        [NapAuth(FunctionKey = FunctionKeys.ProjectSettings, Policy = Constant.Policy.Read)]
        public async Task<ActionResult> Index()
        {
            var model = Context.Shops.Select(p => new ProjectListViewModelItem { Shop = p, NumberOfTickets = p.Tickets.Count });
            ViewBag.IsHost = NapContextHelper.HasRight(FunctionKeys.ProjectSettings, Constant.Policy.Host);
            await model.LoadAsync().ConfigureAwait(false);

            return View(model);
        }

        [Route("project/new")]
        [NapAuth(FunctionKey = FunctionKeys.ProjectSettings, Policy = Constant.Policy.Host)]
        public ActionResult New()
        {
            return View();
        }

        [Route("project/new")]
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.ProjectSettings, Policy = Constant.Policy.Host)]
        public async Task<ActionResult> New(Shop shop)
        {
            if (ModelState.IsValid)
            {
                Context.Shops.Add(shop);
                if (await Context.SaveChangesAsync().ConfigureAwait(false) > 0)
                {
                    return RedirectToAction("Index");
                }
            }

            ModelState.AddModelError(string.Empty, Strings.UnableToCreateProject);
            return View();
        }

        [Route("project/delete/{projectId:int}")]
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.ProjectSettings, Policy = Constant.Policy.Host)]
        public async Task<ActionResult> Delete(int projectId, int reassignTo)
        {
            // does project to delete exist?
            var project = await Context.Shops.FindAsync(projectId).ConfigureAwait(false);
            if (project == null)
            {
                return RedirectToAction("Index");
            }

            // is there more than one project, and does target project exist?
            var numProjects = await Context.Shops.CountAsync().ConfigureAwait(false);
            var reassignProject = await Context.Shops.FindAsync(reassignTo).ConfigureAwait(false);
            if (numProjects < 2 || reassignProject == null)
            {
                return RedirectToAction("Edit", new { projectId });
            }

            // move tickets, then remove project
            var ticketsToReassign = Context.Tickets.Where(t => t.ShopId == projectId);
            foreach (var ticket in ticketsToReassign)
            {
                ticket.ShopId = reassignTo;
            }

            Context.Shops.Remove(project);

            var result = await Context.SaveChangesAsync().ConfigureAwait(false);
            if (result > 0)
            {
                RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, Strings.UnableToRemoveProject);
            return RedirectToAction("Edit", new { projectId });
        }

        [Route("project/{projectId:int}")]
        [NapAuth(FunctionKey = FunctionKeys.ProjectSettings, Policy = Constant.Policy.Host)]
        public async Task<ActionResult> Edit(int projectId)
        {
            var project = await Context.Shops.FindAsync(projectId).ConfigureAwait(false);
            if (project == null)
            {
                return RedirectToAction("Index");
            }

            await AddViewDataForEdit(projectId).ConfigureAwait(false);
            return View(project);
        }

        [Route("project/{projectId:int}")]
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.ProjectSettings, Policy = Constant.Policy.Host)]
        public async Task<ActionResult> Edit(Shop shop, int projectId)
        {
            var existing = await Context.Shops.FindAsync(projectId).ConfigureAwait(false);
            if (ModelState.IsValid && TryUpdateModel(existing))
            {
                var result = await Context.SaveChangesAsync().ConfigureAwait(false);
                if (result > 0)
                {
                    return RedirectToAction("Index");
                }
            }

            await AddViewDataForEdit(projectId).ConfigureAwait(false);
            return View(shop);
        }

        private async Task AddViewDataForEdit(int projectId)
        {
            ViewBag.EnableDelete = await Context.Shops.CountAsync().ConfigureAwait(false) > 1;
            ViewBag.ProjectReassignList = Context.Shops
                .Where(p => p.ShopId != projectId)
                .ToSelectList(p => p.ShopId.ToString(), p => p.ShopName);
        }
    }
}