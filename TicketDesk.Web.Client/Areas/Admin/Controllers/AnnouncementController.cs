namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Client.Controllers;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Models.Announcements;
    using Service.Interface;

    public class AnnouncementController : BaseController
    {
        private readonly IAnnouncementService _announcementService;

        public AnnouncementController(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
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
                Announcements = _announcementService.GetAnnouncements(viewModel.GetQueryCriteria())
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
            // Check datetime
            if (viewModel.Announcement.PublishedDateTime != null)
            {
                var tolerateTime = TimeSpan.FromMinutes(5);

                // Too far in the past or more than 1 year in the future
                if ((DateTime.Now - viewModel.Announcement.PublishedDateTime) > tolerateTime
                    || (viewModel.Announcement.PublishedDateTime - DateTime.Now) > TimeSpan.FromDays(365))
                {
                    ModelState.AddModelError("Announcement.PublishedDateTime", "Date not in range");
                }

                // Published in the past
                // TOLERATE TIME: 5 MINUTES
                if (Math.Abs((DateTime.Now - viewModel.Announcement.PublishedDateTime).Value.TotalMinutes) <=
                    tolerateTime.Minutes && !viewModel.IsPublishedImmediately)
                {
                    viewModel.IsPublishedImmediately = true;
                    // Show confirm modal: Publish announcement immediately
                    return Json(new
                    {
                        success = true,
                        html = RenderRazorViewToString("_Publish", viewModel)
                    });
                }
            }

            if (!ModelState.IsValid)
            {
                return PartialView("_Create", viewModel);
            }

            _announcementService.AddAnnouncement(viewModel.Announcement);

            if (viewModel.IsPublishedImmediately)
            {
                return Json(new
                {
                    success = true,
                    message = "The announcement has been successfully published."
                });
            }

            return Json(new
            {
                success = true,
                message = "The announcement has been successfully created."
            });
        }

        [NapAuth(FunctionKey = FunctionKeys.Announcement, Policy = Constant.Policy.Host)]
        public ActionResult Edit(int id)
        {
            var viewModel = new NewAnnouncementViewModel
            {
                Announcement = _announcementService.GetAnnouncement(id)
            };

            return PartialView("_Edit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NapAuth(FunctionKey = FunctionKeys.Announcement, Policy = Constant.Policy.Host)]
        public ActionResult Edit(NewAnnouncementViewModel viewModel)
        {
            // Check datetime
            if (viewModel.Announcement.PublishedDateTime != null)
            {
                var tolerateTime = TimeSpan.FromMinutes(5);

                // Too far in the past or more than 1 year in the future
                if ((DateTime.Now - viewModel.Announcement.PublishedDateTime) > tolerateTime
                    || (viewModel.Announcement.PublishedDateTime - DateTime.Now) > TimeSpan.FromDays(365))
                {
                    ModelState.AddModelError("Announcement.PublishedDateTime", "Date not in range");
                }

                // Published in the past
                // TOLERATE TIME: 5 MINUTES
                if (Math.Abs((DateTime.Now - viewModel.Announcement.PublishedDateTime).Value.TotalMinutes) <=
                    tolerateTime.Minutes && !viewModel.IsPublishedImmediately)
                {
                    viewModel.IsPublishedImmediately = true;
                    // Show confirm modal: Publish announcement immediately
                    return Json(new
                    {
                        success = true,
                        html = RenderRazorViewToString("_Publish", viewModel)
                    });
                }
            }

            if (!ModelState.IsValid)
            {
                return PartialView("_Edit", viewModel);
            }

            _announcementService.UpdateAnnouncement(viewModel.Announcement);

            if (viewModel.IsPublishedImmediately)
            {
                return Json(new
                {
                    success = true,
                    message = "The announcement has been successfully published."
                });
            }

            return Json(new
            {
                success = true,
                message = "The announcement has been successfully edited."
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
            var result = _announcementService.DeleteAnnouncement(viewModel.Id);

            return Json(new
            {
                success = result,
                message = "The announcement has been successfully deleted"
            });
        }

        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.Announcement, Policy = Constant.Policy.Host)]
        public ActionResult Publish(List<int> Ids)
        {
            _announcementService.PublishAnnouncements(Ids);

            var message = "The announcement has been successfully published.";
            if (Ids.Count > 1)
            {
                message = "The announcements have been successfully published.";
            }

            return Json(new
            {
                success = true,
                message
            });
        }
    }
}