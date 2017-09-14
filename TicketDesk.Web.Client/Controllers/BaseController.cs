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
        public BaseController()
        {
            NapContextHelper = new ContextHelper();
        }

        protected ContextHelper NapContextHelper { get; private set; }

        protected ICurrentSession CurrentSession
        {
            get { return NapContextHelper.CurrentSession; }

            set { NapContextHelper.CurrentSession = value; }
        }

        protected ICurrentContext CurrentContext
        {
            get { return NapContextHelper.CurrentContext; }

            set { NapContextHelper.CurrentContext = value; }
        }

        protected IUserService UserService
        {
            get { return NapContextHelper.UserService; }
        }
    }
}