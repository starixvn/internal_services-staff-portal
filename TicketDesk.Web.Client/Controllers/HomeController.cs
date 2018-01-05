namespace TicketDesk.Web.Client.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Web.Mvc;
    using Domain;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Model;
    using Fanex.NAP.Core.Web.Attribute;
    using Models;
    using Models.Home;

    [RoutePrefix("")]
    [Route("{action=index}")]
    public class HomeController : BaseController
    {
        public HomeController(TdDomainContext context)
        {
            Context = context;
        }

        private TdDomainContext Context { get; }

        [Route("about")]
        public ActionResult About()
        {
            return View();
        }

        [Route("access-denied", Name = "AccessDenied")]
        public ActionResult AccessDenied()
        {
            return View();
        }

        [Route("")]
        [Route("index")]
        [NapAuth(FunctionKey = FunctionKeys.ListAssignedTicket, Policy = Constant.Policy.Read)]
        public ActionResult Index(HomeViewModel viewModel)
        {
            var homeViewModel = viewModel;

            if (homeViewModel?.MyOpenningIssues == null)
            {
                homeViewModel = new HomeViewModel
                {
                    MyOpenningIssues = new MyOpenningIssues()
                };
            }

            homeViewModel.MyOpenningIssues.TotalItem = Context.Tickets.Count(x => x.CreatedBy == CurrentSession.UserId.ToString());

            // Get ticket categories
            var categories = Context.TicketCategories.ToList();

            // Get current user tickets
            var tickets = Context.Tickets
                .Where(x => (x.TicketStatus == TicketStatus.Active || x.TicketStatus == TicketStatus.MoreInfo) &&
                            x.CreatedBy == CurrentSession.UserId.ToString())
                .OrderBy(x => x.CategoryId)
                .ThenByDescending(x => x.TicketId)
                .Skip(homeViewModel.MyOpenningIssues.Skipped)
                .Take(homeViewModel.MyOpenningIssues.ItemPerPage)
                .GroupBy(x => x.CategoryId).ToList();

            PrepareTicketGroups(homeViewModel, categories, tickets);

            return View(homeViewModel);
        }

        private void PrepareTicketGroups(HomeViewModel homeViewModel, List<TicketCategory> categories, List<IGrouping<int, Ticket>> tickets)
        {
            foreach (IGrouping<int, Ticket> grouping in tickets)
            {
                string languageSuffix = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

                var categoryName = languageSuffix == "en"
                    ? categories
                        .Where(x => x.CategoryId == grouping.Key)
                        .Select(x => x.Name)
                        .FirstOrDefault()
                    : categories
                        .Where(x => x.CategoryId == grouping.Key)
                        .Select(x => x.Name_vi)
                        .FirstOrDefault();

                var total = Context.Tickets.Count(x => (x.TicketStatus == TicketStatus.Active || x.TicketStatus == TicketStatus.MoreInfo) &&
                    x.CreatedBy == CurrentSession.UserId.ToString() &&
                    x.CategoryId == grouping.Key);

                TicketGroup ticketGroup = new TicketGroup
                {
                    Name = categoryName,
                    Tickets = grouping,
                    TotalTickets = total
                };

                homeViewModel.MyOpenningIssues.TicketGroups.Add(ticketGroup);
            }
        }

        [Route("internal-error", Name = "InternalError")]
        public ActionResult InternalError()
        {
            return View();
        }

        [Route("menu")]
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

        [Route("not-found", Name = "NotFound")]
        public ActionResult NotFound()
        {
            return View();
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
    }
}