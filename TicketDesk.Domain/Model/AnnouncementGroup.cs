namespace TicketDesk.Domain.Model
{
    using System.Collections.Generic;

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