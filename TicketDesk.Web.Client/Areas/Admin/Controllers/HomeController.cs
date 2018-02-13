namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Client.Controllers;
    using Client.Models;
    using Client.Models.Extensions;
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
            var user = UserService.GetById(CurrentSession.UserId);

            var model = new NavbarViewModel
            {
                MenuItems = PopulateMenus(CurrentSession.MenuItemList),
                IsAuthenticated = CurrentSession.UserId > 0,
                Username = user.Username,
                FullName = $"{user.FirstName} {user.LastName}".Trim(' '),
                IsSidebarExpanded = NapUserSetting.SettingObject.IsSidebarExpanded
            };

            return PartialView("~/Areas/Admin/Views/Shared/_AdminNavbar.cshtml", model);
        }

        public ActionResult Index()
        {
            return View();
        }

        private static List<SspMenuItem> PopulateMenus(IEnumerable<MenuItem> menus)
        {
            var result = new List<SspMenuItem>();

            if (menus == null)
            {
                return result;
            }

            // Filter Administration menus only
            var test = menus
                .Where(x => x.Function.Name == FunctionKeys.Administration)
                .Select(x => (SspMenuItem)x);

            // Add Admin menu
            var adminMenu = test.First(x => x.Parent == null);

            foreach (var menu in test)
            {
                if (menu.Parent?.Id == adminMenu.Id)
                {
                    menu.Children = PopulateChildList(menu, test);
                    result.Add(menu);
                }
            }

            return result;

            ////return menus.Where(x => x.Parent != null && x.Function.Name == FunctionKeys.Administration)
            ////    .Select(x => (SspMenuItem)x)
            ////    .OrderBy(x => x.DisplayOrder)
            ////    .ToList();
        }

        private static List<SspMenuItem> PopulateChildList(SspMenuItem sspMenuItem, IEnumerable<SspMenuItem> menus)
        {
            return menus.Where(x => x.Parent != null && x.Parent.Id == sspMenuItem.Id).ToList();
        }
    }
}