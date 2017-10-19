namespace TicketDesk.PushNotifications
{
    using Model;
    using System.Threading.Tasks;

    public class SubscriberPushNotificationSettingsManager
    {
        public SubscriberPushNotificationSettingsManager(TdPushNotificationContext notificationContext)
        {
            NotificationContext = notificationContext;
        }

        private TdPushNotificationContext NotificationContext { get; }

        public async Task<SubscriberNotificationSetting> GetSettingsForSubscriberAsync(string subscriberId)
        {
            return await NotificationContext.SubscriberPushNotificationSettings.FindAsync(subscriberId).ConfigureAwait(false);
        }

        public void AddSettingsForSubscriber(SubscriberNotificationSetting settings)
        {
            NotificationContext.SubscriberPushNotificationSettings.Add(settings);
        }
    }
}