// TicketDesk - Attribution notice
// Contributor(s):
//
//      Stephen Redd (https://github.com/stephenredd)
//
// This file is distributed under the terms of the Microsoft Public
// License (Ms-PL). See http://opensource.org/licenses/MS-PL
// for the complete terms of use.
//
// For any distribution that contains code from this file, this notice of
// attribution must remain intact, and a copy of the license must be
// provided to the recipient.

namespace TicketDesk.Domain
{
    using Fanex.NAP.Core.Business;
    using Infrastructure;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TicketDesk.Domain.Model;

    public class UserSettingsManager
    {
        public UserSettingsManager(TdDomainContext context)
        {
            Context = context;
        }

        private TdDomainContext Context { get; set; }

        public async Task ResetAllListSettingsForUserAsync(string userId)
        {
            var settings = await GetSettingsForUserAsync(userId);

            settings.ListSettings = new UserTicketListSettingsCollection
            {
                UserTicketListSetting.GetDefaultListSettings(userId, Context.SecurityProvider.NapContextHelper.HasRight(FunctionKeys.ListAssignedTicket, Constant.Policy.Read))
            };
        }

        public async Task<UserSetting> GetSettingsForUserAsync(string userId)
        {
            // Role
            // var isHelpDeskUser = Context.SecurityProvider.IsTdHelpDeskUser || Context.SecurityProvider.IsTdAdministrator;

            var isShowListAssignedTicket = Context.SecurityProvider.NapContextHelper.HasRight(FunctionKeys.ListAssignedTicket, Constant.Policy.Read);
            var settings = await Context.UserSettings.FindAsync(userId);

            // ensure settings exist
            if (settings == null)
            {
                settings = UserSetting.GetDefaultSettingsForUser(userId, isShowListAssignedTicket);
                using (var tempCtx = new TdDomainContext())
                {
                    await tempCtx.UserSettingsManager.AddOrUpdateSettingsForUser(settings);
                    await tempCtx.SaveChangesAsync();
                }
            }

            // ensure that the user has all required lists for their role, if not blow away list settings and recreate
            if (!settings.ListSettings.HasRequiredDefaultListSettings(isShowListAssignedTicket))
            {
                settings.ListSettings = new UserTicketListSettingsCollection
                {
                    UserTicketListSetting.GetDefaultListSettings(userId, isShowListAssignedTicket)
                };
                using (var tempCtx = new TdDomainContext())
                {
                    await tempCtx.UserSettingsManager.AddOrUpdateSettingsForUser(settings);
                    await tempCtx.SaveChangesAsync();
                }
            }

            return settings;
        }

        public async Task AddOrUpdateSettingsForUser(UserSetting settings)
        {
            var existing = await Context.UserSettings.FindAsync(settings.UserId);
            if (existing != null)
            {
                // replace existing with new settings
                // ReSharper disable once RedundantAssignment
                existing = settings;
            }
            else
            {
                Context.UserSettings.Add(settings);
            }
        }

        public async Task<ICollection<UserTicketListSetting>> GetUserListSettingsAsync(string userId)
        {
            var settings = await GetSettingsForUserAsync(userId);
            return settings.ListSettings;
        }

        public async Task<UserTicketListSetting> GetUserListSettingByNameAsync(string listName, string userId)
        {
            var settings = await GetSettingsForUserAsync(userId);
            return settings.GetUserListSettingByName(listName);
        }
    }
}