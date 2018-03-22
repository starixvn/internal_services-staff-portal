namespace TicketDesk.Web.Client.Areas.Admin.Models.Mail
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Domain.Model;
    using Localization.Client;

    public class MailViewModel
    {
        [Display(Name = nameof(Resources.TicketType), ResourceType = typeof(Resources))]
        public int CategoryId { get; set; }

        public IEnumerable<TicketCategory> TicketCategories { get; set; }
    }

    public class ListMailViewModel
    {
        public TicketCategory SelectedTicketCategory { get; set; }
    }
}