namespace TicketDesk.Web.Client.Areas.Admin.Models.CustomField
{
    using System.Collections.Generic;
    using Domain.Model;

    public class CustomFieldSettingViewModel
    {
        public int SelectedCategoryId { get; set; }

        public IEnumerable<TicketCategory> TicketCategoryWithCustomFields { get; set; }

        public IEnumerable<CustomField> CustomFields { get; set; }
    }
}