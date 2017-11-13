namespace TicketDesk.Web.Client.Models
{
    using Domain.Model;
    using System.Collections.Generic;

    public class CustomFieldSettingViewModel
    {
        public int SelectedCategoryId { get; set; }

        public IEnumerable<TicketCategory> TicketCategoryWithCustomFields { get; set; }

        public IEnumerable<Domain.Model.CustomField> CustomFields { get; set; }
    }
}