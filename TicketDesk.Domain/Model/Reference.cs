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
        public DateTime DateAppear { get; set; }

        [StringLength(500, ErrorMessage = "Image link must under 500 characters")]
        [Required]
        public string ImageLink { get; set; }

        [Required]
        public string LastUpdateBy { get; set; }

        [Required]
        public DateTime LastUpdateDate { get; set; }

        [Required]
        public ReferenceType ReferenceType { get; set; }
    }
}