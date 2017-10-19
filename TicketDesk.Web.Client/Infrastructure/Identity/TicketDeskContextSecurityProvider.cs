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

namespace TicketDesk.Web.Client
{
    using Domain.Infrastructure;
    using Fanex.NAP.Core.Business;
    using Microsoft.Ajax.Utilities;
    using System;
    using System.Web.Mvc;
    using Domain;
    using Identity;

    public sealed class TicketDeskContextSecurityProvider : TdDomainSecurityProviderBase
    {
        public TicketDeskContextSecurityProvider()
        {
            UserManager = DependencyResolver.Current.GetService<TicketDeskUserManager>();

            NapContextHelper = UserManager.NapContextHelper;
            NapCurrentUserId = UserManager.NapCurrentUserId;
            CurrentUserId = UserManager.CurrentUserId;
        }

        private TicketDeskUserManager UserManager { get; }

        public override IContextHelper NapContextHelper { get; set; }

        public override int NapCurrentUserId { get; }

        public override string CurrentUserId { get; }

        public override Func<string, string> GetUserDisplayName
        {
            get
            {
                return userId => UserManager.InfoCache.GetUserInfo(userId).IfNotNull(i => i.DisplayName);
            }
        }

        public override bool IsTdHelpDeskUser
        {
            get { return UserManager.IsTdHelpDeskUser(CurrentUserId); }
        }

        public override bool IsTdInternalUser
        {
            get { return UserManager.IsTdInternalUser(CurrentUserId); }
        }

        public override bool IsTdAdministrator
        {
            get { return UserManager.IsTdAdministrator(CurrentUserId); }
        }

        public override bool IsTdPendingUser
        {
            get { return UserManager.IsTdPendingUser(CurrentUserId); }
        }

        public override bool IsReadTicketCenter
        {
            get
            {
                return IsHasTicketPolicy(FunctionKeys.TicketCenter, (int)Constant.Policy.Read);
            }
        }

        public override bool IsHostTicketCenter
        {
            get
            {
                return IsHasTicketPolicy(FunctionKeys.TicketCenter, (int)Constant.Policy.Host);
            }
        }

        public override bool IsReadNewTicket
        {
            get
            {
                return IsHasTicketPolicy(FunctionKeys.NewTicket, (int)Constant.Policy.Read);
            }
        }

        private bool IsHasTicketPolicy(string functionKey, int policy)
        {
            int functionPolicy = NapContextHelper.UserService.GetUserSiteFunctionKeyPolicy(
                NapCurrentUserId,
                NapContextHelper.SiteId,
                functionKey);

            return functionPolicy >= policy;
        }
    }
}