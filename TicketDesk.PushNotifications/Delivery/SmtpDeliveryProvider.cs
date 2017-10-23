namespace TicketDesk.PushNotifications.Delivery
{
    using Localization;
    using Localization.PushNotifications;
    using Model;
    using Newtonsoft.Json.Linq;
    using S22.Mail;
    using System;
    using System.Net;
    using System.Net.Mail;
    using System.Threading;
    using System.Threading.Tasks;

    [LocalizedDescription("SMTPProvider", NameResourceType = typeof(Strings))]
    public sealed class SmtpDeliveryProvider : EmailDeliveryProviderBase
    {
        public SmtpDeliveryProvider(JToken configuration)
        {
            Configuration = configuration == null ?
                new SmtpDeliveryProviderConfiguration() :
                configuration.ToObject<SmtpDeliveryProviderConfiguration>();
        }

        public override Task<bool> SendNotificationAsync(PushNotificationItem notificationItem, object message, CancellationToken ct)
        {
            SmtpDeliveryProviderConfiguration cfg = (SmtpDeliveryProviderConfiguration)Configuration;
            bool sent = false;

            if (message is SerializableMailMessage)
            {
                try
                {
                    var smsg = message as SerializableMailMessage;
                    SmtpClient client = new SmtpClient
                    {
                        Host = cfg.SmtpServer,
                        Port = cfg.SmtpPort ?? 25,
                        EnableSsl = cfg.EnableSsl ?? false
                    };

                    if (!string.IsNullOrEmpty(cfg.SmtpUserName))
                    {
                        client.Credentials = new NetworkCredential(cfg.SmtpUserName, cfg.SmtpPassword);
                    }

                    smsg.To.Add(new MailAddress(notificationItem.Destination.DestinationAddress, notificationItem.Destination.SubscriberName));
                    smsg.From = new MailAddress(cfg.SmtpFromAddress, cfg.SmtpFromDisplayName);

                    client.Send(smsg);
                    sent = true;
                }
                catch (Exception)
                {
                    sent = false;

                    ////// TODO: log this somewhere
                    ////throw new Exception(string.Empty, ex);
                }
            }

            return Task.FromResult(sent);
        }
    }
}