namespace TicketDesk.Web.Client.Models.Home
{
    using System.Collections.Generic;
    using Domain.Model;

    public class HomeViewModel
    {
        public int UnreadAnnouncementQuatity { get; set; }

        public IEnumerable<Announcement> Announcements { get; set; }
    }
}