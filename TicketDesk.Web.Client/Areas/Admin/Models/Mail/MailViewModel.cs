namespace TicketDesk.Web.Client.Areas.Admin.Models.Mail
{
    using System.ComponentModel.DataAnnotations;
    using Core.Enumerations;
    using Domain.Model;
    using Localization.Client;

    public class MailViewModel
    {
        [Display(Name = nameof(Resources.TicketType), ResourceType = typeof(Resources))]
        public TicketCategoryTypes CategoryId { get; set; }
    }

    public class ListMailViewModel
    {
        public TicketCategory SelectedTicketCategory { get; set; }
    }
}