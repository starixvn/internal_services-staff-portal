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

        [Display(Name = "Date Appear")]
        [Required(ErrorMessageResourceName = "ValidateRequiredMessage", ErrorMessageResourceType = typeof(Messages))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-DD-YYYY}", ApplyFormatInEditMode = true)]
        public DateTime DateAppear { get; set; }

        [Display(Name = "Image URL")]
        [Required(ErrorMessageResourceName = "ValidateRequiredMessage", ErrorMessageResourceType = typeof(Messages))]
        [StringLength(500, ErrorMessageResourceName = "ValidateStringLengthMessage", ErrorMessageResourceType = typeof(Messages))]
        [ImageLink]
        public string ImageLink { get; set; }

        [Required]
        public ReferenceType ReferenceType { get; set; }
    }
}