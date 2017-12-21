namespace TicketDesk.Web.Client.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using Domain;
    using Domain.Infrastructure;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Models.Admin;

    [NapAuth(FunctionKey = FunctionKeys.Announcement, Policy = Constant.Policy.Host)]
    public class AdminController : BaseController
    {
        private TdDomainContext Context { get; }

        public AdminController(TdDomainContext context)
        {
            Context = context;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Announcement(AnnouncementViewModel viewModel = null)
        {
            viewModel.Announcements = Context.Announcements.ToList();

            return View(viewModel);
        }
    }
}