namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web.Mvc;
    using Client.Controllers;
    using Criteria.Announcement;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Models.Announcements;
    using Service.Implement;
    using Service.Interface;

    [NapAuth(FunctionKey = FunctionKeys.Announcement, Policy = Constant.Policy.Read)]
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
                    viewModel.ItemPerPage)
                    .ToList();
            }

            return View(viewModel);
        }

        public ActionResult Create()
        {
            var viewModel = new NewAnnouncementViewModel
            {
                Announcement = new Announcement(),
                ActionType = ActionType.Create,
                ModalId = ModalId.CreateAnnouncementModal
            };

            return PartialView("_Create", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NewAnnouncementViewModel viewModel)
        {
            // Check datetime
            if (viewModel.Announcement.PublishedDateTime != null &&
                (viewModel.Announcement.PublishedDateTime - DateTime.Now) > TimeSpan.FromDays(365))
            {
                ModelState.AddModelError("Announcement.PublishedDateTime", "Date not in range");
            }

            if (!ModelState.IsValid)
            {
                return PartialView("_AnnouncementForm", viewModel);
            }

            _announcementService.AddAnnouncement(viewModel.GetNewAnnouncementCriteria(CurrentSession.Username));
            return Json(new { success = true });

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
                ////Date = announcement.PublishedDateTime,
                ////Time = announcement.PublishedDateTime?.TimeOfDay,
                ModalId = ModalId.EditAnnouncementModal,
                ActionType = ActionType.Edit
            };

            return PartialView("_Create", viewModel);
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
                return Json(new { success = true });
            }

            viewModel.ModalId = ModalId.EditAnnouncementModal;
            viewModel.ActionType = ActionType.Edit;

            return PartialView("_AnnouncementForm", viewModel);
        }

        public ActionResult Delete(int id)
        {
            var viewModel = new Announcement { Id = id };
            return PartialView("_Delete", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Announcement viewModel)
        {
            var announcement = _announcementService.GetAnnouncement(viewModel.Id);

            var invalidAnnouncement = ReturnInvalidAnnouncement(announcement);
            if (invalidAnnouncement != null)
            {
                return invalidAnnouncement;
            }

            if (announcement != null)
            {
                _announcementService.DeleteAnnouncement(viewModel.Id);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Publish(IList<Announcement> announcements)
        {
            var criteria = new PublishAnnouncementsCriteria
            {
                Ids = new List<int>(),
                ModifiedBy = CurrentSession.Username
            };

            foreach (var announcement in announcements)
            {
                if (announcement.IsWaitingToPublish)
                {
                    criteria.Ids.Add(announcement.Id);
                }
            }

            _announcementService.PublishAnnouncements(criteria);

            return RedirectToAction("Index", "Announcements");
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
