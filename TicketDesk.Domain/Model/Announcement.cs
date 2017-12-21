namespace TicketDesk.Domain.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Announcement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(2000, ErrorMessage = "Message must under 2000 characters")]
        public string Message { get; set; }

        public DateTime CreatedDateTime { get; set; }

        [Display(Name = "Publish Date")]
        public DateTime PublishedDateTime { get; set; }

        [Display(Name = "Specific to Shop")]
        public int ProjectId { get; set; }

        [Display(Name = "Status")]
        public AnnouncementStatus Status { get; set; }

        [Display(Name = "State")]
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
