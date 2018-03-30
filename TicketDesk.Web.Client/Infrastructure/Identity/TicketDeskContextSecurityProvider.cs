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

        public override bool IsTdHelpDeskUser => UserManager.IsTdHelpDeskUser(CurrentUserId);

        public override bool IsTdInternalUser => UserManager.IsTdInternalUser(CurrentUserId);

        public override bool IsTdAdministrator => UserManager.IsTdAdministrator(CurrentUserId);

        public override bool IsTdPendingUser => UserManager.IsTdPendingUser(CurrentUserId);

        public override bool IsReadTicketCenter => IsHasTicketPolicy(FunctionKeys.Ticket, (int)Constant.Policy.Read);

        public override bool IsHostTicketCenter => IsHasTicketPolicy(FunctionKeys.Ticket, (int)Constant.Policy.Host);

        public override bool IsReadNewTicket => IsHasTicketPolicy(FunctionKeys.Ticket, (int)Constant.Policy.Read);

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