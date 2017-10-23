namespace TicketDesk.Web.Client.Controllers
{
    using Domain;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Context;
    using Fanex.NAP.Core.Web.Session;
    using System.Web.Mvc;

    public class BaseController : Controller
    {
        public BaseController()
        {
            var context = DependencyResolver.Current.GetService<TdDomainContext>();
            NapContextHelper = context.SecurityProvider.NapContextHelper;
        }

        protected IContextHelper NapContextHelper { get; }

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

        protected IUserService UserService => NapContextHelper.UserService;
    }
}