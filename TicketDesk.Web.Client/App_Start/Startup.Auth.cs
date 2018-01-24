namespace TicketDesk.Web.Client
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using Identity;
    using Identity.Migrations;
    using Identity.Model;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin;
    using Microsoft.Owin.Security.Cookies;
    using Owin;
    using SimpleInjector;

    //TODO: Language Aware
    // https://stackoverflow.com/questions/21168955/mvc-5-how-to-define-owin-loginpath-with-localized-routes

    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app, Container container)
        {
            // Create a provider to use
            var url = new UrlHelper(HttpContext.Current.Request.RequestContext);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            var options = new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString(url.Action("SignIn", "User")),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.
                    OnValidateIdentity =
                        SecurityStampValidator.OnValidateIdentity<TicketDeskUserManager, TicketDeskUser>(
                            validateInterval: TimeSpan.FromMinutes(30),
                            regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            };
            app.UseCookieAuthentication(options);

            if (DatabaseConfig.IsFirstRunDemoRefreshEnabled())
            {
                DemoIdentityDataManager.SetupDemoIdentityData(container.GetInstance<TdIdentityContext>(), null);
            }
        }
    }
}