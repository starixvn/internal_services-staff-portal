namespace TicketDesk.Web.Client.Areas.Admin.Models.Announcements
{
    using System.ComponentModel.DataAnnotations;
    using Domain.Model;

    public class NewAnnouncementViewModel
    {
        public Announcement Announcement { get; set; }

        [Display(Name = "Important")]
        public bool IsImportant { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }
    }
}