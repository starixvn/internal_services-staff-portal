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
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public PartialViewResult Menu()
        {
            var user = UserService.GetById(CurrentSession.UserId);

            var model = new NavbarViewModel
            {
                MenuItems = GetMenus(CurrentSession.MenuItemList),
                IsAuthenticated = CurrentSession.UserId > 0,
                Username = user.Username,
                FullName = $"{user.FirstName} {user.LastName}".Trim(' '),
                IsSidebarExpanded = NapUserSetting.SettingObject.IsSidebarExpanded
            };

            return PartialView("~/Areas/Admin/Views/Shared/_AdminNavbar.cshtml", model);
        }

        private static List<SspMenuItem> GetChildMenuList(SspMenuItem sspMenuItem, IEnumerable<MenuItem> menus)
        {
            return menus.Where(x => x.Parent != null && x.Parent.Id == sspMenuItem.Id)
                .OrderBy(x => x.DisplayOrder)
                .Select(x => (SspMenuItem)x)
                .ToList();
        }

        private static List<SspMenuItem> GetMenus(IEnumerable<MenuItem> menus)
        {
            var result = new List<SspMenuItem>();

            if (menus != null)
            {
                // Level 0 - Get Admin Menu
                var adminMenu = menus.First(x => x.Function.Name == FunctionKeys.Administration);

                // Level 1 - Get Admin child menus
                var adminChildMenus = menus.Where(x => x.Parent != null && x.Parent.Id == adminMenu.Id).Select(x => (SspMenuItem)x);

                // Level 2 - Get child of admin child menu
                foreach (SspMenuItem menu in adminChildMenus)
                {
                    menu.Children = GetChildMenuList(menu, menus);
                    result.Add(menu);
                }

                result = result.OrderBy(x => x.DisplayOrder).ToList();
            }

            return result;
        }
    }
}