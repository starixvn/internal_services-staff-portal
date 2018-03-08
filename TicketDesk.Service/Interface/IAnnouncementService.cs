namespace TicketDesk.Service.Interface
{
    using System.Collections.Generic;
    using Criteria.Announcement;
    using Domain.Model;

    public interface IAnnouncementService
    {
        Announcement GetAnnouncement(int id);

        IEnumerable<Announcement> GetAnnouncements(GetAnnouncementsCriteria criteria);

        bool AddAnnouncement(NewAnnouncementCriteria criteria);

        bool UpdateAnnouncement(NewAnnouncementCriteria criteria);

        bool PublishAnnouncements(PublishAnnouncementsCriteria criteria);

        bool DeleteAnnouncement(int id);

        int[] GetAllAnnouncementIds();

        int CountUnreadAnnouncement(string readAnnouncements);

        IEnumerable<Announcement> GetHomeAnnouncements();

        IEnumerable<Announcement> CalculateAnnouncementsReadStatus(IEnumerable<Announcement> announcements, string readAnnouncements);
    }
}