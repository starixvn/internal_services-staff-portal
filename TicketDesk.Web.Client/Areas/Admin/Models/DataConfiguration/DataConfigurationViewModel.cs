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
    }
}