namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System.Linq;
    using System.Net;
    using System.Web.Mvc;
    using Client.Controllers;
    using Domain.Model;
    using Models.Announcements;
    using Service.Implement;
    using Service.Interface;

    [TdAuthorize]
    public class AnnouncementsController : BaseController
    {
        private readonly IAnnouncementService _announcementService;

        public AnnouncementsController(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        public ActionResult Index(AnnouncementViewModel viewModel = null)
        {
            if (viewModel == null)
            {
                viewModel = new AnnouncementViewModel();
            }

            if (viewModel.Announcements == null)
            {
                var announcements = _announcementService.GetAnnouncements(viewModel.GetQueryCriteria());

                viewModel.TotalItem = announcements.Count();

                var paginationService = new PaginationService<Announcement, int>();

                viewModel.Announcements = paginationService.Pagination(
                    announcements,
                    x => x.Id,
                    viewModel.Skipped,
                    viewModel.ItemPerPage);
            }

            return View(viewModel);
        }

        public ActionResult Create()
        {
            var viewModel = new NewAnnouncementViewModel
            {
                Announcement = new Announcement(),
                IsImportant = false
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NewAnnouncementViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _announcementService.AddAnnouncement(viewModel.GetNewAnnouncementCriteria(CurrentSession.Username));

                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var announcement = _announcementService.GetAnnouncement(id.Value);

            var invalidAnnouncement = ReturnInvalidAnnouncement(announcement);
            if (invalidAnnouncement != null)
            {
                return invalidAnnouncement;
            }

            var viewModel = new NewAnnouncementViewModel
            {
                Announcement = announcement,
                IsImportant = announcement.Status == AnnouncementStatus.Important,
                Date = announcement.PublishedDateTime?.Date,
                Time = announcement.PublishedDateTime?.TimeOfDay
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NewAnnouncementViewModel viewModel)
        {
            // Prevent edit published Announcement
            var announcement = _announcementService.GetAnnouncement(viewModel.Announcement.Id);

            var invalidAnnouncement = ReturnInvalidAnnouncement(announcement);
            if (invalidAnnouncement != null)
            {
                return invalidAnnouncement;
            }

            if (ModelState.IsValid)
            {
                _announcementService.UpdateAnnouncement(viewModel.GetNewAnnouncementCriteria(CurrentSession.Username));
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var announcement = _announcementService.GetAnnouncement(id.Value);

            var invalidAnnouncement = ReturnInvalidAnnouncement(announcement);
            if (invalidAnnouncement != null)
            {
                return invalidAnnouncement;
            }

            return View(announcement);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var announcement = _announcementService.GetAnnouncement(id);

            var invalidAnnouncement = ReturnInvalidAnnouncement(announcement);
            if (invalidAnnouncement != null)
            {
                return invalidAnnouncement;
            }

            if (announcement != null)
            {
                _announcementService.DeleteAnnouncement(id);
            }

            return RedirectToAction("Index");
        }

        private ActionResult ReturnInvalidAnnouncement(Announcement announcement)
        {
            if (announcement == null)
            {
                return HttpNotFound();
            }

            if (announcement.State == AnnouncementState.Published)
            {
                return RedirectToAction("Index");
            }

            return null;
        }
    }
}
