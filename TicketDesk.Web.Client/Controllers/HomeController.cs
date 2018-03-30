namespace TicketDesk.Web.Client.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Domain.Infrastructure;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Model;
    using Fanex.NAP.Core.Web.Attribute;
    using Models;
    using Models.Extensions;
    using Models.Home;
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
            var announcements = announcementService.GetHomeAnnouncements();
            var homeViewModel = new HomeViewModel
            {
                UnreadAnnouncementQuatity = announcements.Count(x => !x.IsRead)
            };

            return View(homeViewModel);
        }

        public ActionResult MyOpeningTicket(MyOpenningTicketsViewModel viewModel)
        {
            var myOpenningIssues = viewModel ?? new MyOpenningTicketsViewModel();

            myOpenningIssues.TicketGroups = ticketService.GetUserOpeningTicketsInGroups();

            return PartialView("_MyOpeningTicket", myOpenningIssues);
        }

        public ActionResult MoreAnnouncement()
        {
            var viewModel = new MoreAnnouncementViewModel();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ListAllAnnouncement(MoreAnnouncementViewModel viewModel)
        {
            var criteria = viewModel.GetAnnouncementsCriteria();
            var announcements = announcementService.GetAnnouncements(criteria);
            return PartialView("_ListAllAnnouncements", announcements.OrderByDescending(x => x.CreatedDateTime).ToList());
        }

        public ActionResult GetListAnnouncementByReadState(bool isUnreadOnly)
        {
            var announcements = announcementService.GetHomeAnnouncements();

            announcements = isUnreadOnly ? announcements.Where(x => !x.IsRead).ToList() : announcements.Take(10).ToList();
            return PartialView("_AnnouncementListHomePage", announcements);
        }

        public ActionResult SaveAndCountTotalUnreadAnnouncement(int id)
        {
            var result = announcementService.SetAnnouncementReadStatus(id, CurrentSession.UserId);
            var unreadQuantity = announcementService.GetHomeAnnouncements().Count(x => !x.IsRead);

            return Json(new { result, unreadQuantity }, JsonRequestBehavior.AllowGet);
        }

        [ChildActionOnly]
        public PartialViewResult Menu()
        {
            var model = new NavbarViewModel
            {
                MenuItems = GetMenus(CurrentSession.MenuItemList),
                Username = CurrentSession.Username,
                IsAuthenticated = CurrentSession.UserId > 0
            };

            return PartialView("~/Views/Shared/_Navbar.cshtml", model);
        }

        private static List<SspMenuItem> GetMenus(IEnumerable<MenuItem> menus)
        {
            var result = new List<SspMenuItem>();

            if (menus != null)
            {
                // Level 0 - Get Non-Admin Menu
                foreach (var rawItem in menus)
                {
                    if (rawItem.Parent == null && rawItem.Function.Name != FunctionKeys.Administration)
                    {
                        result.Add(rawItem);
                    }
                }

                // Level 1 - Get children menu item
                foreach (var menuItem in result)
                {
                    menuItem.Children = menus
                        .Where(x => x.Parent != null && x.Parent.Id == menuItem.Id)
                        .Select(x => (SspMenuItem)x)
                        .OrderBy(x => x.DisplayOrder).ToList();
                }

                result = result.OrderBy(x => x.DisplayOrder).ToList();
            }

            return result;
        }
    }
}