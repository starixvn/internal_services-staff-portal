namespace TicketDesk.Criteria.Announcement
{
    using System.Collections.Generic;
    using Domain.Model;

    public class NewAnnouncementCriteria
    {
        public Announcement Announcement { get; set; }
        public bool IsImportant { get; set; }
        public string ModifiedBy { get; set; }
    }

    public class PublishAnnouncementsCriteria
    {
        public IList<int> Ids { get; set; }

        public string ModifiedBy { get; set; }
    }
}