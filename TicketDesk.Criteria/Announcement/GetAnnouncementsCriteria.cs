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
}
