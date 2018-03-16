namespace TicketDesk.Web.Client.Areas.Admin.Models.CustomField
{
    using System.Collections.Generic;
    using Core.Enumerations;
    using Domain.Model;

    public class ListCustomFieldViewModel
    {
        public TicketCategoryTypes SelectedTicketCategoryType { get; set; }

        public IEnumerable<CustomField> CustomFields { get; set; }
    }
}