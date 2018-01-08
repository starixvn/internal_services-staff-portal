namespace TicketDesk.Web.Client.Controllers
{
    using System.Web.Mvc;
    using Domain.Infrastructure;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    // TODO: move to home controller to highlight dashboard top menu
    [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
    public class AnnouncementController : Controller
    {
        // GET: Announcement
        public ActionResult Index()
        {
            return View();
        }
    }
}