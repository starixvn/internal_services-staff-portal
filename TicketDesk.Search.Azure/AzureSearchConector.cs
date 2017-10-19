namespace TicketDesk.Search.Azure
{
    using System;
    using RedDog.Search;
    using RedDog.Search.Http;
    using System.Configuration;
    using System.Linq;

    public abstract class AzureSearchConector : IDisposable
    {
        private ApiConnection _connection;
        private IndexManagementClient _managementClient;
        private IndexQueryClient _queryClient;

        internal IndexManagementClient ManagementClient => _managementClient ?? (_managementClient = new IndexManagementClient(Connection));

        internal IndexQueryClient QueryClient => _queryClient ?? (_queryClient = new IndexQueryClient(Connection));

        internal ApiConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    var svcInfo = TryGetInfoFromConnectionString() ?? TryGetInfoFromAppSettings();

                    _connection = ApiConnection.Create(svcInfo.ServiceName, svcInfo.QueryKey);
                }

                return _connection;
            }
        }

        public static ServiceInfo TryGetInfoFromAppSettings()
        {
            ServiceInfo info = null;
            var service = ConfigurationManager.AppSettings["Azure.Search.ServiceName"];
            var key = ConfigurationManager.AppSettings["Azure.Search.ApiKey"];
            if (service != null && key != null)
            {
                info = new ServiceInfo { ServiceName = service, QueryKey = key };
            }

            return info;
        }

        public static ServiceInfo TryGetInfoFromConnectionString()
        {
            ServiceInfo info = null;
            var conn = ConfigurationManager.ConnectionStrings["AzureSearch"];
            if (conn != null)
            {
                try
                {
                    var parts = conn.ConnectionString
                        .Split(';')
                        .Select(p => p.Split('='))
                        .Select(t => new { key = t[0], value = t[1] }).ToList();

                    var service = parts.FirstOrDefault(p => p.key.Equals("ServiceName"));
                    var key = parts.FirstOrDefault(p => p.key.Equals("ApiKey"));
                    if (service != null && key != null)
                    {
                        info = new ServiceInfo
                        {
                            ServiceName = service.value.Trim(),
                            QueryKey = key.value.Trim()
                        };
                    }
                }
                catch
                {
                    // no need to get all null-checky and collection count happy... if
                    // something went wrong above, then the connection string existed, but
                    // is configured incorrectly
                    throw new ConfigurationErrorsException(
                        "The AzureSearch connection string is invalid, or incorrectly formatted");
                }
            }

            return info;
        }

        public class ServiceInfo
        {
            public string ServiceName { get; set; }

            internal string QueryKey { get; set; }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connection?.Dispose();
                _managementClient?.Dispose();
                _queryClient?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}