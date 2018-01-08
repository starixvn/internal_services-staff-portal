namespace TicketDesk.Web.Client.Models.Home
{
    using System.Collections.Generic;

    public class HomeViewModel
    {
        public MyOpenningIssues MyOpenningIssues { get; set; }

        public IList<AnnouncementGroup> AnnouncementGroups { get; set; }
    }
}