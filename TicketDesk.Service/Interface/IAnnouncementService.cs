namespace TicketDesk.Service.Interface
{
    using System.Collections.Generic;
    using Criteria.Announcement;
    using Domain.Model;

    public interface IAnnouncementService
    {
        Announcement GetAnnouncement(int id);

        IEnumerable<Announcement> GetAnnouncements(GetAnnouncementsCriteria criteria);

        bool AddAnnouncement(Announcement announcement);

        bool UpdateAnnouncement(Announcement announcement);

        bool PublishAnnouncements(IList<int> ids);

        bool DeleteAnnouncement(int id);

        IEnumerable<Announcement> GetHomeAnnouncements();

        bool SetAnnouncementReadStatus(int announcementId, int userId, bool isRead = true);
    }
}