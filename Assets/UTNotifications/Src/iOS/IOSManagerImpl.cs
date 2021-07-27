#if !UNITY_EDITOR && UNITY_IOS

using UnityEngine.iOS;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

namespace UTNotifications
{
    public class ManagerImpl : Manager
    {
        protected override bool InitializeImpl(bool willHandleReceivedNotifications, int startId = 0, bool incrementalId = false)
        {
            m_enabled = false;
            m_pushEnabled = UnityEngine.PlayerPrefs.GetInt(m_pushEnabledOptionName, 1) != 0;
            SetNotificationsEnabled(UnityEngine.PlayerPrefs.GetInt(m_enabledOptionName, 1) != 0, true);

            m_willHandleReceivedNotifications = willHandleReceivedNotifications;
            m_nextPushNotificationId = startId;
            m_incrementalId = incrementalId;

            Initialized = true;

            // Force an update to get clicked/received notifications as fast as possible.
            LateUpdate();

            return true;
        }

        protected override void PostLocalNotificationImpl(LocalNotification notification)
        {
            if (m_enabled)
            {
                NotificationServices.PresentLocalNotificationNow(ToIOSLocalNotification(notification));
            }
        }

        protected override void ScheduleNotificationImpl(ScheduledNotification notification)
        {
            if (m_enabled)
            {
                NotificationServices.ScheduleLocalNotification(ToIOSScheduledNotification(notification));
            }
        }

        //Please note that the actual interval may be different.
        //On iOS there are only fixed options like every minute, every day, every week and so on. So the provided <c>intervalSeconds</c> value will be approximated by one of the available options.
        protected override void ScheduleNotificationRepeatingImpl(ScheduledRepeatingNotification notification)
        {
            if (m_enabled)
            {
                NotificationServices.ScheduleLocalNotification(ToIOSScheduledRepeatingNotification(notification));
            }
        }

        public override bool NotificationsEnabled()
        {
            if (Initialized)
            {
                return m_enabled;
            }
            else
            {
                return UnityEngine.PlayerPrefs.GetInt(m_enabledOptionName, 1) != 0;
            }
        }

        public override bool NotificationsAllowed()
        {
            return _UT_NotificationsAllowed();
        }

        public override void SetNotificationsEnabled(bool enabled)
        {
            SetNotificationsEnabled(enabled, false);
        }

        public override bool PushNotificationsEnabled()
        {
            bool enabled;
            if (Initialized)
            {
                enabled = m_pushEnabled;
            }
            else
            {
                enabled = UnityEngine.PlayerPrefs.GetInt(m_pushEnabledOptionName, 1) != 0;
            }

            return enabled && Settings.Instance.PushNotificationsEnabledIOS && NotificationsEnabled();
        }

        public override bool SetPushNotificationsEnabled(bool enabled)
        {
            // Should be done before (enable == m_pushEnabled) check in order to disable push notifications before Manager initialization
            UnityEngine.PlayerPrefs.SetInt(m_pushEnabledOptionName, enabled ? 1 : 0);

            if (enabled && !Settings.Instance.PushNotificationsEnabledIOS)
            {
                UnityEngine.Debug.LogWarning("Can't enable push notifications: iOS -> Push Notifications are disabled in the UTNotifications Settings");
            }
            
            if (!Initialized || enabled == m_pushEnabled)
            {
                return PushNotificationsEnabled();
            }
            
            m_pushEnabled = enabled;

            if (m_enabled && Settings.Instance.PushNotificationsEnabledIOS)
            {
                if (m_pushEnabled)
                {
                    RegisterForNotifications();
                }
                else
                {
                    NotificationServices.UnregisterForRemoteNotifications();
                }
            }

            return PushNotificationsEnabled();
        }

        protected override void CancelNotificationImpl(int id)
        {
            string idAsString = id.ToString();

            if (NotificationServices.localNotifications != null)
            {
                foreach (var notification in NotificationServices.localNotifications)
                {
                    if (notification.userInfo.Contains(m_idKeyName) && (string)notification.userInfo[m_idKeyName] == idAsString)
                    {
                        NotificationServices.CancelLocalNotification(notification);
                        return;
                    }
                }
            }

            if (NotificationServices.scheduledLocalNotifications != null)
            {
                foreach (var notification in NotificationServices.scheduledLocalNotifications)
                {
                    if (notification.userInfo.Contains(m_idKeyName) && (string)notification.userInfo[m_idKeyName] == idAsString)
                    {
                        NotificationServices.CancelLocalNotification(notification);
                        return;
                    }
                }
            }
        }

        public override void HideNotification(int id)
        {
            if (!CheckInitialized())
            {
                return;
            }
            
            if (NotificationServices.localNotifications == null)
            {
                return;
            }

            string idAsString = id.ToString();

            foreach (var notification in NotificationServices.localNotifications)
            {
                if (notification.userInfo.Contains(m_idKeyName) && (string)notification.userInfo[m_idKeyName] == idAsString)
                {
                    HideNotification(notification);
                    return;
                }
            }
        }

        public override void HideAllNotifications()
        {
            if (!CheckInitialized())
            {
                return;
            }
            
            if (NotificationServices.localNotifications != null)
            {
                foreach (var notification in NotificationServices.localNotifications)
                {
                    HideNotification(notification);
                }
                NotificationServices.ClearLocalNotifications();
            }

            if (NotificationServices.remoteNotificationCount > 0)
            {
                NotificationServices.ClearRemoteNotifications();
            }

            _UT_HideAllNotifications();
        }

        protected override void CancelAllNotificationsImpl()
        {
            HideAllNotifications();
            NotificationServices.CancelAllLocalNotifications();
        }

        protected override bool CleanupObsoleteScheduledNotifications(List<ScheduledNotification> scheduledNotifications)
        {
            if (Initialized && m_enabled)
            {
                var scheduledIds = new HashSet<int>();
                foreach (var notification in NotificationServices.scheduledLocalNotifications)
                {
                    scheduledIds.Add(ExtractId(notification.userInfo));
                }

                return scheduledNotifications.RemoveAll((it) => !scheduledIds.Contains(it.id)) > 0;
            }
            else
            {
                DateTime now = DateTime.Now;
                return scheduledNotifications.RemoveAll((it) => !it.IsRepeating && it.triggerDateTime < now) > 0;
            }
        }

        public override int GetBadge()
        {
            if (!CheckInitialized())
            {
                return 0;
            }
            
            return Math.Max(UnityEngine.PlayerPrefs.GetInt(m_badgeOptionName, 0), _UT_GetIconBadgeNumber());
        }

        public override void SetBadge(int bandgeNumber)
        {
            if (!CheckInitialized())
            {
                return;
            }
            
            if (m_enabled)
            {
                _UT_SetIconBadgeNumber(bandgeNumber);
            }

            UnityEngine.PlayerPrefs.SetInt(m_badgeOptionName, bandgeNumber);
        }

        public override void SubscribeToTopic(string topic)
        {
            NotSupported("topics");
        }

        public override void UnsubscribeFromTopic(string topic)
        {
            NotSupported("topics");
        }

    //protected
        protected void LateUpdate()
        {
            if (!Initialized)
            {
                return;
            }
            
            if (m_initRemoteNotifications)
            {
                if (!string.IsNullOrEmpty(NotificationServices.registrationError))
                {
                    m_initRemoteNotifications = false;
                    _OnPushRegistrationFailed(NotificationServices.registrationError);
                }
                else if (NotificationServices.deviceToken != null && NotificationServices.deviceToken.Length > 0 && OnSendRegistrationIdHasSubscribers())
                {
                    m_initRemoteNotifications = false;
                    _OnSendRegistrationId(m_providerName, ByteArrayToHexString(NotificationServices.deviceToken));
                }
            }

            CleanupDuplications();

            //Handle received/clicked notifications
            int localCount = (NotificationServices.localNotifications != null) ? NotificationServices.localNotifications.Length : 0;
            int remoteCount = (NotificationServices.remoteNotifications != null) ? NotificationServices.remoteNotifications.Length : 0;

            List<ReceivedNotification> list;
            if ((localCount > 0 || remoteCount > 0) && m_willHandleReceivedNotifications && OnNotificationsReceivedHasSubscribers())
            {
                list = new List<ReceivedNotification>(localCount + remoteCount);
            }
            else
            {
                list = null;
            }

            if (list == null && !OnNotificationClickedHasSubscribers())
            {
                return;
            }

            if (localCount > 0)
            {
                foreach (var notification in NotificationServices.localNotifications)
                {
                    ReceivedNotification receivedNotification = null;

                    //Check if the notification was clicked
                    if (OnNotificationClickedHasSubscribers())
                    {
                        int id = ExtractId(notification.userInfo);

                        if (_UT_LocalNotificationWasClicked(id))
                        {
                            receivedNotification = ToReceivedNotification(notification);
                            _OnNotificationClicked(receivedNotification);

                            if (list == null)
                            {
                                break;
                            }
                        }
                    }
                    
                    if (list != null)
                    {
                        if (receivedNotification == null)
                        {
                            receivedNotification = ToReceivedNotification(notification);
                        }

                        list.Add(receivedNotification);
                    }
                }
            }

            if (remoteCount > 0)
            {
                foreach (var notification in NotificationServices.remoteNotifications)
                {
                    ReceivedNotification receivedNotification = null;

                    //Check if the notification was clicked
                    if (OnNotificationClickedHasSubscribers() && _UT_PushNotificationWasClicked(notification.alertBody))
                    {
                        receivedNotification = ToReceivedNotification(notification);
                        _OnNotificationClicked(receivedNotification);

                        if (list == null)
                        {
                            break;
                        }
                    }

                    if (list != null)
                    {
                        if (receivedNotification == null)
                        {
                            receivedNotification = ToReceivedNotification(notification);
                        }

                        list.Add(receivedNotification);
                    }
                }
            }

            if (list != null && list.Count > 0)
            {
                HideAllNotifications();
                _OnNotificationsReceived(list);
            }
        }

    //private
        private void SetNotificationsEnabled(bool enabled, bool inInitialization)
        {
            UnityEngine.PlayerPrefs.SetInt(m_enabledOptionName, enabled ? 1 : 0);

            if (!inInitialization && !Initialized)
            {
                if (!enabled)
                {
                    NotificationServices.UnregisterForRemoteNotifications();
                }

                return;
            }

            if (m_enabled == enabled)
            {
                return;
            }

            if (enabled)
            {
                RegisterForNotifications();

                if (!inInitialization)
                {
                    // Reschedule local notifications, if required
                    var scheduledNotifications = ScheduledNotifications;

                    if (scheduledNotifications != null && scheduledNotifications.Count > 0)
                    {
                        DateTime nowPlus2Sec = DateTime.Now.AddSeconds(2);

                        foreach (var scheduledNotification in scheduledNotifications)
                        {
                            var notification = scheduledNotification.IsRepeating
                                ? ToIOSScheduledRepeatingNotification((ScheduledRepeatingNotification)scheduledNotification)
                                : ToIOSScheduledNotification(scheduledNotification);

                            if (notification.fireDate != DateTime.MinValue && notification.fireDate <= nowPlus2Sec)
                            {
                                if (!scheduledNotification.IsRepeating)
                                {
                                    notification.fireDate = nowPlus2Sec;
                                }
                                else
                                {
                                    notification.fireDate = NextFutureFireDate(notification.fireDate, notification.repeatInterval, nowPlus2Sec);
                                }
                            }

                            NotificationServices.ScheduleLocalNotification(notification);
                        }
                    }

                    SetBadge(GetBadge());
                }
            }
            else
            {
                CancelAllNotificationsImpl();
                NotificationServices.UnregisterForRemoteNotifications();
                _UT_SetIconBadgeNumber(0);
            }

            m_enabled = enabled;
        }

        private static DateTime NextFutureFireDate(DateTime fireDate, CalendarUnit repeatInterval, DateTime earliestFireDate)
        {
            double diffSeconds = (earliestFireDate - fireDate).TotalSeconds;

            if (diffSeconds < 0)
            {
                return fireDate;
            }

            double period = RepeatIntervalToSeconds(repeatInterval, fireDate);
            if (diffSeconds / period < 256.0)
            {
                // Try to be as accurate as possible, but don't take more than 256 iterations
                while (fireDate < earliestFireDate)
                {
                    fireDate = fireDate.AddSeconds(RepeatIntervalToSeconds(repeatInterval, fireDate));
                }
            }
            else
            {
                fireDate = fireDate.AddSeconds(((int)(diffSeconds / period) + 1) * period);
            }

            return fireDate;
        }

        private ReceivedNotification ToReceivedNotification(UnityEngine.iOS.LocalNotification notification)
        {
            string title, text;
#if UNITY_2018_2_OR_NEWER
            title = notification.alertTitle;
            text = notification.alertBody;
#else
            ExtractTitleAndText(notification.alertBody, out title, out text);
#endif
            int id = ExtractId(notification.userInfo);

            LocalNotification localNotification;
            if (notification.fireDate != DateTime.MinValue)
            {
                DateTime triggerDateTime = notification.fireDate;

                if (notification.repeatInterval > CalendarUnit.Era)
                {
                    localNotification = new ScheduledRepeatingNotification(triggerDateTime, (int)RepeatIntervalToSeconds(notification.repeatInterval, triggerDateTime), title, text, id);
                }
                else
                {
                    localNotification = new ScheduledNotification(triggerDateTime, title, text, id);
                }
            }
            else
            {
                localNotification = new LocalNotification(title, text, id);
            }

            Dictionary<string, string> userData = UserInfoToDictionaryOfStrings(notification.userInfo);
            if (userData != null)
            {
                if (userData.ContainsKey(m_idKeyName))
                {
                    userData.Remove(m_idKeyName);
                }

                localNotification.SetUserData(userData);
            }

            localNotification.SetBadgeNumber(notification.applicationIconBadgeNumber);
            string profile = ExtractNotificationProfile(notification.soundName);
            if (!string.IsNullOrEmpty(profile))
            {
                localNotification.SetNotificationProfile(profile);
            }

            return new ReceivedNotification(localNotification);
        }

        private ReceivedNotification ToReceivedNotification(RemoteNotification notification)
        {
            int id = ExtractId(notification.userInfo);
            if (id == -1 && m_incrementalId)
            {
                id = m_nextPushNotificationId++;
            }

            string title, text;
#if UNITY_2018_2_OR_NEWER
            title = notification.alertTitle;
            text = notification.alertBody;
#else
            ExtractTitleAndText(notification.alertBody, out title, out text);
#endif

            Dictionary<string, string> userData = UserInfoToDictionaryOfStrings(notification.userInfo);
            if (userData != null && userData.ContainsKey(m_idKeyName))
            {
                userData.Remove(m_idKeyName);
            }

            PushNotification pushNotification = new PushNotification(PushNotificationsProvider.APNS, title, text, id, userData, ExtractNotificationProfile(notification.soundName), notification.applicationIconBadgeNumber, null);
            return new ReceivedNotification(pushNotification);
        }

        private UnityEngine.iOS.LocalNotification ToIOSLocalNotification(LocalNotification notification)
        {
            CancelNotificationImpl(notification.id);

            var iosNotification = new UnityEngine.iOS.LocalNotification();
            if (notification.badgeNumber >= 0)
            {
                iosNotification.applicationIconBadgeNumber = notification.badgeNumber;
            }
#if UNITY_2018_2_OR_NEWER
            iosNotification.alertTitle = notification.title;
            iosNotification.alertBody = notification.text;
#else
            iosNotification.alertAction = notification.title;
            iosNotification.alertBody = string.IsNullOrEmpty(notification.title) ? notification.text : notification.title + "\n" + notification.text;
            iosNotification.hasAction = true;
#endif

            if (!string.IsNullOrEmpty(notification.notificationProfile))
            {
                iosNotification.soundName = m_soundNamePrefix + notification.notificationProfile;
            }
            else
            {
                iosNotification.soundName = UnityEngine.iOS.LocalNotification.defaultSoundName;
            }
            var userInfo = (notification.userData != null) ? new Dictionary<string, string>(notification.userData) : new Dictionary<string, string>();
            userInfo.Add(m_idKeyName, notification.id.ToString());
            userInfo.Add(m_timestampKey, DateTime.UtcNow.ToFileTimeUtc().ToString());
            iosNotification.userInfo = userInfo;

            return iosNotification;
        }

        private UnityEngine.iOS.LocalNotification ToIOSScheduledNotification(ScheduledNotification notification)
        {
            var iosNotification = ToIOSLocalNotification(notification);

            iosNotification.fireDate = notification.triggerDateTime;

            return iosNotification;
        }

        private UnityEngine.iOS.LocalNotification ToIOSScheduledRepeatingNotification(ScheduledRepeatingNotification notification)
        {
            var iosNotification = ToIOSScheduledNotification(notification);

            if (notification.intervalSeconds <= 0)
            {
                return iosNotification;
            }

            if (notification.intervalSeconds < 3)
            {
                //Approximate it to every second if the desired intervalSeconds is less then 3 seconds
                iosNotification.repeatInterval = CalendarUnit.Second;
            }
            else if (notification.intervalSeconds < TimeUtils.MinutesToSeconds(3))
            {
                //Approximate it to every minute if the desired intervalSeconds is less then 3 minutes
                iosNotification.repeatInterval = CalendarUnit.Minute;
            }
            else if (notification.intervalSeconds < TimeUtils.HoursToSeconds(3))
            {
                //Approximate it to every hour if the desired intervalSeconds is less then 3 hours
                iosNotification.repeatInterval = CalendarUnit.Hour;
            }
            else if (notification.intervalSeconds < TimeUtils.DaysToSeconds(3))
            {
                //Approximate it to every day if the desired intervalSeconds is less then 3 days
                iosNotification.repeatInterval = CalendarUnit.Day;
            }
            else if (notification.intervalSeconds < TimeUtils.WeeksToSeconds(3))
            {
                //Approximate it to every week if the desired intervalSeconds is less then 3 weeks
                iosNotification.repeatInterval = CalendarUnit.Week;
            }
            else if (notification.intervalSeconds < TimeUtils.DaysToSeconds(2 * 30))
            {
                //Approximate it to every month if the desired intervalSeconds is less then 2 months
                iosNotification.repeatInterval = CalendarUnit.Month;
            }
            else if (notification.intervalSeconds < TimeUtils.DaysToSeconds(183))
            {
                //Approximate it to every quarter if the desired intervalSeconds is less then half a year
                iosNotification.repeatInterval = CalendarUnit.Quarter;
            }
            else if (notification.intervalSeconds < TimeUtils.DaysToSeconds(3 * 365))
            {
                //Approximate it to every year if the desired intervalSeconds is less then 3 years
                iosNotification.repeatInterval = CalendarUnit.Year;
            }
            else
            {
                UnityEngine.Debug.LogWarning("Suspicious intervalSeconds value provided: " + notification.intervalSeconds);
                iosNotification.repeatInterval = CalendarUnit.Era;
            }

            return iosNotification;
        }

        private void HideNotification(UnityEngine.iOS.LocalNotification notification)
        {
            if (notification.repeatInterval > CalendarUnit.Era)
            {
                var replacementNotification = new UnityEngine.iOS.LocalNotification();
                replacementNotification.applicationIconBadgeNumber = notification.applicationIconBadgeNumber;
#if UNITY_2018_2_OR_NEWER
                replacementNotification.alertTitle = notification.alertTitle;
#endif
                replacementNotification.alertBody = notification.alertBody;
                replacementNotification.alertAction = notification.alertAction;
                replacementNotification.hasAction = notification.hasAction;
                replacementNotification.fireDate = NextFutureFireDate(notification.fireDate, notification.repeatInterval, DateTime.Now.AddSeconds(2));
                replacementNotification.repeatInterval = notification.repeatInterval;
                replacementNotification.soundName = notification.soundName;
                replacementNotification.userInfo = notification.userInfo;
                
                NotificationServices.ScheduleLocalNotification(replacementNotification);
            }
            
            NotificationServices.CancelLocalNotification(notification);
        }

        private Dictionary<string, string> UserInfoToDictionaryOfStrings(IDictionary userInfo)
        {
            if (userInfo == null || userInfo.Count == 0)
            {
                return null;
            }

            Dictionary<string, string> userData = new Dictionary<string, string>();
            foreach (var key in userInfo.Keys)
            {
                if (key is string)
                {
                    object value = userInfo[key];
                    if (value is string)
                    {
                        userData.Add((string)key, (string)value);
                    }
                }
            }

            return userData;
        }

        private static double RepeatIntervalToSeconds(CalendarUnit repeatInterval, DateTime baseTime)
        {
            DateTime futureTime;

            switch (repeatInterval)
            {
            case CalendarUnit.Year:
                futureTime = baseTime.AddYears(1);
                break;
                
            case CalendarUnit.Quarter:
                futureTime = baseTime.AddMonths(3);
                break;

            case CalendarUnit.Month:
                futureTime = baseTime.AddMonths(1);
                break;

            case CalendarUnit.Week:
                futureTime = baseTime.AddDays(7);
                break;

            case CalendarUnit.Day:
                futureTime = baseTime.AddDays(1);
                break;
                
            case CalendarUnit.Hour:
                futureTime = baseTime.AddHours(1);
                break;
                
            case CalendarUnit.Minute:
                futureTime = baseTime.AddMinutes(1);
                break;
                
            case CalendarUnit.Second:
                futureTime = baseTime.AddSeconds(1);
                break;
                
            default:
                UnityEngine.Debug.LogWarning("Unexpected repeatInterval: " + repeatInterval);
                return TimeUtils.DaysToSeconds(100 * 365);
            }

            return (futureTime - baseTime).TotalSeconds;
        }

        private void RegisterForNotifications()
        {
            m_initRemoteNotifications = Settings.Instance.PushNotificationsEnabledIOS && m_pushEnabled;
            NotificationServices.RegisterForNotifications(NotificationType.Badge | NotificationType.Alert | NotificationType.Sound, m_initRemoteNotifications);
        }

        private static int ExtractId(IDictionary userInfo)
        {
            return userInfo != null && userInfo.Contains(m_idKeyName) ? int.Parse((string)userInfo[m_idKeyName]) : -1;
        }

        private static long ExtractTimestamp(IDictionary userInfo)
        {
            return userInfo != null && userInfo.Contains(m_timestampKey) ? long.Parse((string)userInfo[m_timestampKey]) : 0;
        }

#if !UNITY_2018_2_OR_NEWER
        private static void ExtractTitleAndText(string alertBody, out string title, out string text)
        {
            if (alertBody == null)
            {
                title = "";
                text = "";
                return;
            }

            int endlinePos = alertBody.IndexOf('\n');
            if (endlinePos < 0)
            {
                title = "";
                text = alertBody;
            }
            else
            {
                title = alertBody.Substring(0, endlinePos);
                text = alertBody.Substring(endlinePos + 1);
            }
        }
#endif

        private static string ExtractNotificationProfile(string soundName)
        {
            if (soundName == UnityEngine.iOS.LocalNotification.defaultSoundName)
            {
                return null;
            }
            else
            {
                return string.IsNullOrEmpty(soundName) ? "" : soundName.Replace(m_soundNamePrefix, "");
            }
        }

        private static string ByteArrayToHexString(byte[] byteArray)
        {
            string hex = BitConverter.ToString(byteArray);
            return hex.Replace("-", "").ToLower();
        }

        private void CleanupDuplications()
        {
            const float CHECK_INTERVAL = 1.1f;
            
            float time = UnityEngine.Time.unscaledTime;
            if (time - lastDuplicationCheck < CHECK_INTERVAL)
            {
                return;
            }
            lastDuplicationCheck = time;
            
            var dictionary = new Dictionary<int, List<UnityEngine.iOS.LocalNotification>>();
            if (NotificationServices.scheduledLocalNotifications != null)
            {
                foreach (var notification in NotificationServices.scheduledLocalNotifications)
                {
                    int id = notification.userInfo != null ? ExtractId(notification.userInfo) : -1;
                    if (dictionary.ContainsKey(id))
                    {
                        dictionary[id].Add(notification);
                    }
                    else
                    {
                        dictionary.Add(id, new List<UnityEngine.iOS.LocalNotification>{notification});
                    }
                }
            }

            foreach (var entry in dictionary)
            {
                var list = entry.Value;
                if (list.Count > 1)
                {
                    UnityEngine.Debug.Log("UTNotifications: local notification duplication with id " + entry.Key + " detected. Cleaning up.");

                    // Sort by timestamps (newest first)
                    list.Sort((x, y) => ExtractTimestamp(y.userInfo).CompareTo(ExtractTimestamp(x.userInfo)));

                    // Remove outdated duplications
                    for (int i = 1; i < list.Count; ++i)
                    {
                        NotificationServices.CancelLocalNotification(list[i]);
                    }
                }
            }
        }

        [DllImport("__Internal")]
        private static extern int _UT_GetIconBadgeNumber();

        [DllImport("__Internal")]
        private static extern void _UT_SetIconBadgeNumber(int value);

        [DllImport("__Internal")]
        private static extern void _UT_HideAllNotifications();

        [DllImport("__Internal")]
        private static extern bool _UT_LocalNotificationWasClicked(int id);

        [DllImport("__Internal")]
        private static extern bool _UT_PushNotificationWasClicked(string body);

        [DllImport("__Internal")]
        private static extern bool _UT_NotificationsAllowed();

        private const string m_idKeyName = "id";
        private const string m_timestampKey = "created_timestamp";
        private const string m_enabledOptionName = "_UT_NOTIFICATIONS_ENABLED";
        private const string m_pushEnabledOptionName = "_UT_NOTIFICATIONS_PUSH_ENABLED";
        private const string m_badgeOptionName = "_UT_NOTIFICATIONS_BADGE_VALUE";
        private const string m_soundNamePrefix = "Data/Raw/";
        private static readonly string m_providerName = PushNotificationsProvider.APNS.ToString();
        private bool m_willHandleReceivedNotifications;
        private bool m_initRemoteNotifications;
        private int m_nextPushNotificationId;
        private bool m_incrementalId;
        private bool m_enabled = false;
        private bool m_pushEnabled = false;
        private float lastDuplicationCheck = 0;
    }
}
#endif