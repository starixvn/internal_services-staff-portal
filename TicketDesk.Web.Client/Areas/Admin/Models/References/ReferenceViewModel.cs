namespace TicketDesk.Web.Client.Areas.Admin.Models.References
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Core.Enumerations;
    using Core.Resources;

    public class ReferenceViewModel
    {
        [Required]
        [Display(Name = "Date Appear")]
        public DateTime DateAppear { get; set; }

        [Required]
        [StringLength(500, ErrorMessageResourceName = "ValidateStringLengthMessage", ErrorMessageResourceType = typeof(Messages))]
        [Remote("IsValidImageUrl", "References", AdditionalFields = "ImageLink", ErrorMessageResourceName = "InvalidImageURL", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "Image URL")]
        public string ImageLink { get; set; }

        [Required]
        public ReferenceType ReferenceType { get; set; }
    }
}