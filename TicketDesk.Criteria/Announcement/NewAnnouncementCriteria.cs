namespace TicketDesk.Criteria.Announcement
{
    using System;
    using Domain.Model;

    public class NewAnnouncementCriteria
    {
        public Announcement Announcement { get; set; }
        public bool IsImportant { get; set; }
        public DateTime? Date { get; set; }
        ////public TimeSpan? Time { get; set; }
        public string ModifiedBy { get; set; }
    }
}