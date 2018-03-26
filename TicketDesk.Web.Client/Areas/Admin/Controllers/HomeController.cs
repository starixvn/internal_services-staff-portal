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
                MenuItems = PopulateMenus(CurrentSession.MenuItemList),
                IsAuthenticated = CurrentSession.UserId > 0,
                Username = user.Username,
                FullName = $"{user.FirstName} {user.LastName}".Trim(' '),
                IsSidebarExpanded = NapUserSetting.SettingObject.IsSidebarExpanded
            };

            return PartialView("~/Areas/Admin/Views/Shared/_AdminNavbar.cshtml", model);
        }

        private static List<SspMenuItem> GetChildMenuList(SspMenuItem sspMenuItem, IEnumerable<SspMenuItem> menus)
        {
            return menus.Where(x => x.Parent != null && x.Parent.Id == sspMenuItem.Id).OrderBy(x => x.DisplayOrder).ToList();
        }

        private static List<SspMenuItem> PopulateMenus(IEnumerable<MenuItem> menus)
        {
            var result = new List<SspMenuItem>();

            if (menus != null)
            {
                // Filter Administration menus only
                var adminMenus = menus
                    .Where(x => x.Function.Name == FunctionKeys.Administration)
                    .Select(x => (SspMenuItem)x)
                    .ToList();

                // Add Admin menu
                var adminMenu = adminMenus.First(x => x.Parent == null);

                foreach (var menu in adminMenus)
                {
                    if (menu.Parent?.Id == adminMenu.Id)
                    {
                        menu.Children = GetChildMenuList(menu, adminMenus);
                        result.Add(menu);
                    }
                }

                result = result.OrderBy(x => x.DisplayOrder).ToList();
            }

            return result;
        }
    }
}