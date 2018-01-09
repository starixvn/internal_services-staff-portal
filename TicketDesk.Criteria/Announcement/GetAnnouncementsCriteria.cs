namespace TicketDesk.Criteria.Announcement
{
    using System;
    using Domain.Model;

    public class GetAnnouncementsCriteria
    {
        public DateTime? PublishedFrom { get; set; }
        public DateTime? PublishedTo { get; set; }
        public AnnouncementStatus? Status { get; set; }
        public AnnouncementState? State { get; set; }
    }

    public class NewAnnouncementCriteria
    {
        public Announcement Announcement { get; set; }
        public bool IsImportant { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan? Time { get; set; }
        public string ModifiedBy { get; set; }
    }
}
