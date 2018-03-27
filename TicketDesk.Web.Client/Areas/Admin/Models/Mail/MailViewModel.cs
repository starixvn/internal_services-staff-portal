namespace TicketDesk.Web.Client.Areas.Admin.Models.Mail
{
    using System.ComponentModel.DataAnnotations;
    using Core.Enumerations;
    using Localization.Client;

    public class MailViewModel
    {
        [Display(Name = nameof(Resources.TicketType), ResourceType = typeof(Resources))]
        public TicketCategoryTypes CategoryId { get; set; }
    }
}