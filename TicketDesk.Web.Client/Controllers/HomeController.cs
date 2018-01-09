namespace TicketDesk.Web.Client.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Web.Mvc;
    using Criteria.Announcement;
    using Domain;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Model;
    using Fanex.NAP.Core.Web.Attribute;
    using Models;
    using Models.Home;
    using Service.Implement;
    using Service.Interface;

    ////[RoutePrefix("")]
    ////[Route("{action=index}")]
    public class HomeController : BaseController
    {
        public HomeController(TdDomainContext context, IAnnouncementService announcementService)
        {
            Context = context;
            _announcementService = announcementService;
        }

        private TdDomainContext Context { get; }

        private readonly IAnnouncementService _announcementService;

        ////[Route("about")]
        public ActionResult About()
        {
            return View();
        }

        ////[Route("")]
        ////[Route("index")]
        [NapAuth(FunctionKey = FunctionKeys.ListAssignedTicket, Policy = Constant.Policy.Read)]
        public ActionResult Index(HomeViewModel viewModel)
        {
            var homeViewModel = viewModel;

            if (homeViewModel?.MyOpenningIssues == null)
            {
                homeViewModel = new HomeViewModel
                {
                    MyOpenningIssues = new MyOpenningIssues(),
                    AnnouncementGroups = new List<AnnouncementGroup>()
                };
            }

            homeViewModel.MyOpenningIssues.TotalItem =
                Context.Tickets.Count(x => x.CreatedBy == CurrentSession.UserId.ToString());

            PopulateHomeTickets(homeViewModel.MyOpenningIssues);
            homeViewModel.AnnouncementGroups = PopulateHomeAnnouncements();

            return View(homeViewModel);
        }

        private void PopulateHomeTickets(MyOpenningIssues myOpenningIssues)
        {
            // Get ticket categories
            var categories = Context.TicketCategories.ToList();

            // Get current user tickets
            var tickets = Context.Tickets
                .Where(x =>
                    (x.TicketStatus == TicketStatus.Active || x.TicketStatus == TicketStatus.MoreInfo)
                    && x.CreatedBy == CurrentSession.UserId.ToString())
                .OrderBy(x => x.CategoryId)
                .ThenByDescending(x => x.TicketId)
                .Skip(myOpenningIssues.Skipped)
                .Take(myOpenningIssues.ItemPerPage)
                .GroupBy(x => x.CategoryId).ToList();

            myOpenningIssues.TicketGroups = PrepareTicketGroups(categories, tickets);
        }

        private IList<AnnouncementGroup> PopulateHomeAnnouncements()
        {
            // ANNOUNCEMENTS

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

                var announcementGroup = new AnnouncementGroup
                {
                    Name = group.Key.Value.ToString("d"),
                    Announcements = group
                };

                result.Add(announcementGroup);
            }

            return result;
        }

        private IList<TicketGroup> PrepareTicketGroups(IList<TicketCategory> categories, IEnumerable<IGrouping<int, Ticket>> tickets)
        {
            var result = new List<TicketGroup>();
            foreach (var grouping in tickets)
            {
                var languageSuffix = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

                var categoryName = languageSuffix == "en"
                    ? categories
                        .Where(x => x.CategoryId == grouping.Key)
                        .Select(x => x.Name)
                        .FirstOrDefault()
                    : categories
                        .Where(x => x.CategoryId == grouping.Key)
                        .Select(x => x.Name_vi)
                        .FirstOrDefault();

                var total = Context.Tickets.Count(x =>
                    (x.TicketStatus == TicketStatus.Active || x.TicketStatus == TicketStatus.MoreInfo)
                    && x.CreatedBy == CurrentSession.UserId.ToString()
                    && x.CategoryId == grouping.Key);

                var ticketGroup = new TicketGroup
                {
                    Name = categoryName,
                    Tickets = grouping,
                    TotalItems = total
                };

                result.Add(ticketGroup);
            }

            return result;
        }

        ////[Route("menu")]
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

        private static List<Models.Extensions.MenuItem> PopulateMenus(IEnumerable<MenuItem> menus)
        {
            var result = new List<Models.Extensions.MenuItem>();

            if (menus == null)
            {
                return result;
            }

            // Get parent menu item
            foreach (var rawItem in menus)
            {
                if (rawItem.Parent == null)
                {
                    result.Add(rawItem);
                }
            }

            // Get children menu item
            foreach (var menuItem in result)
            {
                menuItem.Children = menus
                    .Where(x => x.Parent != null && x.Parent.Id == menuItem.Id)
                    .Select(x => (Models.Extensions.MenuItem)x)
                    .OrderBy(x => x.DisplayOrder);
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

            return View(moreannouncementViewModel);
        }
    }
}