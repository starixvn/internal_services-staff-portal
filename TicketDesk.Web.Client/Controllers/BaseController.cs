namespace TicketDesk.Web.Client.Controllers
{
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Context;
    using Fanex.NAP.Core.Web.Session;
    using System.Web.Mvc;

    public class BaseController : Controller
    {
        private readonly IUserService _userService;

        public BaseController() : this(null, null, null)
        {
        }

        public BaseController(
            ICurrentSession currentSession,
            ICurrentContext currentContext,
            IUserService userService)
        {
            CurrentSession = currentSession ?? new CurrentSession();
            CurrentContext = currentContext ?? new CurrentContext();
            _userService = userService ?? new UserService();
        }

        protected ICurrentSession CurrentSession { get; }

        protected ICurrentContext CurrentContext { get; }

        protected IUserService UserService => _userService;

        protected int SiteId => NapSettings.Settings.SiteId;

        protected string Environment => NapSettings.Settings.Environment;

        protected bool HasRight(string functionKey, Constant.Policy policy)
        {
            int functionPolicy = UserService.GetUserSiteFunctionKeyPolicy(
                CurrentSession.UserId,
                SiteId,
                functionKey);

            return functionPolicy >= (int)policy;
        }
    }
}