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
        private readonly ICurrentSession _currentSession;
        private readonly ICurrentContext _currentContext;
        private readonly IUserService _userService;

        public ContextHelper(
            ICurrentSession currentSession,
            ICurrentContext currentContext,
            IUserService userService)
        {
            _currentSession = currentSession;
            _currentContext = currentContext;
            _userService = userService;
        }

        public int SiteId => NapSettings.Settings.SiteId;

        public string Environment => NapSettings.Settings.Environment;

        public ICurrentSession CurrentSession => _currentSession;

        public ICurrentContext CurrentContext => _currentContext;

        public IUserService UserService => _userService;

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