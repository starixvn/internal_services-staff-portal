namespace TicketDesk.Domain.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using DelegateDecompiler;

    public class Announcement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Announcement Content")]
        [StringLength(5000, ErrorMessage = "Message must under 5000 characters")]
        [Required(ErrorMessage = "Announcement Message is required")]
        public string Message { get; set; }
        
        public DateTime CreatedDateTime { get; set; }

        [Display(Name = "Publish Date")]
        public DateTime? PublishedDateTime { get; set; }

        [Display(Name = "Specific to Shop")]
        public int ProjectId { get; set; }

        [Display(Name = "Status")]
        [Required(ErrorMessage = "Announcement status is required")]
        public AnnouncementStatus Status { get; set; }

        [Display(Name = "State")]
        [NotMapped]
        [Computed]
        public AnnouncementState State
        {
            get
            {
                var result = AnnouncementState.Published;

                if (PublishedDateTime >= DateTime.Now)
                {
                    result = AnnouncementState.Scheduled;
                }

                return result;
            }
        }

        public DateTime LastModified { get; set; }

        public string ModifiedBy { get; set; }
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
