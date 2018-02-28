namespace TicketDesk.Web.Client.Models.Home
{
    using System.Collections.Generic;

    public class AnnouncementViewModel
    {
        public int UnreadAnnouncementQuatity { get; set; }

        public IList<AnnouncementGroup> AnnouncementGroups { get; set; }
    }
}