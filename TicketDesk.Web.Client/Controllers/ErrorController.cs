namespace TicketDesk.Web.Client.Controllers
{
    using System.Web.Mvc;

    public class ErrorController : Controller
    {
        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult InternalError()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }
    }
}