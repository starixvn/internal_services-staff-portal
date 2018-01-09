namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System.Web.Mvc;
    using Client.Controllers;
    using Client.Models;
    using Domain.Infrastructure;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    [NapAuth(FunctionKey = FunctionKeys.Administration, Policy = Constant.Policy.Read)]
    public class HomeController : BaseController
    {
        [ChildActionOnly]
        public PartialViewResult Menu()
        {
            var model = new NavbarViewModel
            {
                Username = CurrentSession.Username,
                IsAuthenticated = CurrentSession.UserId > 0
            };

            return PartialView("~/Areas/Admin/Views/Shared/_AdminNavbar.cshtml", model);
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}