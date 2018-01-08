namespace TicketDesk.Web.Client.Models.Home
{
    using System.Collections.Generic;
    using Domain.Model;

    public class AnnouncementGroup
    {
        public AnnouncementGroup()
        {
            Announcements = new List<Announcement>();
        }

        public string Name { get; set; }
        
        public IEnumerable<Announcement> Announcements { get; set; }
    }
}