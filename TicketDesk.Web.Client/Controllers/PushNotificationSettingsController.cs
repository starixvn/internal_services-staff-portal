// TicketDesk - Attribution notice
// Contributor(s):
//
//      Stephen Redd (https://github.com/stephenredd)
//
// This file is distributed under the terms of the Microsoft Public
// License (Ms-PL). See http://opensource.org/licenses/MS-PL
// for the complete terms of use.
//
// For any distribution that contains code from this file, this notice of
// attribution must remain intact, and a copy of the license must be
// provided to the recipient.

namespace TicketDesk.Web.Client.Controllers
{
    using Domain.Infrastructure;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Domain;
    using Domain.Model;
    using PushNotifications;
    using PushNotifications.Delivery;
    using PushNotifications.Model;

    [RoutePrefix("admin/push-notification-settings")]
    [Route("{action=index}")]
    public class PushNotificationSettingsController : BaseController
    {
        public PushNotificationSettingsController(TdPushNotificationContext noteContext, TdDomainContext domainContext)
        {
            ViewBag.Saved = false;

            NoteContext = noteContext;
            DomainContext = domainContext;
        }

        private TdPushNotificationContext NoteContext { get; }

        private TdDomainContext DomainContext { get; }

        [NapAuth(FunctionKey = FunctionKeys.PushNotificationSettings, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            ViewBag.CurrentRootUrl = GetCurrentRootUrl();
            ViewBag.SiteRootUrl = GetRootUrlSetting();
            ViewBag.IsHost = NapContextHelper.HasRight(FunctionKeys.PushNotificationSettings, Constant.Policy.Host);

            var databaseSetting = NoteContext.TicketDeskPushNotificationSettings;

            return View(databaseSetting);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.PushNotificationSettings, Policy = Constant.Policy.Host)]
        public ActionResult Index(ApplicationPushNotificationSetting settings, string siteRootUrl)
        {
            var databaseSetting = NoteContext.TicketDeskPushNotificationSettings;
            if (ModelState.IsValid && TryUpdateModel(databaseSetting))
            {
                NoteContext.SaveChanges();
                DomainContext.TicketDeskSettings.ClientSettings.Settings["DefaultSiteRootUrl"] = siteRootUrl.TrimEnd('/');
                DomainContext.SaveChanges();
                ViewBag.Saved = true;
            }

            ViewBag.CurrentRootUrl = GetCurrentRootUrl();
            ViewBag.SiteRootUrl = GetRootUrlSetting();
            ViewBag.IsHost = NapContextHelper.HasRight(FunctionKeys.PushNotificationSettings, Constant.Policy.Host);

            Task.Delay(500).ContinueWith(t => System.Web.HttpRuntime.UnloadAppDomain()).ConfigureAwait(false);

            return View(databaseSetting);
        }

        [Route("list-providers")]
        [NapAuth(FunctionKey = FunctionKeys.PushNotificationSettings, Policy = Constant.Policy.Read)]
        public ActionResult ListDeliveryProviderSettings()
        {
            var interfaceType = typeof(IPushNotificationDeliveryProvider);
            var classes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => !t.IsAbstract && interfaceType.IsAssignableFrom(t)).ToList();

            var model = new Dictionary<Type, ApplicationPushNotificationSetting.PushNotificationDeliveryProviderSetting>();

            foreach (var c in classes)
            {
                var settings = NoteContext.TicketDeskPushNotificationSettings
                    .DeliveryProviderSettings
                    .FirstOrDefault(p => p.ProviderAssemblyQualifiedName == c.AssemblyQualifiedName);
                model.Add(c, settings ?? new ApplicationPushNotificationSetting.PushNotificationDeliveryProviderSetting());
            }

            ViewBag.IsHost = NapContextHelper.HasRight(FunctionKeys.PushNotificationSettings, Constant.Policy.Host);

            return PartialView(model);
        }

        [Route("configure-provider")]
        [NapAuth(FunctionKey = FunctionKeys.PushNotificationSettings, Policy = Constant.Policy.Read)]
        public ActionResult ConfigureDeliveryProvider(string providerAssemblyQualifiedName)
        {
            var provider = GetProvider(providerAssemblyQualifiedName);
            var settings = GetOrCreatePushNotificationDeliveryProviderSettings(provider);
            if (settings == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.IsHost = NapContextHelper.HasRight(FunctionKeys.PushNotificationSettings, Constant.Policy.Host);
            return View(settings);
        }

        [HttpPost]
        [Route("configure-provider")]
        [NapAuth(FunctionKey = FunctionKeys.PushNotificationSettings, Policy = Constant.Policy.Host)]
        public async Task<ActionResult> ConfigureDeliveryProvider(
            string providerAssemblyQualifiedName,
            bool isEnabled,
            FormCollection form)
        {
            var provider = GetProvider(providerAssemblyQualifiedName);
            var settings = GetOrCreatePushNotificationDeliveryProviderSettings(provider);

            if (ModelState.IsValid)
            {
                if (settings == null)
                {
                    return RedirectToAction("Index");
                }

                // reflection to get the TryUpdateMethod
                var method =
                    GetType()
                        .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                        .First(m => m.Name == "TryUpdateModel" && m.IsGenericMethod && m.GetParameters().Length == 1)
                        .MakeGenericMethod(provider.Configuration.GetType());

                // TryUpdateModel call
                if ((bool)method.Invoke(this, new[] { provider.Configuration }))
                {
                    settings.ProviderConfigurationData = JObject.FromObject(provider.Configuration);
                    settings.IsEnabled = isEnabled;
                    await NoteContext.SaveChangesAsync().ConfigureAwait(false);
                    return RedirectToAction("Index");
                }
            }

            ViewBag.IsHost = NapContextHelper.HasRight(FunctionKeys.PushNotificationSettings, Constant.Policy.Host);

            return View(settings);
        }

        private ApplicationPushNotificationSetting.PushNotificationDeliveryProviderSetting
            GetOrCreatePushNotificationDeliveryProviderSettings(IPushNotificationDeliveryProvider provider)
        {
            var settings = NoteContext.TicketDeskPushNotificationSettings.DeliveryProviderSettings.FirstOrDefault(
                s => s.ProviderAssemblyQualifiedName == provider.GetType().AssemblyQualifiedName);
            if (settings == null)
            {
                settings = ApplicationPushNotificationSetting.PushNotificationDeliveryProviderSetting.FromProvider(provider);

                // created new settings, add to context (will not be saved here, but may be committed by caller
                NoteContext.TicketDeskPushNotificationSettings.DeliveryProviderSettings.Add(settings);
            }

            return settings;
        }

        private static IPushNotificationDeliveryProvider GetProvider(string providerTypeName)
        {
            return PushNotificationDeliveryManager.DeliveryProviders.FirstOrDefault(p => p.GetType().AssemblyQualifiedName == providerTypeName) ??
                   PushNotificationDeliveryManager.CreateDefaultDeliveryProviderInstance(Type.GetType(providerTypeName));
        }

        private string GetRootUrlSetting()
        {
            var root = DomainContext.TicketDeskSettings.ClientSettings.GetDefaultSiteRootUrl();
            if (string.IsNullOrEmpty(root))
            {
                root = GetCurrentRootUrl();
            }

            return root;
        }

        private string GetCurrentRootUrl()
        {
            return Request.Url == null ? string.Empty : Request.Url.GetLeftPart(UriPartial.Authority) + Url.Content("~/").TrimEnd('/');
        }
    }
}