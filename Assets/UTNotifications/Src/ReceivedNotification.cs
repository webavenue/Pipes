using System.Collections.Generic;

namespace UTNotifications
{
    /// <summary>
    /// A received or clicked notification of any type.
    /// </summary>
    public class ReceivedNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:UTNotifications.ReceivedNotification"/> class.
        /// </summary>
        /// <param name="notification">Notification that was received / clicked.</param>
        public ReceivedNotification(Notification notification)
        {
            this.notification = notification;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:UTNotifications.ReceivedNotification"/> class.
        /// </summary>
        /// <param name="json">Received notification json.</param>
        public ReceivedNotification(JSONNode json)
        {
            this.notification = Notification.FromJson(json);
        }

        /// <summary>
        /// Returns the underlying notification object.
        /// </summary>
        /// <value>The underlying notification object.</value>
        public Notification notification
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the received notification title.
        /// </summary>
        /// <value>The received notification title.</value>
        public string title
        {
            get
            {
                return this.notification.title;
            }
        }

        /// <summary>
        /// Gets the received notification text.
        /// </summary>
        /// <value>The received notification text.</value>
        public string text
        {
            get
            {
                return this.notification.text;
            }
        }

        /// <summary>
        /// Gets the received notification id.
        /// </summary>
        /// <value>The received notification id.</value>
        public int id
        {
            get
            {
                return this.notification.id;
            }
        }

        /// <summary>
        /// Gets the received notification user data if any.
        /// </summary>
        /// <value>The received notification user data or null.</value>
        public virtual IDictionary<string, string> userData
        {
            get
            {
                return this.notification.userData;
            }
        }

        /// <summary>
        /// Gets the received notification profile if any.
        /// </summary>
        /// <value>The received notification profile or null.</value>
        public string notificationProfile
        {
            get
            {
                return this.notification.notificationProfile;
            }
        }

        /// <summary>
        /// Gets the received notification badge number.
        /// </summary>
        /// <value>The received notification badge number.</value>
        public int badgeNumber
        {
            get
            {
                return this.notification.badgeNumber;
            }
        }

        /// <summary>
        /// Gets the received notification buttons if any.
        /// </summary>
        /// <value>The received notification buttons or null.</value>
        public ICollection<Button> buttons
        {
            get
            {
                return this.notification.buttons;
            }
        }
    }

    /// <summary>
    /// A clicked notification of any type.
    /// </summary>
    public class ClickedNotification : ReceivedNotification
    {
        /// <summary>
        /// Used instead of clicked button index if the notification itself was clicked and not its button.
        /// </summary>
        public const int BUTTON_NONE = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:UTNotifications.ClickedNotification"/> class.
        /// </summary>
        /// <param name="notification">Notification that was received / clicked.</param>
        /// <param name="clickedButtonIndex">Clicked button index.</param>
        public ClickedNotification(Notification notification, int clickedButtonIndex)
            : base(notification)
        {
            this.clickedButtonIndex = clickedButtonIndex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:UTNotifications.ClickedNotification"/> class.
        /// </summary>
        /// <param name="json">Clicked notification json.</param>
        public ClickedNotification(JSONNode json)
            : base(json)
        {
            int buttonIndex;
            if (!int.TryParse(json["buttonIndex"].Value, out buttonIndex))
            {
                buttonIndex = BUTTON_NONE;
            }

            this.clickedButtonIndex = buttonIndex;
        }

        /// <summary>
        /// Gets the user data of a clicked notification or a button that was clicked.
        /// </summary>
        /// <value>The user data.</value>
        public override IDictionary<string, string> userData
        {
            get
            {
                if (this.clickedButtonIndex >= 0 && this.notification.buttons != null && this.clickedButtonIndex < this.notification.buttons.Count)
                {
                    if (this.notification.buttons is IList<Button>)
                    {
                        return ((IList<Button>)this.notification.buttons)[this.clickedButtonIndex].userData;
                    }
                    else
                    {
                        int i = 0;
                        foreach (Button it in this.notification.buttons)
                        {
                            if (i++ == this.clickedButtonIndex)
                            {
                                return it.userData;
                            }
                        }

                        // Invalid clicked button index
                        return base.userData;
                    }
                }
                else
                {
                    return base.userData;
                }
            }
        }

        /// <summary>
        /// The index of the clicked button or <c>ClickedNotification.BUTTON_NONE</c>.
        /// </summary>
        public readonly int clickedButtonIndex;
    }
}