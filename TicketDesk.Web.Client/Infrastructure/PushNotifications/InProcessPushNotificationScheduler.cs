namespace TicketDesk.Web.Client.Infrastructure
{
    using PushNotifications;
    using System;
    using System.Threading;
    using System.Web.Hosting;

    public static class InProcessPushNotificationScheduler
    {
        private static readonly Timer Timer = new Timer(OnTimerElapsed);

        public static void Start(int interval)
        {
            Timer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(interval * 60 * 1000));
        }

        private static void OnTimerElapsed(object sender)
        {
            HostingEnvironment.QueueBackgroundWorkItem(
                async ct =>
                {
                    var i = 1;
                    while (i > 0)
                    {
                        i = await PushNotificationDeliveryManager.SendNextReadyNotificationAsync(ct).ConfigureAwait(false);
                    }
                });
        }
    }
}