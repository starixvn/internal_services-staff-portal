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
    using Microsoft.AspNet.Identity;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Domain;
    using Domain.Migrations;
    using PushNotifications;
    using PushNotifications.Migrations;
    using Identity;
    using Identity.Migrations;

    [RoutePrefix("admin/data-management")]
    [Route("{action=index}")]
    public class DataManagementController : Controller
    {
        public DataManagementController(
            TdIdentityContext identityContext,
            TdPushNotificationContext pushNotificationContext,
            TicketDeskSignInManager signInManager)
        {
            IdentityContext = identityContext;
            PushNotificationContext = pushNotificationContext;
            SignInManager = signInManager;
        }

        private TdIdentityContext IdentityContext { get; set; }

        private TdPushNotificationContext PushNotificationContext { get; set; }

        private TicketDeskSignInManager SignInManager { get; set; }

        [Route("demo")]
        [NapAuth(FunctionKey = FunctionKeys.DataManagement, Policy = Constant.Policy.Read)]
        public ActionResult Demo()
        {
            return View();
        }

        [Route("remove-demo-data")]
        [NapAuth(FunctionKey = FunctionKeys.DataManagement, Policy = Constant.Policy.Read)]
        public ActionResult RemoveDemoData()
        {
            using (var ctx = new TdDomainContext(null))
            {
                DemoDataManager.RemoveAllData(ctx);
            }

            DemoIdentityDataManager.RemoveIdentity(IdentityContext, User.Identity.GetUserId());
            DemoPushNotificationDataManager.RemoveAllPushNotificationData(PushNotificationContext);
            ViewBag.DemoDataRemoved = true;
            Task.Delay(500).ContinueWith(t => System.Web.HttpRuntime.UnloadAppDomain()).ConfigureAwait(false);

            return View("Demo");
        }

        [Route("create-demo-data")]
        [NapAuth(FunctionKey = FunctionKeys.DataManagement, Policy = Constant.Policy.Read)]
        public ActionResult CreateDemoData()
        {
            using (var ctx = new TdDomainContext(null))
            {
                DemoDataManager.SetupDemoData(ctx);
            }

            DemoIdentityDataManager.SetupDemoIdentityData(IdentityContext, User.Identity.GetUserId());
            DemoPushNotificationDataManager.SetupDemoPushNotificationData(PushNotificationContext);
            ViewBag.DemoDataCreated = true;
            Task.Delay(500).ContinueWith(t => System.Web.HttpRuntime.UnloadAppDomain()).ConfigureAwait(false);

            return View("Demo");
        }
    }
}