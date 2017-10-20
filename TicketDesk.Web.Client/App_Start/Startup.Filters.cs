namespace TicketDesk.Web.Client
{
    using Infrastructure.Filters;
    using System.Web.Mvc;

    public partial class Startup
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new LocalizationFilterAttribute());
            filters.Add(new CustomJsonResultAttribute());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
