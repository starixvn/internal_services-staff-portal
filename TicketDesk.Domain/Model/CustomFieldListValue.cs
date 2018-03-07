namespace TicketDesk.Domain.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;
    using TicketDesk.Localization.Client;

    public class CustomFieldListValue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int CustomFieldId { get; set; }

        [Display(Name = nameof(Resources.DisplayNameEN), ResourceType = typeof(Resources))]
        [StringLength(50, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = nameof(Resources.StringLengthMaxErrorMessage))]
        [Remote(action: "IsDisplayNameEnUnique", controller: "DataConfiguration", AdditionalFields = nameof(CustomFieldId), ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = nameof(Resources.AlreadyExsistedMessage))]
        [AllowHtml]
        public string DisplayName { get; set; }

        [Display(Name = nameof(Resources.DisplayNameVI), ResourceType = typeof(Resources))]
        [StringLength(50, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = nameof(Resources.StringLengthMaxErrorMessage))]
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = nameof(Resources.RequiredErrorMessage))]
        [Remote(action: "IsDisplayNameViUnique", controller: "DataConfiguration", AdditionalFields = nameof(CustomFieldId), ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = nameof(Resources.AlreadyExsistedMessage))]
        [AllowHtml]
        public string DisplayName_vi { get; set; }

        [Display(Name = nameof(Resources.Description), ResourceType = typeof(Resources))]
        [StringLength(200, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = nameof(Resources.StringLengthMaxErrorMessage))]
        [AllowHtml]
        public string Description { get; set; }

        [Display(Name = nameof(Resources.Active), ResourceType = typeof(Resources))]
        public bool IsActive { get; set; }
    }
}