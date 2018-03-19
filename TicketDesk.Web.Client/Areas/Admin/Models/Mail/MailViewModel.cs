namespace TicketDesk.Web.Client.Areas.Admin.Models.Mail
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Domain.Model;

    public class MailViewModel
    {
        [Display(Name = "Ticket Type")]
        public int CategoryId { get; set; }

        public IEnumerable<TicketCategory> TicketCategories { get; set; }
    }

    public class ListMailViewModel
    {
        public TicketCategory SelectedTicketCategory { get; set; }
    }
}