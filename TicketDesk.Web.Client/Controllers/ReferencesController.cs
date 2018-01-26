namespace TicketDesk.Web.Client.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Enumerations;
    using Domain;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    [RoutePrefix("References")]
    public class ReferencesController : BaseController
    {
        public ReferencesController(TdDomainContext context)
        {
            Context = context;
        }

        private TdDomainContext Context { get; set; }

        public ActionResult Index()
        {
            var items = GetIndexSubMenuItems();

            return View("_SubmenuNavigation", items);
        }

        [NapAuth(FunctionKey = FunctionKeys.FileManagement, Policy = Constant.Policy.Read)]
        [Route("bet-of-the-day")]
        public ActionResult BetOfTheDay()
        {
            var reference = GetReferenceSuitable(ReferenceType.BetOfTheDay);
            return View(reference);
        }

        [NapAuth(FunctionKey = FunctionKeys.FileManagement, Policy = Constant.Policy.Read)]
        [Route("featured-event")]
        public ActionResult FeaturedEvent()
        {
            var reference = GetReferenceSuitable(ReferenceType.FeaturedEvent);

            return View(reference);
        }

        [NapAuth(FunctionKey = FunctionKeys.FileManagement, Policy = Constant.Policy.Read)]
        [Route("newsletter")]
        public ActionResult Newsletter()
        {
            var references = Context.References.Where(r => r.ReferenceType == ReferenceType.NewsLetter).ToList();
            return View(references);
        }

        [NapAuth(FunctionKey = FunctionKeys.FileManagement, Policy = Constant.Policy.Read)]
        [Route("trading-limit")]
        public ActionResult TradingLimit()
        {
            var reference = GetReferenceSuitable(ReferenceType.TradingLimits);

            return View(reference);
        }

        [NapAuth(FunctionKey = FunctionKeys.FileManagement, Policy = Constant.Policy.Read)]
        [Route("tv-schedules")]
        public ActionResult TvSchedules()
        {
            var reference = GetReferenceSuitable(ReferenceType.TvSchedule);
            return View(reference);
        }

        private Reference GetReferenceSuitable(ReferenceType referenceType)
        {
            var references = Context.References.Where(t => t.ReferenceType == referenceType)
                                               .OrderByDescending(r => r.DateAppear)
                                               .ToList();
            var reference = references.FirstOrDefault(r => DateTime.Compare(r.DateAppear, DateTime.Now) <= 0);
            if (reference == null)
            {
                reference = new Reference();
            }
            return reference;
        }
    }
}