namespace TicketDesk.Web.Client.Controllers
{
    using Domain.Infrastructure;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Localization;
    using System;
    using System.Web;
    using System.Web.Mvc;
    using Models.Settings;

    [RoutePrefix("settings")]
    [Route("{action=index}")]
    public class LanguageSettingController : BaseController
    {
        [Route("language")]
        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            LanguageSettingsViewModel viewModel = new LanguageSettingsViewModel();

            return View(viewModel);
        }

        [Route("language/set-language")]
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.NewTicket, Policy = Constant.Policy.Read)]
        public ActionResult SetLanguage(LanguageSettingsViewModel viewModel)
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