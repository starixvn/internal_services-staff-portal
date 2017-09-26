// TicketDesk - Attribution notice
// Contributor(s):
//
//      Stephen Redd (https://github.com/stephenredd)
//
// This file is distributed under the terms of the Microsoft Public
// License (Ms-PL). See http://opensource.org/licenses/MS-PL
// for the complete terms of use.
//
// For any distribution that contains code from this file, this notice of
// attribution must remain intact, and a copy of the license must be
// provided to the recipient.

using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketDesk.IO
{
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

        private CloudQueue Queue { get; set; }

        public bool IsConfigured
        {
            get { return Queue != null; }
        }

        public async Task EnqueueItemsAsync(IEnumerable<object> items)
        {
            var tasks = items.Select(EnqueueItemAsyc).ToList();
            await Task.WhenAll(tasks);
        }

        private async Task EnqueueItemAsyc(object item)
        {
            var jsonItem = JsonConvert.SerializeObject(item);
            var message = new CloudQueueMessage(jsonItem);
            await Queue.AddMessageAsync(message);
        }
    }
}