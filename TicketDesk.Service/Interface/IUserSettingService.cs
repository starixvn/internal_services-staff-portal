namespace TicketDesk.Service.Interface
{
    public interface IUserSettingService
    {
        bool UpdateIsSideBarExpanded(int userId, bool isSideBarExpanded);
    }
}
