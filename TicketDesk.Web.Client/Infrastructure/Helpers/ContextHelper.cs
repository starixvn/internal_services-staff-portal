namespace TicketDesk.Web.Client.Infrastructure.Helpers
{
    using Domain;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Context;
    using Fanex.NAP.Core.Web.Session;

    public class ContextHelper : IContextHelper
    {
        public ContextHelper(
            ICurrentSession currentSession,
            ICurrentContext currentContext,
            IUserService userService)
        {
            _currentSession = currentSession;
            _currentContext = currentContext;
            _userService = userService;
        }

        private readonly ICurrentSession _currentSession;

        private readonly ICurrentContext _currentContext;

        private readonly IUserService _userService;

        public ICurrentSession CurrentSession()
        {
            return _currentSession;
        }

        public ICurrentContext CurrentContext()
        {
            return _currentContext;
        }

        public IUserService UserService()
        {
            return _userService;
        }

        public int GetSiteId() => NapSettings.Settings.SiteId;

        public string GetEnvironment() => NapSettings.Settings.Environment;

        public bool HasRight(string functionKey, Constant.Policy policy)
        {
            int functionPolicy = UserService().GetUserSiteFunctionKeyPolicy(
                CurrentSession().UserId,
                GetSiteId(),
                functionKey);

            return functionPolicy >= (int)policy;
        }
    }
}