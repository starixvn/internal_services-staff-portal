namespace TicketDesk.Web.Client.Models.Extensions
{
    using System.Collections.Generic;

    public static class MenuIconMapper
    {
        public static Dictionary<string, string> MenuIconDictionary { get; set; } = new Dictionary<string, string>
        {
            { "/", "icon-home" },
            { "/Ticket/", "icon-myticket" },
            { "/Ticket/NewFacilityTicket", "icon-facility" },
            { "/Ticket/NewFeedbackTicket", "icon-feedback" },
            { "/Ticket/NewHardwareTicket", "icon-hardware" },
            { "/Ticket/NewSoftwareTicket", "icon-software" },
            { "/Ticket/new-network", "icon-network" },
            { "/Ticket/NewCustomerSupportTicket", "icon-customersupport" },
            { "/Ticket/NewIncidentTicket", "icon-incident" },
            { "/TicketList", "icon-ticketlist" },
            { "/IdUploader", "icon-upload" },
            { "/Reference", "icon-references" },
            { "/Reference/TvSchedules", "icon-tv" },
            { "/Reference/FeaturedEvents", "icon-feature" },
            { "/Reference/BetOfTheDay", "icon-bet" },
            { "/Reference/TradingLimits", "icon-trading" },
            { "/Reference/Newsletters", "icon-newsletter" },
            { "/Admin", "icon-admin" },
            { "/Admin/Announcement", "icon-announcements" },
            { "/Admin/References", "icon-references" },
            { "/Admin/References/TVSchedules", "icon-admin" },
            { "/Admin/References/BetOfTheDay", "icon-admin" },
            { "/Admin/References/TradingLimits", "icon-admin" },
            { "/Admin/References/FeatureEvents", "icon-admin" },
            { "/Admin/References/NewsLetter", "icon-admin" },
            { "/Admin/Shop", "icon-shop" },
            { "/Admin/DataConfiguration", "icon-data" },
            { "/Admin/TicketList", "icon-ticketlist" },
            { "/Admin/Mail", "icon-mail" },
            { "/admin/custom-field-settings", "icon-admin-customfieldsettings" },
            { "/file-manager", "icon-filemanager" }
        };
    }
}