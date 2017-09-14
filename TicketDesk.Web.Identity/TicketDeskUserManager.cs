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

namespace TicketDesk.Web.Identity
{
    using System;
    using Microsoft.AspNet.Identity;
    using TicketDesk.Web.Identity.Model;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Configuration;

    public class TicketDeskUserManager : UserManager<TicketDeskUser>
    {
        public TicketDeskUserManager(IUserStore<TicketDeskUser> store)
            : base(store)
        {
            ConfigureUserManager();
            UserService = new UserService();
            FunctionService = new FunctionService();
            SiteId = NapSettings.Settings.SiteId;
        }

        public IUserService UserService { get; private set; }

        public IFunctionService FunctionService { get; private set; }

        public int SiteId { get; private set; }

        public UserDisplayInfoCache InfoCache
        { get { return new UserDisplayInfoCache(this); } }

        public bool IsTdHelpDeskUser(string userId)
        {
            return this.IsInRole(userId, "TdHelpDeskUsers") || IsTdAdministrator(userId);
        }

        public bool IsTdInternalUser(string userId)
        {
            return this.IsInRole(userId, "TdInternalUsers") || IsTdHelpDeskUser(userId);
        }

        public bool IsTdAdministrator(string userId)
        {
            return this.IsInRole(userId, "TdAdministrators");
        }

        public bool IsTdPendingUser(string userId)
        {
            return this.IsInRole(userId, "TdPendingUsers");
        }

        private void ConfigureUserManager()
        {
            UserValidator = new UserValidator<TicketDeskUser>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            PasswordValidator = new PasswordValidator
            {
                RequiredLength = 5,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            UserLockoutEnabledByDefault = true;
            DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            MaxFailedAccessAttemptsBeforeLockout = 5;
        }
    }
}