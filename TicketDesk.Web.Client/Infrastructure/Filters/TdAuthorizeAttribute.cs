namespace TicketDesk.Web.Client
{
    using System;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class TdAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext.Result is HttpUnauthorizedResult && filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult("AccessDenied", null, false);
            }
        }
    }
}
