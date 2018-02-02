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

        public static string IsActiveRoute(this HtmlHelper html, string napRoute)
        {
            var currentRoute = HttpContext.Current.Request.Url.AbsolutePath;

            var request = HttpContext.Current.Request;
            var controller = request.RequestContext.RouteData.Values["controller"].ToString();

            if(napRoute == "/" && controller == "Home")
            {
                return "active";
            }

            if (napRoute == "/Admin/Announcements" && controller == "Announcements")
            {
                return "active";
            }

            if (currentRoute.Contains(napRoute) && napRoute != "/")
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