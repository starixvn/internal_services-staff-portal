namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System.Web.Mvc;
    using Client.Controllers;
    using Client.Models;

    [TdAuthorize]
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
    }
}