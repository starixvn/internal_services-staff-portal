namespace TicketDesk.Web.Client.Controllers
{
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Domain;
    using Domain.Migrations;
    using Identity;
    using Identity.Model;
    using Microsoft.AspNet.Identity;
    using Models;
    ////using PushNotifications;
    ////using PushNotifications.Model;

    [RoutePrefix("first-run-setup")]
    [Route("{action=index}")]
    [WhenSetupEnabled]
    public class FirstRunSetupController : Controller
    {
        public FirstRunSetupController(
            TicketDeskUserManager userManager)
        {
            UserManager = userManager;
            Model = new SystemInfoViewModel();
        }

        private TicketDeskUserManager UserManager { get; }

        private SystemInfoViewModel Model { get; }

        // GET: FirstRunSetup
        [Route("", Name = "first-run-setup")]
        [Route("index")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("create-database")]
        public async Task<ActionResult> CreateDatabase(string email, string password, string displayName)
        {
            using (var ctx = new TdDomainContext(null))
            {
                Database.SetInitializer(new MigrateDatabaseToLatestVersion<TdDomainContext, Configuration>(true));
                ctx.Database.Initialize(true);
            }

            var filter = GlobalFilters.Filters.FirstOrDefault(f => f.Instance is DbSetupFilterAttribute);
            if (filter != null)
            {
                GlobalFilters.Filters.Remove(filter.Instance);
            }

            Database.SetInitializer(new TdIdentityDbInitializer());
            var existingUser = UserManager.FindByName(email);
            TicketDeskUser newUser = null;
            if (existingUser == null)
            {
                newUser = new TicketDeskUser { UserName = email, Email = email, DisplayName = displayName };
                await UserManager.CreateAsync(newUser, password).ConfigureAwait(false);
                await UserManager.AddToRoleAsync(newUser.Id, "TdAdministrators").ConfigureAwait(false);
            }
            else
            {
                // should only happen if user entered one of the demo user names when setting up the DB
                // reset the password to the one the user entered here, and set the admin role if needed
                var token = await UserManager.GeneratePasswordResetTokenAsync(existingUser.Id).ConfigureAwait(false);
                await UserManager.ResetPasswordAsync(existingUser.Id, token, password).ConfigureAwait(false);
                existingUser.DisplayName = displayName;
                await UserManager.UpdateAsync(existingUser).ConfigureAwait(false);
                if (!UserManager.IsTdAdministrator(existingUser.Id))
                {
                    await UserManager.AddToRoleAsync(existingUser.Id, "TdAdministrators").ConfigureAwait(false);
                }
            }

            ////Database.SetInitializer(new TdPushNotificationDbInitializer());

            ////Startup.ConfigurePushNotifications();
            ////if (newUser != null)
            ////{
            ////    using (var notificationContext = new TdPushNotificationContext())
            ////    {
            ////        notificationContext.SubscriberPushNotificationSettingsManager.AddSettingsForSubscriber(
            ////            new SubscriberNotificationSetting
            ////            {
            ////                SubscriberId = newUser.Id,
            ////                IsEnabled = true,
            ////                PushNotificationDestinations = new[]
            ////                {
            ////                    new PushNotificationDestination
            ////                    {
            ////                        DestinationType = "email",
            ////                        DestinationAddress = newUser.Email,
            ////                        SubscriberName = newUser.DisplayName
            ////                    }
            ////                }
            ////            });
            ////        notificationContext.SaveChanges();
            ////    }
            ////}

            ////UpdateSearchIndex();
            return RedirectToAction("Index");
        }

        ////[ChildActionOnly]
        ////public ActionResult AzureInfo()
        ////{
        ////    if (!Model.AzureInfo.HasAzureServices)
        ////    {
        ////        return new EmptyResult();
        ////    }

        ////    return PartialView("_AzureInfo", Model);
        ////}

        [ChildActionOnly]
        public ActionResult DatabaseInfo()
        {
            return PartialView("_DatabaseInfo", Model);
        }

        [ChildActionOnly]
        public ActionResult NewDatabase()
        {
            if (SystemInfoViewModel.DatabaseStatusViewModel.IsDatabaseReady ||
                SystemInfoViewModel.DatabaseStatusViewModel.IsLegacyDatabase)
            {
                return new EmptyResult();
            }

            ////if (Model.AzureInfo.IsAzureWebSite)
            ////{
            ////    ViewBag.ErrorAzureDbDoesNotExist = !Model.DatabaseStatus.DatabaseExists;
            ////    ViewBag.WarnNotAnAzureDb = !Model.AzureInfo.IsSqlAzure;
            ////}

            return View("_NewDatabase", Model);
        }

        [ChildActionOnly]
        public ActionResult LegacyDatabase()
        {
            if (SystemInfoViewModel.DatabaseStatusViewModel.IsDatabaseReady ||
                !SystemInfoViewModel.DatabaseStatusViewModel.IsLegacyDatabase)
            {
                return new EmptyResult();
            }

            return View("_LegacyDatabase", Model);
        }

        [ChildActionOnly]
        public ActionResult LegacySecurity()
        {
            if (!SystemInfoViewModel.DatabaseStatusViewModel.IsDatabaseReady ||
                !SystemInfoViewModel.DatabaseStatusViewModel.HasLegacySecurityObjects)
            {
                return new EmptyResult();
            }

            return View("_LegacySecurity", Model);
        }

        [ChildActionOnly]
        public ActionResult SetupCompleteInfo()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();

            if (!SystemInfoViewModel.DatabaseStatusViewModel.IsDatabaseReady ||
                !Model.DatabaseStatus.IsCompatibleWithEfModel ||
                SystemInfoViewModel.DatabaseStatusViewModel.HasLegacySecurityObjects)
            {
                return new EmptyResult();
            }

            return View("_SetupCompleteInfo", Model);
        }
    }
}