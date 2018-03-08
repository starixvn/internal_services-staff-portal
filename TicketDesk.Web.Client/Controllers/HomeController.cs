namespace TicketDesk.Web.Client.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Enumerations;
    using Criteria.Announcement;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Model;
    using Fanex.NAP.Core.Web.Attribute;
    using Models;
    using Models.Extensions;
    using Models.Home;
    using Service.Implement;
    using Service.Interface;
    using Service.Interface.Admin;

    public class HomeController : BaseController
    {
        private readonly IAnnouncementService _announcementService;
        private readonly ITicketService _ticketService;

        public HomeController(IAnnouncementService announcementService, ITicketService ticketService)
        {
            _announcementService = announcementService;
            _ticketService = ticketService;
        }

        public ActionResult MyOpeningTicket(MyOpenningTicketsViewModel viewModel)
        {
            var myOpenningIssues = viewModel ?? new MyOpenningTicketsViewModel();

            myOpenningIssues.TicketGroups = _ticketService.GetUserOpeningTicketsInGroups(
                CurrentSession.UserId,
                myOpenningIssues.Skipped,
                myOpenningIssues.ItemPerPage);

            myOpenningIssues.TotalItem = _ticketService.GetUserTotalOpeningTickets(CurrentSession.UserId);

            return PartialView("_MyOpeningTicket", myOpenningIssues);
        }

        [NapAuth(FunctionKey = FunctionKeys.ListAssignedTicket, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            ////if (homeViewModel?.Annoucements == null)
            ////{
            ////    homeViewModel = new HomeViewModel
            ////    {
            ////        Annoucements = new AnnouncementViewModel()
            ////    };
            ////}

            var homeViewModel = PopulateHomeAnnouncements();

            return View(homeViewModel);
        }

        private HomeViewModel PopulateHomeAnnouncements()
        {
            var result = new List<AnnouncementGroup>();

            // Get top 10 announcement by published time
            var criteria = new GetAnnouncementsCriteria
            {
                State = AnnouncementState.Published
            };

            var availableAnnouncements = _announcementService.GetAnnouncements(criteria);

            var paginationService = new PaginationService<Announcement, DateTime?>();
            var announcements = paginationService.Pagination(availableAnnouncements, x => x.PublishedDateTime, 0, 10)
                .GroupBy(x => x.PublishedDateTime?.Date)
                .OrderByDescending(x => x.Key);
            foreach (var group in announcements)
            {
                if (group.Key == null)
                {
                    continue;
                }

                foreach (var announcement in group)
                {
                    SetIsReadAnnouncement(announcement);
                }

                var announcementGroup = new AnnouncementGroup
                {
                    Name = group.Key.Value.ToString("d"),
                    Announcements = group
                };

                result.Add(announcementGroup);
            }

            var homeViewModel = new HomeViewModel();
            homeViewModel.UnreadAnnouncementQuatity = GetUnreadAnnouncementQuantity();
            homeViewModel.AnnouncementGroups = result;

            ////var anouncementViewModel = new AnnouncementViewModel
            ////{
            ////    UnreadAnnouncementQuatity = GetUnreadAnnouncementQuantity(),
            ////    AnnouncementGroups = result
            ////};

            return homeViewModel;
        }

        private int GetUnreadAnnouncementQuantity()
        {
            var allAnnouncementIds = _announcementService.GetAllAnnouncementIds();

            int total = allAnnouncementIds.Length;
            foreach (int id in allAnnouncementIds)
            {
                if (NapUserSetting.SettingObject.ReadAnnouncements != null)
                {
                    var readAnnouncements = NapUserSetting.SettingObject.ReadAnnouncements.Split(',');
                    if (Array.Exists(readAnnouncements, e => e == id.ToString()))
                    {
                        total--;
                    }
                }
            }
            return total;
        }

        private void SetIsReadAnnouncement(Announcement announcement)
        {
            if (NapUserSetting.SettingObject.ReadAnnouncements != null)
            {
                var readAnnouncements = NapUserSetting.SettingObject.ReadAnnouncements.Split(',');
                if (Array.Exists(readAnnouncements, e => e == announcement.Id.ToString()))
                {
                    announcement.IsRead = true;
                }
            }
        }

        [ChildActionOnly]
        public PartialViewResult Menu()
        {
            var model = new NavbarViewModel
            {
                MenuItems = PopulateMenus(CurrentSession.MenuItemList),
                Username = CurrentSession.Username,
                IsAuthenticated = CurrentSession.UserId > 0
            };

            return PartialView("~/Views/Shared/_Navbar.cshtml", model);
        }

        private static List<SspMenuItem> PopulateMenus(IEnumerable<MenuItem> menus)
        {
            var result = new List<SspMenuItem>();

            if (menus == null)
            {
                return result;
            }

            // Get parent menu item
            foreach (var rawItem in menus)
            {
                if (rawItem.Parent == null && rawItem.Function.Name != FunctionKeys.Administration)
                {
                    result.Add(rawItem);
                }
            }

            // Get children menu item
            foreach (var menuItem in result)
            {
                menuItem.Children = menus
                    .Where(x => x.Parent != null && x.Parent.Id == menuItem.Id)
                    .Select(x => (SspMenuItem)x)
                    .OrderBy(x => x.DisplayOrder).ToList();
            }

            return result.OrderBy(x => x.DisplayOrder).ToList();
        }

        public ActionResult MoreAnnouncement(MoreAnnouncementViewModel viewModel)
        {
            var moreannouncementViewModel = viewModel ?? new MoreAnnouncementViewModel();

            var announcements =
                _announcementService.GetAnnouncements(moreannouncementViewModel.GetAnnouncementsCriteria());

            moreannouncementViewModel.TotalItem = announcements.Count();

            var paginationService = new PaginationService<Announcement, int>();
            moreannouncementViewModel.Announcements = paginationService.Pagination(announcements, x => x.Id,
                moreannouncementViewModel.Skipped, moreannouncementViewModel.ItemPerPage).ToList();

            foreach (Announcement annoucement in moreannouncementViewModel.Announcements)
            {
                SetIsReadAnnouncement(annoucement);
            }

            return View(moreannouncementViewModel);
        }
    }
}