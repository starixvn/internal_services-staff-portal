namespace TicketDesk.Domain.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class UserReadAnnouncement
    {
        [Key, Column(Order = 0)]
        public int AnnouncementId { get; set; }

        [Key, Column(Order = 1)]
        public int UserId { get; set; }

        public bool IsRead { get; set; }
    }
}
