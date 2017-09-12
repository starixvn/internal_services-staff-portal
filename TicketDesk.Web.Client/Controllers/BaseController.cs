namespace TicketDesk.Web.Client.Controllers
{
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Model;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Context;
    using Fanex.NAP.Core.Web.Session;
    using System.Web.Mvc;

    public class BaseController : Controller
    {
        private readonly ICurrentSession currentSession;
        private readonly ICurrentContext currentContext;
        private readonly IUserService userService;

        public BaseController()
        {
            this.currentSession = new CurrentSession();
            this.currentContext = new CurrentContext();
            this.userService = new UserService();
        }

        protected ICurrentSession CurrentSession
        {
            get
            {
                return currentSession;
            }
        }

        protected ICurrentContext CurrentContext
        {
            get
            {
                return currentContext;
            }
        }

        protected int SiteId
        {
            get
            {
                return NapSettings.Settings.SiteId;
            }
        }

        protected string Environment
        {
            get
            {
                return NapSettings.Settings.Environment;
            }
        }

        public bool HasRight(string functionKey, Constant.Policy policy)
        {
            int functionPolicy = userService.GetUserSiteFunctionKeyPolicy(
                CurrentSession.UserId,
                SiteId,
                functionKey);

            return functionPolicy >= (int)policy;
        }

        protected void SaveUserSession(User user, int onlineUserId, int level)
        {
            currentSession.UserId = user.Id;
            currentSession.Username = user.Username;
            currentSession.OnUserId = onlineUserId;
            currentSession.SessionId = Session.SessionID;
            currentSession.Level = level;
        }
    }
}