namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using Client.Controllers;
    using Domain.Infrastructure;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Localization;
    using Models.Language;

    [TdAuthorize]
    public class LanguagesController : BaseController
    {
        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            var viewModel = new LanguagesViewModel();

            return View(viewModel);
        }
        
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        public ActionResult SetLanguage(LanguagesViewModel viewModel)
        {
            CultureHelper.SetCurrentCulture(viewModel.SelectedLanguage);

            var cookie = new HttpCookie("_culture", viewModel.SelectedLanguage) { Expires = DateTime.Today.AddYears(1) };
            Response.SetCookie(cookie);

            if (Request.UrlReferrer != null && !string.IsNullOrEmpty(Request.UrlReferrer.AbsoluteUri))
            {
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}