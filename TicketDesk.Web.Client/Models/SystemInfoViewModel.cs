namespace TicketDesk.Web.Client.Models
{
    using Domain;
    using System.Configuration;
    using System.Data.SqlClient;

    public class SystemInfoViewModel
    {
        private DatabaseInfoViewModel _databaseInfo;
        private DatabaseStatusViewModel _databaseStatus;

        public DatabaseInfoViewModel DatabaseInfo => _databaseInfo ?? (_databaseInfo = new DatabaseInfoViewModel());

        public DatabaseStatusViewModel DatabaseStatus => _databaseStatus ?? (_databaseStatus = new DatabaseStatusViewModel());

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
    }
}