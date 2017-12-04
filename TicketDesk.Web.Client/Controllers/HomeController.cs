namespace TicketDesk.Web.Client.Controllers
{
    using Domain;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Model;
    using Fanex.NAP.Core.Web.Attribute;
    using Models;
    using Models.Home;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Web.Mvc;

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
        public ActionResult Index(int page = 1, int itemPerPage = 20)
        {
            HomeViewModel homeViewModel = new HomeViewModel
            {
                MyOpenningIssues = new MyOpenningIssues
                {
                    TicketGroups = new List<TicketGroup>(),
                    CurrentPage = page,
                    ItemPerPage = itemPerPage,
                    Action = "Index",
                    Controller = "Home"
                }
            };


            foreach (SelectListItem item in homeViewModel.MyOpenningIssues.ItemPerPageList)
            {
                if (itemPerPage.ToString() == item.Value)
                {
                    item.Selected = true;
                }
            }

            // Get ticket categories
            var categories = Context.TicketCategories.ToList();

            var databasePage = page < 1 ? 0 : page - 1;

            int skippedItem = databasePage * itemPerPage;

            if (skippedItem < 0)
            {
                skippedItem = 0;
            }

            homeViewModel.MyOpenningIssues.TotalItem = Context.Tickets.Count(x => x.CreatedBy == CurrentSession.UserId.ToString());
            
            // Get current user tickets
            var tickets = Context.Tickets
                .Where(x => (x.TicketStatus == TicketStatus.Active || x.TicketStatus == TicketStatus.MoreInfo) &&
                            x.CreatedBy == CurrentSession.UserId.ToString())
                .OrderByDescending(x => x.CategoryId)
                .ThenByDescending(x => x.TicketId)
                .Skip(skippedItem)
                .Take(itemPerPage)
                .GroupBy(x => x.CategoryId).ToList();

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

            return View(homeViewModel);
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