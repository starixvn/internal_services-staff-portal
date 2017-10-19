namespace TicketDesk.Search.Azure
{
    using Common;
    using RedDog.Search.Model;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class AzureSearchLocatorProvider : AzureSearchConector, ISearchLocatorProvider
    {
        private const string IndexName = "ticketdesk-searchindex";

        public async Task<IEnumerable<SearchResultItem>> SearchAsync(string searchText, int projectId)
        {
            var query = new SearchQuery(searchText)
            {
                SearchFields = "id,title,details,tags,events",
                Select = "id",
            };

            if (projectId != default(int))
            {
                query.Filter = $"projectid eq '{projectId}'";
            }

            var result = await QueryClient.SearchAsync(IndexName, query).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                return
                    result.Body.Records.Select(
                        r =>
                            new SearchResultItem
                            {
                                Id = int.Parse((string)r.Properties["id"]),
                                SearchScore = (float)r.Score
                            });
            }

            return new SearchResultItem[0];
        }
    }
}