namespace TicketDesk.Web.Client.Controllers
{
    using System.Web.Mvc;

    public class ErrorController : Controller
    {
        ////[Route("access-denied", Name = "AccessDenied")]
        public ActionResult AccessDenied()
        {
            return View();
        }

        ////[Route("internal-error", Name = "InternalError")]
        public ActionResult InternalError()
        {
            return View();
        }

        ////[Route("not-found", Name = "NotFound")]
        public ActionResult NotFound()
        {
            return View();
        }
    }
}