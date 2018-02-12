namespace TicketDesk.Web.Client.Areas.Admin.Models.Announcements
{
    using System.Collections.Generic;
    using Domain.Model;

    public class ListAnnouncementViewModel
    {
        public IList<Announcement> Announcements { get; set; }

        public bool IsPublishImmediately { get; set; }
    }
}