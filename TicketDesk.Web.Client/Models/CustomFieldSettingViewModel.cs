namespace TicketDesk.Web.Client.Models
{
    using System.Collections.Generic;

    public class CustomFieldSettingViewModel
    {
        public IEnumerable<Domain.Model.CustomField> CustomFields { get; set; }
    }
}