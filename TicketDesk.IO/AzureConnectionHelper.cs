namespace TicketDesk.IO
{
    using Microsoft.Azure;
    using Microsoft.WindowsAzure.Storage;
    using System.Configuration;

    public static class AzureConnectionHelper
    {
        private static CloudStorageAccount _cloudStorageAccount;

        internal static CloudStorageAccount CloudStorageAccount
        {
            get
            {
                if (_cloudStorageAccount == null)
                {
                    var connectionString = CloudConfigConnString ?? ConfigManagerConnString;
                    CloudStorageAccount cloudStorageAccount;
                    if (CloudStorageAccount.TryParse(connectionString, out cloudStorageAccount))
                    {
                        _cloudStorageAccount = cloudStorageAccount;
                    }
                }

                return _cloudStorageAccount;
            }
        }

        /// <summary>
        /// Gets the connection string from cloud configuration manager if it can see it.
        /// </summary>
        /// <remarks>
        /// Typically, this can only get connections defined as an appSetting in web.config or app.config files
        /// </remarks>
        /// <value>The cloud configuration connection string.</value>
        public static string CloudConfigConnString
        {
            get { return CloudConfigurationManager.GetSetting("AzureStorage"); }
        }

        /// <summary>
        /// Gets the connection string from regular configuration manager.
        /// </summary>
        /// <remarks>
        /// This get the connection from the connection strings section of web.config or app.config
        /// </remarks>
        /// <value>The configuration manager connection string.</value>
        public static string ConfigManagerConnString
        {
            get
            {
                var conn = ConfigurationManager.ConnectionStrings["AzureStorage"];
                return conn == null ? null : conn.ConnectionString;
            }
        }
    }
}