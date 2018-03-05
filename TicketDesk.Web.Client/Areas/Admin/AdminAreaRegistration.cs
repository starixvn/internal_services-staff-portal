namespace TicketDesk.Web.Client.Areas.Admin
{
    using System.Web.Mvc;
    using Core.Enumerations;

    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Admin";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                RouteNames.AdminDefault.ToString(),
                "Admin/{controller}/{action}/{id}",
                new { controller = "Announcement", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "TicketDesk.Web.Client.Areas.Admin.Controllers" }
            );
        }
    }
}