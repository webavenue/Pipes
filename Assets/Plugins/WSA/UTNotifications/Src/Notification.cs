#if WSA_PLUGIN

using System.Collections.Generic;
using System;

namespace UTNotifications.WSA
{
    public sealed class Notification
    {
        public Notification(long triggerUnixTime, int intervalSeconds, string provider, string title, string text, int id, IDictionary<string, string> userData, string notificationProfile, int badgeNumber)
        {
            this.triggerUnixTime = triggerUnixTime;
            this.intervalSeconds = intervalSeconds;
            this.provider = provider;
            this.title = title;
            this.text = text;
            this.id = id;
            this.userData = userData;
            this.notificationProfile = notificationProfile;
            this.badgeNumber = badgeNumber;
        }

        public long TriggerUnixTime
        {
            get
            {
                return triggerUnixTime;
            }
        }

        public int IntervalSeconds
        {
            get
            {
                return intervalSeconds;
            }
        }

        public string Provider
        {
            get
            {
                return provider;
            }
        }

        public string Title
        {
            get
            {
                return title;
            }
        }

        public string Text
        {
            get
            {
                return text;
            }
        }

        public int Id
        {
            get
            {
                return id;
            }
        }

        public IDictionary<string, string> UserData
        {
            get
            {
                return userData;
            }
        }

        public string NotificationProfile
        {
            get
            {
                return notificationProfile;
            }
        }

        public int BadgeNumber
        {
            get
            {
                return badgeNumber;
            }
        }

        private readonly long triggerUnixTime;
        private readonly int intervalSeconds;
        private readonly string provider;
        private readonly string title;
        private readonly string text;
        private readonly int id;
        private readonly IDictionary<string, string> userData;
        private readonly string notificationProfile;
        private readonly int badgeNumber;
    }
}

#endif