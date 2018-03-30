namespace TicketDesk.Service.Implement
{
    using Domain;
    using Interface;

    public class UserSettingService : EntityFrameworkBaseService, IUserSettingService
    {
        private NapUserSetting GetNapUserSetting(int userId)
        {
            return context.NapUserSettings.Find(userId) ?? new NapUserSetting
            {
                SettingObject = new NapUserSettingObject
                {
                    IsSidebarExpanded = false
                }
            };
        }

        private NapUserSetting CreateNapUserSetting(int userId)
        {
            var currentSetting = GetNapUserSetting(userId);

            if (currentSetting.UserId == 0)
            {
                // Add
                currentSetting.UserId = userId;
                context.NapUserSettings.Add(currentSetting);
                context.SaveChanges();
            }

            return currentSetting;
        }

        private bool UpdateNapUserSetting(NapUserSetting napUserSetting)
        {
            var entry = context.Entry(napUserSetting);
            entry.Property(x => x.JsonSetting).IsModified = true;
            var result = context.SaveChanges();

            return result > 0;
        }

        public bool UpdateIsSideBarExpanded(int userId, bool isSideBarExpanded)
        {
            var currentSetting = CreateNapUserSetting(userId);

            currentSetting.SettingObject.IsSidebarExpanded = isSideBarExpanded;

            return UpdateNapUserSetting(currentSetting);
        }
    }
}