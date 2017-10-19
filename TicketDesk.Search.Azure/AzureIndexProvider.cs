namespace TicketDesk.Search.Azure
{
    using Common;
    using RedDog.Search.Model;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    public class AzureIndexProvider : AzureSearchConector, ISearchIndexProvider
    {
        private const string IndexName = "ticketdesk-searchindex";

        /// <summary>
        /// Gets a value indicating whether this provider is correctly configured and available for use.
        /// </summary>
        /// <value><c>true</c> if this provider is available; otherwise, <c>false</c>.</value>
        public bool IsConfigured
        {
            get
            {
                var svcInfo = TryGetInfoFromConnectionString() ?? TryGetInfoFromAppSettings();

                return !string.IsNullOrEmpty(svcInfo?.ServiceName) && !string.IsNullOrEmpty(svcInfo.QueryKey);
            }
        }

        public async Task<bool> RunIndexMaintenanceAsync()
        {
            return await CreateIndexAsync().ConfigureAwait(false);
        }

        public async Task<bool> AddItemsToIndexAsync(IEnumerable<SearchIndexItem> items)
        {
            var ret = true;
            foreach (var item in items)
            {
                // TODO: can we do this in parallel with Azure search?
                if (!await SendItemToIndexAsync(item.ToIndexOperation()).ConfigureAwait(false) && ret)
                {
                    // TODO: beef this up so we return a list of failures for the caller to handle
                    // if any item fails, return false - but go ahead and process the rest of the batch

                    ret = false;
                }
            }

            return ret;
        }

        public async Task<bool> RemoveIndexAsync()
        {
            var ret = true;
            if (await IndexExistsAsync().ConfigureAwait(false))
            {
                var result = await ManagementClient.DeleteIndexAsync(IndexName).ConfigureAwait(false);
                if (!result.IsSuccess)
                {
                    Trace.Write("Error: " + result.Error.Message);
                }

                ret = result.IsSuccess;
            }

            return ret;
        }

        internal async Task<bool> SendItemToIndexAsync(IndexOperation itemOperation)
        {
            var result = await ManagementClient.PopulateAsync(IndexName, itemOperation).ConfigureAwait(false);
            if (!result.IsSuccess)
            {
                Trace.Write("Error: " + result.Error.Message);
            }

            return result.IsSuccess;
        }

        /// <summary>
        /// Creates the index if it doesn't exist, will not overwrite an existing index.
        /// </summary>
        /// <returns>Task{System.Boolean} - True when index created, or when index already exists. False when something breaks.</returns>
        internal async Task<bool> CreateIndexAsync()
        {
            var ret = true;
            var exists = await IndexExistsAsync().ConfigureAwait(false);
            if (!exists)
            {
                var index = GetIndexDefinition();
                var result = await ManagementClient.CreateIndexAsync(index).ConfigureAwait(false);
                if (!result.IsSuccess)
                {
                    Trace.WriteLine("Error: " + result.Error.Message);
                }

                ret = result.IsSuccess;
            }

            return ret;
        }

        internal async Task<bool> IndexExistsAsync()
        {
            var ret = false;
            var result = await ManagementClient.GetIndexesAsync().ConfigureAwait(false);
            if (result.IsSuccess)
            {
                ret = result.Body.Any(i => i.Name.Equals(IndexName));
            }
            else
            {
                Trace.Write("Error: " + result.Error.Message);
            }

            return ret;
        }

        private static Index GetIndexDefinition()
        {
            var index = new Index(IndexName)
                .WithStringField("id", opt => opt.IsKey().IsRetrievable().IsSearchable().IsSortable().IsFilterable())
                .WithStringField("projectid", opt => opt.IsRetrievable().IsSortable().IsFilterable())
                .WithStringField("title", opt => opt.IsRetrievable().IsSearchable())
                .WithStringField("status", opt => opt.IsRetrievable().IsSearchable().IsSortable().IsFilterable())
                .WithDateTimeField("lastupdatedate", opt => opt.IsSortable().IsFilterable())
                .WithStringField("details", opt => opt.IsSearchable())
                .WithStringCollectionField("tags", opt => opt.IsRetrievable().IsSearchable().IsFilterable())
                .WithStringCollectionField("events", opt => opt.IsSearchable());

            index.ScoringProfiles = new Collection<ScoringProfile>
            {
                new ScoringProfile
                {
                    Name = "fieldBooster",
                    Text = new ScoringProfileText
                    {
                        Weights = new Dictionary<string, double>
                        {
                            { "id", 3D },
                            { "tags", 2.5D },
                            { "title", 2D },
                            { "events", 0.75D }
                        }
                    },
                    Functions =
                    {
                        new ScoringProfileFunction
                        {
                            Type = ScoringProfileFunctionType.Freshness,
                            FieldName = "lastupdatedate",
                            Boost = 1.25d,
                            Interpolation = InterpolationType.Quadratic,
                            Freshness = new ScoringProfileFunctionFreshness
                            {
                                BoostingDuration = new TimeSpan(5, 0, 0)
                            }
                        }
                    }
                }
            };

            index.DefaultScoringProfile = "fieldBooster";
            return index;
        }
    }
}