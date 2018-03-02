namespace TicketDesk.Domain.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Shop
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShopId { get; set; }

        [Display(Name = "Shop Name")]
        [StringLength(100, ErrorMessage = "{0} must under {1} characters")]
        [Required(ErrorMessage = "{0} is required")]
        public string ShopName { get; set; }

        [Display(Name = "Shop Code")]
        [StringLength(50, ErrorMessage = "{0} must under {1} characters")]
        public string ShopCode { get; set; }

        [Display(Name = "Contact Name")]
        [StringLength(70, ErrorMessage = "{0} must under {1} characters")]
        public string ContactName { get; set; }

        [Display(Name = "Contact Number")]
        [StringLength(20, ErrorMessage = "{0} must under {1} characters")]
        public string ContactNumber { get; set; }

        [Display(Name = "Email Address")]
        [StringLength(254, ErrorMessage = "{0} must under {1} characters")]
        public string Email { get; set; }

        [Display(Name = "Shop Address")]
        [StringLength(500, ErrorMessage = "{0} must under {1} characters")]
        public string Address { get; set; }

        [Display(Name = "Shop Description")]
        [StringLength(500, ErrorMessage = "{0} must under {1} characters")]
        public string ShopDescription { get; set; }

        public bool IsActive { get; set; }

        public string LastUpdatedBy { get; set; }

        public DateTime LastUpdatedDate { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}