namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Client.Controllers;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Models.Announcements;
    using Service.Interface;
    using Localization.Client;

    public class AnnouncementController : BaseController
    {
        private readonly IAnnouncementService announcementService;
        private readonly TimeSpan tolerateTime = TimeSpan.FromMinutes(5);

        public AnnouncementController(IAnnouncementService announcementService)
        {
            this.announcementService = announcementService;
        }

        [NapAuth(FunctionKey = FunctionKeys.Announcement, Policy = Constant.Policy.Read)]
        public ActionResult AnnouncementList(AnnouncementViewModel viewModel)
        {
            var listAnnouncementViewModel = new ListAnnouncementViewModel
            {
                Announcements = announcementService.GetAnnouncements(viewModel.GetQueryCriteria())
                    .OrderByDescending(x => x.LastModified),
                HaveHostPermission = IsHaveHostPermission(FunctionKeys.Announcement, Constant.Policy.Host)
            };

            return PartialView("_AnouncementList", listAnnouncementViewModel);
        }

        public ActionResult CheckPublishedDateTime(NewAnnouncementViewModel viewModel)
        {
            var result = viewModel.Announcement.PublishedDateTime == null ||
                     (((DateTime.Now - viewModel.Announcement.PublishedDateTime) <= tolerateTime) &&
                      ((viewModel.Announcement.PublishedDateTime - DateTime.Now) <= TimeSpan.FromDays(365)));

            // Too far in the past or more than 1 year in the future
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [NapAuth(FunctionKey = FunctionKeys.Announcement, Policy = Constant.Policy.Host)]
        public ActionResult Create()
        {
            var viewModel = new NewAnnouncementViewModel
            {
                Announcement = new Announcement()
            };

            return PartialView("_Create", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NapAuth(FunctionKey = FunctionKeys.Announcement, Policy = Constant.Policy.Host)]
        public ActionResult Create(NewAnnouncementViewModel viewModel)
        {
            announcementService.AddAnnouncement(viewModel.Announcement);

            return Json(new
            {
                success = true,
                message = Resources.SingleAnnouncementCreatedMessage
            });
        }

        [NapAuth(FunctionKey = FunctionKeys.Announcement, Policy = Constant.Policy.Host)]
        public ActionResult Delete(int id)
        {
            var viewModel = new Announcement { Id = id };
            return PartialView("_Delete", viewModel);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.Announcement, Policy = Constant.Policy.Host)]
        public ActionResult Delete(Announcement viewModel)
        {
            var result = announcementService.DeleteAnnouncement(viewModel.Id);

            return Json(new
            {
                success = result,
                message = Resources.SingleAnnouncementDeletedMessage
            });
        }

        [NapAuth(FunctionKey = FunctionKeys.Announcement, Policy = Constant.Policy.Host)]
        public ActionResult Edit(int id)
        {
            var viewModel = new NewAnnouncementViewModel
            {
                Announcement = announcementService.GetAnnouncement(id)
            };

            return PartialView("_Edit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NapAuth(FunctionKey = FunctionKeys.Announcement, Policy = Constant.Policy.Host)]
        public ActionResult Edit(NewAnnouncementViewModel viewModel)
        {
            announcementService.UpdateAnnouncement(viewModel.Announcement);

            return Json(new
            {
                success = true,
                message = Resources.SingleAnnouncementEditedMessage
            });
        }

        [NapAuth(FunctionKey = FunctionKeys.Announcement, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            var today = DateTime.Now;
            var viewModel = new AnnouncementViewModel
            {
                PublishedFrom = today.AddDays(-7),
                PublishedTo = today,
                Type = null,
                State = null,
                HaveHostPermission = IsHaveHostPermission(FunctionKeys.Announcement, Constant.Policy.Host)
            };

            return View(viewModel);
        }

        [NapAuth(FunctionKey = FunctionKeys.Announcement, Policy = Constant.Policy.Host)]
        public ActionResult IsPublishImmediately(NewAnnouncementViewModel viewModel)
        {
            // Check datetime
            // Published in the past
            // TOLERATE TIME: -/+5 MINUTES

            var result = viewModel.Announcement.PublishedDateTime != null
                         &&
                         (Math.Abs((DateTime.Now - viewModel.Announcement.PublishedDateTime).Value.TotalMinutes) <=
                          tolerateTime.Minutes);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.Announcement, Policy = Constant.Policy.Host)]
        public ActionResult Publish(int[] ids)
        {
            announcementService.PublishAnnouncements(ids);

            var message = ids.Length > 1
                ? Resources.MultipleAnnouncementPublishedMessage
                : Resources.SingleAnnouncementPublishedMessage;

            return Json(new
            {
                success = true,
                message
            });
        }

        [NapAuth(FunctionKey = FunctionKeys.Announcement, Policy = Constant.Policy.Host)]
        public ActionResult Publish(NewAnnouncementViewModel viewModel)
        {
            announcementService.UpdateAnnouncement(viewModel.Announcement);

            return Json(new
            {
                success = true,
                message = Resources.SingleAnnouncementPublishedMessage
            });
        }

        [NapAuth(FunctionKey = FunctionKeys.Announcement, Policy = Constant.Policy.Host)]
        public ActionResult QuickPublish(int[] ids)
        {
            return PartialView("_QuickPublish", ids);
        }
    }
}