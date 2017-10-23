namespace TicketDesk.PushNotifications
{
    using Delivery;
    using Model;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Management.Instrumentation;
    using System.Threading;
    using System.Threading.Tasks;

    public static class PushNotificationDeliveryManager
    {
        private static ICollection<IPushNotificationDeliveryProvider> _deliveryProviders;

        public static ICollection<IPushNotificationDeliveryProvider> DeliveryProviders
        {
            get
            {
                if (_deliveryProviders == null)
                {
                    _deliveryProviders = new List<IPushNotificationDeliveryProvider>();
                    using (var context = new TdPushNotificationContext())
                    {
                        var providerConfigs = context.TicketDeskPushNotificationSettings.DeliveryProviderSettings;
                        foreach (var prov in providerConfigs)
                        {
                            if (prov.IsEnabled)
                            {
                                _deliveryProviders.Add(CreateDeliveryProviderInstance(prov));
                            }
                        }
                    }
                }

                return _deliveryProviders;
            }

            set
            {
                _deliveryProviders = value;
            }
        }

        public static IPushNotificationDeliveryProvider CreateDeliveryProviderInstance(
            ApplicationPushNotificationSetting.PushNotificationDeliveryProviderSetting settings)
        {
            var provType = Type.GetType(settings.ProviderAssemblyQualifiedName);
            if (provType != null)
            {
                var ci = provType.GetConstructor(new[] { typeof(JObject) });
                if (ci != null)
                {
                    return (IPushNotificationDeliveryProvider)ci.Invoke(new object[] { settings.ProviderConfigurationData });
                }
            }

            return null;
        }

        public static IPushNotificationDeliveryProvider CreateDefaultDeliveryProviderInstance(Type providerType)
        {
            var ci = providerType.GetConstructor(new[] { typeof(JObject) });
            if (ci == null)
            {
                throw new InstanceNotFoundException("Cannot locate a constructor for " + typeof(JObject).Name);
            }

            return (IPushNotificationDeliveryProvider)ci.Invoke(new object[] { null });
        }

        private static bool CheckPushNotificationItem(PushNotificationItem left, PushNotificationItem right)
        {
            return left.ContentSourceId == right.ContentSourceId
                   && left.ContentSourceType == right.ContentSourceType
                   && left.SubscriberId == right.SubscriberId
                   && left.DestinationId == right.DestinationId;
        }

        public static async Task SendNotificationAsync(
            int contentSourceId,
            string contentSourceType,
            string subscriberId,
            int destinationId)
        {
            using (var context = new TdPushNotificationContext())
            {
                PushNotificationItem createdItem = new PushNotificationItem
                {
                    ContentSourceId = contentSourceId,
                    ContentSourceType = contentSourceType,
                    SubscriberId = subscriberId,
                    DestinationId = destinationId
                };

                var readyNote = await context.PushNotificationItems
                    .FirstOrDefaultAsync(n => CheckPushNotificationItem(n, createdItem)
                        && (n.DeliveryStatus == PushNotificationItemStatus.Scheduled
                            || n.DeliveryStatus == PushNotificationItemStatus.Retrying)).ConfigureAwait(false);

                if (readyNote == null)
                {
                    return;
                }

                await SendNotificationMessageAsync(context, readyNote, CancellationToken.None).ConfigureAwait(false);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public static async Task<int> SendNextReadyNotificationAsync(CancellationToken ct)
        {
            using (var context = new TdPushNotificationContext())
            {
                // get the next notification that is ready to send
                var readyNote =
                    await context.PushNotificationItems.Include(n => n.Destination).OrderBy(n => n.ScheduledSendDate).FirstOrDefaultAsync(
                        n => (n.DeliveryStatus == PushNotificationItemStatus.Scheduled || n.DeliveryStatus == PushNotificationItemStatus.Retrying)
                             && n.ScheduledSendDate <= DateTimeOffset.Now,
                        ct).ConfigureAwait(false);

                if (readyNote == null)
                {
                    return 0;
                }

                await SendNotificationMessageAsync(context, readyNote, ct).ConfigureAwait(false);
                return await context.SaveChangesAsync(ct).ConfigureAwait(false);
            }
        }

        private static async Task SendNotificationMessageAsync(TdPushNotificationContext context, PushNotificationItem readyNote, CancellationToken ct)
        {
            var retryMax = context.TicketDeskPushNotificationSettings.RetryAttempts;
            var retryIntv = context.TicketDeskPushNotificationSettings.RetryIntervalMinutes;

            // find a provider for the notification destination type
            var provider = DeliveryProviders.FirstOrDefault(p => p.DestinationType == readyNote.Destination.DestinationType);
            if (provider == null)
            {
                // no provider
                readyNote.DeliveryStatus = PushNotificationItemStatus.NotAvailable;
                readyNote.ScheduledSendDate = null;
            }
            else
            {
                await provider.SendReadyMessageAsync(readyNote, retryMax, retryIntv, ct).ConfigureAwait(false);
            }
        }
    }
}