namespace TicketDesk.Web.Client.Areas.Admin.Models.Mail
{
    using Domain.Model;

    public class MailListViewModel
    {
        public TicketCategory SelectedTicketCategory { get; set; }

        public bool IsSuccess { get; set; }
    }
}