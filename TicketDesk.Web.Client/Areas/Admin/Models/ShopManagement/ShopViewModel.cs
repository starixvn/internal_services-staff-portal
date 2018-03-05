namespace TicketDesk.Web.Client.Areas.Admin.Models.ShopManagement
{
    using System.ComponentModel.DataAnnotations;
    using Localization.Client;

    public class ShopViewModel
    {
        public int? ShopId { get; set; }

        [Display(Name = "Shop Name")]
        [StringLength(100)]
        [MaxLength(100, ErrorMessageResourceName = "StringLengthMaxErrorMessage", ErrorMessageResourceType = typeof(Resources))]
        [Required(ErrorMessageResourceName = "RequiredErrorMessage", ErrorMessageResourceType = typeof(Resources))]
        public string ShopName { get; set; }

        [Display(Name = "Shop Code")]
        [StringLength(50)]
        [MaxLength(50, ErrorMessageResourceName = "StringLengthMaxErrorMessage", ErrorMessageResourceType = typeof(Resources))]
        public string ShopCode { get; set; }

        [Display(Name = "Contact Name")]
        [StringLength(70)]
        [MaxLength(70, ErrorMessageResourceName = "StringLengthMaxErrorMessage", ErrorMessageResourceType = typeof(Resources))]
        public string ContactName { get; set; }

        [Display(Name = "Contact Number")]
        [StringLength(20)]
        [MaxLength(20, ErrorMessageResourceName = "StringLengthMaxErrorMessage", ErrorMessageResourceType = typeof(Resources))]
        public string ContactNumber { get; set; }

        [Display(Name = "Email Address")]
        [StringLength(254)]
        [MaxLength(254, ErrorMessageResourceName = "StringLengthMaxErrorMessage", ErrorMessageResourceType = typeof(Resources))]
        public string Email { get; set; }

        [Display(Name = "Shop Address")]
        [StringLength(500)]
        [MaxLength(500, ErrorMessageResourceName = "StringLengthMaxErrorMessage", ErrorMessageResourceType = typeof(Resources))]
        public string Address { get; set; }

        public bool IsActive { get; set; }
    }
}