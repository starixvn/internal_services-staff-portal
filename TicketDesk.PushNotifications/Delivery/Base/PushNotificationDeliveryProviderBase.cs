namespace TicketDesk.PushNotifications.Delivery
{
    using Model;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class PushNotificationDeliveryProviderBase : IPushNotificationDeliveryProvider
    {
        public abstract string DestinationType { get; }

        public abstract IDeliveryProviderConfiguration Configuration { get; set; }

        public abstract Task<object> GenerateMessageAsync(PushNotificationItem notificationItem, CancellationToken ct);

        public abstract Task<bool> SendNotificationAsync(PushNotificationItem notificationItem, object message, CancellationToken ct);

        public async Task SendReadyMessageAsync(PushNotificationItem notificationItem, int retryMax, int retryIntv, CancellationToken ct)
        {
            // do the meat
            var message = await GenerateMessageAsync(notificationItem, ct).ConfigureAwait(false);
            var result = await SendNotificationAsync(notificationItem, message, ct).ConfigureAwait(false);

            // if we're in a retry case, increment retry count
            if (notificationItem.DeliveryStatus == PushNotificationItemStatus.Retrying)
            {
                notificationItem.RetryCount++;
            }

            if (result)
            {
                // if sent, mark sent and remove schedule
                notificationItem.DeliveryStatus = PushNotificationItemStatus.Sent;
                notificationItem.ScheduledSendDate = null;
            }
            else
            {
                if (notificationItem.RetryCount <= retryMax)
                {
                    // mark for retry, update schedule
                    notificationItem.DeliveryStatus = PushNotificationItemStatus.Retrying;
                    if (notificationItem.ScheduledSendDate != null)
                    {
                        notificationItem.ScheduledSendDate =
                            notificationItem.ScheduledSendDate.Value.AddMinutes(retryIntv ^ notificationItem.RetryCount);
                    }
                }
                else
                {
                    // too many retry attempts, mark fail and clear schedule
                    notificationItem.DeliveryStatus = PushNotificationItemStatus.Failed;
                    notificationItem.ScheduledSendDate = null;
                }
            }
        }
    }
}