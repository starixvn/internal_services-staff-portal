namespace TicketDesk.Web.Client
{
    using Fanex.Data.Configuration;
    using Microsoft.ApplicationInsights.Extensibility;
    using System;
    using System.Diagnostics;
    using App_Start;

    public class Global : System.Web.HttpApplication
    {
        protected static void Application_Start(object sender, EventArgs e)
        {
            DisableApplicationInsightsOnDebug();
            DbSettingProviderManager.Current.Start();
        }

        /// <summary>
        /// Disables the application insights locally.
        /// </summary>
        [Conditional("DEBUG")]
        private static void DisableApplicationInsightsOnDebug()
        {
            TelemetryConfiguration.Active.DisableTelemetry = true;
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // Method intentionally left empty.
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // Method intentionally left empty.
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            AuthConfig.RegisterAuth(Context);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // Method intentionally left empty.
        }

        protected static void Session_End(object sender, EventArgs e)
        {
            AuthConfig.SignOut();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            // Method intentionally left empty.
        }
    }
}