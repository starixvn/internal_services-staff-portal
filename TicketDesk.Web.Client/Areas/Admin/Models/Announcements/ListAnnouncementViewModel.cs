namespace TicketDesk.Web.Client.Areas.Admin.Models.Announcements
{
    using System.Collections.Generic;
    using Domain.Model;

    public class ListAnnouncementViewModel
    {
        public IEnumerable<Announcement> Announcements { get; set; }

        public bool HaveHostPermission { get; set; }
    }
}