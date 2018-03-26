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

    public class HomeController : BaseController
    {
        private readonly IAnnouncementService announcementService;
        private readonly ITicketService ticketService;

        public HomeController(IAnnouncementService announcementService, ITicketService ticketService)
        {
            this.announcementService = announcementService;
            this.ticketService = ticketService;
        }

        [NapAuth(FunctionKey = FunctionKeys.ListAssignedTicket, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            var homeViewModel = new HomeViewModel
            {
                UnreadAnnouncementQuatity = announcementService.CountUnreadAnnouncement(NapUserSetting.SettingObject.ReadAnnouncements)
            };

            return View(homeViewModel);
        }

        public ActionResult MyOpeningTicket(MyOpenningTicketsViewModel viewModel)
        {
            var myOpenningIssues = viewModel ?? new MyOpenningTicketsViewModel();

            ////myOpenningIssues.TicketGroups = ticketService.GetUserOpeningTicketsInGroups(
            ////    CurrentSession.UserId,
            ////    myOpenningIssues.Skipped,
            ////    myOpenningIssues.ItemPerPage);

            myOpenningIssues.TicketGroups = ticketService.GetUserOpeningTicketsInGroups();

            ////myOpenningIssues.TotalItem = ticketService.GetUserTotalOpeningTickets(CurrentSession.UserId);

            return PartialView("_MyOpeningTicket", myOpenningIssues);
        }

        public ActionResult MoreAnnouncement(MoreAnnouncementViewModel viewModel)
        {
            var moreannouncementViewModel = viewModel ?? new MoreAnnouncementViewModel();

            var announcements = announcementService.GetAnnouncements(moreannouncementViewModel.GetAnnouncementsCriteria());
            announcements = announcementService.CalculateAnnouncementsReadStatus(
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

        public ActionResult GetListAnnouncementByReadState(bool isUnreadOnly)
        {
            var announcements = announcementService.GetHomeAnnouncements();
            announcements = announcementService.CalculateAnnouncementsReadStatus(
                    announcements,
                    NapUserSetting.SettingObject.ReadAnnouncements);

            announcements = isUnreadOnly ? announcements.Where(x => !x.IsRead).ToList() : announcements.Take(10).ToList();
            return PartialView("_AnnouncementListHomePage", announcements);
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
    }
}