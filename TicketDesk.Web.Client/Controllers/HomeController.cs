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
        private TdDomainContext Context { get; }

        public HomeController(TdDomainContext context)
        {
            Context = context;
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

        [Route("")]
        [Route("index")]
        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            HomeViewModel homeViewModel = new HomeViewModel { TicketGroups = new List<TicketGroup>() };

            // Get current user tickets
            var tickets = Context.Tickets
                .Where(x => (x.TicketStatus == TicketStatus.Active || x.TicketStatus == TicketStatus.MoreInfo))
                .GroupBy(x => x.CategoryId).ToList();

            // Get ticket categories
            var categories = Context.TicketCategories.ToList();

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
                menuItem.Children = menus.Where(x => x.Parent != null && x.Parent.Id == menuItem.Id)
                    .Select(x => (Models.Extensions.MenuItem)x);
            }

            return result.OrderBy(x => x.DisplayOrder).ToList();
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

        [Route("not-found", Name = "NotFound")]
        public ActionResult NotFound()
        {
            return View();
        }

        [Route("internal-error", Name = "InternalError")]
        public ActionResult InternalError()
        {
            return View();
        }
    }
}