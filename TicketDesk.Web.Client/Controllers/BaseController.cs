namespace TicketDesk.Web.Client.Controllers
{
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Context;
    using Fanex.NAP.Core.Web.Session;
    using Infrastructure.Helpers;
    using System.Web.Mvc;

    public class BaseController : Controller
    {
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
            UserService = userService ?? new UserService();

            NapContextHelper = new ContextHelper(CurrentSession, CurrentContext, UserService);
        }

        protected ContextHelper NapContextHelper { get; }

        protected ICurrentSession CurrentSession { get; }

        protected ICurrentContext CurrentContext { get; }

        protected IUserService UserService { get; }
    }
}