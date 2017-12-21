namespace TicketDesk.Web.Client.Models.Admin
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Domain.Model;
    using Home;

    public class AnnouncementViewModel : Pagination
    {
        [Display(Name = "From date")]
        public DateTime FromDate { get; set; }

        [Display(Name = "To date")]
        public DateTime ToDate { get; set; }

        [Display(Name = "Status")]
        public AnnouncementStatus Status { get; set; }

        [Display(Name = "State")]
        public AnnouncementState State { get; set; }

        public IList<Announcement> Announcements { get; set; }
    }
}