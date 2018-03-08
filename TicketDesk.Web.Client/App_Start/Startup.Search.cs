namespace TicketDesk.Web.Client
{
    using Domain;
    using Domain.Model;
    ////using Search.Azure;
    ////using Search.Common;
    ////using Search.Lucene;
    using System.Linq;
    using System.Web.Hosting;

    public partial class Startup
    {
        public static void ConfigureSearch()
        {
            ////TdSearchContext.Configure(GetSearchConfiguration);

            ////TdSearchContext.Current().IndexManager.RunIndexMaintenanceAsync();

            // register for static ticket changed event handler
            TdDomainContext.TicketsChanged += (sender, ticketChanges) =>
            {
                // ReSharper disable once EmptyGeneralCatchClause
                try
                {
                    var changes = ticketChanges as Ticket[] ?? ticketChanges.ToArray();

                    // deletes (such as the demo data manager removing everything) will fire
                    // this, but ticketChanges will be empty in those cases
                    ////if (changes.Any())
                    ////{
                    ////    // add to search index
                    ////    var searchItems = changes.ToSeachIndexItems().ToArray();

                    ////    HostingEnvironment.QueueBackgroundWorkItem(
                    ////        async ct =>
                    ////            await
                    ////                TdSearchContext.Current().IndexManager.AddItemsToIndexAsync(searchItems)
                    ////                    .ConfigureAwait(false));
                    ////}
                }
                catch
                {
                    // TODO: log this somewhere
                }
            };
        }

        /// <summary>
        /// Gets a search configuration by scanning available index providers in order of preference
        /// and taking the first available one found.
        /// </summary>
        /// <remarks>
        /// This is supplied to the search system as a func and invoked when needed.
        /// </remarks>
        /// <returns>SearchContextConfiguration.</returns>
        ////private static TdSearchContextConfiguration GetSearchConfiguration()
        ////{
        ////    // TODO: when we move to a plug-in model, this should be refactored to use an applicaiton setting.
        ////    // Application settings will determine the order of preference for providers
        ////    // Func will cheack each in order, returing the first match found.
        ////    // By default, the system will assume that any custom plug-ins installed are
        ////    // preferred (in alphabetical order if more than one), then Azure, and finally
        ////    // Lucene as a last resort provider.
        ////    //
        ////    // It would be nice if there were a way to detect if a provider was configured
        ////    // and ready for use without fully instantiating the provider, but this may be
        ////    // cumbersome since interfaces can't specify static members, and any other
        ////    // mechanism seems like it would be somewhat convoluted for would-be plug-in
        ////    // autors to implement.
        ////    var azureProvider = new AzureIndexProvider();
        ////    var config = azureProvider.IsConfigured ?
        ////        new TdSearchContextConfiguration(azureProvider, new AzureSearchLocatorProvider()) :
        ////        new TdSearchContextConfiguration(new LuceneIndexProvider(), new LuceneSearchLocatorProvider());
        ////    return config;
        ////}
    }
}