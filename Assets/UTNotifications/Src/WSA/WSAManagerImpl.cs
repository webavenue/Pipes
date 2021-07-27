#if !UNITY_EDITOR && (UNITY_WSA || UNITY_METRO) && (!ENABLE_IL2CPP || ENABLE_WINMD_SUPPORT)

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UTNotifications.WSA;

namespace UTNotifications
{
    public class ManagerImpl : Manager, IInitializeHandler
    {
        protected override bool InitializeImpl(bool willHandleReceivedNotifications, int startId, bool incrementalId)
        {
            NotificationTools.Initialize(willHandleReceivedNotifications, startId, incrementalId, this, Settings.Instance.PushNotificationsEnabledWindows && pushEnabled, Settings.Instance.WindowsDontShowWhenRunning, Settings.Instance.PushPayloadTitleFieldName, Settings.Instance.PushPayloadTextFieldName, Settings.Instance.PushPayloadUserDataParentFieldName, Settings.Instance.PushPayloadNotificationProfileFieldName, Settings.Instance.PushPayloadIdFieldName, Settings.Instance.PushPayloadBadgeFieldName);
            Initialized = true;
            return true;
        }

        protected override void PostLocalNotificationImpl(LocalNotification notification)
        {
            NotificationTools.PostLocalNotification(notification.title, notification.text, notification.id, notification.userData, notification.notificationProfile);
        }

        protected override void ScheduleNotificationImpl(ScheduledNotification notification)
        {
            NotificationTools.ScheduleNotification((int)(notification.triggerDateTime - DateTime.Now).TotalSeconds, notification.title, notification.text, notification.id, notification.userData, notification.notificationProfile);
        }

        protected override void ScheduleNotificationRepeatingImpl(ScheduledRepeatingNotification notification)
        {
            NotificationTools.ScheduleNotificationRepeating((int)(notification.triggerDateTime - DateTime.Now).TotalSeconds, notification.intervalSeconds, notification.title, notification.text, notification.id, notification.userData, notification.notificationProfile);
        }

        public override bool NotificationsEnabled()
        {
            return NotificationTools.NotificationsEnabled();
        }

        public override void SetNotificationsEnabled(bool enabled)
        {
            NotificationTools.SetNotificationsEnabled(enabled);
        }

        public override bool PushNotificationsEnabled()
        {
            return Settings.Instance.PushNotificationsEnabledWindows && NotificationsEnabled() && pushEnabled;
        }

        public override bool NotificationsAllowed()
        {
            //Always allowed on WSA
            return true;
        }

        public override bool SetPushNotificationsEnabled(bool enabled)
        {
            if (enabled != pushEnabled)
            {
                pushEnabled = enabled;

                if (enabled)
                {
                    if (Initialized && Settings.Instance.PushNotificationsEnabledWindows)
                    {
                        NotificationTools.EnablePushNotifications(this);
                    }
                }
                else
                {
                    NotificationTools.DisablePushNotifications();
                }
            }

            return PushNotificationsEnabled();
        }

        protected override void CancelNotificationImpl(int id)
        {
            NotificationTools.CancelNotification(id);
        }

        protected override void CancelAllNotificationsImpl()
        {
            NotificationTools.CancelAllNotifications();
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
            NotSupported("badges");
            return 0;
        }

        public override void SetBadge(int bandgeNumber)
        {
            NotSupported("badges");
        }

        public override void SubscribeToTopic(string topic)
        {
            NotSupported("topics");
        }

        public override void UnsubscribeFromTopic(string topic)
        {
            NotSupported("topics");
        }

        protected override bool CleanupObsoleteScheduledNotifications(List<ScheduledNotification> scheduledNotifications)
        {
            DateTime now = DateTime.Now;
            return scheduledNotifications.RemoveAll((it) => !it.IsRepeating && (!NotificationTools.NotificationRegistered(it.id) || it.triggerDateTime < now)) > 0;
        }

        public void OnInitializationComplete(string registrationId)
        {
            m_registrationId = registrationId;
        }

        public void OnInitializationError(string error, bool wnsIssue)
        {
            Debug.LogError(string.Format("UTNotifications: error initializing {0}: {1}", wnsIssue ? "WNS" : "notifications", error));
            if (wnsIssue)
            {
                _OnPushRegistrationFailed(error);
            }
        }

        protected void Update()
        {
            if (!Initialized)
            {
                return;
            }

            if (m_registrationId != null && OnSendRegistrationIdHasSubscribers())
            {
                _OnSendRegistrationId(m_providerName, m_registrationId);
                m_registrationId = null;
            }

            if (Time.time - m_lastTimeUpdated >= m_updateEverySeconds)
            {
                IList<WSA.Notification> received;
                WSA.Notification clicked;
                NotificationTools.HandleReceivedNotifications(UnityEngine.WSA.Application.arguments, out received, out clicked);
                if (clicked != null && OnNotificationClickedHasSubscribers())
                {
                    _OnNotificationClicked(new ReceivedNotification(WSANotificationToNotification(clicked)));
                }

                if (received != null && received.Count > 0 && OnNotificationsReceivedHasSubscribers())
                {
                    List<ReceivedNotification> receivedNotifications = new List<ReceivedNotification>();
                    foreach (var it in received)
                    {
                        receivedNotifications.Add(new ReceivedNotification(WSANotificationToNotification(it)));
                    }

                    _OnNotificationsReceived(receivedNotifications);
                }

                NotificationTools.UpdateWhenRunning();
                m_lastTimeUpdated = Time.time;
            }
        }

        private static Notification WSANotificationToNotification(WSA.Notification it)
        {
            Notification notification;
            if (!string.IsNullOrEmpty(it.Provider))
            {
                notification = new PushNotification((PushNotificationsProvider)Enum.Parse(typeof(PushNotificationsProvider), it.Provider), it.Title, it.Text, it.Id);
            }
            else if (it.TriggerUnixTime > 0)
            {
                DateTime triggerDateTime = TimeUtils.UnixTimestampMillisToDateTime(it.TriggerUnixTime);

                if (it.IntervalSeconds > 0)
                {
                    notification = new ScheduledRepeatingNotification(triggerDateTime, it.IntervalSeconds, it.Title, it.Text, it.Id);
                }
                else
                {
                    notification = new ScheduledNotification(triggerDateTime, it.Title, it.Text, it.Id);
                }
            }
            else
            {
                notification = new LocalNotification(it.Title, it.Text, it.Id);
            }

            notification.SetUserData(it.UserData);
            notification.SetNotificationProfile(it.NotificationProfile);
            notification.SetBadgeNumber(it.BadgeNumber);
            
            return notification;
        }

        private bool pushEnabled
        {
            get
            {
                return PlayerPrefs.GetInt(m_pushEnabledKey, 1) != 0;
            }

            set
            {
                PlayerPrefs.SetInt(m_pushEnabledKey, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

        private volatile string m_registrationId = null;
        private float m_lastTimeUpdated = 0;
        private const float m_updateEverySeconds = 2.0f;
        private const string m_providerName = "WNS";
        private const string m_pushEnabledKey = "_UT_NOTIFICATIONS_PUSH_ENABLED";
    }
}
#endif