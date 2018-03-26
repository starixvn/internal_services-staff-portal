namespace TicketDesk.Domain.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;
    using Core.Enumerations;
    using DelegateDecompiler;
    using TicketDesk.Localization.Client;

    public class Announcement : IValidatableObject
    {
        private readonly int MINPUBLISHEDTIME = 5; // minutes
        private readonly int MAXPUBLISHEDTIME = 365; // days

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
        public DateTime? PublishedDateTime { get; set; }

        [NotMapped]
        public bool IsPublished
        {
            get
            {
                var result = PublishedDateTime != null && (Math.Abs((DateTime.Now - PublishedDateTime).Value.TotalMinutes) <= MINPUBLISHEDTIME);
                return result;
            }
        }

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

        public DateTime LastModified { get; set; }

        public string ModifiedBy { get; set; }

        [NotMapped]
        public bool IsRead { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IList<ValidationResult> result = new List<ValidationResult>();

            var isValid = PublishedDateTime == null ||
                     (((DateTime.Now - PublishedDateTime).Value.TotalMinutes <= MINPUBLISHEDTIME) &&
                      ((PublishedDateTime - DateTime.Now).Value.TotalDays <= MAXPUBLISHEDTIME));

            if (!isValid)
            {
                var validation = new ValidationResult(Resources.DateNotInRange, new[] { nameof(PublishedDateTime) });

                result.Add(validation);
            }

            return result;
        }
    }
}