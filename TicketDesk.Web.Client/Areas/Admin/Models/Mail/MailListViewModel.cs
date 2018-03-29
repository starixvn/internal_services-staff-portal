namespace TicketDesk.Web.Client.Areas.Admin.Models.Mail
{
    using Domain.Model;

    public class MailListViewModel
    {
        public TicketCategory SelectedTicketCategory { get; set; }

        public bool IsSuccessful { get; set; }

        public bool IsHaveHostPermission { get; set; }
    }
}