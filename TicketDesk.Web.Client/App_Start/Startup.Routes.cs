namespace TicketDesk.Web.Client
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using Core.Enumerations;

    public partial class Startup
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            routes.MapRoute(
                name: RouteNames.Default.ToString(),
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "TicketDesk.Web.Client.Controllers" });
        }
    }
}