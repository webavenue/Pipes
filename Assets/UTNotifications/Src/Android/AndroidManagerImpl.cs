#if !UNITY_EDITOR && UNITY_ANDROID

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UTNotifications
{
    public class ManagerImpl : Manager
    {
        protected override bool InitializeImpl(bool willHandleReceivedNotifications, int startId = 0, bool incrementalId = false)
        {
            Initialized = false;

            m_willHandleReceivedNotifications = willHandleReceivedNotifications;

            bool allowUpdatingGooglePlayIfRequired = false;
            switch (Settings.Instance.AllowUpdatingGooglePlayIfRequired)
            {
            case Settings.GooglePlayUpdatingIfRequiredMode.DISABLED:
                allowUpdatingGooglePlayIfRequired = false;
                break;

            case Settings.GooglePlayUpdatingIfRequiredMode.EVERY_INITIALIZE:
                allowUpdatingGooglePlayIfRequired = true;
                break;

            case Settings.GooglePlayUpdatingIfRequiredMode.ONCE:
                const string prefKey = "_UT_NOTIFICATIONS_GP_UPDATING_WAS_ALLOWED";
                allowUpdatingGooglePlayIfRequired = (PlayerPrefs.GetInt(prefKey, 0) == 0);
                if (allowUpdatingGooglePlayIfRequired)
                {
                    PlayerPrefs.SetInt(prefKey, 1);
                }
                break;
            }

            try
            {
                using (AndroidJavaClass manager = new AndroidJavaClass("universal.tools.notifications.Manager"))
                {
                    manager.CallStatic("initialize", Settings.Instance.PushNotificationsEnabledFirebase, Settings.Instance.PushNotificationsEnabledAmazon, willHandleReceivedNotifications, startId, incrementalId, (int)Settings.Instance.AndroidShowNotificationsMode, Settings.Instance.AndroidRestoreScheduledNotificationsAfterReboot, (int)Settings.Instance.AndroidNotificationsGrouping, (int)Settings.Instance.AndroidScheduleTimerType, Settings.Instance.AndroidShowLatestNotificationOnly, Settings.Instance.AndroidScheduleExact, Settings.Instance.PushPayloadTitleFieldName, Settings.Instance.PushPayloadTextFieldName, Settings.Instance.PushPayloadUserDataParentFieldName, Settings.Instance.PushPayloadNotificationProfileFieldName, Settings.Instance.PushPayloadIdFieldName, Settings.Instance.PushPayloadBadgeFieldName, Settings.Instance.PushPayloadButtonsParentName, ProfilesSettingsJson(), allowUpdatingGooglePlayIfRequired);
                    Initialized = true;

                    LateUpdate();

                    return true;
                }
            }
            catch (AndroidJavaException e)
            {
                Debug.LogError("UTNotifications: Failed to initialize: " + e.ToString() + "\nPlease make sure that Play Services Resolver successfully added native libraries to Assets/Plugins/Android");

                Initialized = false;
                return false;
            }
        }

        protected override void PostLocalNotificationImpl(LocalNotification notification)
        {
            try
            {
                using (AndroidJavaClass manager = new AndroidJavaClass("universal.tools.notifications.Manager"))
                {
                    manager.CallStatic("postNotification", ToBase64(notification.title), ToBase64(notification.text), notification.id, ToBase64(ToString(JsonUtils.ToJson(notification.userData))), notification.notificationProfile, notification.badgeNumber, ToBase64(ToString(JsonUtils.ToJson(notification.buttons))));
                }
            }
            catch (AndroidJavaException e)
            {
                Debug.LogException(e);
            }
        }

        protected override void ScheduleNotificationImpl(ScheduledNotification notification)
        {
            try
            {
                using (AndroidJavaClass manager = new AndroidJavaClass("universal.tools.notifications.Manager"))
                {
                    manager.CallStatic("scheduleNotification", TimeUtils.ToSecondsFromNow(notification.triggerDateTime), ToBase64(notification.title), ToBase64(notification.text), notification.id, ToBase64(ToString(JsonUtils.ToJson(notification.userData))), notification.notificationProfile, notification.badgeNumber, ToBase64(ToString(JsonUtils.ToJson(notification.buttons))));
                }
            }
            catch (AndroidJavaException e)
            {
                Debug.LogException(e);
            }
        }

        protected override void ScheduleNotificationRepeatingImpl(ScheduledRepeatingNotification notification)
        {
            try
            {
                using (AndroidJavaClass manager = new AndroidJavaClass("universal.tools.notifications.Manager"))
                {
                    manager.CallStatic("scheduleNotificationRepeating", TimeUtils.ToSecondsFromNow(notification.triggerDateTime), notification.intervalSeconds, ToBase64(notification.title), ToBase64(notification.text), notification.id, ToBase64(ToString(JsonUtils.ToJson(notification.userData))), notification.notificationProfile, notification.badgeNumber, ToBase64(ToString(JsonUtils.ToJson(notification.buttons))));
                }
            }
            catch (AndroidJavaException e)
            {
                Debug.LogException(e);
            }
        }

        public override bool NotificationsEnabled()
        {
            try
            {
                using (AndroidJavaClass manager = new AndroidJavaClass("universal.tools.notifications.Manager"))
                {
                    return manager.CallStatic<bool>("notificationsEnabled");
                }
            }
            catch (AndroidJavaException e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public override bool NotificationsAllowed()
        {
            try
            {
                using (AndroidJavaClass manager = new AndroidJavaClass("universal.tools.notifications.Manager"))
                {
                    return manager.CallStatic<bool>("notificationsAllowed");
                }
            }
            catch (AndroidJavaException e)
            {
                Debug.LogException(e);
                return true;
            }
        }

        public override void SetNotificationsEnabled(bool enabled)
        {
            try
            {
                using (AndroidJavaClass manager = new AndroidJavaClass("universal.tools.notifications.Manager"))
                {
                    manager.CallStatic("setNotificationsEnabled", enabled);
                }
            }
            catch (AndroidJavaException e)
            {
                Debug.LogException(e);
            }
        }

        public override bool PushNotificationsEnabled()
        {
            try
            {
                using (AndroidJavaClass manager = new AndroidJavaClass("universal.tools.notifications.Manager"))
                {
                    return manager.CallStatic<bool>("pushNotificationsEnabled") &&
                            ((Settings.Instance.PushNotificationsEnabledFirebase && manager.CallStatic<bool>("fcmProviderAvailable", false)) ||
                            (Settings.Instance.PushNotificationsEnabledAmazon && manager.CallStatic<bool>("admProviderAvailable")));
                }
            }
            catch (AndroidJavaException e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public override bool SetPushNotificationsEnabled(bool enabled)
        {
            try
            {
                using (AndroidJavaClass manager = new AndroidJavaClass("universal.tools.notifications.Manager"))
                {
                    manager.CallStatic("setPushNotificationsEnabled", enabled);
                    return PushNotificationsEnabled();
                }
            }
            catch (AndroidJavaException e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        protected override void CancelNotificationImpl(int id)
        {
            try
            {
                using (AndroidJavaClass manager = new AndroidJavaClass("universal.tools.notifications.Manager"))
                {
                    manager.CallStatic("cancelNotification", id);
                }
            }
            catch (AndroidJavaException e)
            {
                Debug.LogException(e);
            }

            HideNotification(id);
        }

        public override void HideNotification(int id)
        {
            if (!CheckInitialized())
            {
                return;
            }

            try
            {
                using (AndroidJavaClass manager = new AndroidJavaClass("universal.tools.notifications.Manager"))
                {
                    manager.CallStatic("hideNotification", id);
                }
            }
            catch (AndroidJavaException e)
            {
                Debug.LogException(e);
            }
        }

        public override void HideAllNotifications()
        {
            if (!CheckInitialized())
            {
                return;
            }

            try
            {
                using (AndroidJavaClass manager = new AndroidJavaClass("universal.tools.notifications.Manager"))
                {
                    manager.CallStatic("hideAllNotifications");
                }
            }
            catch (AndroidJavaException e)
            {
                Debug.LogException(e);
            }
        }

        protected override void CancelAllNotificationsImpl()
        {
            try
            {
                using (AndroidJavaClass manager = new AndroidJavaClass("universal.tools.notifications.Manager"))
                {
                    manager.CallStatic("cancelAllNotifications");
                }
            }
            catch (AndroidJavaException e)
            {
                Debug.LogException(e);
            }
        }

        public override int GetBadge()
        {
            try
            {
                using (AndroidJavaClass manager = new AndroidJavaClass("universal.tools.notifications.Manager"))
                {
                    return manager.CallStatic<int>("getBadge");
                }
            }
            catch (AndroidJavaException e)
            {
                Debug.LogException(e);
                return 0;
            }
        }
        
        public override void SetBadge(int bandgeNumber)
        {
            try
            {
                using (AndroidJavaClass manager = new AndroidJavaClass("universal.tools.notifications.Manager"))
                {
                    manager.CallStatic("setBadge", bandgeNumber);
                }
            }
            catch (AndroidJavaException e)
            {
                Debug.LogException(e);
            }
        }

        public override void SubscribeToTopic(string topic)
        {
            if (!CheckInitialized())
            {
                return;
            }

            try
            {
                using (AndroidJavaClass manager = new AndroidJavaClass("universal.tools.notifications.Manager"))
                {
                    manager.CallStatic("subscribeToTopic", topic);
                }
            }
            catch (AndroidJavaException e)
            {
                Debug.LogException(e);
            }
        }

        public override void UnsubscribeFromTopic(string topic)
        {
            if (!CheckInitialized())
            {
                return;
            }

            try
            {
                using (AndroidJavaClass manager = new AndroidJavaClass("universal.tools.notifications.Manager"))
                {
                    manager.CallStatic("unsubscribeFromTopic", topic);
                }
            }
            catch (AndroidJavaException e)
            {
                Debug.LogException(e);
            }
        }

        protected override bool CleanupObsoleteScheduledNotifications(List<ScheduledNotification> scheduledNotifications)
        {
            try
            {
                using (AndroidJavaClass manager = new AndroidJavaClass("universal.tools.notifications.Manager"))
                {
                    string scheduledIdsJsonString = manager.CallStatic<string>("getStoredScheduledNotificationIdsJson");
                    JSONArray scheduledIdsJson = (JSONArray)JSON.Parse(scheduledIdsJsonString);

                    return scheduledNotifications.RemoveAll((it) =>
                    {
                        foreach (var id in scheduledIdsJson)
                        {
                            if (it.id == ((JSONNode)id).AsInt)
                            {
                                // It's still in the list. Keep it.
                                return false;
                            }
                        }

                        // Notification is not registered anymore. Cleanup!
                        return true;
                    }) > 0;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public void _OnAndroidIdReceived(string providerAndId)
        {
            // _OnAndroidIdReceived can be invoked inside initialize(...), it means initialzation is successful.
            Initialized = true;

            if (OnSendRegistrationIdHasSubscribers())
            {
                JSONNode json = JSON.Parse(providerAndId);
                _OnSendRegistrationId(json[0].Value, json[1].Value);
            }
        }

        public void _OnAndroidPushRegistrationFailed(string error)
        {
            // _OnAndroidPushRegistrationFailed can be invoked inside initialize(...),
            // it means basic initialzation is successful, though registration for push notifications has failed.
            Initialized = true;
            _OnPushRegistrationFailed(error);
        }

        protected void LateUpdate()
        {
            if (!Initialized)
            {
                return;
            }

            m_timeToCheckForIncomingNotifications -= Time.unscaledDeltaTime;
            if (m_timeToCheckForIncomingNotifications > 0)
            {
                return;
            }

            m_timeToCheckForIncomingNotifications = m_timeBetweenCheckingForIncomingNotifications;

            if (OnNotificationClickedHasSubscribers())
            {
                try
                {
                    using (AndroidJavaClass manager = new AndroidJavaClass("universal.tools.notifications.Manager"))
                    {
                        HandleClickedNotification(manager.CallStatic<string>("getClickedNotificationPacked"));
                    }
                }
                catch (AndroidJavaException e)
                {
                    Debug.LogException(e);
                }
            }

            if (m_willHandleReceivedNotifications && OnNotificationsReceivedHasSubscribers())
            {
                try
                {
                    using (AndroidJavaClass manager = new AndroidJavaClass("universal.tools.notifications.Manager"))
                    {
                        HandleReceivedNotifications(manager.CallStatic<string>("getReceivedNotificationsPacked"));
                    }
                }
                catch (AndroidJavaException e)
                {
                    Debug.LogException(e);
                }
            }
        }

        protected void OnApplicationQuit()
        {
            try
            {
                using (AndroidJavaClass manager = new AndroidJavaClass("universal.tools.notifications.Manager"))
                {
                    manager.CallStatic("onApplicationQuit");
                }
            }
            catch (AndroidJavaException e)
            {
                Debug.LogException(e);
            }
        }

        private void HandleClickedNotification(string receivedNotificationPacked)
        {
            if (!string.IsNullOrEmpty(receivedNotificationPacked))
            {
                ReceivedNotification notification;
                try
                {
                    notification = ParseReceivedNotification(JSON.Parse(receivedNotificationPacked), true);
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                    return;
                }

                _OnNotificationClicked(notification);
            }
        }

        private void HandleReceivedNotifications(string receivedNotificationsPacked)
        {
            if (string.IsNullOrEmpty(receivedNotificationsPacked) || receivedNotificationsPacked == "[]")
            {
                return;
            }

            List<ReceivedNotification> receivedNotifications = new List<ReceivedNotification>();

            JSONNode notificationsList = JSON.Parse(receivedNotificationsPacked);
            for (int i = 0; i < notificationsList.Count; ++i)
            {
                try
                {
                    JSONNode json = notificationsList[i];

                    ReceivedNotification receivedNotification = ParseReceivedNotification(json, false);

                    //Update out-of-date notifications
                    bool updated = false;
                    for (int j = 0; j < receivedNotifications.Count; ++j)
                    {
                        if (receivedNotifications[j].id == receivedNotification.id)
                        {
                            receivedNotifications[j] = receivedNotification;
                            updated = true;
                            break;
                        }
                    }
                    if (!updated)
                    {
                        receivedNotifications.Add(receivedNotification);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }

            if (receivedNotifications.Count > 0)
            {
                _OnNotificationsReceived(receivedNotifications);
            }
        }

        private static ReceivedNotification ParseReceivedNotification(JSONNode json, bool clicked)
        {
            if (clicked)
            {
                return new ClickedNotification(json);
            }
            else
            {
                return new ReceivedNotification(json);
            }
        }

        private static string ProfilesSettingsJson()
        {
            JSONArray json = new JSONArray();

            foreach (var it in Settings.Instance.NotificationProfiles)
            {
                JSONClass node = new JSONClass();
                node.Add("id", new JSONData(it.profileName != Settings.DEFAULT_PROFILE_NAME ? it.profileName : Settings.DEFAULT_PROFILE_NAME_INTERNAL));
                node.Add("name", !string.IsNullOrEmpty(it.androidChannelName) ? it.androidChannelName : it.profileName);
                node.Add("description", it.androidChannelDescription ?? "");
                node.Add("high_priority", new JSONData(it.androidHighPriority));
                if (it.colorSpecified)
                {
                    Color32 color32 = it.androidColor;
                    int intColor = color32.a << 24 | color32.r << 16 | color32.g << 8 | color32.b;
                    node.Add("color", new JSONData(intColor));
                }

                json.Add(node);
            }
            
            return json.ToString();
        }

        private static string ToString(object o)
        {
            if (o != null)
            {
                return o.ToString();
            }
            else
            {
                return null;
            }
        }

        private static string ToBase64(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            else
            {
                return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(str));
            }
        }

        private bool m_willHandleReceivedNotifications;
        private const float m_timeBetweenCheckingForIncomingNotifications = 0.5f;
        private float m_timeToCheckForIncomingNotifications = 0;
    }
}
#endif //UNITY_ANDROID