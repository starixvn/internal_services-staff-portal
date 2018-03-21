// [assembly: OwinStartup(typeof(Startup))]
namespace TicketDesk.Web.Client
{
    using Owin;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            RegisterBundles(BundleTable.Bundles);

            var container = RegisterStructureMap(app);

            ConfigureDatabase();
            ////ConfigureSearch();
            ConfigureAuth(app, container);
            ////ConfigurePushNotifications();
            ////ConfigureApplicationInsights();

            MvcHandler.DisableMvcResponseHeader = true;
        }
    }
}