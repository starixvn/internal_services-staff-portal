namespace TicketDesk.Web.Client.Areas.Admin.Models.Announcements
{
    using System;
    using System.Collections.Generic;
    using Client.Models.Home;
    using Domain.Model;

    public class AnnouncementViewModel : Pagination
    {
        public IEnumerable<Announcement> Announcements { get; set; }

        public DateTime? PublishedFrom { get; set; }

        public DateTime? PublishedTo { get; set; }

        public AnnouncementStatus Status { get; set; }

        public AnnouncementState State { get; set; }
    }
}