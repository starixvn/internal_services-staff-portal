namespace TicketDesk.Service.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Domain;
    using Interface;

    public class NapUserSettingService : EntityFrameworkBaseService, INapUserSettingService
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

        public bool UpdateUnreadAnnouncements(int userId, int alreadyReadAnnouncementId)
        {
            var currentSetting = CreateNapUserSetting(userId);

            var readAnnouncementIds = new List<int>();
            if (!string.IsNullOrEmpty(currentSetting.SettingObject.ReadAnnouncements))
            {
                readAnnouncementIds = currentSetting.SettingObject.ReadAnnouncements
                    .Split(',')
                    .Distinct()
                    .Select(int.Parse)
                    .ToList();

                if (!readAnnouncementIds.Contains(alreadyReadAnnouncementId))
                {
                    readAnnouncementIds.Add(alreadyReadAnnouncementId);
                    readAnnouncementIds.Sort();
                }
            }

            currentSetting.SettingObject.ReadAnnouncements = string.Join(",", readAnnouncementIds);

            return UpdateNapUserSetting(currentSetting);
        }
    }
}