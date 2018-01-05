namespace TicketDesk.Web.Client.Areas.Admin.Models.Announcements
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Client.Models.Home;
    using Domain.Model;

    public class AnnouncementViewModel : Pagination
    {
        public IEnumerable<Announcement> Announcements { get; set; }

        [Display(Name = "Published From")]
        public DateTime? PublishedFrom { get; set; }

        [Display(Name = "Published To")]
        public DateTime? PublishedTo { get; set; }

        [Display(Name = "Status")]
        public AnnouncementStatus? Status { get; set; }

        [Display(Name = "State")]
        public AnnouncementState? State { get; set; }
    }
}