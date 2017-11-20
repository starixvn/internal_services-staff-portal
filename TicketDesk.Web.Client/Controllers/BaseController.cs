namespace TicketDesk.Web.Client.Controllers
{
    using Domain;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Context;
    using Fanex.NAP.Core.Web.Session;
    using Infrastructure.Extensions;
    using Models.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Security;

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

        protected List<MenuItem> GetIndexSubMenuItems()
        {
            string routeValue = RouteData.Values["controller"].ToString().ToLower(CultureInfo.InvariantCulture);

            // Get the correct parent menu id
            MenuItem parentMenu = CurrentSession.MenuItemList.First(x =>
                x.Parent == null && x.Url.Contains(routeValue, StringComparison.OrdinalIgnoreCase));

            var subMenuItems = CurrentSession.MenuItemList.Where(x => x.Parent != null && x.Parent.Id == parentMenu.Id)
                .ToList();

            return subMenuItems.Select(menuItem => (MenuItem)menuItem).OrderBy(x => x.DisplayOrder).ToList();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (CurrentSession.UserId <= 0 && !IsCurrentRouteSignIn())
            {
                FormsAuthentication.SignOut();
                var url = Url.Action("SignIn", "User", new { returnUrl = Request.Url?.AbsolutePath });

                filterContext.Result = Redirect(url);
            }
        }

        private bool IsCurrentRouteSignIn()
        {
            var signInRoute = Url.Action("SignIn", "User");
            if (Request.Url == null)
            {
                return false;
            }

            return signInRoute == Request.Url.AbsolutePath;
        }
    }
}