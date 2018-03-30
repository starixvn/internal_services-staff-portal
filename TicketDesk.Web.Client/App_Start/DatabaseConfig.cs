namespace TicketDesk.Web.Client
{
    using Configuration = Domain.Migrations.Configuration;
    using Domain;
    using Domain.Migrations;
    ////using Search.Common;
    using System;
    using System.Configuration;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;

    public static class DatabaseConfig
    {
        public static bool IsDatabaseReady
        {
            get
            {
                bool result;
                using (var ctx = new TdDomainContext(null))
                {
                    result = (ctx.Database.Exists() && !IsEmptyDatabase(ctx)) && !IsLegacyDatabase(ctx);
                }

                return result;
            }
        }

        public static void RegisterDatabase()
        {
            var setupEnabled = ConfigurationManager.AppSettings["ticketdesk:SetupEnabled"];
            var firstRunEnabled = !string.IsNullOrEmpty(setupEnabled) &&
                setupEnabled.Equals("true", StringComparison.InvariantCultureIgnoreCase);

            if (firstRunEnabled && !IsDatabaseReady)
            {
                // add a global filter to send requests to the database managment first run functions
                GlobalFilters.Filters.Add(new DbSetupFilterAttribute());
            }
            else
            {
                // run any pending migrations automatically to bring the DB up to date
                Database.SetInitializer(new MigrateDatabaseToLatestVersion<TdDomainContext, Configuration>(true));
                using (var ctx = new TdDomainContext(null))
                {
                    try
                    {
                        ctx.Database.Initialize(!ctx.Database.CompatibleWithModel(true));
                    }
                    catch (Exception)
                    {
                        // no metadata in DB, force run initializer anyway
                        ctx.Database.Initialize(true);
                    }

                    if (IsFirstRunDemoRefreshEnabled())
                    {
                        DemoDataManager.SetupDemoData(ctx);

                        // TODO: duplicated in FirstRunSetup controller, should refactor extension method or something... just not sure what the most appropriate place is
                        ////HostingEnvironment.QueueBackgroundWorkItem(async ct =>
                        ////{
                        ////    using (var dctx = new TdDomainContext(null))
                        ////    {
                        ////        await TdSearchContext.Current().IndexManager.RunIndexMaintenanceAsync().ConfigureAwait(false);
                        ////        var searchItems = dctx.Tickets.Include("TicketEvents").ToSeachIndexItems();
                        ////        await TdSearchContext.Current().IndexManager.AddItemsToIndexAsync(searchItems).ConfigureAwait(false);
                        ////    }
                        ////});
                    }
                }
            }
        }

        public static bool IsEmptyDatabase()
        {
            using (var ctx = new TdDomainContext(null))
            {
                return IsEmptyDatabase(ctx);
            }
        }

        public static bool IsLegacyDatabase()
        {
            using (var ctx = new TdDomainContext(null))
            {
                return IsLegacyDatabase(ctx);
            }
        }

        public static bool HasLegacySecurity()
        {
            using (var ctx = new TdDomainContext(null))
            {
                return HasLegacySecurity(ctx);
            }
        }

        public static bool IsFirstRunDemoRefreshEnabled()
        {
            var demoRefresh = ConfigurationManager.AppSettings["ticketdesk:ResetDemoDataOnStartup"];
            var firstRunDemoRefresh = !string.IsNullOrEmpty(demoRefresh)
                                    && demoRefresh.Equals("true", StringComparison.InvariantCultureIgnoreCase)
                                    && IsDatabaseReady;

            // only do this if database was ready on startup, otherwise migrator will take care of it
            return firstRunDemoRefresh;
        }

        private static bool IsEmptyDatabase(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var numTables = context.Database.SqlQuery<int>(
                "SELECT count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'");
            bool hasTables = numTables.First() > 0;

            return !hasTables;
        }

        private static bool HasLegacySecurity(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var hasLegacy = false;
            try
            {
                var numMembers = context.Database.SqlQuery<int>("select count(*) from aspnet_Membership").First();
                hasLegacy = numMembers > 0;
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
                // eat any exception
            }

            return hasLegacy;
        }

        private static bool IsLegacyDatabase(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var isLegacy = false;
            try
            {
                var isTable = context.Database.SqlQuery<int>(@"(SELECT COUNT(TABLE_NAME)
                                                                  FROM INFORMATION_SCHEMA.TABLES
                                                                  WHERE TABLE_SCHEMA = 'dbo'
                                                                  AND TABLE_NAME = 'Settings')");
                if (isTable.Any())
                {
                    var it = isTable.First();
                    if (it > 0)
                    {
                        var oldVersion = context.Database.SqlQuery<string>("select SettingValue from Settings where SettingName = 'Version'");
                        isLegacy = oldVersion.Any() && oldVersion.First().Equals("2.0.2");
                    }
                }
            }
            catch
            {
                // eat any exception, we'll assume that if the db exists, but we can't read the settings, then it is an just empty new db
            }

            return isLegacy;
        }
    }
}