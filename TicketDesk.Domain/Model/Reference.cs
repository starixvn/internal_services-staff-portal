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

        public DateTime DateAppear { get; set; }

        [StringLength(500, ErrorMessage = "Image link must under 500 characters")]
        public string ImageLink { get; set; }

        public string LastUpdateby { get; set; }

        public DateTime LastUpdateDate { get; set; }

        public ReferenceType ReferenceType { get; set; }
    }
}