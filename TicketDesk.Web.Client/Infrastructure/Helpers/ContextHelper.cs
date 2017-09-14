namespace TicketDesk.Web.Client.Infrastructure.Helpers
{
    using Domain;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Context;
    using Fanex.NAP.Core.Web.Session;

    public class ContextHelper : IContextHelper
    {
        public ContextHelper()
        {
            CurrentSession = new CurrentSession();
            CurrentContext = new CurrentContext();
            UserService = new UserService();
        }

        public ICurrentSession CurrentSession { get; set; }

        public ICurrentContext CurrentContext { get; set; }

        public IUserService UserService { get; }

        public int SiteId => NapSettings.Settings.SiteId;

        public string Environment => NapSettings.Settings.Environment;

        public bool HasRight(string functionKey, Constant.Policy policy)
        {
            int functionPolicy = UserService.GetUserSiteFunctionKeyPolicy(
                CurrentSession.UserId,
                SiteId,
                functionKey);

            return functionPolicy >= (int)policy;
        }
    }
}