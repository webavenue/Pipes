using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_2018_1_OR_NEWER
using UnityEngine.Networking;
#endif

namespace UTNotifications
{
    /// <summary>
    /// The sample demonstrating using various features of UTNotifications.
    /// </summary>
    public class UTNotificationsSample : MonoBehaviour
    {
        /// <summary>
        /// The only (singleton) instance of UTNotificationsSample. 
        /// </summary>
        public static UTNotificationsSample Instance
        {
            get
            {
                return instance;
            }
        }

        public ValidatedInputField DemoServerURLInputField;
        public Text NotifyAllText;
        public Text InitializeText;
        public Toggle NotificationsEnabledToggle;
        public CreateNotificationDialog CreateNotificationDialog;
        public NotificationDetailsDialog NotificationDetailsDialog;

        /// <summary>
        /// Initializes UTNotifications.Manager. You must always initialize it before accessing any UTNotifications' methods.
        /// <seealso cref="UTNotifications.Manager.Initialize"/> <seealso cref="UTNotifications.Manager.SetBadge"/>
        /// </summary>
        public void Initialize()
        {
            // Returns true if initialization is successfull.
            bool initialized = Manager.Instance.Initialize(true, 0, false);
            if (initialized)
            {
                Debug.Log("UTNotifications: Initialized successfully");
            }
            else
            {
                Debug.LogWarning("UTNotifications: Initialization failed!");
                this.InitializeText.text = this.initializeTextOriginal + "\nInitialization Error! Please check the logs.";
            }
        }

        /// <summary>
        /// Requests UTNotifications DemoServer to push a user defined notification to all its registered devices.
        /// </summary>
        public void NotifyAll()
        {
            // Sample CreateNotificationDialog is used to setup the notification title, text and so on.
            this.CreateNotificationDialog.Show("Push Notify All Registered Devices", false, false, (title, text, id, notificationProfile, badge, hasImage, hasButtons) =>
            {
                // Sends the notify message to DemoServer in a coroutine to avoid blocking the application UI due to network operations.
                StartCoroutine(NotifyAll(title, text, id, notificationProfile, badge));
            });
        }

        /// <summary>
        /// Sends a "notify" request to DemoServer to push a specified notification to all its registered devices.
        /// See also Assets/UTNotifications/Editor/DemoServer/src/main/java/com/universal_tools/demoserver/PushNotificator.java.
        /// </summary>
        /// <param name="title">Notification title</param>
        /// <param name="text">Notification text</param>
        /// <param name="id">The notification ID (any notifications with the same id will be replaced by this one). <seealso cref="UTNotifications.Manager.CancelNotification"/> <seealso cref="UTNotifications.Manager.HideNotification"/></param>
        /// <param name="notificationProfile">(Optional) The name of the notification profile (sound, icon and other notification attributes).</param>
        /// <param name="badgeNumber">(Optional) The badge number, to be displayed on top of application icon, when supported. Use Notification.BADGE_NOT_SPECIFIED (default) to keep previous value, 0 to hide the badge.</param>
        /// <returns>IEnumerator to be run as a coroutine.</returns>
        public IEnumerator NotifyAll(string title, string text, int id, string notificationProfile, int badgeNumber)
        {
            title = EscapeURL(title);
            text = EscapeURL(text);

            string noCache = "&_NO_CACHE=" + UnityEngine.Random.value; // A simple trick to avoid caching, so the request is sent to the server even with the same arguments as before.

            bool finished = false;
            StartCoroutine(HttpRequest(string.Format("{0}/notify?title={1}&text={2}&id={3}&badge={4}{5}{6}", this.DemoServerURLInputField.text, title, text, id, badgeNumber, (string.IsNullOrEmpty(notificationProfile) ? "" : "&notification_profile=" + notificationProfile), noCache),
                null,
                (successText) =>
                {
                    finished = true;
                    this.NotifyAllText.text = this.notifyAllTextOriginal + "\n" + successText;
                },
                (errorText, error) =>
                {
                    finished = true;

                    this.NotifyAllText.text = this.notifyAllTextOriginal + "\nError: " + error + " " + errorText;
                    if (!CheckAndroidCleartextAllowed(new System.Uri(this.DemoServerURLInputField.text).Host))
                    {
                        this.NotifyAllText.text += "\n" + CleartextHint;
                    }
                }));
            
            int dots = 0;
            do
            {
                string sendingText = this.notifyAllTextOriginal + "\nSending";
                dots = (dots + 1) % 4;
                for (int i = 0; i < dots; ++i)
                {
                    sendingText += '.';
                }
                
                this.NotifyAllText.text = sendingText;
                yield return new WaitForSeconds(0.15f);
            }
            while (!finished);
        }

        /// <summary>
        /// Immediately posts a user defined notification.
        /// <seealso cref="UTNotifications.Manager.PostLocalNotification"/>
        /// </summary>
        /// <remarks>
        /// Please note, that with the default settings, you can't see any notifications while the app is running in foreground.
        /// It can be changed for Android in UTNotifications settings.
        /// </remarks>
        public void CreateLocalNotification()
        {
            // Sample CreateNotificationDialog is used to setup the notification title, text and so on.
            this.CreateNotificationDialog.Show("Create Local Notification", true, true, (title, text, id, notificationProfile, badge, hasImage, hasButtons) =>
            {
                Manager.Instance.PostLocalNotification(title, text, id, UserData(hasImage), notificationProfile, badge, Buttons(hasButtons));
            });
        }

        /// <summary>
        /// Schedules the system to post a user defined notification in 30 seconds once.
        /// <seealso cref="UTNotifications.Manager.ScheduleNotification" />
        /// </summary>
        /// <remarks>
        /// Please note, that with the default settings, you can't see any notifications while the app is running in foreground.
        /// It can be changed for Android in UTNotifications settings.
        /// </remarks>
        public void ScheduleLocalNotification()
        {
            // Sample CreateNotificationDialog is used to setup the notification title, text and so on.
            this.CreateNotificationDialog.Show("Schedule Local Notification", true, true, (title, text, id, notificationProfile, badge, hasImage, hasButtons) =>
            {
                // ScheduleNotification can accept System.DateTime or an integer number of seconds to post the notification in.
                Manager.Instance.ScheduleNotification(30, title, text, id, UserData(hasImage), notificationProfile, badge, Buttons(hasButtons));
            });
        }

        /// <summary>
        /// Schedules the system to post a user defined notification in 10 seconds and then show it every 25 seconds until it's cancelled.
        /// <seealso cref="UTNotifications.Manager.ScheduleNotificationRepeating"/>
        /// </summary>
        /// <remarks>
        /// Please note, that with the default settings, you can't see any notifications while the app is running in foreground.
        /// It can be changed for Android in UTNotifications settings.
        /// </remarks>
        public void ScheduleRepeatingLocalNotification()
        {
            // Sample CreateNotificationDialog is used to setup the notification title, text and so on.
            this.CreateNotificationDialog.Show("Schedule Local Notification", true, true, (title, text, id, notificationProfile, badge, hasImage, hasButtons) =>
            {
                // ScheduleNotificationRepeating can accept System.DateTime or an integer number of seconds to post the notification in (the first time).
                Manager.Instance.ScheduleNotificationRepeating(System.DateTime.Now.AddSeconds(10), 25, title, text, id, UserData(hasImage), notificationProfile, badge, Buttons(hasButtons));
            });
        }

        /// <summary>
        /// Hides the notification with the specified ID (ignored if the specified notification is not found).
        /// <seealso cref="UTNotifications.Manager.HideNotification"/>
        /// </summary>
        /// <remarks>
        /// If the specified notification is scheduled or repeating, all the future shows will remain scheduled. Not supported and will be ignored on Windows Store/Universal Windows Platform.
        /// </remarks>
        /// <param name="id">ID of the notification to hide.</param>
        public void Hide(int id)
        {
            Manager.Instance.HideNotification(id);
            NotificationDetailsDialog.Hide(id);
        }

        /// <summary>
        /// Cancels the notification with the specified ID (ignored if the specified notification is not found).
        /// <seealso cref="UTNotifications.Manager.CancelNotification"/>
        /// </summary>
        /// <remarks> 
        /// If the specified notification is scheduled or repeating all the future shows will be also canceled.
        /// </remarks>
        /// <param name="id">ID of the notification to cancel.</param>
        public void Cancel(int id)
        {
            Manager.Instance.CancelNotification(id);
            NotificationDetailsDialog.Hide(id);
        }

        /// <summary>
        /// Cancels all the notifiations of the app and resets the app icon badge.
        /// <seealso cref="UTNotifications.Manager.CancelAllNotifications"/> <seealso cref="UTNotifications.Manager.SetBadge"/>
        /// </summary>
        public void CancelAll()
        {
            Manager.Instance.CancelAllNotifications();
            Manager.Instance.SetBadge(0);
            NotificationDetailsDialog.CancelAll();
        }

        /// <summary>
        /// Increments the app icon badge number, if supported by the target platform.
        /// <seealso cref="UTNotifications.Manager.SetBadge"/> <seealso cref="UTNotifications.Manager.GetBadge"/>
        /// </summary>
        public void IncrementBadge()
        {
            Manager.Instance.SetBadge(Manager.Instance.GetBadge() + 1);
        }

        /// <summary>
        /// Enables or disables showing notifications.
        /// <seealso cref="UTNotifications.Manager.SetNotificationsEnabled"/> <seealso cref="UTNotifications.Manager.NotificationsEnabled"/>
        /// </summary>
        /// <param name="value">Whether notifications are allowed to be shown.</param>
        public void OnNotificationsEnabledToggleValueChanged(bool value)
        {
            if (value != Manager.Instance.NotificationsEnabled())
            {
                Manager.Instance.SetNotificationsEnabled(value);
            }
        }

        /// <summary>
        /// Generates a sample user data dictionary, which optionally might define an image to be shown in an Android notification.
        /// </summary>
        protected Dictionary<string, string> UserData(bool hasImage)
        {
            Dictionary<string, string> userData = new Dictionary<string, string>();
            userData.Add("user", "data");

            if (hasImage)
            {
                // (Android only) shows a notification with a random cat image from the web
                userData.Add("image_url", "https://thecatapi.com/api/images/get?format=src&type=png&size=med");
            }

            return userData;
        }

        /// <summary>
        /// Optionally generates Android notification buttons.
        /// </summary>
        protected List<Button> Buttons(bool hasButtons)
        {
            if (!hasButtons)
            {
                return null;
            }

            List<Button> buttons = new List<Button>
            {
                // (Android only) Just a simple button with some custom user data assigned
                new Button("Open App", new Dictionary<string, string> { { "button", "first" } }),
                
                // (Android only) "open_url" in userData opens an URL on a notification click instead of the application. Can be used for the whole notification or a specific button, like here.
                new Button("Open URL", new Dictionary<string, string> { { "open_url", "https://assetstore.unity.com/packages/tools/utnotifications-professional-local-push-notification-plugin-37767" }, { "button", "second" } })
            };

            return buttons;
        }

        /// <summary>
        /// Handles the successful initialization of the manager.
        /// <seealso cref="UTNotifications.Manager.OnSendRegistrationId"/>
        /// </summary>
        protected void OnInitialized()
        {
            // Clears the application icon badge.
            Manager.Instance.SetBadge(0);
        }

        /// <summary>
        /// A wrapper for coroutine SendRegistrationId(string userId, string providerName, string registrationId).
        /// <seealso cref="UTNotifications.Manager.OnSendRegistrationId"/>
        /// </summary>
        protected void SendRegistrationId(string providerName, string registrationId)
        {
            // (Android & FCM Only) Subscribe the device for a topic, so any FCM notifications sent to that topic will be received by the device
            Manager.Instance.SubscribeToTopic("all");

            string userId = SampleUtils.GenerateDeviceUniqueIdentifier();
            StartCoroutine(SendRegistrationId(userId, providerName, registrationId));
        }

        /// <summary>
        /// Reports an error with registering for push notifications.
        /// <seealso cref="UTNotifications.Manager.OnPushRegistrationFailed"/>
        /// </summary>
        /// <param name="error">Error message.</param>
        protected void OnPushRegistrationFailed(string error)
        {
            Debug.LogError(error);
            this.InitializeText.text = this.initializeTextOriginal + "\nPush Registration Failed: " + error;
        }

        /// <summary>
        /// Sends the unique device push notifications registrationId to DemoServer in a coroutine.
        /// See also Assets/UTNotifications/Editor/DemoServer/src/main/java/com/universal_tools/demoserver/Registrator.java.
        /// </summary>
        protected IEnumerator SendRegistrationId(string userId, string providerName, string registrationId)
        {
            WWWForm wwwForm = new WWWForm();
            
            wwwForm.AddField("uid", userId);
            wwwForm.AddField("provider", providerName);
            wwwForm.AddField("id", registrationId);

            bool finished = false;
            StartCoroutine(HttpRequest(this.DemoServerURLInputField.text + "/register",
                wwwForm,
                (successText) =>
                {
                    finished = true;
                    this.InitializeText.text = this.initializeTextOriginal + "\n" + successText;
                },
                (errorText, error) =>
                {
                    finished = true;

                    this.InitializeText.text = this.initializeTextOriginal + "\nError Sending Push Registration Id to DemoServer:\n" + error + " " + errorText;
                    if (!CheckAndroidCleartextAllowed(new System.Uri(this.DemoServerURLInputField.text).Host))
                    {
                        this.InitializeText.text += "\n" + CleartextHint;
                    }
                }));

            int dots = 0;
            do
            {
                string text = this.initializeTextOriginal + "\nSending registrationId";
                dots = (dots + 1) % 4;
                for (int i = 0; i < dots; ++i)
                {
                    text += '.';
                }
                
                this.InitializeText.text = text;
                yield return new WaitForSeconds(0.15f);
            }
            while (!finished);
        }

        private static IEnumerator HttpRequest(string uri, WWWForm wwwForm, UnityEngine.Events.UnityAction<string> onSuccess, UnityEngine.Events.UnityAction<string, string> onError)
        {
#if UNITY_2018_1_OR_NEWER
            using (UnityWebRequest webRequest = wwwForm != null ? UnityWebRequest.Post(uri, wwwForm) : UnityWebRequest.Get(uri))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    onError(webRequest.downloadHandler.text, webRequest.error);
                }
                else
                {
                    onSuccess(webRequest.downloadHandler.text);
                }
            }
#else
            WWW www = wwwForm != null ? new WWW(uri, wwwForm) : new WWW(uri);
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                onError(www.text, www.error);
            }
            else
            {
                onSuccess(www.text);
            }
#endif
        }

        private string EscapeURL(string stringToUrlEscape)
        {
#if UNITY_2018_1_OR_NEWER
            return UnityWebRequest.EscapeURL(stringToUrlEscape);
#else
            return WWW.EscapeURL(stringToUrlEscape);
#endif
        }

        private static bool CheckAndroidCleartextAllowed(string hostname)
        {
#if UNITY_ANDROID
            try
            {
                using (AndroidJavaClass manager = new AndroidJavaClass("universal.tools.notifications.Manager"))
                {
                    if (!manager.CallStatic<bool>("isCleartextPermitted", hostname))
                    {
                        Debug.LogWarning("Starting with Android 9 (API level 28), cleartext support is disabled by default.");
                        Application.OpenURL("https://stackoverflow.com/a/50834600");
                        return false;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
#endif

            return true;
        }

        /// <summary>
        /// Handles notification clicks (both cases: when the app was running or shut down).
        /// <seealso cref="UTNotifications.Manager.OnNotificationClicked"/>
        /// </summary>
        /// <param name="notification">ReceivedNotification that was clicked.</param>
        protected void OnNotificationClicked(ReceivedNotification notification)
        {
            NotificationDetailsDialog.OnClicked(notification);
        }


        /// <summary>
        /// Handles one or more received/shown notifications (both cases: when the app was running or shut down).
        /// <seealso cref="UTNotifications.Manager.OnNotificationsReceived"/>
        /// </summary>
        /// <param name="receivedNotifications">List of ReceivedNotification that were received.</param>
        protected void OnNotificationsReceived(IList<ReceivedNotification> receivedNotifications)
        {
            foreach (var it in receivedNotifications)
            {
                NotificationDetailsDialog.OnReceived(it);
            }
        }

        /// <summary>
        /// Enforces only a single instance of UTNotificationsSample to exist at a time.
        /// </summary>
        private void Awake()
        {
            if (instance != null)
            {
                throw new UnityException("Creating the second instance of UTNotificationsSample...");
            }

            instance = this;
        }

        /// <summary>
        /// Subscribes to UTNotifications.Manager events and then initializes it.
        /// <seealso cref="UTNotifications.Manager.OnInitialized"/>
        /// <seealso cref="UTNotifications.Manager.OnSendRegistrationId"/>
        /// <seealso cref="UTNotifications.Manager.OnPushRegistrationFailed"/>
        /// <seealso cref="UTNotifications.Manager.OnNotificationClicked"/>
        /// <seealso cref="UTNotifications.Manager.OnNotificationsReceived"/>
        /// <seealso cref="UTNotifications.Manager.Initialize"/>
        /// </summary>
        private void Start()
        {
            Debug.Assert(this.DemoServerURLInputField != null, "DemoServerURLInputField is not set in " + this.gameObject.name);
            Debug.Assert(this.NotifyAllText != null, "NotifyAllText is not set in " + this.gameObject.name);
            Debug.Assert(this.InitializeText != null, "InitializeText is not set in " + this.gameObject.name);
            Debug.Assert(this.NotificationsEnabledToggle != null, "NotificationsEnabledToggle is not set in " + this.gameObject.name);
            Debug.Assert(this.CreateNotificationDialog != null, "CreateNotificationDialog is not set in " + this.gameObject.name);
            Debug.Assert(this.NotificationDetailsDialog != null, "NotificationDetailsDialog is not set in " + this.gameObject.name);

            var moreButton = MoreButton.FindInstance();
            if (moreButton != null)
            {
                moreButton.MenuItems = new MoreButton.PopupMenuItem[] {new MoreButton.PopupMenuItem("EXIT", () => Application.Quit())};
            }

            this.notifyAllTextOriginal = this.NotifyAllText.text;
            this.initializeTextOriginal = this.InitializeText.text;
            this.NotificationsEnabledToggle.onValueChanged.AddListener(OnNotificationsEnabledToggleValueChanged);
            
            Manager notificationsManager = Manager.Instance; // Get/create the only instance of UTNotifications.Manager

            // It's important to subscribe to these events BEFORE initializing UTNotifications.Manager
            notificationsManager.OnInitialized += OnInitialized;
            notificationsManager.OnSendRegistrationId += SendRegistrationId;
            notificationsManager.OnPushRegistrationFailed += OnPushRegistrationFailed;
            notificationsManager.OnNotificationClicked += OnNotificationClicked;
            notificationsManager.OnNotificationsReceived += OnNotificationsReceived;    //Let's handle incoming notifications (local and push)

            if (DemoServerURLInputField.IsValid())
            {
                Initialize();
            }
        }

        /// <summary>
        /// Updates the NotificationsEnabledToggle and closes the app when escape/Android back key is pressed.
        /// <seealso cref="UTNotifications.Manager.NotificationsEnabled"/>
        /// </summary>
        private void Update()
        {
            if (Manager.Instance.Initialized)
            {
                this.NotificationsEnabledToggle.isOn = Manager.Instance.NotificationsEnabled();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        /// <summary>
        /// Unsubscribes from all UTNotifications.Manager events when the sample gets destroyed.
        /// </summary>
        private void OnDestroy()
        {
            Manager notificationsManager = Manager.Instance;
            if (notificationsManager != null)
            {
                notificationsManager.OnInitialized -= OnInitialized;
                notificationsManager.OnSendRegistrationId -= SendRegistrationId;
                notificationsManager.OnPushRegistrationFailed -= OnPushRegistrationFailed;
                notificationsManager.OnNotificationClicked -= OnNotificationClicked;
                notificationsManager.OnNotificationsReceived -= OnNotificationsReceived;
            }

            if (instance == this)
            {
                instance = null;
            }
        }

        private static UTNotificationsSample instance = null;

        private static readonly string CleartextHint = "Please enable cleartext HTTP traffic in Assets/Plugins/AndroidManifest.xml.";

        private string notifyAllTextOriginal;
        private string initializeTextOriginal;
    }
}