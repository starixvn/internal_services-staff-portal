namespace TicketDesk.Web.Client.Areas.Admin.Models.ShopManagement
{
    using System.ComponentModel.DataAnnotations;

    public class ShopViewModel
    {
        public int? ShopId { get; set; }

        [Display(Name = "Shop Name")]
        [MaxLength(100, ErrorMessage = "{0} must under {1} characters")]
        [Required(ErrorMessage = "{0} is required")]
        public string ShopName { get; set; }

        [Display(Name = "Shop Code")]
        [MaxLength(50, ErrorMessage = "{0} must under {1} characters")]
        public string ShopCode { get; set; }

        [Display(Name = "Contact Name")]
        [MaxLength(70, ErrorMessage = "{0} must under {1} characters")]
        public string ContactName { get; set; }

        [Display(Name = "Contact Number")]
        [MaxLength(20, ErrorMessage = "{0} must under {1} characters")]
        public string ContactNumber { get; set; }

        [Display(Name = "Email Address")]
        [MaxLength(254, ErrorMessage = "{0} must under {1} characters")]
        public string Email { get; set; }

        [Display(Name = "Shop Address")]
        [MaxLength(500, ErrorMessage = "{0} must under {1} characters")]
        public string Address { get; set; }

        public bool IsActive { get; set; }
    }
}