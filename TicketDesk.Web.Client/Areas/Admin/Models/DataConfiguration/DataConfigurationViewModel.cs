namespace TicketDesk.Web.Client.Areas.Admin.Models.DataConfiguration
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Domain.Model;

    public class DataConfigurationViewModel
    {
        [Display(Name = "Field")]
        public int SelectedFieldId { get; set; }

        public IEnumerable<EditableField> EditableCustomFields { get; set; }

        public IEnumerable<CustomFieldListValue> CustomFieldListValues { get; set; }
    }

    public class AddCustomFieldListValueViewModel
    {
        [Display(Name = "Display Name EN")]
        [StringLength(50, ErrorMessage = "{0} must be less than {1} characters")]
        public string DisplayName { get; set; }

        [Display(Name = "Display Name VI")]
        [StringLength(50, ErrorMessage = "{0} must be less than {1} characters")]
        [Required(ErrorMessage = "{0} is required!")]
        public string DisplayName_vi { get; set; }

        [Display(Name = "Description")]
        [StringLength(200, ErrorMessage = "{0} must be less than {1} characters")]
        public string Description { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}