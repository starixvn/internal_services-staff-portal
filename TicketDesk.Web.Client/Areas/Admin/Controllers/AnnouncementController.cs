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
    using Localization.Client;
    using Models.Announcements;
    using Service.Interface;

    public class AnnouncementController : BaseController
    {
        private readonly IAnnouncementService announcementService;

        public AnnouncementController(IAnnouncementService announcementService)
        {
            this.announcementService = announcementService;
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
            var result = false;

            if (ModelState.IsValid)
            {
                result = announcementService.AddAnnouncement(viewModel.Announcement);
            }

            var message = result ? Resources.SingleAnnouncementCreatedMessage : Resources.CreateAnnouncementError;

            return Json(new
            {
                success = result,
                message
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
            var result = false;

            if (ModelState.IsValid)
            {
                result = announcementService.UpdateAnnouncement(viewModel.Announcement);
            }

            var message = result ? Resources.SingleAnnouncementEditedMessage : Resources.EditAnnouncementError;

            return Json(new
            {
                success = result,
                message
            });
        }

        [NapAuth(FunctionKey = FunctionKeys.Announcement, Policy = Constant.Policy.Host)]
        public ActionResult IsPublishImmediately(NewAnnouncementViewModel viewModel)
        {
            var result = viewModel.Announcement.IsPublished;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [NapAuth(FunctionKey = FunctionKeys.Announcement, Policy = Constant.Policy.Host)]
        public ActionResult Publish(NewAnnouncementViewModel viewModel)
        {
            var result = false;

            if(ModelState.IsValid)
            {
                result = announcementService.UpdateAnnouncement(viewModel.Announcement);
            }

            var message = result ? Resources.SingleAnnouncementPublishedMessage : Resources.PublishAnnouncementError;

            return Json(new
            {
                success = result,
                message
            });
        }

        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.Announcement, Policy = Constant.Policy.Host)]
        public ActionResult Publish(int[] ids)
        {
            var result = announcementService.PublishAnnouncements(ids);

            var message = result
                ? (ids.Length > 1
                    ? Resources.MultipleAnnouncementPublishedMessage
                    : Resources.SingleAnnouncementPublishedMessage)
                : Resources.PublishAnnouncementError;

            return Json(new
            {
                success = result,
                message
            });
        }

        [NapAuth(FunctionKey = FunctionKeys.Announcement, Policy = Constant.Policy.Host)]
        public ActionResult QuickPublish(int[] ids)
        {
            return PartialView("_QuickPublish", ids);
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
            var result = false;

            if (ModelState.IsValid)
            {
                result = announcementService.DeleteAnnouncement(viewModel.Id);
            }

            var message = result ? Resources.SingleAnnouncementDeletedMessage : Resources.DeleteAnnouncementError;

            return Json(new
            {
                success = result,
                message
            });
        }
    }
}