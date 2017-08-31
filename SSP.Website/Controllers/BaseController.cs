namespace SSP.Website.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.AspNetCore.Mvc.Filters;
    using SSP.Core.Constants;
    using SSP.Models.Account;
    using System;
    using System.Net;

    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var controllerName = descriptor.ControllerName;
            var urlRequest = context.HttpContext.Request.Path;
            var encodeUrl = WebUtility.UrlEncode(urlRequest);
            var user = HttpContext.Session.Get<UserSession>(Constants.CurrentSessionKey);

            if ((user == null || user.UserId <= 0) && !controllerName.Equals("Account", StringComparison.OrdinalIgnoreCase))
            {
                var url = "~/Account/Login?ReturnUrl=" + encodeUrl;
                context.Result = Redirect(url);
            }
        }
    }
}