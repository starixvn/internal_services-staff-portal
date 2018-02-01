namespace TicketDesk.Web.Client.Infrastructure.Extensions
{
    using System.Web;
    using System.Web.Mvc;

    public static class HtmlHelperExtensions
    {
        public static string IsActiveRoute(this HtmlHelper html, string actionName, string controllerName)
        {
            var action = html.ViewContext.RouteData.Values["action"] as string;
            var controller = html.ViewContext.RouteData.Values["controller"] as string;

            return (controllerName == controller && actionName == action) ? "active" : string.Empty;
        }

        public static string IsActiveRoute(this HtmlHelper html, string route)
        {
            var currentRoute = HttpContext.Current.Request.Url.AbsolutePath;

            if (currentRoute == "/" && route == "/" || (currentRoute.Contains("Home") && route == "/"))
            {
                // Home
                return "active";
            }

            if (currentRoute == "/Admin" && route == "/Admin/Announcements" || (currentRoute.Contains("Admin") && route == "/Admin/Announcements"))
            {
                // Home
                return "active";
            }

            if (currentRoute.Contains(route) && route != "/")
            {
                return "active";
            }

            return string.Empty;
        }

        public static string IsExactMatch(this HtmlHelper html, string route)
        {
            var currentRoute = HttpContext.Current.Request.Url.AbsolutePath;

            if (currentRoute == route && route != "/")
            {
                return "active";
            }

            return string.Empty;
        }
    }
}