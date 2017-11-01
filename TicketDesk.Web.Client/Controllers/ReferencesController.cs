namespace TicketDesk.Web.Client.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using Domain.Infrastructure;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using System.Web.Mvc;
    using Models.Extensions;

    [RoutePrefix("References")]
    public class ReferencesController : BaseController
    {
        // GET: References
        public ActionResult Index()
        {
            string routeValue = RouteData.Values.First().Value.ToString();

            // Get the correct parent menu id
            MenuItem parentMenu = CurrentSession.MenuItemList.First(x => x.Parent == null && x.Url.Contains(routeValue));
            var subMenuItems = CurrentSession.MenuItemList.Where(x => x.Parent != null && x.Parent.Id == parentMenu.Id).ToList();

            List<MenuItem> items = new List<MenuItem>();
            foreach (Fanex.NAP.Core.Model.MenuItem menuItem in subMenuItems)
            {
                items.Add(menuItem);
            }

            return View("_SubmenuNavigation", items);
        }

        [NapAuth(FunctionKey = FunctionKeys.FileManagement, Policy = Constant.Policy.Read)]
        [Route("bet-of-the-day")]
        public ActionResult BetOfTheDay()
        {
            return View();
        }

        [NapAuth(FunctionKey = FunctionKeys.FileManagement, Policy = Constant.Policy.Read)]
        [Route("featured-event")]
        public ActionResult FeaturedEvent()
        {
            return View();
        }

        [NapAuth(FunctionKey = FunctionKeys.FileManagement, Policy = Constant.Policy.Read)]
        [Route("newsletter")]
        public ActionResult Newsletter()
        {
            return View();
        }

        [NapAuth(FunctionKey = FunctionKeys.FileManagement, Policy = Constant.Policy.Read)]
        [Route("trading-limit")]
        public ActionResult TradingLimit()
        {
            return View();
        }

        [NapAuth(FunctionKey = FunctionKeys.FileManagement, Policy = Constant.Policy.Read)]
        [Route("tv-schedules")]
        public ActionResult TvSchedules()
        {
            return View();
        }
    }
}