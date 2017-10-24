namespace TicketDesk.Web.Client.Controllers
{
    using Domain;
    using Domain.Legacy;
    using Domain.Migrations;
    using Domain.Model;
    using Identity;
    using Identity.Model;
    using Microsoft.AspNet.Identity;
    using Models;
    using PushNotifications;
    using PushNotifications.Model;
    using Search.Common;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.Mvc;

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

        [Route("legacy-migration")]
        public ActionResult LegacyMigration()
        {
            var identityCtx = DependencyResolver.Current.GetService<TdIdentityContext>();
            var userManager = DependencyResolver.Current.GetService<TicketDeskUserManager>();
            var roleManager = DependencyResolver.Current.GetService<TicketDeskRoleManager>();
            if (LegacySecurityMigrator.MigrateSecurity(identityCtx, userManager, roleManager))
            {
                LegacySecurityMigrator.RemoveLegacyMembershipObjects(identityCtx);
            }

            return RedirectToAction("Index");
        }

        [Route("upgrade-database")]
        public async Task<ActionResult> UpgradeDatabase()
        {
            LegacyBackgroundMigrator.BeginMigration(new LegacyMigrator());

            await Task.FromResult(true).ConfigureAwait(false);

            return RedirectToAction("CheckUpgradeProgress");
        }

        [Route("check-upgrade-progress")]
        public ActionResult CheckUpgradeProgress()
        {
            if (LegacyBackgroundMigrator.MigrationTask != null)
            {
                ViewBag.Refresh = "True";
                ViewBag.Messages = LegacyBackgroundMigrator.MessageStack.ToString()
                    .Replace(System.Environment.NewLine, "<br />");
                var upgradeSuccess = false;
                switch (LegacyBackgroundMigrator.MigrationTask.Status)
                {
                    case TaskStatus.Running:
                    case TaskStatus.WaitingForChildrenToComplete:
                    case TaskStatus.WaitingToRun:
                    case TaskStatus.WaitingForActivation:
                        ViewBag.Status = "Running";
                        ViewBag.Alert = "alert-info";
                        break;

                    case TaskStatus.Canceled:
                        ViewBag.Status = "Canceled";
                        ViewBag.Alert = "alert-error";
                        ViewBag.Refresh = "False";
                        LegacyBackgroundMigrator.Reset();
                        break;

                    case TaskStatus.Faulted:
                        ViewBag.Status = "Failed";
                        ViewBag.Alert = "alert-error";
                        ViewBag.Refresh = "False";
                        LegacyBackgroundMigrator.Reset();
                        break;

                    case TaskStatus.RanToCompletion:
                        ViewBag.Status = "Completed";
                        ViewBag.Alert = "alert-success";
                        ViewBag.Refresh = "False";
                        upgradeSuccess = true;
                        LegacyBackgroundMigrator.Reset();
                        break;
                    default:
                        ViewBag.Status = LegacyBackgroundMigrator.MigrationTask.Status.ToString();
                        ViewBag.Alert = "alert-warning";
                        break;
                }

                SetupApplication(upgradeSuccess);
            }

            return View();
        }

        private static void SetupApplication(bool upgradeSuccess)
        {
            if (upgradeSuccess)
            {
                var filter = GlobalFilters.Filters.FirstOrDefault(f => f.Instance is DbSetupFilterAttribute);
                if (filter != null)
                {
                    GlobalFilters.Filters.Remove(filter.Instance);
                }

                Database.SetInitializer(new TdIdentityDbInitializer());
                Database.SetInitializer(new TdPushNotificationDbInitializer());
                Startup.ConfigurePushNotifications();
                UpdateSearchIndex();
            }
        }

        private static void UpdateSearchIndex()
        {
            // TODO: duplicated in DatabaseConfig.cs, should refactor extension method or something... just not sure what the most appropriate place is
            HostingEnvironment.QueueBackgroundWorkItem(async (ct) =>
            {
                using (var ctx = new TdDomainContext(null))
                {
                    await TdSearchContext.Current().IndexManager.RunIndexMaintenanceAsync().ConfigureAwait(false);
                    var searchItems = ctx.Tickets.Include("TicketEvents").ToSeachIndexItems();
                    await TdSearchContext.Current().IndexManager.AddItemsToIndexAsync(searchItems).ConfigureAwait(false);
                }
            });
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

            Database.SetInitializer(new TdPushNotificationDbInitializer());

            Startup.ConfigurePushNotifications();
            if (newUser != null)
            {
                using (var notificationContext = new TdPushNotificationContext())
                {
                    notificationContext.SubscriberPushNotificationSettingsManager.AddSettingsForSubscriber(
                        new SubscriberNotificationSetting
                        {
                            SubscriberId = newUser.Id,
                            IsEnabled = true,
                            PushNotificationDestinations = new[]
                            {
                                new PushNotificationDestination
                                {
                                    DestinationType = "email",
                                    DestinationAddress = newUser.Email,
                                    SubscriberName = newUser.DisplayName
                                }
                            }
                        });
                    notificationContext.SaveChanges();
                }
            }

            UpdateSearchIndex();
            return RedirectToAction("Index");
        }

        [ChildActionOnly]
        public ActionResult AzureInfo()
        {
            if (!Model.AzureInfo.HasAzureServices)
            {
                return new EmptyResult();
            }

            return PartialView("_AzureInfo", Model);
        }

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

            if (Model.AzureInfo.IsAzureWebSite)
            {
                ViewBag.ErrorAzureDbDoesNotExist = !Model.DatabaseStatus.DatabaseExists;
                ViewBag.WarnNotAnAzureDb = !Model.AzureInfo.IsSqlAzure;
            }

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