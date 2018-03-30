namespace TicketDesk.Web.Client.Models
{
    using Domain;
    ////using Search.Azure;
    using System.Configuration;
    using System.Data.SqlClient;

    public class SystemInfoViewModel
    {
        private DatabaseInfoViewModel _databaseInfo;
        private DatabaseStatusViewModel _databaseStatus;
        ////private AzureInfoViewModel _azureInfo;

        public DatabaseInfoViewModel DatabaseInfo => _databaseInfo ?? (_databaseInfo = new DatabaseInfoViewModel());

        public DatabaseStatusViewModel DatabaseStatus => _databaseStatus ?? (_databaseStatus = new DatabaseStatusViewModel());

        ////public AzureInfoViewModel AzureInfo => _azureInfo ?? (_azureInfo = new AzureInfoViewModel(DatabaseInfo.DbEdition, DatabaseInfo.DatabaseName));

        public class DatabaseInfoViewModel
        {
            public DatabaseInfoViewModel()
            {
                var connectionString = ConfigurationManager.ConnectionStrings["TicketDesk"].ConnectionString;
                DbEdition = GetSqlEdition(connectionString);
                IsLocalDb = GetIsLocalDb(connectionString);

                var builder = new SqlConnectionStringBuilder(connectionString);
                var dsource = builder.DataSource.Split('\\');
                ServerName = dsource.Length > 0 ? dsource[0] : null;
                SqlInstance = dsource.Length > 1 ? dsource[1] : null;

                IsFileDatabase = !string.IsNullOrEmpty(builder.AttachDBFilename);
                DatabaseName = IsFileDatabase ? builder.AttachDBFilename : builder.InitialCatalog;
            }

            public string DbEdition { get; }

            public bool IsLocalDb { get; }

            public bool IsFileDatabase { get; }

            public string ServerName { get; }

            public string SqlInstance { get; }

            public string DatabaseName { get; }

            private static string GetSqlEdition(string connectionString)
            {
                string edition = null;
                try
                {
                    var conn = GetMasterCatalogConnection(connectionString);
                    var cmd = conn.CreateCommand();
                    cmd.CommandTimeout = 5;
                    cmd.CommandText = "SELECT SERVERPROPERTY('Edition')";
                    conn.Open();
                    edition = cmd.ExecuteScalar() as string;
                    conn.Close();
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                    // TODO
                }

                return edition;
            }

            private static bool GetIsLocalDb(string connectionString)
            {
                bool isLocalDb = false;
                try
                {
                    var conn = GetMasterCatalogConnection(connectionString);
                    var cmd = conn.CreateCommand();
                    cmd.CommandTimeout = 5;
                    cmd.CommandText = "SELECT SERVERPROPERTY('IsLocalDB')";
                    conn.Open();
                    var ldb = cmd.ExecuteScalar() as int?;
                    isLocalDb = ldb.HasValue;
                    conn.Close();
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                    // TODO
                }

                return isLocalDb;
            }

            private static SqlConnection GetMasterCatalogConnection(string connectionString)
            {
                var builder = new SqlConnectionStringBuilder(connectionString)
                {
                    AttachDBFilename = string.Empty,
                    InitialCatalog = "Master",
                    ConnectTimeout = 5
                };
                return new SqlConnection(builder.ToString());
            }
        }

        public class DatabaseStatusViewModel
        {
            public DatabaseStatusViewModel()
            {
                using (var ctx = new TdDomainContext(null))
                {
                    DatabaseExists = ctx.Database.Exists();
                    IsCompatibleWithEfModel = DatabaseExists && !IsEmptyDatabase && ctx.Database.CompatibleWithModel(false);
                }
            }

            public bool DatabaseExists { get; }

            public static bool IsDatabaseReady => DatabaseConfig.IsDatabaseReady;

            public static bool IsLegacyDatabase => DatabaseConfig.IsLegacyDatabase();

            private static bool IsEmptyDatabase => DatabaseConfig.IsEmptyDatabase();

            public bool IsCompatibleWithEfModel { get; }

            public static bool HasLegacySecurityObjects => DatabaseConfig.HasLegacySecurity();
        }

        ////public class AzureInfoViewModel
        ////{
        ////    public AzureInfoViewModel(string databaseEdition, string databaseName)
        ////    {
        ////        AzureWebSiteName = Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME");
        ////        IsAzureWebSite = !string.IsNullOrEmpty(AzureWebSiteName);
        ////        ////IsSqlAzure = databaseEdition != null && databaseEdition.IndexOf("azure", StringComparison.InvariantCultureIgnoreCase) >= 0;
        ////        ////AzureSqlService = IsSqlAzure ? databaseName : null;
        ////        ////AzureSearchService = GetAzureSearchServiceName();
        ////        ////AzureStorageService = GetAzureStorageServiceName();
        ////    }

        ////    public string AzureWebSiteName { get; }

        ////    public bool IsAzureWebSite { get; }

        ////    ////public string AzureSqlService { get; }

        ////    ////public bool IsSqlAzure { get; }

        ////    ////public string AzureSearchService { get; }

        ////    ////public string AzureStorageService { get; }

        ////    ////public bool HasAzureServices => IsAzureWebSite || IsSqlAzure || !string.IsNullOrEmpty(AzureSearchService) || !string.IsNullOrEmpty(AzureStorageService);

        ////    ////private static string GetAzureSearchServiceName()
        ////    ////{
        ////    ////    string svc = null;
        ////    ////    var azureSearchInfo = AzureSearchConector.TryGetInfoFromConnectionString() ?? AzureSearchConector.TryGetInfoFromAppSettings();
        ////    ////    if (azureSearchInfo != null)
        ////    ////    {
        ////    ////        svc = azureSearchInfo.ServiceName;
        ////    ////    }

        ////    ////    return svc;
        ////    ////}

        ////    ////private static string GetAzureStorageServiceName()
        ////    ////{
        ////    ////    string serviceName = null;
        ////    ////    var azureStore = !string.IsNullOrEmpty(AzureConnectionHelper.ConfigManagerConnString)
        ////    ////        ? AzureConnectionHelper.ConfigManagerConnString
        ////    ////        : AzureConnectionHelper.CloudConfigConnString;

        ////    ////    if (!string.IsNullOrEmpty(azureStore))
        ////    ////    {
        ////    ////        try
        ////    ////        {
        ////    ////            var parts = azureStore.Split(';').Select(p => p.Split('=')).Select(t => new { key = t[0], value = t[1] }).ToList();
        ////    ////            var service = parts.FirstOrDefault(p => p.key.Equals("AccountName", StringComparison.InvariantCultureIgnoreCase));
        ////    ////            if (service != null)
        ////    ////            {
        ////    ////                serviceName = service.value;
        ////    ////            }
        ////    ////        }
        ////    ////        // ReSharper disable once EmptyGeneralCatchClause
        ////    ////        catch
        ////    ////        {
        ////    ////            // TODO
        ////    ////        }
        ////    ////    }

        ////    ////    return serviceName;
        ////    ////}
        ////}
    }
}