namespace TicketDesk.Domain.Model
{
    using System;

    public class Announcement
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime PublishedDateTime { get; set; }

        public int ProjectId { get; set; }

        /// <summary>
        /// 0: Normal
        /// 1: Important
        /// </summary>
        public AnnouncementStatus Status { get; set; }

        /// <summary>
        /// 0: Scheduled
        /// 1: Published
        /// </summary>
        public AnnouncementState State { get; set; }
    }

    public enum AnnouncementStatus
    {
        Normal,
        Important
    }

    public enum AnnouncementState
    {
        Scheduled,
        Published
    }
}
