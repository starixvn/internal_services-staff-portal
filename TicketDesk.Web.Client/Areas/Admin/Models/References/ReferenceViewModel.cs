namespace TicketDesk.Web.Client.Areas.Admin.Models.References
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using TicketDesk.Core.Enumerations;

    public class ReferenceViewModel
    {
        [Required]
        [Display(Name = "Date Appear")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime DateAppear { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Image URL must under 500 characters")]
        [Display(Name = "Image URL")]
        public string ImageLink { get; set; }

        [Required]
        public ReferenceType ReferenceType { get; set; }
    }
}