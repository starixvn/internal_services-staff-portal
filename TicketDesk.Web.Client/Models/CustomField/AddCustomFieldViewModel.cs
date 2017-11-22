namespace TicketDesk.Web.Client.Models.CustomField
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class AddCustomFieldViewModel
    {
        [Display(Name = "Apply to categories")]
        public ICollection<int> TicketCategoryIds { get; set; }

        public IEnumerable<SelectListItem> TicketCategories { get; set; }

        [Display(Name = "Type")]
        public int CustomFieldTypeId { get; set; }

        public IEnumerable<SelectListItem> CustomFieldTypes { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Name VI")]
        public string Name_vi { get; set; }

        [Display(Name = "Values")]
        public string Values { get; set; }

        [Display(Name = "Values_vi")]
        public string Values_vi { get; set; }

        [Display(Name = "Is Enabled")]
        public bool IsEnabled { get; set; }

        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }
    }
}