namespace TicketDesk.Web.Client.Controllers
{
    using System.Web.Mvc;
    using Core.Enumerations;
    using Domain.Infrastructure;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Service.Interface;

    [NapAuth(FunctionKey = FunctionKeys.Reference, Policy = Constant.Policy.Read)]
    public class ReferenceController : BaseController
    {
        private readonly IReferenceService referenceService;

        public ReferenceController(IReferenceService referenceService)
        {
            this.referenceService = referenceService;
        }

        public ActionResult Index()
        {
            var items = GetIndexSubMenuItems();

            return View("_SubmenuNavigation", items);
        }

        public ActionResult BetOfTheDay()
        {
            var reference = referenceService.GetRelevantReference(ReferenceType.BetOfTheDay);
            return View(reference);
        }

        public ActionResult FeaturedEvents()
        {
            var reference = referenceService.GetRelevantReference(ReferenceType.FeaturedEvent);

            return View(reference);
        }

        public ActionResult Newsletters()
        {
            var references = referenceService.GetListReference(ReferenceType.Newsletters);
            return View(references);
        }

        public ActionResult TradingLimits()
        {
            var reference = referenceService.GetRelevantReference(ReferenceType.TradingLimits);

            return View(reference);
        }

        public ActionResult TvSchedules()
        {
            var reference = referenceService.GetRelevantReference(ReferenceType.TvSchedule);
            return View(reference);
        }
    }
}