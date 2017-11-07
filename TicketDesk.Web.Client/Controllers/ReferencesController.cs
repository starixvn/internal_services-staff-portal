namespace TicketDesk.Web.Client.Controllers
{
    using Domain.Infrastructure;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using System.Web.Mvc;

    [RoutePrefix("References")]
    public class ReferencesController : BaseController
    {
        // GET: References
        public ActionResult Index()
        {
            var items = GetIndexSubMenuItems();

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