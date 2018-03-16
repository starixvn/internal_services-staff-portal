namespace TicketDesk.Web.Client.Areas.Admin.Models.CustomField
{
    using System.ComponentModel.DataAnnotations;
    using Core.Enumerations;
    using Localization.Client;

    public class CustomFieldsViewModel
    {
        [Display(Name = nameof(Resources.TicketCategory), ResourceType = typeof(Resources))]
        public TicketCategoryTypes SelectedTicketCategoryType { get; set; }

        ////public IEnumerable<TicketCategory> TicketCategories { get; set; }
    }
}