namespace TicketDesk.Web.Client.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;

    public class MenuController : BaseController
    {
        // GET: Menu
        public ActionResult _Header()
        {
            MenuItemService menuItemService = new MenuItemService();
            var menus = menuItemService.GetBySite(NapSettings.Settings.SiteId).Where(x => x.Parent == null);

            return PartialView(menus);
        }
    }
}