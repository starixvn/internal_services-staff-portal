namespace TicketDesk.Domain.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Core.Enumerations;
    using Core.Model;
    using TicketDesk.Localization.Client;

    public class Reference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Date Appear")]
        [Required(ErrorMessageResourceName = "RequiredErrorMessage", ErrorMessageResourceType = typeof(Resources))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-DD-YYYY}", ApplyFormatInEditMode = true)]
        public DateTime DateAppear { get; set; }

        [Display(Name = "Image URL")]
        [Required(ErrorMessageResourceName = "RequiredErrorMessage", ErrorMessageResourceType = typeof(Resources))]
        [StringLength(500, ErrorMessageResourceName = "StringLengthMaxErrorMessage", ErrorMessageResourceType = typeof(Resources))]
        [ImageLink]
        public string ImageLink { get; set; }

        [Required]
        public string LastUpdateBy { get; set; }

        [Required]
        public DateTime LastUpdateDate { get; set; }

        [Required]
        public ReferenceType ReferenceType { get; set; }
    }
}