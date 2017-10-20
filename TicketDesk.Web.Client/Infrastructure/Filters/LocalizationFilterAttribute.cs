namespace TicketDesk.Web.Client.Infrastructure.Filters
{
    using Localization;
    using System;
    using System.Web;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class LocalizationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cookie = HttpContext.Current.Request.Cookies["_culture"];
            var name = cookie?.Value;

            if (string.IsNullOrEmpty(name))
            {
                CultureHelper.SetDefaultCulture();
                return;
            }

            CultureHelper.SetCurrentCulture(name);
        }
    }
}