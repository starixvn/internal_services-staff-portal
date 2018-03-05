namespace TicketDesk.Web.Client.Areas.Admin.Models.DataConfiguration
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Localization.Client;

    public class AddCustomFieldListValueViewModel
    {
        public string ModalTitle { get; set; }

        public int CustomFieldId { get; set; }

        [Display(Name = nameof(Resources.DisplayNameEN), ResourceType = typeof(Resources))]
        [StringLength(50, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = nameof(Resources.StringLengthMaxErrorMessage))]
        [Remote(action: "IsDisplayNameEnUnique", controller: "DataConfiguration", AdditionalFields = nameof(CustomFieldId), ErrorMessage = "The {0} already exists.")]
        public string DisplayName { get; set; }

        [Display(Name = nameof(Resources.DisplayNameVI), ResourceType = typeof(Resources))]
        [StringLength(50, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = nameof(Resources.StringLengthMaxErrorMessage))]
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = nameof(Resources.RequiredErrorMessage))]
        [Remote(action: "IsDisplayNameViUnique", controller: "DataConfiguration", AdditionalFields = nameof(CustomFieldId), ErrorMessage = "The {0} already exists.")]
        public string DisplayName_vi { get; set; }

        [Display(Name = nameof(Resources.Description), ResourceType = typeof(Resources))]
        [StringLength(200, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = nameof(Resources.StringLengthMaxErrorMessage))]
        public string Description { get; set; }

        [Display(Name = nameof(Resources.Active), ResourceType = typeof(Resources))]
        public bool IsActive { get; set; }

        public bool IsSuccessful { get; set; }
    }
}