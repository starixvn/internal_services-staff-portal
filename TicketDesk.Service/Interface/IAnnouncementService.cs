namespace TicketDesk.Service.Interface
{
    using System.Linq;
    using Criteria.Announcement;
    using Domain.Model;

    public interface IAnnouncementService
    {
        Announcement GetAnnouncement(int id);

        IQueryable<Announcement> GetAnnouncements(GetAnnouncementsCriteria criteria);

        bool AddAnnouncement(NewAnnouncementCriteria criteria);

        bool UpdateAnnouncement(NewAnnouncementCriteria criteria);

        bool DeleteAnnouncement(int id);

        IQueryable<Announcement> Pagination(IQueryable<Announcement> originalList, int skipped, int took);
    }
}
