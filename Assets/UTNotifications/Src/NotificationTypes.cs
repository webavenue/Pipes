using System;
using System.Collections.Generic;
using System.Globalization;

namespace UTNotifications
{
    /// <summary>
    /// Push notifications provider type.
    /// </summary>
    public enum PushNotificationsProvider
    {
        APNS,
        FCM,
        ADM,
        WNS
    }

    /// <summary>
    /// Abstract base class for any notification type:
    /// immediate, scheduled, scheduled repeating or push.
    /// </summary>
    public abstract class Notification
    {
        /// <summary>
        /// Use instead of a badge number to keep the previous badge number.
        /// </summary>
        public const int BADGE_NOT_SPECIFIED = -1;

        /// <summary>
        /// The title.
        /// </summary>
        public readonly string                      title;

        /// <summary>
        /// The text.
        /// </summary>
        public readonly string                      text;

        /// <summary>
        /// The id.
        /// </summary>
        public readonly int                         id;

        /// <summary>
        /// The user data provided by you in <c>Manager.PostLocalNotification</c>, <c>Manager.ScheduleNotification</c> or <c>Manager.ScheduleNotificationRepeating</c>
        /// or by your server in a push notification payload.
        /// </summary>
        /// <remarks>
        /// When the ReceivedNotification is an argument of Manager.OnNotificationClicked event handler, stores the user data of the clicked notification button
        /// (if specified). When the notification itself is clicked (even when there are custom buttons), stores the user data of the notification itself.
        /// </remarks>
        public IDictionary<string, string> userData { get; private set; }

        /// <summary>
        /// The name of the notification profile (sound, icon and other notification attributes).
        /// </summary>
        public string notificationProfile { get; private set; }

        /// <summary>
        /// The badge number (default value is BADGE_NOT_SPECIFIED).
        /// </summary>
        public int badgeNumber { get; private set; }

        /// <summary>
        /// Notification buttons (when supported by the platform).
        /// </summary>
        public ICollection<Button> buttons { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:UTNotifications.Notification"/> class.
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="text">Text.</param>
        /// <param name="id">Id.</param>
        public Notification(string title, string text, int id)
        {
            this.title = title;
            this.text = text;
            this.id = id;
            this.badgeNumber = BADGE_NOT_SPECIFIED;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:UTNotifications.Notification"/> class.
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="text">Text.</param>
        /// <param name="id">Id.</param>
        /// <param name="userData">User data (optional).</param>
        /// <param name="notificationProfile">Notification profile (optional).</param>
        /// <param name="badgeNumber">Badge number.</param>
        /// <param name="buttons">Buttons (optional).</param>
        public Notification(string title, string text, int id, IDictionary<string, string> userData, string notificationProfile, int badgeNumber, ICollection<Button> buttons)
            : this(title, text, id)
        {
            this.userData = userData;
            this.notificationProfile = notificationProfile;
            this.badgeNumber = badgeNumber;
            this.buttons = buttons;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:UTNotifications.Notification"/> class.
        /// </summary>
        /// <param name="json">Notification json.</param>
        public Notification(JSONNode json)
        {
            this.title = json["title"].Value;
            this.text = json["text"].Value;
            this.id = json["id"].AsInt;
            this.userData = JsonUtils.ToUserData(json["userData"]);
            this.notificationProfile = string.IsNullOrEmpty(json["notificationProfile"].Value) ? null : json["notificationProfile"].Value;
            this.badgeNumber = string.IsNullOrEmpty(json["badgeNumber"].Value) ? BADGE_NOT_SPECIFIED : json["badgeNumber"].AsInt;
            this.buttons = JsonUtils.ToButtons(json["buttons"]);
        }

        /// <summary>
        /// Converts to a notification json.
        /// </summary>
        /// <returns>The json.</returns>
        public virtual JSONClass ToJson()
        {
            JSONClass json = new JSONClass();

            json.Add("title", new JSONData(this.title));
            json.Add("text", new JSONData(this.text));
            json.Add("id", new JSONData(this.id));
            if (this.userData != null && this.userData.Count > 0)
            {
                json.Add("userData", JsonUtils.ToJson(this.userData));
            }
            if (!string.IsNullOrEmpty(this.notificationProfile))
            {
                json.Add("notificationProfile", new JSONData(this.notificationProfile));
            }
            if (this.badgeNumber != BADGE_NOT_SPECIFIED)
            {
                json.Add("badgeNumber", new JSONData(this.badgeNumber));
            }
            if (this.buttons != null && this.buttons.Count > 0)
            {
                json.Add("buttons", JsonUtils.ToJson(this.buttons));
            }

            return json;
        }

        /// <summary>
        /// Factory method to create a <see cref="T:UTNotifications.Notification"/>
        /// object from a notification json.
        /// </summary>
        /// <returns>Notification.</returns>
        /// <param name="json">Notification json.</param>
        public static Notification FromJson(JSONNode json)
        {
            if (!(json["provider"] is JSONLazyCreator))
            {
                return new PushNotification(json);
            }

            if (!(json["intervalSeconds"] is JSONLazyCreator))
            {
                return new ScheduledRepeatingNotification(json);
            }

            if (!(json["triggerDateTime"] is JSONLazyCreator) || !(json["triggerAtSystemTimeMillis"] is JSONLazyCreator))
            {
                return new ScheduledNotification(json);
            }

            return new LocalNotification(json);
        }

        /// <summary>
        /// Sets the user data.
        /// </summary>
        /// <returns>this.</returns>
        /// <param name="userData">User data.</param>
        public Notification SetUserData(IDictionary<string, string> userData)
        {
            this.userData = userData;
            return this;
        }

        /// <summary>
        /// Sets the notification profile.
        /// </summary>
        /// <returns>this.</returns>
        /// <param name="notificationProfile">Notification profile.</param>
        public Notification SetNotificationProfile(string notificationProfile)
        {
            this.notificationProfile = notificationProfile;
            return this;
        }

        /// <summary>
        /// Sets the badge number.
        /// </summary>
        /// <returns>this.</returns>
        /// <param name="badgeNumber">Badge number.</param>
        public Notification SetBadgeNumber(int badgeNumber)
        {
            this.badgeNumber = badgeNumber;
            return this;
        }

        /// <summary>
        /// Sets the buttons.
        /// </summary>
        /// <returns>this.</returns>
        /// <param name="buttons">Buttons.</param>
        public Notification SetButtons(ICollection<Button> buttons)
        {
            this.buttons = buttons;
            return this;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:UTNotifications.Notification"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:UTNotifications.Notification"/>.</returns>
        public override string ToString()
        {
            return ToJson().ToString();
        }
    }

    /// <summary>
    /// Local notification.
    /// </summary>
    public class LocalNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:UTNotifications.LocalNotification"/> class.
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="text">Text.</param>
        /// <param name="id">Id.</param>
        /// <param name="userData">User data (optional).</param>
        /// <param name="notificationProfile">Notification profile (optional).</param>
        /// <param name="badgeNumber">Badge number.</param>
        /// <param name="buttons">Buttons (optional).</param>
        public LocalNotification(string title, string text, int id, IDictionary<string, string> userData, string notificationProfile, int badgeNumber, ICollection<Button> buttons)
            : base(title, text, id, userData, notificationProfile, badgeNumber, buttons)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:UTNotifications.LocalNotification"/> class.
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="text">Text.</param>
        /// <param name="id">Id.</param>
        public LocalNotification(string title, string text, int id)
            : base(title, text, id)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:UTNotifications.LocalNotification"/> class.
        /// </summary>
        /// <param name="json">Notification json.</param>
        public LocalNotification(JSONNode json)
            : base(json)
        {
        }

        /// <summary>
        /// Sets the user data.
        /// </summary>
        /// <returns>this.</returns>
        /// <param name="userData">User data.</param>
        public new LocalNotification SetUserData(IDictionary<string, string> userData)
        {
            base.SetUserData(userData);
            return this;
        }

        /// <summary>
        /// Sets the notification profile.
        /// </summary>
        /// <returns>this.</returns>
        /// <param name="notificationProfile">Notification profile.</param>
        public new LocalNotification SetNotificationProfile(string notificationProfile)
        {
            base.SetNotificationProfile(notificationProfile);
            return this;
        }

        /// <summary>
        /// Sets the badge number.
        /// </summary>
        /// <returns>this.</returns>
        /// <param name="badgeNumber">Badge number.</param>
        public new LocalNotification SetBadgeNumber(int badgeNumber)
        {
            base.SetBadgeNumber(badgeNumber);
            return this;
        }

        /// <summary>
        /// this.
        /// </summary>
        /// <returns>The buttons.</returns>
        /// <param name="buttons">Buttons.</param>
        public new LocalNotification SetButtons(ICollection<Button> buttons)
        {
            base.SetButtons(buttons);
            return this;
        }
    }

    /// <summary>
    /// Push notification.
    /// </summary>
    public class PushNotification : Notification
    {
        /// <summary>
        /// The push notifications provider that delivered the notification:
        /// APNS / FCM / ADM / WNS.
        /// </summary>
        public readonly PushNotificationsProvider provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:UTNotifications.PushNotification"/> class.
        /// </summary>
        /// <param name="provider">Push notifications provider.</param>
        /// <param name="title">Title.</param>
        /// <param name="text">Text.</param>
        /// <param name="id">Id.</param>
        /// <param name="userData">User data (optional).</param>
        /// <param name="notificationProfile">Notification profile (optional).</param>
        /// <param name="badgeNumber">Badge number.</param>
        /// <param name="buttons">Buttons (optional).</param>
        public PushNotification(PushNotificationsProvider provider, string title, string text, int id, IDictionary<string, string> userData, string notificationProfile, int badgeNumber, ICollection<Button> buttons)
            : base(title, text, id, userData, notificationProfile, badgeNumber, buttons)
        {
            this.provider = provider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:UTNotifications.PushNotification"/> class.
        /// </summary>
        /// <param name="provider">Push notifications provider.</param>
        /// <param name="title">Title.</param>
        /// <param name="text">Text.</param>
        /// <param name="id">Id.</param>
        public PushNotification(PushNotificationsProvider provider, string title, string text, int id)
            : base(title, text, id)
        {
            this.provider = provider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:UTNotifications.PushNotification"/> class.
        /// </summary>
        /// <param name="json">Notification json.</param>
        public PushNotification(JSONNode json)
            : base(json)
        {
            this.provider = (PushNotificationsProvider)Enum.Parse(typeof(PushNotificationsProvider), json["provider"].Value);
        }

        /// <summary>
        /// Converts to a notification json.
        /// </summary>
        /// <returns>The json.</returns>
        public override JSONClass ToJson()
        {
            JSONClass json = base.ToJson();

            json.Add("provider", new JSONData(this.provider.ToString()));

            return json;
        }

        /// <summary>
        /// Sets the user data.
        /// </summary>
        /// <returns>this.</returns>
        /// <param name="userData">User data.</param>
        public new PushNotification SetUserData(IDictionary<string, string> userData)
        {
            base.SetUserData(userData);
            return this;
        }

        /// <summary>
        /// Sets the notification profile.
        /// </summary>
        /// <returns>this.</returns>
        /// <param name="notificationProfile">Notification profile.</param>
        public new PushNotification SetNotificationProfile(string notificationProfile)
        {
            base.SetNotificationProfile(notificationProfile);
            return this;
        }

        /// <summary>
        /// Sets the badge number.
        /// </summary>
        /// <returns>this.</returns>
        /// <param name="badgeNumber">Badge number.</param>
        public new PushNotification SetBadgeNumber(int badgeNumber)
        {
            base.SetBadgeNumber(badgeNumber);
            return this;
        }

        /// <summary>
        /// Sets the buttons.
        /// </summary>
        /// <returns>this.</returns>
        /// <param name="buttons">Buttons.</param>
        public new PushNotification SetButtons(ICollection<Button> buttons)
        {
            base.SetButtons(buttons);
            return this;
        }
    }

    /// <summary>
    /// Scheduled notification.
    /// </summary>
    public class ScheduledNotification : LocalNotification
    {
        /// <summary>
        /// Date time (in device local time zone) the notification is scheduled for / was shown.
        /// </summary>
        public readonly DateTime triggerDateTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:UTNotifications.ScheduledNotification"/> class.
        /// </summary>
        /// <param name="triggerDateTime">Date time (in device local time zone) the notification is scheduled for / was shown.</param>
        /// <param name="title">Title.</param>
        /// <param name="text">Text.</param>
        /// <param name="id">Id.</param>
        /// <param name="userData">User data (optional).</param>
        /// <param name="notificationProfile">Notification profile (optional).</param>
        /// <param name="badgeNumber">Badge number.</param>
        /// <param name="buttons">Buttons (optional).</param>
        public ScheduledNotification(DateTime triggerDateTime, string title, string text, int id, IDictionary<string, string> userData, string notificationProfile, int badgeNumber, ICollection<Button> buttons)
            : base(title, text, id, userData, notificationProfile, badgeNumber, buttons)
        {
            this.triggerDateTime = triggerDateTime;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:UTNotifications.ScheduledNotification"/> class.
        /// </summary>
        /// <param name="triggerDateTime">Date time (in device local time zone) the notification is scheduled for / was shown.</param>
        /// <param name="title">Title.</param>
        /// <param name="text">Text.</param>
        /// <param name="id">Id.</param>
        public ScheduledNotification(DateTime triggerDateTime, string title, string text, int id)
            : base(title, text, id)
        {
            this.triggerDateTime = triggerDateTime;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:UTNotifications.ScheduledNotification"/> class.
        /// </summary>
        /// <param name="json">Notification json.</param>
        public ScheduledNotification(JSONNode json)
            : base(json)
        {
            this.triggerDateTime = !(json["triggerDateTime"] is JSONLazyCreator) ? ParseDateTime(json["triggerDateTime"].Value) : TimeUtils.UnixTimestampMillisToDateTime(double.Parse(json["triggerAtSystemTimeMillis"].Value));
        }

        /// <summary>
        /// Converts to a notification json.
        /// </summary>
        /// <returns>The json.</returns>
        public override JSONClass ToJson()
        {
            JSONClass json = base.ToJson();

            json.Add("triggerDateTime", new JSONData(this.triggerDateTime.ToString(CultureInfo.InvariantCulture)));

            return json;
        }

        /// <summary>
        /// Sets the user data.
        /// </summary>
        /// <returns>this.</returns>
        /// <param name="userData">User data.</param>
        public new ScheduledNotification SetUserData(IDictionary<string, string> userData)
        {
            base.SetUserData(userData);
            return this;
        }

        /// <summary>
        /// Sets the notification profile.
        /// </summary>
        /// <returns>this.</returns>
        /// <param name="notificationProfile">Notification profile.</param>
        public new ScheduledNotification SetNotificationProfile(string notificationProfile)
        {
            base.SetNotificationProfile(notificationProfile);
            return this;
        }

        /// <summary>
        /// Sets the badge number.
        /// </summary>
        /// <returns>this.</returns>
        /// <param name="badgeNumber">Badge number.</param>
        public new ScheduledNotification SetBadgeNumber(int badgeNumber)
        {
            base.SetBadgeNumber(badgeNumber);
            return this;
        }

        /// <summary>
        /// Sets the buttons.
        /// </summary>
        /// <returns>this.</returns>
        /// <param name="buttons">Buttons.</param>
        public new ScheduledNotification SetButtons(ICollection<Button> buttons)
        {
            base.SetButtons(buttons);
            return this;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:UTNotifications.ScheduledNotification"/> is repeating.
        /// </summary>
        /// <value><c>true</c> if is repeating; otherwise, <c>false</c>.</value>
        public virtual bool IsRepeating
        {
            get
            {
                return false;
            }
        }

        private static DateTime ParseDateTime(string value)
        {
            try
            {
                return DateTime.Parse(value, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                // Handle locale-specific format (to handle previosly saved notifications correctly)
                return DateTime.Parse(value);
            }
        }
    }

    /// <summary>
    /// Scheduled repeating notification.
    /// </summary>
    public class ScheduledRepeatingNotification : ScheduledNotification
    {
        /// <summary>
        /// The interval repeating interval in seconds.
        /// </summary>
        /// <remarks>
        /// Please note that the actual interval may be different.
        /// On iOS there are only fixed options like every minute, every day,
        /// every week and so on. So the provided <c>intervalSeconds</c> value
        /// will be approximated by one of the available options.
        /// </remarks>
        public readonly int intervalSeconds;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:UTNotifications.ScheduledRepeatingNotification"/> class.
        /// </summary>
        /// <param name="triggerDateTime">Date time (in device local time zone) the notification is scheduled for / was shown the first time.</param>
        /// <param name="intervalSeconds">Interval seconds.</param>
        /// <param name="title">Title.</param>
        /// <param name="text">Text.</param>
        /// <param name="id">Id.</param>
        /// <param name="userData">User data (optional).</param>
        /// <param name="notificationProfile">Notification profile (optional).</param>
        /// <param name="badgeNumber">Badge number.</param>
        /// <param name="buttons">Buttons (optional).</param>
        public ScheduledRepeatingNotification(DateTime triggerDateTime, int intervalSeconds, string title, string text, int id, IDictionary<string, string> userData, string notificationProfile, int badgeNumber, ICollection<Button> buttons)
            : base(triggerDateTime, title, text, id, userData, notificationProfile, badgeNumber, buttons)
        {
            this.intervalSeconds = intervalSeconds;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:UTNotifications.ScheduledRepeatingNotification"/> class.
        /// </summary>
        /// <param name="triggerDateTime">Date time (in device local time zone) the notification is scheduled for / was shown the first time.</param>
        /// <param name="intervalSeconds">Interval seconds.</param>
        /// <param name="title">Title.</param>
        /// <param name="text">Text.</param>
        /// <param name="id">Id.</param>
        public ScheduledRepeatingNotification(DateTime triggerDateTime, int intervalSeconds, string title, string text, int id)
            : base(triggerDateTime, title, text, id)
        {
            this.intervalSeconds = intervalSeconds;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:UTNotifications.ScheduledRepeatingNotification"/> class.
        /// </summary>
        /// <param name="json">Notification json.</param>
        public ScheduledRepeatingNotification(JSONNode json)
            : base(json)
        {
            this.intervalSeconds = json["intervalSeconds"].AsInt;
        }

        /// <summary>
        /// Converts to a notification json.
        /// </summary>
        /// <returns>The json.</returns>
        public override JSONClass ToJson()
        {
            JSONClass json = base.ToJson();

            json.Add("intervalSeconds", new JSONData(this.intervalSeconds.ToString()));

            return json;
        }

        /// <summary>
        /// Sets the user data.
        /// </summary>
        /// <returns>this.</returns>
        /// <param name="userData">User data.</param>
        public new ScheduledRepeatingNotification SetUserData(IDictionary<string, string> userData)
        {
            base.SetUserData(userData);
            return this;
        }

        /// <summary>
        /// Sets the notification profile.
        /// </summary>
        /// <returns>this.</returns>
        /// <param name="notificationProfile">Notification profile.</param>
        public new ScheduledRepeatingNotification SetNotificationProfile(string notificationProfile)
        {
            base.SetNotificationProfile(notificationProfile);
            return this;
        }

        /// <summary>
        /// Sets the badge number.
        /// </summary>
        /// <returns>this.</returns>
        /// <param name="badgeNumber">Badge number.</param>
        public new ScheduledRepeatingNotification SetBadgeNumber(int badgeNumber)
        {
            base.SetBadgeNumber(badgeNumber);
            return this;
        }

        /// <summary>
        /// Sets the buttons.
        /// </summary>
        /// <returns>this.</returns>
        /// <param name="buttons">Buttons.</param>
        public new ScheduledRepeatingNotification SetButtons(ICollection<Button> buttons)
        {
            base.SetButtons(buttons);
            return this;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:UTNotifications.ScheduledRepeatingNotification"/> is repeating (i.e. intervalSeconds > 0).
        /// </summary>
        /// <value><c>true</c> if is repeating; otherwise, <c>false</c>.</value>
        public override bool IsRepeating
        {
            get
            {
                return this.intervalSeconds > 0;
            }
        }
    }
}