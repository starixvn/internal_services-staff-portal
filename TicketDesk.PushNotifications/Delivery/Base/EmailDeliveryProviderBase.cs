namespace TicketDesk.PushNotifications.Delivery
{
    using Model;
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class EmailDeliveryProviderBase : PushNotificationDeliveryProviderBase
    {
        public override IDeliveryProviderConfiguration Configuration { get; set; }

        public override string DestinationType => "email";

        public override Task<object> GenerateMessageAsync(PushNotificationItem notificationItem, CancellationToken ct)
        {
            var memorydata = Convert.FromBase64String(notificationItem.MessageContent);
            using (var rs = new MemoryStream(memorydata))
            {
                var sf = new BinaryFormatter();
                return Task.FromResult(sf.Deserialize(rs));
            }
        }
    }
}