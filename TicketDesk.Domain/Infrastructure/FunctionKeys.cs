namespace TicketDesk.Domain.Infrastructure
{
    public static class FunctionKeys
    {
        public const string ListAssignedTicket = "ListAssignedTicket";
        public const string NewTicket = "NewTicket";
        public const string TicketList = "TicketList";
        public const string IdUploader = "IdUploader";
        public const string Reference = "Reference";

        public const string FileManagement = "FileManagement";

        #region Ticket Types

        public const string Facility = "Facility";
        public const string Feedback = "Feedback";
        public const string Hardware = "Hardware";
        public const string Software = "Software";
        public const string CustomerSupport = "CustomerSupport";
        public const string Incident = "Incident";

        #endregion

        #region Admin

        public const string Administration = "Administration";
        public const string Announcement = "Announcement";
        public const string References = "References";
        public const string Languages = "Languages";
        public const string CustomFields = "CustomFields";
        public const string ShopManagement = "ShopManagement";
        public const string DataConfiguration = "DataConfiguration";
        public const string MailManagement = "MailManagement";
        public const string TicketListAdmin = "TicketListAdmin";

        #endregion Admin
    }
}