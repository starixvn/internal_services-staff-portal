namespace TicketDesk.Web.Client.Controllers
{
    using Domain;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Model;
    using Fanex.NAP.Core.Web.Attribute;
    using Localization;
    using Models;
    using Models.Home;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
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
        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult Index(int page = 1, int itemPerPage = 20)
        {
            HomeViewModel homeViewModel = new HomeViewModel
            {
                TicketGroups = new List<TicketGroup>(),
                CurrentPage = page,
                ItemPerPage = itemPerPage
            };

            foreach (SelectListItem item in homeViewModel.ItemPerPageList)
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

            var count = Context.Tickets.Count(x => x.CreatedBy == CurrentSession.UserId.ToString());

            homeViewModel.TotalPage = count % itemPerPage > 0
                ? (count / itemPerPage) + 1
                : count / itemPerPage;

            // Get current user tickets
            var tickets = Context.Tickets
                .Where(x => (x.TicketStatus == TicketStatus.Active || x.TicketStatus == TicketStatus.MoreInfo) &&
                            x.CreatedBy == CurrentSession.UserId.ToString())
                .OrderByDescending(x => x.TicketId)
                .Skip(skippedItem)
                .Take(itemPerPage)
                .GroupBy(x => x.CategoryId).ToList();
            
            foreach (IGrouping<int, Ticket> grouping in tickets)
            {
                var categoryName = categories
                    .Where(x => x.CategoryId == grouping.Key)
                    .Select(x => x.Name)
                    .FirstOrDefault();

                TicketGroup ticketGroup = new TicketGroup
                {
                    Name = categoryName,
                    Tickets = grouping
                };

                homeViewModel.TicketGroups.Add(ticketGroup);
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

        [Route("language")]
        public ActionResult SetLanguage(string name)
        {
            CultureHelper.SetCurrentCulture(name);

            var cookie = new HttpCookie("_culture", name) { Expires = DateTime.Today.AddYears(1) };
            Response.SetCookie(cookie);

            if (Request.UrlReferrer != null && !string.IsNullOrEmpty(Request.UrlReferrer.AbsoluteUri))
            {
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            }

            return RedirectToAction("Index", "Home");
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