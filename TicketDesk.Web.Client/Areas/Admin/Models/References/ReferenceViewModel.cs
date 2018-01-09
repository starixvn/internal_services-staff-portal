namespace TicketDesk.Web.Client.Areas.Admin.Models.References
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using TicketDesk.Core.Enumerations;

    public class ReferenceViewModel
    {
        [Required]
        [Display(Name = "Date Appear")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM-DD-YYYY}", ApplyFormatInEditMode = true)]
        public DateTime DateAppear { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Image URL must under 500 characters")]
        [Remote("IsValidImageUrl", "References", AdditionalFields = "ImageLink", ErrorMessage = "The request content cannot be loaded. Please try again.")]
        [Display(Name = "Image URL")]
        public string ImageLink { get; set; }

        [Required]
        public ReferenceType ReferenceType { get; set; }
    }
}