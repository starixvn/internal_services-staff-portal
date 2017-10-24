namespace TicketDesk.Search.Common
{
    using System;
    using System.Configuration;

    public class TdSearchContext
    {
        private static TdSearchContext _current;

        internal TdSearchContext(TdSearchContextConfiguration configuration)
        {
            IndexSearcher = configuration.SearchLocatorProvider;
            IndexManager = configuration.SearchIndexProvider;
        }

        private static Func<TdSearchContextConfiguration> GetConfigurationFunc { get; set; }

        public static void Configure(Func<TdSearchContextConfiguration> getConfigurationFunc)
        {
            GetConfigurationFunc = getConfigurationFunc;
        }

        public static TdSearchContext Current()
        {
            if (GetConfigurationFunc == null)
            {
                throw new ConfigurationErrorsException(
                    "Attempt to access search system before TicketDeskSearchContext has been configured");
            }

            return _current ?? (_current = new TdSearchContext(GetConfigurationFunc()));
        }

        public ISearchLocatorProvider IndexSearcher { get; set; }

        public ISearchIndexProvider IndexManager { get; set; }
    }
}