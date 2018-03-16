namespace TicketDesk.Web.Client.Areas.Admin.Models.ShopManagement
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Criteria.Shop;
    using Localization.Client;

    public class ShopViewModel
    {
        public int? ShopId { get; set; }

        [AllowHtml]
        [Display(Name = nameof(Resources.ShopName), ResourceType = typeof(Resources))]
        [MaxLength(100, ErrorMessageResourceName = nameof(Resources.StringLengthMaxErrorMessage), ErrorMessageResourceType = typeof(Resources))]
        [Required(ErrorMessageResourceName = nameof(Resources.RequiredErrorMessage), ErrorMessageResourceType = typeof(Resources))]
        public string ShopName { get; set; }

        [AllowHtml]
        [Display(Name = nameof(Resources.ShopCode), ResourceType = typeof(Resources))]
        [MaxLength(50, ErrorMessageResourceName = nameof(Resources.StringLengthMaxErrorMessage), ErrorMessageResourceType = typeof(Resources))]
        public string ShopCode { get; set; }

        [AllowHtml]
        [Display(Name = nameof(Resources.ContactName), ResourceType = typeof(Resources))]
        [MaxLength(70, ErrorMessageResourceName = nameof(Resources.StringLengthMaxErrorMessage), ErrorMessageResourceType = typeof(Resources))]
        public string ContactName { get; set; }

        [AllowHtml]
        [Display(Name = nameof(Resources.ContactNumber), ResourceType = typeof(Resources))]
        [MaxLength(20, ErrorMessageResourceName = nameof(Resources.StringLengthMaxErrorMessage), ErrorMessageResourceType = typeof(Resources))]
        public string ContactNumber { get; set; }

        [AllowHtml]
        [Display(Name = nameof(Resources.Email), ResourceType = typeof(Resources))]
        [MaxLength(254, ErrorMessageResourceName = nameof(Resources.StringLengthMaxErrorMessage), ErrorMessageResourceType = typeof(Resources))]
        public string Email { get; set; }

        [AllowHtml]
        [Display(Name = nameof(Resources.ShopAddress), ResourceType = typeof(Resources))]
        [MaxLength(500, ErrorMessageResourceName = nameof(Resources.StringLengthMaxErrorMessage), ErrorMessageResourceType = typeof(Resources))]
        public string Address { get; set; }

        [Display(Name = nameof(Resources.Active), ResourceType = typeof(Resources))]
        public bool IsActive { get; set; }

        public ShopCriteria GetCriteria()
        {
            return new ShopCriteria
            {
                ShopName = ShopName,
                ShopCode = ShopCode,
                ContactName = ContactName,
                ContactNumber = ContactNumber,
                Email = Email,
                Address = Address,
                IsActive = IsActive,
            };
        }
    }
}