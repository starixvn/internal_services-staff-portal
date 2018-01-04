namespace TicketDesk.Domain.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using TicketDesk.Core.Enumerations;

    public class Reference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date Appear")]
        public DateTime DateAppear { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Image URL must under 500 characters")]
        [Display(Name = "Image URL")]
        public string ImageLink { get; set; }

        [Required]
        public string LastUpdateBy { get; set; }

        [Required]
        public DateTime LastUpdateDate { get; set; }

        [Required]
        public ReferenceType ReferenceType { get; set; }
    }
}