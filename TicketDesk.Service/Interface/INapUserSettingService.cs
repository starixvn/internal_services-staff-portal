namespace TicketDesk.Service.Interface
{
    public interface INapUserSettingService
    {
        bool UpdateIsSideBarExpanded(int userId, bool isSideBarExpanded);

        bool UpdateUnreadAnnouncements(int userId, int alreadyReadAnnouncementId);
    }
}
