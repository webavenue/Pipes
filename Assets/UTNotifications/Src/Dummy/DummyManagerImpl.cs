#if UNITY_EDITOR || (!UNITY_IOS && !UNITY_ANDROID && !UNITY_WSA && !UNITY_METRO) || ((UNITY_WSA || UNITY_METRO) && ENABLE_IL2CPP && !ENABLE_WINMD_SUPPORT)

using System.Collections.Generic;

namespace UTNotifications
{
    public class ManagerImpl : Manager
    {
    //public
        public override bool NotificationsEnabled()
        {
            return false;
        }

        public override bool NotificationsAllowed()
        {
            return false;
        }

        public override void SetNotificationsEnabled(bool enabled)
        {
            NotSupported();
        }

        public override bool PushNotificationsEnabled()
        {
            NotSupported();
            return false;
        }

        public override bool SetPushNotificationsEnabled(bool enable)
        {
            NotSupported();
            return false;
        }

        public override void HideNotification(int id)
        {
            NotSupported();
        }

        public override void HideAllNotifications()
        {
            NotSupported();
        }

        public override int GetBadge()
        {
            NotSupported();
            return 0;
        }

        public override void SetBadge(int bandgeNumber)
        {
            NotSupported();
        }

        public override void SubscribeToTopic(string topic)
        {
            NotSupported();
        }

        public override void UnsubscribeFromTopic(string topic)
        {
            NotSupported();
        }

        protected override bool InitializeImpl(bool willHandleReceivedNotifications, int startId, bool incrementalId)
        {
            NotSupported();
            return false;
        }

        protected override void PostLocalNotificationImpl(LocalNotification notification)
        {
            NotSupported();
        }

        protected override void ScheduleNotificationImpl(ScheduledNotification notification)
        {
            NotSupported();
        }

        protected override void ScheduleNotificationRepeatingImpl(ScheduledRepeatingNotification notification)
        {
            NotSupported();
        }

        protected override void CancelNotificationImpl(int id)
        {
            NotSupported();
        }

        protected override void CancelAllNotificationsImpl()
        {
            NotSupported();
        }

        protected override bool CleanupObsoleteScheduledNotifications(List<ScheduledNotification> scheduledNotifications)
        {
            return false;
        }
    }
}
#endif