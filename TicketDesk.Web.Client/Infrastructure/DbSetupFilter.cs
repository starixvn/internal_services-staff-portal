namespace TicketDesk.Web.Client
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DbSetupFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // we can only get here if the DB needs attention
            var ctlr = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            // only allowed to go to data management or login
            if (ctlr != "FirstRunSetup")
            {
                filterContext.Result = new RedirectToRouteResult(
                    "first-run-setup",
                    new RouteValueDictionary(
                        new
                        {
                            Action = "Index",
                            Controller = "FirstRunSetup",
                        }));
            }

            base.OnActionExecuting(filterContext);
        }
    }
}