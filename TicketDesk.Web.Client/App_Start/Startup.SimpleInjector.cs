namespace TicketDesk.Web.Client
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using Domain;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Identity;
    using Identity.Model;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin;
    using Owin;
    using Service.Implement;
    using Service.Interface;
    using SimpleInjector;
    using SimpleInjector.Advanced;
    using SimpleInjector.Integration.Web;
    using SimpleInjector.Integration.Web.Mvc;

    public partial class Startup
    {
        public static Container RegisterStructureMap(IAppBuilder app)
        {
            var container = GetInitializedContainer(app);

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));

            return container;
        }

        public static Container GetInitializedContainer(IAppBuilder app)
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();
            container.RegisterSingleton(app);

            // allows objects to be reused when inside web request, or created fresh when used on background threads or outside a request context
            var hybridLifestyle = Lifestyle.CreateHybrid(() => HttpContext.Current != null, new WebRequestLifestyle(), Lifestyle.Transient);

            container.Register<TicketDeskContextSecurityProvider>(Lifestyle.Scoped);
            container.Register(
                () => HttpContext.Current != null ? new TdDomainContext(container.GetInstance<TicketDeskContextSecurityProvider>()) : new TdDomainContext(),
                hybridLifestyle);

            container.Register(() => new TdIdentityContext(), hybridLifestyle);

            container.Register<IUserStore<TicketDeskUser>>(() => new UserStore<TicketDeskUser>(container.GetInstance<TdIdentityContext>()), hybridLifestyle);

            container.Register<IRoleStore<TicketDeskRole, string>>(
                () => new RoleStore<TicketDeskRole>(container.GetInstance<TdIdentityContext>()),
                hybridLifestyle);

            container.Register(
                () =>
                {
                    IOwinContext context;
                    try
                    {
                        context = HttpContext.Current.GetOwinContext();
                    }
                    catch (InvalidOperationException)
                    {
                        // avoid exception when this is called before the owin environment is fully initialized
                        if (container.IsVerifying())
                        {
                            return new FakeAuthenticationManager();
                        }

                        throw;
                    }

                    return context.Authentication;
                },
                Lifestyle.Scoped);

            container.Register<SignInManager<TicketDeskUser, string>, TicketDeskSignInManager>(Lifestyle.Scoped);
            container.Register<TicketDeskRoleManager>(hybridLifestyle);

            container.Register<ITicketService>(() => new TicketService(new MailService(), new FileManagerService()), hybridLifestyle);

            container.Register<IUserService>(() => new UserService(), hybridLifestyle);
            container.Register<IGroupService>(() => new GroupService(), hybridLifestyle);
            container.Register<ISiteService>(() => new SiteService(), hybridLifestyle);
            container.Register<IMenuItemService>(() => new MenuItemService(), hybridLifestyle);
            container.Register<IFunctionService>(() => new FunctionService(), hybridLifestyle);
            container.Register<IFileManagerService>(() => new FileManagerService(), hybridLifestyle);

            container.Register<IAnnouncementService>(() => new AnnouncementService(), hybridLifestyle);
            container.Register<IReferenceService>(() => new ReferenceService(), hybridLifestyle);
            container.Register<IDataConfigurationService>(() => new DataConfigurationService(), hybridLifestyle);
            container.Register<IShopService>(() => new ShopService(), hybridLifestyle);
            container.Register<IIdUploaderService>(() => new IdUploaderService(), hybridLifestyle);
            container.Register<INapUserSettingService>(() => new NapUserSettingService(), hybridLifestyle);
            container.Register<ICustomFieldService>(() => new CustomFieldService(), hybridLifestyle);
            container.Register<ITicketCategoryService>(() => new TicketCategoryService(), hybridLifestyle);
            container.Register<IMailService>(() => new MailService(), hybridLifestyle);

            container.RegisterInitializer<TicketDeskUserManager>(manager => manager.ConfigureDataProtection(app));
            ////container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            container.RegisterMvcControllers(typeof(Startup).Assembly);

            return container;
        }
    }
}