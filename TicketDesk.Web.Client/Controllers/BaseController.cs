namespace TicketDesk.Web.Client.Controllers
{
    using Fanex.NAP.Core.Model;
    using Fanex.NAP.Core.Web.Context;
    using Fanex.NAP.Core.Web.Session;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    public class BaseController : Controller
    {
        private readonly ICurrentSession currentSession;

        private readonly ICurrentContext currentContext;

        public BaseController()
        {
            this.currentSession = new CurrentSession();
            this.currentContext = new CurrentContext();
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