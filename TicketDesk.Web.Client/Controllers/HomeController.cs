namespace TicketDesk.Web.Client.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
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
            var announcements = _announcementService.GetHomeAnnouncements();
            announcements = _announcementService.CalculateAnnouncementsReadStatus(
                    announcements,
                    NapUserSetting.SettingObject.ReadAnnouncements);

            var homeViewModel = new HomeViewModel
            {
                Announcements = announcements,
                UnreadAnnouncementQuatity = _announcementService.CountUnreadAnnouncement(NapUserSetting.SettingObject.ReadAnnouncements)
            };

            return View(homeViewModel);
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

            var announcements = _announcementService.GetAnnouncements(moreannouncementViewModel.GetAnnouncementsCriteria());
            announcements = _announcementService.CalculateAnnouncementsReadStatus(
                announcements,
                NapUserSetting.SettingObject.ReadAnnouncements);

            moreannouncementViewModel.TotalItem = announcements.Count();

            var paginationService = new PaginationService<Announcement, DateTime?>();

            moreannouncementViewModel.Announcements = paginationService.Pagination(
                announcements,
                x => x.PublishedDateTime,
                moreannouncementViewModel.Skipped,
                moreannouncementViewModel.ItemPerPage).ToList();

            return View(moreannouncementViewModel);
        }
    }
}