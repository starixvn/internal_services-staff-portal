namespace TicketDesk.Web.Client.Areas.Admin.Models.DataConfiguration
{
    using System.Collections.Generic;
    using Domain.Model;

    public class ListCustomFieldListValueViewModel
    {
        public int SelectedFieldId { get; set; }

        public string ModalTitle { get; set; }

        public IEnumerable<CustomFieldListValue> CustomFieldListValues { get; set; }

        public bool HaveHostPermission { get; set; }
    }
}