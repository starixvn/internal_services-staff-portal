namespace TicketDesk.Web.Client.Models.Extensions
{
    using System.Collections.Generic;

    public static class MenuIconMapper
    {
        public static Dictionary<string, string> MenuIconDictionary { get; set; } = new Dictionary<string, string>
        {
            { "/", "icon-home" },
            { "/ticket/", "icon-myticket" },
            { "/ticket/new-facility", "icon-facility" },
            { "/ticket/new-feedback", "icon-feedback" },
            { "/ticket/new-hardware", "icon-hardware" },
            { "/ticket/new-software", "icon-software" },
            { "/ticket/new-network", "icon-network" },
            { "/ticket/new-customersupport", "icon-customersupport" },
            { "/ticket/new-incident", "icon-incident" },
            { "/tickets", "icon-ticketlist" },
            { "/IdUploader", "icon-upload" },
            { "/References", "icon-references" },
            { "/References/tv-schedules", "icon-tv" },
            { "/References/featured-event", "icon-feature" },
            { "/References/bet-of-the-day", "icon-bet" },
            { "/References/trading-limit", "icon-trading" },
            { "/References/newsletter", "icon-newsletter" },
            { "/Admin", "icon-admin" },
            { "/Admin/Announcements", "icon-announcements" },
            { "/Admin/References", "icon-references" },
            { "/Admin/References/TVSchedules", "icon-admin" },
            { "/Admin/References/BetOfTheDay", "icon-admin" },
            { "/Admin/References/TradingLimits", "icon-admin" },
            { "/Admin/References/FeatureEvents", "icon-admin" },
            { "/Admin/References/NewsLetter", "icon-admin" },
            { "/Admin/ShopManagement", "icon-shop" },
            { "/Admin/DataConfiguration", "icon-data" },
            { "/admin/application-settings", "icon-admin-appsettings" },
            { "/admin/projects", "icon-admin-shop" },
            { "/admin/push-notification-settings", "icon-admin-notificationsettings" },
            { "/admin/data-management/demo", "icon-admin-datademo" },
            { "/admin/search-manager", "icon-admin-searchmanager" },
            { "/admin/custom-field-settings", "icon-admin-customfieldsettings" },
            { "/file-manager", "icon-filemanager" }
        };
    }
}