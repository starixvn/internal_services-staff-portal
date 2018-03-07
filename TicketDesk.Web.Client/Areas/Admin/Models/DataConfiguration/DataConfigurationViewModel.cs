namespace TicketDesk.Web.Client.Areas.Admin.Models.DataConfiguration
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Domain.Model;
    using Localization.Client;

    public class DataConfigurationViewModel
    {
        [Display(Name = nameof(Resources.Field), ResourceType = typeof(Resources))]
        public int SelectedFieldId { get; set; }

        public IEnumerable<EditableField> EditableCustomFields { get; set; }
    }
}