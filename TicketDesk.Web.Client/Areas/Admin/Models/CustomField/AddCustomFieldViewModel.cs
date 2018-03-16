namespace TicketDesk.Web.Client.Areas.Admin.Models.CustomField
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Localization.Client;

    public class AddCustomFieldViewModel
    {
        [Display(Name = nameof(Resources.ApplyToCategories), ResourceType = typeof(Resources))]
        public ICollection<int> TicketCategoryIds { get; set; }

        public IEnumerable<SelectListItem> TicketCategories { get; set; }

        [Display(Name = nameof(Resources.Type), ResourceType = typeof(Resources))]
        public int CustomFieldTypeId { get; set; }

        public IEnumerable<SelectListItem> CustomFieldTypes { get; set; }

        [Display(Name = nameof(Resources.Name), ResourceType = typeof(Resources))]
        public string Name { get; set; }

        [Display(Name = nameof(Resources.NameVI), ResourceType = typeof(Resources))]
        public string Name_vi { get; set; }

        [Display(Name = nameof(Resources.Values), ResourceType = typeof(Resources))]
        public string Values { get; set; }

        [Display(Name = nameof(Resources.Values_vi), ResourceType = typeof(Resources))]
        public string Values_vi { get; set; }

        [Display(Name = "Is Required")]
        public bool IsRequired { get; set; }

        public string Regex { get; set; }

        [Display(Name = "Regex Error Message")]
        public string RegexValidationErrorMessage { get; set; }

        [Display(Name = "Min Length")]
        public int MinLength { get; set; }

        [Display(Name = "Max Length")]
        public int MaxLength { get; set; }

        [Display(Name = nameof(Resources.IsEnabled), ResourceType = typeof(Resources))]
        public bool IsEnabled { get; set; }

        [Display(Name = nameof(Resources.DisplayOrder), ResourceType = typeof(Resources))]
        public int DisplayOrder { get; set; }

        [Display(Name = nameof(Resources.IsAllowedToEditListValue), ResourceType = typeof(Resources))]
        public bool IsAllowedToEditListValue { get; set; }
    }
}