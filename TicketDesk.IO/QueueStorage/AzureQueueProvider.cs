namespace TicketDesk.IO
{
    using Microsoft.WindowsAzure.Storage.Queue;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides services for adding simple messages to an azure storage queue
    /// </summary>
    public class AzureQueueProvider
    {
        public AzureQueueProvider(string indexName)
        {
            var account = AzureConnectionHelper.CloudStorageAccount;
            if (account != null)
            {
                var queueClient = account.CreateCloudQueueClient();
                Queue = queueClient.GetQueueReference(indexName);
                var t = Queue.CreateIfNotExistsAsync();
                t.Wait();
            }
        }

        private CloudQueue Queue { get; }

        public bool IsConfigured => Queue != null;

        public async Task EnqueueItemsAsync(IEnumerable<object> items)
        {
            var tasks = items.Select(EnqueueItemAsyc).ToList();
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private async Task EnqueueItemAsyc(object item)
        {
            var jsonItem = JsonConvert.SerializeObject(item);
            var message = new CloudQueueMessage(jsonItem);
            await Queue.AddMessageAsync(message).ConfigureAwait(false);
        }
    }
}