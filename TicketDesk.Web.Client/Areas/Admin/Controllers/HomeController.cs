namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Client.Controllers;
    using Client.Models;
    using Domain.Infrastructure;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Model;
    using Fanex.NAP.Core.Web.Attribute;

    [NapAuth(FunctionKey = FunctionKeys.Administration, Policy = Constant.Policy.Read)]
    public class HomeController : BaseController
    {
        [ChildActionOnly]
        public PartialViewResult Menu()
        {
            var model = new NavbarViewModel
            {
                MenuItems = PopulateMenus(CurrentSession.MenuItemList),
                Username = CurrentSession.Username,
                IsAuthenticated = CurrentSession.UserId > 0
            };

            return PartialView("~/Areas/Admin/Views/Shared/_AdminNavbar.cshtml", model);
        }

        public ActionResult Index()
        {
            return View();
        }

        private static List<Client.Models.Extensions.MenuItem> PopulateMenus(IEnumerable<MenuItem> menus)
        {
            var result = new List<Client.Models.Extensions.MenuItem>();

            if (menus == null)
            {
                return result;
            }

            // Filter Administration menus only
            return menus.Where(x => x.Parent != null && x.Function.Name == FunctionKeys.Administration)
                .Select(x => (Client.Models.Extensions.MenuItem)x)
                .OrderBy(x => x.DisplayOrder)
                .ToList();
        }
    }
}