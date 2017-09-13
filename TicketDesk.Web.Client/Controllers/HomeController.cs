// TicketDesk - Attribution notice
// Contributor(s):
//
//      Stephen Redd (https://github.com/stephenredd)
//
// This file is distributed under the terms of the Microsoft Public
// License (Ms-PL). See http://opensource.org/licenses/MS-PL
// for the complete terms of use.
//
// For any distribution that contains code from this file, this notice of
// attribution must remain intact, and a copy of the license must be
// provided to the recipient.

namespace TicketDesk.Web.Client.Controllers
{
    using Fanex.NAP.Core.Model;
    using Localization;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;

    [RoutePrefix("")]
    [Route("{action=index}")]
    public class HomeController : BaseController
    {
        [Route("language")]
        public ActionResult SetLanguage(string name)
        {
            CultureHelper.SetCurrentCulture(name);

            var cookie = new HttpCookie("_culture", name) { Expires = DateTime.Today.AddYears(1) };
            Response.SetCookie(cookie);

            if (Request.UrlReferrer != null && !string.IsNullOrEmpty(Request.UrlReferrer.AbsoluteUri))
            {
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            }

            return RedirectToAction("Index", "Home");
        }

        [Route("")]
        [Route("index")]
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public PartialViewResult Menu()
        {
            var model = new NavbarViewModel
            {
                MenuItems = CurrentSession.MenuItemList ?? new List<MenuItem>(),
                Username = CurrentSession.Username,
                IsAuthenticated = CurrentSession.UserId > 0
            };

            return PartialView("~/Views/Shared/_Navbar.cshtml", model);
        }

        [Route("about")]
        public ActionResult About()
        {
            return View();
        }

        [Route("access-denied", Name = "AccessDenied")]
        public ActionResult AccessDenied()
        {
            return View();
        }

        [Route("not-found", Name = "NotFound")]
        public ActionResult NotFound()
        {
            return View();
        }

        [Route("internal-error", Name = "InternalError")]
        public ActionResult InternalError()
        {
            return View();
        }
    }
}