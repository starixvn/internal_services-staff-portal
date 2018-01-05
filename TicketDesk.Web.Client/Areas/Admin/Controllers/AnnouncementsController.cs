namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Web.Mvc;
    using Client.Controllers;
    using Domain;
    using Domain.Model;
    using Models.Announcements;

    [TdAuthorize]
    public class AnnouncementsController : BaseController
    {
        private readonly TdDomainContext context = new TdDomainContext();

        public ActionResult Index(AnnouncementViewModel viewModel = null)
        {
            if (viewModel?.Announcements == null)
            {
                viewModel = new AnnouncementViewModel();

                IQueryable<Announcement> announcements = context.Announcements;

                if (viewModel.PublishedFrom != null)
                {
                    announcements = announcements.Where(x => viewModel.PublishedFrom <= x.PublishedDateTime);
                }

                if (viewModel.PublishedTo != null)
                {
                    announcements = announcements.Where(x => x.PublishedDateTime <= viewModel.PublishedTo);
                }

                if (viewModel.Status != null)
                {
                    announcements = announcements.Where(x => x.Status == viewModel.Status);
                }

                if (viewModel.State != null)
                {
                    announcements = announcements.Where(x => x.State == viewModel.State);
                }

                viewModel.Announcements = announcements.ToList();
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
                CommitAnnouncement(viewModel);

                context.Announcements.Add(viewModel.Announcement);
                context.SaveChanges();

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

            var announcement = context.Announcements.Find(id);

            if (announcement == null)
            {
                return HttpNotFound();
            }

            if (announcement.State == AnnouncementState.Published)
            {
                return RedirectToAction("Index");
            }

            var viewModel = new NewAnnouncementViewModel
            {
                Announcement = announcement,
                IsImportant = announcement.Status == AnnouncementStatus.Important,
                Date = announcement.PublishedDateTime.Date,
                Time = announcement.PublishedDateTime.TimeOfDay
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NewAnnouncementViewModel viewModel)
        {
            // Prevent edit published Announcement
            var announcement = context.Announcements.Find(viewModel.Announcement.Id);

            var invalidAnnouncement = ReturnInvalidAnnouncement(announcement);
            if (invalidAnnouncement != null)
            {
                return invalidAnnouncement;
            }

            if (ModelState.IsValid)
            {
                CommitAnnouncement(viewModel);

                context.Entry(viewModel.Announcement).State = EntityState.Modified;
                context.SaveChanges();
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

            var announcement = context.Announcements.Find(id);

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
            var announcement = context.Announcements.Find(id);

            var invalidAnnouncement = ReturnInvalidAnnouncement(announcement);
            if (invalidAnnouncement != null)
            {
                return invalidAnnouncement;
            }

            if (announcement != null)
            {
                context.Announcements.Remove(announcement);
                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }

            base.Dispose(disposing);
        }

        private void CommitAnnouncement(NewAnnouncementViewModel viewModel)
        {
            // Set date-time
            viewModel.Announcement.CreatedDateTime = DateTime.Now;
            viewModel.Announcement.LastModified = DateTime.Now;

            // Set published date-time
            if (viewModel.Date != null && viewModel.Time != null)
            {
                var publishedDateTime = viewModel.Date.Value;
                publishedDateTime = publishedDateTime.AddSeconds(viewModel.Time.Value.TotalSeconds);

                viewModel.Announcement.PublishedDateTime = publishedDateTime;
            }

            viewModel.Announcement.ModifiedBy = CurrentSession.Username;

            viewModel.Announcement.Status = viewModel.IsImportant ? AnnouncementStatus.Important : AnnouncementStatus.Normal;
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
