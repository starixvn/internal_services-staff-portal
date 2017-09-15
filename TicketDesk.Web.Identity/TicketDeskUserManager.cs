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
    using Domain.Infrastructure;
    using Domain;
    using Fanex.NAP.Core.Business;

    public class TicketDeskUserManager : UserManager<TicketDeskUser>
    {
        public TicketDeskUserManager(IUserStore<TicketDeskUser> store)
            : base(store)
        {
            ConfigureUserManager();

            //NAP configure
            NapContextHelper = new ContextHelper();
            NapCurrentUserId = NapContextHelper.CurrentSession.UserId;
            CurrentUserId = NapCurrentUserId.ToString();
        }

        public IContextHelper NapContextHelper { get; }

        public int NapCurrentUserId { get; }

        public string CurrentUserId { get; }

        public UserDisplayInfoCache InfoCache
        { get { return new UserDisplayInfoCache(this); } }

        public bool IsTdHelpDeskUser(string userId)
        {
            return IsHasTicketManagementPolicy(userId, (int)Constant.Policy.Host);
        }

        public bool IsTdInternalUser(string userId)
        {
            return IsHasTicketManagementPolicy(userId, (int)Constant.Policy.Read);
        }

        public bool IsTdAdministrator(string userId)
        {
            return IsTdHelpDeskUser(userId);
        }

        public bool IsTdPendingUser(string userId)
        {
            return false;
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

        private bool IsHasTicketManagementPolicy(string userId, int policy)
        {
            int functionPolicy = NapContextHelper.UserService.GetUserSiteFunctionKeyPolicy(
                int.Parse(userId),
                NapContextHelper.SiteId,
                FunctionKeys.TicketManagement);

            return functionPolicy >= policy;
        }
    }
}