namespace TicketDesk.Web.Client.Controllers
{
    using System.Web.Mvc;
    using Domain.Infrastructure;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    [NapAuth(FunctionKey = FunctionKeys.Announcement, Policy = Constant.Policy.Host)]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Announcement()
        {
            return View();
        }
    }
}