namespace TicketDesk.Domain.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using TicketDesk.Localization.Client;

    public class Shop
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShopId { get; set; }

        [Display(Name = nameof(Resources.ShopName), ResourceType = typeof(Resources))]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = nameof(Resources.StringLengthMaxErrorMessage))]
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = nameof(Resources.RequiredErrorMessage))]
        public string ShopName { get; set; }

        [Display(Name = nameof(Resources.ShopCode), ResourceType = typeof(Resources))]
        [StringLength(50, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = nameof(Resources.StringLengthMaxErrorMessage))]
        public string ShopCode { get; set; }

        [Display(Name = nameof(Resources.ContactName), ResourceType = typeof(Resources))]
        [StringLength(70, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = nameof(Resources.StringLengthMaxErrorMessage))]
        public string ContactName { get; set; }

        [Display(Name = nameof(Resources.ContactNumber), ResourceType = typeof(Resources))]
        [StringLength(20, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = nameof(Resources.StringLengthMaxErrorMessage))]
        public string ContactNumber { get; set; }

        [Display(Name = nameof(Resources.Email), ResourceType = typeof(Resources))]
        [StringLength(254, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = nameof(Resources.StringLengthMaxErrorMessage))]
        public string Email { get; set; }

        [Display(Name = nameof(Resources.ShopAddress), ResourceType = typeof(Resources))]
        [StringLength(500, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = nameof(Resources.StringLengthMaxErrorMessage))]
        public string Address { get; set; }

        [Display(Name = nameof(Resources.ShopDescription), ResourceType = typeof(Resources))]
        [StringLength(500, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = nameof(Resources.StringLengthMaxErrorMessage))]
        public string ShopDescription { get; set; }

        public bool IsActive { get; set; }

        public string LastUpdatedBy { get; set; }

        public DateTime LastUpdatedDate { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}