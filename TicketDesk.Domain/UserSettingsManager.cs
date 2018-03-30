////namespace TicketDesk.Domain
////{
////    using Fanex.NAP.Core.Business;
////    using Infrastructure;
////    using Model;
////    using System.Collections.Generic;
////    using System.Threading.Tasks;

////    public class UserSettingsManager
////    {
////        public UserSettingsManager(TdDomainContext context)
////        {
////            Context = context;
////        }

////        private TdDomainContext Context { get; }

////        public async Task ResetAllListSettingsForUserAsync(string userId)
////        {
////            var settings = await GetSettingsForUserAsync(userId).ConfigureAwait(false);

////            settings.ListSettings = new UserTicketListSettingsCollection
////            {
////                UserTicketListSetting.GetDefaultListSettings(userId, Context.SecurityProvider.NapContextHelper.HasRight(FunctionKeys.ListAssignedTicket, Constant.Policy.Read))
////            };
////        }

////        public async Task<UserSetting> GetSettingsForUserAsync(string userId)
////        {
////            // Role
////            var isShowListAssignedTicket = Context.SecurityProvider.NapContextHelper.HasRight(FunctionKeys.ListAssignedTicket, Constant.Policy.Read);
////            var settings = await Context.UserSettings.FindAsync(userId).ConfigureAwait(false);

////            // ensure settings exist
////            if (settings == null)
////            {
////                settings = UserSetting.GetDefaultSettingsForUser(userId, isShowListAssignedTicket);
////                using (var tempCtx = new TdDomainContext())
////                {
////                    await tempCtx.UserSettingsManager.AddOrUpdateSettingsForUser(settings).ConfigureAwait(false);
////                    await tempCtx.SaveChangesAsync().ConfigureAwait(false);
////                }
////            }

////            // ensure that the user has all required lists for their role, if not blow away list settings and recreate
////            if (!settings.ListSettings.HasRequiredDefaultListSettings(isShowListAssignedTicket))
////            {
////                settings.ListSettings = new UserTicketListSettingsCollection
////                {
////                    UserTicketListSetting.GetDefaultListSettings(userId, isShowListAssignedTicket)
////                };
////                using (var tempCtx = new TdDomainContext())
////                {
////                    await tempCtx.UserSettingsManager.AddOrUpdateSettingsForUser(settings).ConfigureAwait(false);
////                    await tempCtx.SaveChangesAsync().ConfigureAwait(false);
////                }
////            }

////            return settings;
////        }

////        public async Task AddOrUpdateSettingsForUser(UserSetting settings)
////        {
////            var existing = await Context.UserSettings.FindAsync(settings.UserId).ConfigureAwait(false);

////            if (existing == null)
////            {
////                Context.UserSettings.Add(settings);
////            }
////        }

////        public async Task<ICollection<UserTicketListSetting>> GetUserListSettingsAsync(string userId)
////        {
////            var settings = await GetSettingsForUserAsync(userId).ConfigureAwait(false);
////            return settings.ListSettings;
////        }

////        public async Task<UserTicketListSetting> GetUserListSettingByNameAsync(string listName, string userId)
////        {
////            var settings = await GetSettingsForUserAsync(userId).ConfigureAwait(false);
////            return settings.GetUserListSettingByName(listName);
////        }
////    }
////}