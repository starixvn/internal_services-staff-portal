namespace TicketDesk.Web.Client.Models.Home
{
    using System.Collections.Generic;
    using Domain.Model;

    public class HomeViewModel
    {
        ////public AnnouncementViewModel Annoucements { get; set; }

        public int UnreadAnnouncementQuatity { get; set; }

        public IList<AnnouncementGroup> AnnouncementGroups { get; set; }
    }
}