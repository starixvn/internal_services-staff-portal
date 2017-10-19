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
    using Models;

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
            var model = Context.Projects.Select(p => new ProjectListViewModelItem { Project = p, NumberOfTickets = p.Tickets.Count });
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
        public async Task<ActionResult> New(Project project)
        {
            if (ModelState.IsValid)
            {
                Context.Projects.Add(project);
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
            var project = await Context.Projects.FindAsync(projectId).ConfigureAwait(false);
            if (project == null)
            {
                return RedirectToAction("Index");
            }

            // is there more than one project, and does target project exist?
            var numProjects = await Context.Projects.CountAsync().ConfigureAwait(false);
            var reassignProject = await Context.Projects.FindAsync(reassignTo).ConfigureAwait(false);
            if (numProjects < 2 || reassignProject == null)
            {
                return RedirectToAction("Edit", new { projectId });
            }

            // move tickets, then remove project
            var ticketsToReassign = Context.Tickets.Where(t => t.ProjectId == projectId);
            foreach (var ticket in ticketsToReassign)
            {
                ticket.ProjectId = reassignTo;
            }

            Context.Projects.Remove(project);

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
            var project = await Context.Projects.FindAsync(projectId).ConfigureAwait(false);
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
        public async Task<ActionResult> Edit(Project project, int projectId)
        {
            var existing = await Context.Projects.FindAsync(projectId).ConfigureAwait(false);
            if (ModelState.IsValid && TryUpdateModel(existing))
            {
                var result = await Context.SaveChangesAsync().ConfigureAwait(false);
                if (result > 0)
                {
                    return RedirectToAction("Index");
                }
            }

            await AddViewDataForEdit(projectId).ConfigureAwait(false);
            return View(project);
        }

        private async Task AddViewDataForEdit(int projectId)
        {
            ViewBag.EnableDelete = await Context.Projects.CountAsync().ConfigureAwait(false) > 1;
            ViewBag.ProjectReassignList = Context.Projects
                .Where(p => p.ProjectId != projectId)
                .ToSelectList(p => p.ProjectId.ToString(), p => p.ProjectName);
        }
    }
}