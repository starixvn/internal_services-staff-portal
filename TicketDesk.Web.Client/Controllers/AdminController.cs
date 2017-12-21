namespace TicketDesk.Web.Client.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using Domain;
    using Domain.Infrastructure;
    using Domain.Model;
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

        public ActionResult AddAnnouncement()
        {
            Announcement newAnnouncement = new Announcement();
            return View(newAnnouncement);
        }

        [HttpPost]
        public ActionResult AddAnnouncement(Announcement announcement)
        {
            announcement.State = AnnouncementState.Scheduled;
            Context.Announcements.Add(announcement);

            Context.SaveChanges();

            return RedirectToAction("Announcement", "Admin");
        }
    }
}