namespace TicketDesk.Web.Client.Areas.Admin.Models.References
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Core.Enumerations;
    using Core.Model;
    using Core.Resources;

    public class ReferenceViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "DateAppearRequired", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "Date Appear")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-DD-YYYY}", ApplyFormatInEditMode = true)]
        public DateTime DateAppear { get; set; }

        [Required(ErrorMessageResourceName = "ImageUrlRequired", ErrorMessageResourceType = typeof(Messages))]
        [StringLength(500, ErrorMessageResourceName = "ValidateStringLengthMessage", ErrorMessageResourceType = typeof(Messages))]
        //[Remote("IsValidImageUrl", "References", ErrorMessageResourceName = "InvalidImageURL", ErrorMessageResourceType = typeof(Messages))]
        [ImageLinkAttribute]
        [Display(Name = "Image URL")]
        public string ImageLink { get; set; }

        [Required]
        public ReferenceType ReferenceType { get; set; }
    }
}