namespace TicketDesk.Web.Client.Areas.Admin.Models.Announcements
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Domain.Model;

    public class NewAnnouncementViewModel
    {
        public Announcement Announcement { get; set; }

        [Display(Name = "Important")]
        public bool IsImportant { get; set; }

        public DateTime? Date { get; set; }

        public TimeSpan? Time { get; set; }
    }
}