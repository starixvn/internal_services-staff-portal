namespace TicketDesk.Web.Client.Models.CustomField
{
    using System.Collections.Generic;
    using Domain.Model;

    public class CustomFieldSettingViewModel
    {
        public int SelectedCategoryId { get; set; }

        public IEnumerable<TicketCategory> TicketCategoryWithCustomFields { get; set; }

        public IEnumerable<Domain.Model.CustomField> CustomFields { get; set; }
    }
}