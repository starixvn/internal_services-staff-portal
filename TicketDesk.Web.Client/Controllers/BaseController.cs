namespace TicketDesk.Web.Client.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Security;
    using Domain;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Context;
    using Fanex.NAP.Core.Web.Session;
    using Infrastructure.Extensions;
    using Models.Extensions;

    public class BaseController : Controller
    {
        protected readonly TdDomainContext context;
        protected NapUserSetting NapUserSetting;

        public BaseController()
        {
            context = DependencyResolver.Current.GetService<TdDomainContext>();
            NapContextHelper = context.SecurityProvider.NapContextHelper;
        }

        protected IContextHelper NapContextHelper { get; }

        protected ICurrentSession CurrentSession
        {
            get => NapContextHelper.CurrentSession;
            set => NapContextHelper.CurrentSession = value;
        }


        protected ICurrentContext CurrentContext
        {
            get => NapContextHelper.CurrentContext;
            set => NapContextHelper.CurrentContext = value;
        }


        protected IUserService UserService => NapContextHelper.UserService;

        protected List<SspMenuItem> GetIndexSubMenuItems()
        {
            string routeValue = RouteData.Values["controller"].ToString().ToLower(CultureInfo.InvariantCulture);

            // Get the correct parent menu id
            SspMenuItem parentSspMenu = CurrentSession.MenuItemList.First(x =>
                x.Parent == null && x.Url.Contains(routeValue, StringComparison.OrdinalIgnoreCase));

            var subMenuItems = CurrentSession.MenuItemList.Where(x => x.Parent != null && x.Parent.Id == parentSspMenu.Id)
                .ToList();

            return subMenuItems.Select(menuItem => (SspMenuItem)menuItem).OrderBy(x => x.DisplayOrder).ToList();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (CurrentSession.UserId <= 0 && !IsCurrentRouteSignIn())
            {
                FormsAuthentication.SignOut();
                var url = Url.Action("SignIn", "User", new { returnUrl = Request.Url?.AbsolutePath, Area = "" });

                filterContext.Result = Redirect(url);
            }
            else
            {
                NapUserSetting = context.NapUserSettings.Find(CurrentSession.UserId);
                if (NapUserSetting == null)
                {
                    NapUserSetting = new NapUserSetting
                    {
                        SettingObject = new NapUserSettingObject
                        {
                            IsSidebarExpanded = false
                        }
                    };
                }
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

        protected void ReValidateModelState(object viewModel)
        {
            ModelState.Clear();
            TryValidateModel(viewModel);
        }

        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                    viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                    ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}