namespace TicketDesk.Domain.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;
    using Core.Enumerations;
    using DelegateDecompiler;
    using TicketDesk.Localization.Client;

    public class Announcement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = nameof(Resources.AnnouncementContent), ResourceType = typeof(Resources))]
        [StringLength(5000, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = nameof(Resources.StringLengthMaxErrorMessage))]
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = nameof(Resources.RequiredErrorMessage))]
        [AllowHtml]
        public string Message { get; set; }

        public DateTime CreatedDateTime { get; set; }

        [Display(Name = nameof(Resources.PublishDate), ResourceType = typeof(Resources))]
        [Remote(controller: "Announcement", action: "CheckPublishedDateTime", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = nameof(Resources.DateNotInRange))]
        public DateTime? PublishedDateTime { get; set; }

        public int ShopId { get; set; }

        [Display(Name = nameof(Resources.Type), ResourceType = typeof(Resources))]
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = nameof(Resources.RequiredErrorMessage))]
        public AnnouncementType Type { get; set; }

        [Display(Name = nameof(Resources.State), ResourceType = typeof(Resources))]
        [NotMapped]
        [Computed]
        public AnnouncementState State
        {
            get
            {
                var result = AnnouncementState.Published;

                if (PublishedDateTime == null || PublishedDateTime >= DateTime.Now)
                {
                    result = AnnouncementState.Scheduled;
                }

                return result;
            }
        }

        [NotMapped]
        public bool IsWaitingToPublish { get; set; }

        public DateTime LastModified { get; set; }

        public string ModifiedBy { get; set; }

        [NotMapped]
        public bool IsRead { get; set; }
    }
}