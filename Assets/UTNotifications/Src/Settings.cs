#if !UNITY_WEBPLAYER && !UNITY_SAMSUNGTV

#if UNITY_2018_3_OR_NEWER
#define USE_SETTINGS_PROVIDER
#endif

using UnityEngine;
using System.IO;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using System.Text.RegularExpressions;
#endif

namespace UTNotifications
{
    /// <summary>
    /// UTNotifications settings. Edit in Unity Editor: <c>"Edit -> Project Settings -> UTNotifications"</c>
    /// </summary>
    public class Settings : ScriptableObject
    {
    //public
        public static Settings Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = Resources.Load(m_assetName) as Settings;

#if UNITY_EDITOR && UNITY_2018_1_OR_NEWER
                    if (m_instance == null && AssetDatabase.GetMainAssetTypeAtPath(AssetPath) != null)
                    {
                        AssetDatabase.ForceReserializeAssets(new string[] { AssetPath });
                        AssetDatabase.Refresh();
                        m_instance = Resources.Load(m_assetName) as Settings;
                    }
#endif

                    if (m_instance == null)
                    {
                        m_instance = CreateInstance<Settings>();
#if UNITY_EDITOR
                        EditorApplication.update += Update;
#endif
                    }
#if UNITY_EDITOR
                    else
                    {
                        m_instance.CheckAssetVersionUpdated();
                    }
#endif
                }

                return m_instance;
            }
        }

#if UNITY_EDITOR
        public static bool ExportMode
        {
            get
            {
                string[] args = System.Environment.GetCommandLineArgs();
                foreach (string arg in args)
                {
                    if (arg == "-gvh_disable")
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public static string FullPath(string assetsRelatedPath)
        {
            string fullPath = Path.Combine(Application.dataPath, assetsRelatedPath);
            if (Path.DirectorySeparatorChar != '/')
            {
                fullPath = fullPath.Replace('/', Path.DirectorySeparatorChar);
            }

            return fullPath;
        }

        public static string MainAndroidManifestFullPath
        {
            get
            {
                return FullPath("Plugins/Android/AndroidManifest.xml");
            }
        }

        public static string WSAWinmdPluginPath
        {
            get
            {
                return FullPath("Plugins/WSA/UTNotifications.winmd");
            }
        }

        public static string AssetsRelativeRootPath
        {
            get
            {
                return "UTNotifications";
            }
        }

        public static string PluginsFullPath
        {
            get
            {
                return FullPath("Plugins");
            }
        }

        public static string AssetsRelativeResourcesPath
        {
            get
            {
                return Path.Combine(AssetsRelativeRootPath, "Resources");
            }
        }

        public static string AssetsRelatedEditorPath
        {
            get
            {
                return Path.Combine(AssetsRelativeRootPath, "Editor");
            }
        }

        public static string AssetsRelatedDemoServerPath
        {
            get
            {
                return Path.Combine(AssetsRelatedEditorPath, "DemoServer");
            }
        }

        public static string GoogleServicesJsonFilePath
        {
            get
            {
                return Path.Combine(PluginsFullPath, "Android/google-services.json");
            }
        }

        public static string AndroidDependenciesXmlFilePath
        {
            get
            {
                return FullPath("UTNotifications/Editor/UTNotificationsAndroidDependencies.xml");
            }
        }

        public static string AndroidMinDependenciesXmlFilePath
        {
            get
            {
                return FullPath("UTNotifications/Editor/UTNotificationsAndroidMinDependencies.xml");
            }
        }

        public static string AssetPath
        {
            get
            {
                return Path.Combine(Path.Combine("Assets", AssetsRelativeResourcesPath), m_assetName + ".asset");
            }
        }
#endif

        public const string Version = "1.8.4";

        public List<NotificationProfile> NotificationProfiles
        {
            get
            {
                if (m_notificationProfiles.Count == 0 || m_notificationProfiles[0].profileName != DEFAULT_PROFILE_NAME)
                {
                    m_notificationProfiles.Insert(0, new NotificationProfile() { profileName = DEFAULT_PROFILE_NAME });
                }

                return m_notificationProfiles;
            }
        }

        public string PushPayloadTitleFieldName
        {
            get
            {
                return m_pushPayloadTitleFieldName;
            }
            set
            {
                if (m_pushPayloadTitleFieldName != value)
                {
                    m_pushPayloadTitleFieldName = value;
#if UNITY_EDITOR
                    Save();
#endif
                }
            }
        }

        public string PushPayloadTextFieldName
        {
            get
            {
                return m_pushPayloadTextFieldName;
            }
            set
            {
                if (m_pushPayloadTextFieldName != value)
                {
                    m_pushPayloadTextFieldName = value;
#if UNITY_EDITOR
                    Save();
#endif
                }
            }
        }

        public string PushPayloadIdFieldName
        {
            get
            {
                return m_pushPayloadIdFieldName;
            }
            set
            {
                if (m_pushPayloadIdFieldName != value)
                {
                    m_pushPayloadIdFieldName = value;
#if UNITY_EDITOR
                    Save();
#endif
                }
            }
        }

        public string PushPayloadUserDataParentFieldName
        {
            get
            {
                return m_pushPayloadUserDataParentFieldName;
            }
            set
            {
                if (m_pushPayloadUserDataParentFieldName != value)
                {
                    m_pushPayloadUserDataParentFieldName = value;
#if UNITY_EDITOR
                    Save();
#endif
                }
            }
        }

        public string PushPayloadNotificationProfileFieldName
        {
            get
            {
                return m_pushPayloadNotificationProfileFieldName;
            }
            set
            {
                if (m_pushPayloadNotificationProfileFieldName != value)
                {
                    m_pushPayloadNotificationProfileFieldName = value;
#if UNITY_EDITOR
                    Save();
#endif
                }
            }
        }

        public string PushPayloadBadgeFieldName
        {
            get
            {
                return m_pushPayloadBadgeFieldName;
            }
            set
            {
                if (m_pushPayloadBadgeFieldName != value)
                {
                    m_pushPayloadBadgeFieldName = value;
#if UNITY_EDITOR
                    Save();
#endif
                }
            }
        }

        public string PushPayloadButtonsParentName
        {
            get
            {
                return m_pushPayloadButtonsParentName;
            }
            set
            {
                if (m_pushPayloadButtonsParentName != value)
                {
                    m_pushPayloadButtonsParentName = value;
#if UNITY_EDITOR
                    Save();
#endif
                }
            }
        }

        public string GooglePlayServicesLibVersionMin
        {
            get
            {
                return m_googlePlayServicesLibVersionMin;
            }
        }

        public string GooglePlayServicesLibVersion
        {
            get
            {
                return m_googlePlayServicesLibVersion;
            }
            set
            {
                if (m_googlePlayServicesLibVersion != value)
                {
                    m_googlePlayServicesLibVersion = value;
#if UNITY_EDITOR
                    Save();
#endif
                }
            }
        }

        public string AndroidLegacySupportLibVersionMin
        {
            get
            {
                return m_androidLegacySupportLibVersionMin;
            }
        }

        public string AndroidLegacySupportLibVersion
        {
            get
            {
                return m_androidLegacySupportLibVersion;
            }
            set
            {
                if (m_androidLegacySupportLibVersion != value)
                {
                    m_androidLegacySupportLibVersion = value;
#if UNITY_EDITOR
                    Save();
#endif
                }
            }
        }

        public string ShortcutBadgerVersionMin
        {
            get
            {
                return m_shortcutBadgerVersionMin;
            }
        }

        public string ShortcutBadgerVersion
        {
            get
            {
                return m_shortcutBadgerVersion;
            }
            set
            {
                if (m_shortcutBadgerVersion != value)
                {
                    m_shortcutBadgerVersion = value;
#if UNITY_EDITOR
                    Save();
#endif
                }
            }
        }

        public bool PushNotificationsEnabledIOS
        {
            get
            {
                return m_pushNotificationsEnabledIOS;
            }
            set
            {
                if (m_pushNotificationsEnabledIOS != value)
                {
                    m_pushNotificationsEnabledIOS = value;
#if UNITY_EDITOR
                    Save();
#endif
                }
            }
        }

        public bool PushNotificationsEnabledFirebase
        {
            get
            {
                return m_pushNotificationsEnabledFirebase;
            }
            set
            {
                if (m_pushNotificationsEnabledFirebase != value)
                {
                    m_pushNotificationsEnabledFirebase = value;
#if UNITY_EDITOR
                    GetAndroidResourceLibFolder(true);
                    Save();
#endif
                }
            }
        }

        public bool PushNotificationsEnabledAmazon
        {
            get
            {
                return m_pushNotificationsEnabledAmazon;
            }
            set
            {
                if (m_pushNotificationsEnabledAmazon != value)
                {
                    m_pushNotificationsEnabledAmazon = value;
#if UNITY_EDITOR
                    Save();
#endif
                }
            }
        }

        public bool PushNotificationsEnabledWindows
        {
            get
            {
                return m_pushNotificationsEnabledWindows;
            }
            set
            {
                if (m_pushNotificationsEnabledWindows != value)
                {
                    m_pushNotificationsEnabledWindows = value;
#if UNITY_EDITOR
                    Save();
#endif
                }
            }
        }

        public ShowNotifications AndroidShowNotificationsMode
        {
            get
            {
                return m_androidShowNotificationsMode;
            }
            set
            {
                if (m_androidShowNotificationsMode != value)
                {
                    m_androidShowNotificationsMode = value;
#if UNITY_EDITOR
                    Save();
#endif
                }
            }
        }

        public bool AndroidRestoreScheduledNotificationsAfterReboot
        {
            get
            {
                return m_androidRestoreScheduledNotificationsAfterReboot;
            }
#if UNITY_EDITOR
            set
            {
                if (m_androidRestoreScheduledNotificationsAfterReboot != value)
                {
                    m_androidRestoreScheduledNotificationsAfterReboot = value;
                    Save();
                }
            }
#endif
        }

        public NotificationsGroupingMode AndroidNotificationsGrouping
        {
            get
            {
                return m_androidNotificationsGrouping;
            }

            set
            {
                if (m_androidNotificationsGrouping != value)
                {
                    m_androidNotificationsGrouping = value;
#if UNITY_EDITOR
                    Save();
#endif
                }
            }
        }

        public ScheduleTimerType AndroidScheduleTimerType
        {
            get
            {
                return m_androidScheduleTimerType;
            }

            set
            {
                if (m_androidScheduleTimerType != value)
                {
                    m_androidScheduleTimerType = value;
#if UNITY_EDITOR
                    Save();
#endif
                }
            }
        }

        public GooglePlayUpdatingIfRequiredMode AllowUpdatingGooglePlayIfRequired
        {
            get
            {
                return m_allowUpdatingGooglePlayIfRequired;
            }

            set
            {
                if (m_allowUpdatingGooglePlayIfRequired != value)
                {
                    m_allowUpdatingGooglePlayIfRequired = value;
#if UNITY_EDITOR
                    Save();
#endif
                }
            }
        }

		public bool AndroidShowLatestNotificationOnly
		{
			get
			{
				return m_androidShowLatestNotificationOnly;
			}

#if UNITY_EDITOR
			set
			{
				if (value != m_androidShowLatestNotificationOnly)
				{
					m_androidShowLatestNotificationOnly = value;
					Save();
				}
			}
#endif
		}

        public bool AndroidScheduleExact
        {
            get
            {
                return m_androidScheduleExact;
            }

#if UNITY_EDITOR
            set
            {
                if (value != m_androidScheduleExact)
                {
                    m_androidScheduleExact = value;
                    Save();
                }
            }
#endif
        }

#if UNITY_EDITOR
        private static readonly Regex md5Regex = new Regex("MD5\\s*\\:\\s*");
        private static readonly Regex sha256Regex = new Regex("SHA256\\s*\\:\\s*");
        public static string[] GetAndroidDebugSignatureMD5AndSHA256()
        {
#if UNITY_EDITOR_WIN
            string homeDir = System.Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            string javaHome = JavaHome;

            string keytool = string.IsNullOrEmpty(javaHome) ? "keytool" : Path.Combine(javaHome, @"bin\keytool.exe");
#else
            string homeDir = System.Environment.GetEnvironmentVariable("HOME");
            string keytool = "keytool";
#endif

            string debugKeystore = Path.Combine(homeDir, ".android/debug.keystore");
            if (!File.Exists(debugKeystore))
            {
                string error = "<debug.keystore file not found. Please build an Android version at least once.>";
                Debug.LogError(error);
                return new string[] { error, error };
            }

            string keytoolOutput = null;
            try
            {
                keytoolOutput = RunCommand(keytool, "-list -v -alias androiddebugkey -storepass android -keystore \"" + debugKeystore + "\"");
            }
            catch
            {
                Debug.LogError("Failed to locate keytool. Please make sure it's available in your PATH.");
                return new string[] { "", "" };
            }

            Match md5Match = md5Regex.Match(keytoolOutput);
            Match sha256Match = sha256Regex.Match(keytoolOutput);

            if (!md5Match.Success || !sha256Match.Success)
            {
                string message = "<Unable to read \"debug.keystore\" file. Look http://stackoverflow.com/questions/8576732/there-is-no-debug-keystore-in-android-folder >";
                Debug.LogError(message + "\n" + keytoolOutput);
                return new string[]{message, message};
            }
            else
            {
                int indexMD5 = md5Match.Index + md5Match.Length;
                int indexSHA256 = sha256Match.Index + sha256Match.Length;

                return new string[] {
                    keytoolOutput.Substring(indexMD5, keytoolOutput.IndexOf('\n', indexMD5) - indexMD5),
                    keytoolOutput.Substring(indexSHA256, keytoolOutput.IndexOf('\n', indexSHA256) - indexSHA256)
                };
            }
        }

        public static string GetAmazonAPIKey()
        {
            string file = Path.Combine(PluginsFullPath, "Android/assets/api_key.txt");

            if (File.Exists(file))
            {
                return File.ReadAllText(file);
            }
            else
            {
                return "";
            }
        }

        public static void SetAmazonAPIKey(string value)
        {
            if (Settings.ExportMode)
            {
                return;
            }
            
            if (string.IsNullOrEmpty(value))
            {
                string file = Path.Combine(PluginsFullPath, "Android/assets/api_key.txt");
                
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
            else
            {
                string dir = Path.Combine(PluginsFullPath, "Android/assets");
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                File.WriteAllText(dir + "/api_key.txt", value);
            }

            AssetDatabase.Refresh();
        }
#endif

        public bool WindowsDontShowWhenRunning
        {
            get
            {
                return m_windowsDontShowWhenRunning;
            }

#if UNITY_EDITOR
            set
            {
                if (m_windowsDontShowWhenRunning != value)
                {
                    m_windowsDontShowWhenRunning = value;
                    Save();
                }
            }
#endif
        }

#if UNITY_EDITOR
        public string WindowsIdentityName
        {
            get
            {
                return PlayerSettings.WSA.packageName;
            }

            set
            {
                PlayerSettings.WSA.packageName = value;
            }
        }

        public string WindowsCertificatePublisher
        {
            get
            {
                return PlayerSettings.WSA.certificateIssuer;
            }
        }

        public bool WindowsCertificateIsCorrect(string publisher)
        {
            if (string.IsNullOrEmpty(publisher))
            {
                return false;
            }

            //Correct certificate publisher format: 00E3DE9D-D280-4DAF-907B-9DC894310E32
            return System.Text.RegularExpressions.Regex.IsMatch(publisher, "^[A-F0-9]{8}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{12}$");
        }

        public const string WRONG_CERTIFICATE_MESSAGE = "Wrong Windows Store certificate! Please create right one in Unity platform settings or associate app with the store in Visual Studio in order to make push notifications work. For details please read the UTNotifications manual.";
#endif

#if UNITY_EDITOR
        public static string GetAndroidResourceLibFolder(bool prepareResource = false)
        {
            string assetsRelativePath = "Android/UTNotificationsRes";
            string path = Path.Combine(PluginsFullPath, assetsRelativePath);
            if (prepareResource)
            {
                PrepareAndroidResource(path);
            }

            // Make sure the permissions are set correctly
            if (Directory.Exists(path))
            {
                string assetPath = Path.Combine("Assets/Plugins", assetsRelativePath);
                PluginImporter pluginImporter = (PluginImporter)AssetImporter.GetAtPath(assetPath);
                if (pluginImporter != null && pluginImporter.GetCompatibleWithAnyPlatform())
                {
                    pluginImporter.SetCompatibleWithAnyPlatform(false);
                    pluginImporter.SetCompatibleWithEditor(false);
                    pluginImporter.SetCompatibleWithPlatform(BuildTarget.Android, true);
                    pluginImporter.SaveAndReimport();
                    AssetDatabase.ImportAsset(assetPath);
                }
            }

            return path;
        }

        public static string GetAndroidResourceFolder(string resType, bool prepareResource = false)
        {
            string path = Path.Combine(GetAndroidResourceLibFolder(prepareResource), "res/" + resType);
            if (prepareResource && !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        public static string GetIOSResourceFolder()
        {
            return Path.Combine(PluginsFullPath, "iOS/Raw");
        }

#if !USE_SETTINGS_PROVIDER
        [MenuItem(m_settingsMenuItem)]
#endif
        public static void EditSettings()
        {
            EditorPrefs.SetBool(m_shownEditorPrefKey, true);
            Selection.activeObject = Instance;
        }

#if USE_SETTINGS_PROVIDER
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var editor = Editor.CreateEditor(Instance);
            return new SettingsProvider("UTNotifications", SettingsScope.Project)
            {
                label = "UTNotifications",
                keywords = new HashSet<string>(new[] { "UTNotifications", "Push", "Notification" }),
                guiHandler = (searchContext) =>
                {
                    editor.OnInspectorGUI();
                }
            };
        }
#endif

        public void Save()
        {
            if (Settings.ExportMode)
            {
                return;
            }
            
#if UNITY_ANDROID
            AndroidManifestManager.Update();
#endif
            EditorUtility.SetDirty(this);
        }
#endif

        public const string DEFAULT_PROFILE_NAME = "default";
        public const string DEFAULT_PROFILE_NAME_INTERNAL = "__default_profile";

        /// <summary>
        /// Stores attributes of a notification profile.
        /// </summary>
        [System.Serializable]
        public struct NotificationProfile
        {
            public string profileName;
            public string iosSound;
            public string androidChannelName;
            public string androidChannelDescription;
            public string androidIcon;
            public string androidLargeIcon;
            public string androidIcon5Plus;
            [UnityEngine.Serialization.FormerlySerializedAs("androidIconBGColorSpecified")]
            public bool colorSpecified;
            [UnityEngine.Serialization.FormerlySerializedAs("androidIconBGColor")]
            public Color androidColor;
            public string androidSound;
            public bool androidHighPriority;
        }

        public enum ShowNotifications
        {
            WHEN_CLOSED_OR_IN_BACKGROUND = 0,
            WHEN_CLOSED = 1,
            ALWAYS = 2
        }

        public enum NotificationsGroupingMode
        {
            /// <summary>
            /// Don't group
            /// </summary>
            NONE = 0,

            /// <summary>
            /// Group by notifications profiles
            /// </summary>
            BY_NOTIFICATION_PROFILES = 1,

            /// <summary>
            /// Use "notification_group" user data value as a grouping key
            /// </summary>
            FROM_USER_DATA = 2,

            /// <summary>
            /// All the app's notifications will belong to a single group
            /// </summary>
            ALL_IN_A_SINGLE_GROUP = 3
        }

        public enum ScheduleTimerType
        {
            /// <summary>
            /// See https://developer.android.com/reference/android/app/AlarmManager#RTC_WAKEUP
            /// </summary>
            RTC_WAKEUP = 0,

            /// <summary>
            /// See https://developer.android.com/reference/android/app/AlarmManager#RTC
            /// </summary>
            RTC = 1,

            /// <summary>
            /// See https://developer.android.com/reference/android/app/AlarmManager#ELAPSED_REALTIME_WAKEUP
            /// </summary>
            ELAPSED_REALTIME_WAKEUP = 2,

            /// <summary>
            /// See https://developer.android.com/reference/android/app/AlarmManager#ELAPSED_REALTIME
            /// </summary>
            ELAPSED_REALTIME = 3
        }

        public enum GooglePlayUpdatingIfRequiredMode
        {
            /// <summary>
            /// Don't suggest updating.
            /// </summary>
            DISABLED = 0,

            /// <summary>
            /// If required, suggest updating only once.
            /// </summary>
            ONCE = 1,

            /// <summary>
            /// If required, suggest updating every time UTNotifications.Manager is initialized.
            /// </summary>
            EVERY_INITIALIZE = 2
        }

#if UNITY_EDITOR
        public static string AndroidBundleId
        {
            get
            {
                return PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
            }
        }
#endif  // UNITY_EDITOR

        //private
#if UNITY_EDITOR
        private static string RunCommand(string command, string args)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = args;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            
            string output = process.StandardOutput.ReadToEnd();
            
            process.WaitForExit();

            return output;
        }
#endif

        [SerializeField]
        private List<NotificationProfile> m_notificationProfiles = new List<NotificationProfile>();

        [SerializeField]
        private string m_pushPayloadTitleFieldName = "title";

        [SerializeField]
        private string m_pushPayloadTextFieldName = "text";

        [SerializeField]
        private string m_pushPayloadIdFieldName = "id";

        [SerializeField]
        private string m_pushPayloadUserDataParentFieldName = "";

        [SerializeField]
        private string m_pushPayloadNotificationProfileFieldName = "notification_profile";

        [SerializeField]
        private string m_pushPayloadBadgeFieldName = "badge_number";

        [SerializeField]
        private string m_pushPayloadButtonsParentName = "buttons";

        [SerializeField]
        private string m_googlePlayServicesLibVersion = m_googlePlayServicesLibVersionMin;
        private static readonly string m_googlePlayServicesLibVersionMin = "20.0.0+";

        [SerializeField]
        private string m_androidLegacySupportLibVersion = m_androidLegacySupportLibVersionMin;
        private static readonly string m_androidLegacySupportLibVersionMin = "1.0.0+";

        [SerializeField]
        private string m_shortcutBadgerVersion = m_shortcutBadgerVersionMin;
        private static readonly string m_shortcutBadgerVersionMin = "1.1.22+";

        [SerializeField]
        private ShowNotifications m_androidShowNotificationsMode = ShowNotifications.WHEN_CLOSED_OR_IN_BACKGROUND;

#pragma warning disable 414
        [SerializeField]
        private bool m_android4CompatibilityMode = false;
#pragma warning restore 414

        [SerializeField]
        private bool m_androidRestoreScheduledNotificationsAfterReboot = true;
        
        [SerializeField]
        private NotificationsGroupingMode m_androidNotificationsGrouping = NotificationsGroupingMode.BY_NOTIFICATION_PROFILES;

        [SerializeField]
        private ScheduleTimerType m_androidScheduleTimerType = ScheduleTimerType.ELAPSED_REALTIME_WAKEUP;

        [SerializeField]
        private bool m_androidShowLatestNotificationOnly = false;

        [SerializeField]
        private bool m_androidScheduleExact = false;

        [SerializeField]
        private bool m_pushNotificationsEnabledIOS = false;

        [SerializeField]
        private bool m_pushNotificationsEnabledFirebase = false;

        [SerializeField]
        private bool m_pushNotificationsEnabledAmazon = false;

        [SerializeField]
        private bool m_pushNotificationsEnabledWindows = false;

        [SerializeField]
        private GooglePlayUpdatingIfRequiredMode m_allowUpdatingGooglePlayIfRequired = GooglePlayUpdatingIfRequiredMode.ONCE;

#pragma warning disable 414
        [SerializeField]
        private string m_assetVersionSaved = "";

        [SerializeField]
        private bool m_windowsDontShowWhenRunning = true;
#pragma warning restore 414

        private const string m_assetName = "UTNotificationsSettings";
#if USE_SETTINGS_PROVIDER
        private const string m_settingsMenuItem = "Edit/Project Settings... -> UTNotifications";
#else
        private const string m_settingsMenuItem = "Edit/Project Settings/UTNotifications";
#endif
        private static Settings m_instance;

#if UNITY_EDITOR
        private static void Update()
        {
            EditorApplication.update -= Update;

            if (Settings.ExportMode)
            {
                return;
            }

            string resourcesFullPath = FullPath(AssetsRelativeResourcesPath);
            
            if (!Directory.Exists(resourcesFullPath))
            {
                Directory.CreateDirectory(resourcesFullPath);
                AssetDatabase.Refresh();
            }

            AssetDatabase.CreateAsset(m_instance, AssetPath);
            m_instance.SaveVersion();
            AssetDatabase.Refresh();
        }

        private static string m_shownEditorPrefKey
        {
            get
            {
                if (m_shownEditorPrefKeyCached == null)
                {
                    m_shownEditorPrefKeyCached = "UTNotificationsSettingsShown." + PlayerSettings.productName;
                }

                return m_shownEditorPrefKeyCached;
            }
        }

        private static string m_shownEditorPrefKeyCached = null;

        [InitializeOnLoad]
        public class SettingsHelper : ScriptableObject
        {
            static SettingsHelper()
            {
                EditorApplication.update += Update;
            }

#pragma warning disable RECS0146 // Member hides static member from outer class
            private static void Update()
#pragma warning restore RECS0146
            {
                if (!EditorPrefs.GetBool(m_shownEditorPrefKey, false) &&
                    !File.Exists(Path.Combine(FullPath(AssetsRelativeResourcesPath), m_assetName + ".asset")))
                {
                    if (EditorUtility.DisplayDialog("UTNotifications", "Please configure UTNotifications.\nYou can always edit its settings in menu:\n" + m_settingsMenuItem, "Now", "Later"))
                    {
                        EditSettings();
                    }
                }
                EditorPrefs.SetBool(m_shownEditorPrefKey, true);

                Instance.CheckOutdatedSettings();
                Instance.CheckAndroidPlugin();
                Instance.CheckWSAPlugin();

                EditorApplication.update -= Update;
            }
        }

        private static void PrepareAndroidResource(string androidResLibPath)
        {
            if (Settings.ExportMode)
            {
                return;
            }

            bool refresh = false;

            if (!Directory.Exists(androidResLibPath))
            {
                Directory.CreateDirectory(androidResLibPath);
                refresh = true;
            }

            refresh |= CopyAndroidResTemplate("Res", androidResLibPath);
            if (Settings.Instance.PushNotificationsEnabledFirebase)
            {
                refresh |= CopyAndroidResTemplate("FCMRes", androidResLibPath);
            }

            if (refresh)
            {
                AssetDatabase.Refresh();
            }
        }

        private static bool CopyAndroidResTemplate(string resTemplateFolder, string androidResLibPath)
        {
            bool refresh = false;

            string resLibTemplatePath = Path.Combine(Settings.FullPath(Settings.AssetsRelatedEditorPath), "Android/" + resTemplateFolder);
            FileInfo[] fileInfos = new DirectoryInfo(resLibTemplatePath).GetFiles();

            foreach (FileInfo fileInfo in fileInfos)
            {
                if (!fileInfo.Name.StartsWith(".", System.StringComparison.Ordinal) && !fileInfo.Name.EndsWith(".meta", System.StringComparison.Ordinal))
                {
                    string targetFileName = Path.Combine(androidResLibPath, fileInfo.Name);
                    File.Copy(fileInfo.FullName, targetFileName, true);
                    refresh = true;
                }
            }

            return refresh;
        }

        private void CheckOutdatedSettings()
        {
            CheckAndroid4Compatibility();
            CheckNotificationProfiles();
        }

        private void CheckAndroid4Compatibility()
        {
            if (m_android4CompatibilityMode)
            {
                EditorUtility.DisplayDialog("UTNotifications", "Android 4.4 Compatibility Mode option was automatically disabled as it's not supported by Android 6+.\nPlease configure the default notification profile and any other profiles to avoid pure white square notification icons on Android 5+.", "OK");
                m_android4CompatibilityMode = false;
                Save();
            }
        }

        private void CheckNotificationProfiles()
        {
            var profiles = NotificationProfiles;
            if (profiles != null)
            {
                if (!Directory.Exists(GetAndroidResourceLibFolder()))
                {
                    foreach (var it in profiles)
                    {
                        if (!string.IsNullOrEmpty(it.androidIcon) && !string.IsNullOrEmpty(it.androidIcon5Plus) && !string.IsNullOrEmpty(it.androidLargeIcon) && !string.IsNullOrEmpty(it.androidSound))
                        {
                            EditorUtility.DisplayDialog("UTNotifications", "Notification Profiles Android storage structure changed in UTNotifications 1.6.\nPlease configure all notification profiles once again!", "OK");

                            for (int i = 0; i < profiles.Count; ++i)
                            {
                                var profile = profiles[i];
                                profile.androidIcon = profile.androidIcon5Plus = profile.androidLargeIcon = profile.androidSound = string.Empty;
                                profiles[i] = profile;
                            }

                            Save();
                            return;
                        }
                    }
                }
            }
        }

        private void CheckAndroidPlugin()
        {
#if UNITY_ANDROID
            CheckAndroidManifest();
            CheckFacebookSDK();
            CheckResManifest();
            CheckProguard();
#endif
            // Make sure Android is the only enabled platform for the res folder (if any)
            GetAndroidResourceLibFolder(false);
        }

        private void RemoveEclipseADTPrefix(string path, string extenstion)
        {
            string targetFileName = path + "/." + extenstion;
            if (!File.Exists(targetFileName))
            {
                string originalFileName = path + "/eclipse_adt." + extenstion;
                if (File.Exists(originalFileName))
                {
                    File.Copy(originalFileName, targetFileName);
                }
            }
        }

#if UNITY_ANDROID
        private void CheckAndroidManifest()
        {
            AndroidManifestManager.Update();
        }

        private void CheckFacebookSDK()
        {
			bool updateAssetDatabase = false;
			
            string facebookAndroidLibsPath = Path.Combine(Application.dataPath, "FacebookSDK/Plugins/Android/libs");

            if (Directory.Exists(facebookAndroidLibsPath))
            {
                string[] facebookSupportLibraryFiles = Directory.GetFiles(facebookAndroidLibsPath, "support-v4-*");

                if (facebookSupportLibraryFiles != null)
                {
                    foreach (string it in facebookSupportLibraryFiles)
                    {
						updateAssetDatabase = true;
                        Debug.Log("UTNotifications: deleting a duplicated Android library: " + it);
                        File.Delete(it);
                    }
                }
            }

			if (updateAssetDatabase)
			{
				UnityEditor.AssetDatabase.Refresh();
			}
        }

        private void CheckResManifest()
        {
            string resManifestFile = Path.Combine(GetAndroidResourceLibFolder(), "AndroidManifest.xml");
            if (File.Exists(resManifestFile) && File.ReadAllText(resManifestFile).Contains("android:minSdkVersion"))
            {
                // Updates an outdated manifest
                string defaultResManifestFile = Path.Combine(Settings.FullPath(Settings.AssetsRelatedEditorPath), "Android/Res/AndroidManifest.xml");
                File.Copy(defaultResManifestFile, resManifestFile, true);
            }
        }

        private void CheckProguard()
        {
#if UNITY_2017_1_OR_NEWER
            string proguardFileText;
            if (EditorUserBuildSettings.androidDebugMinification != AndroidMinification.None ||
                EditorUserBuildSettings.androidReleaseMinification != AndroidMinification.None)
            {
                string proguardFileName = Path.Combine(PluginsFullPath, "Android/proguard-user.txt");
                if (!File.Exists(proguardFileName))
                {
                    Debug.Log("UTNotifications: Enabling User Proguard File");

                    string proguardFileNameDisabled = proguardFileName + ".DISABLED";
                    if (File.Exists(proguardFileNameDisabled))
                    {
                        proguardFileText = File.ReadAllText(proguardFileNameDisabled);
                        File.Move(proguardFileNameDisabled, proguardFileName);
                    }
                    else
                    {
                        proguardFileText = "";
                    }
                }
                else
                {
                    proguardFileText = File.ReadAllText(proguardFileName);
                }

                string updatedProguardFileText = AddLineIfMissing(proguardFileText, "# UTNotifications Proguard Config (please do not edit)");
                updatedProguardFileText = AddLineIfMissing(updatedProguardFileText, "-keep public class universal.tools.notifications.* { *; }");
                updatedProguardFileText = AddLineIfMissing(updatedProguardFileText, "-dontwarn com.amazon.device.messaging.*");
                updatedProguardFileText = AddLineIfMissing(updatedProguardFileText, "-dontwarn universal.tools.notifications.AdmIntentService");
                if (updatedProguardFileText != proguardFileText)
                {
                    Debug.Log("UTNotifications: Adding Proguard exclusions");
                    File.WriteAllText(proguardFileName, updatedProguardFileText);
        
                    AssetDatabase.Refresh();
                }
            }
#endif
        }

        private static string AddLineIfMissing(string text, string line)
        {
            string result;
            if (!text.Contains(line))
            {
                result = text + "\n" + line;
            }
            else
            {
                result = text;
            }

            return result;
        }
#endif

        private void CheckWSAPlugin()
        {
            if (Settings.ExportMode)
            {
                return;
            }
        
            CheckWSADllPlatforms();
        }

        private void CheckWSADllPlatforms()
        {
            string pluginPath = "Assets/Plugins/UTNotifications.dll";

            PluginImporter pluginImporter = ((PluginImporter)AssetImporter.GetAtPath(pluginPath));
            if (pluginImporter.GetCompatibleWithAnyPlatform() || pluginImporter.GetCompatibleWithPlatform(BuildTarget.WSAPlayer))
            {
                pluginImporter.SetCompatibleWithAnyPlatform(false);
                pluginImporter.SetCompatibleWithPlatform(BuildTarget.WSAPlayer, false);
                pluginImporter.SetCompatibleWithEditor(true);
                pluginImporter.SaveAndReimport();
                AssetDatabase.ImportAsset(pluginPath);
            }
        }

#if UNITY_EDITOR
        private void CheckAssetVersionUpdated()
        {
            if (m_assetVersionSaved != Version)
            {
                string updateMessage = "";
                int savedVersionIndex = -1;
                for (int i = 0; i < m_assetBundleUpdateMessages.Length; ++i)
                {
                    if (m_assetBundleUpdateMessages[i].version == m_assetVersionSaved)
                    {
                        savedVersionIndex = i;
                        break;
                    }
                }

                for (int i = savedVersionIndex + 1; i < m_assetBundleUpdateMessages.Length; ++i)
                {
                    updateMessage += m_assetBundleUpdateMessages[i].text + "\n";
                }

                if (updateMessage.Length > 0)
                {
                    const string lastLine = "\nWould you like to open UTNotifications Settings?";
                    if (EditorUtility.DisplayDialog("UTNotifications has been updated to version " + Version, updateMessage + lastLine, "Yes", "No"))
                    {
                        EditSettings();
                    }
                }

                SaveVersion();
            }
        }

        private void SaveVersion()
        {
            m_assetVersionSaved = Version;
            Save();
        }
#endif

        private bool MoveFilesStartingWith(string pathFrom, string pathTo, string startsWith)
        {
            bool moved = false;

            if (Directory.Exists(pathFrom))
            {
                if (Settings.ExportMode)
                {
                    return false;
                }
            
                string[] files = Directory.GetFiles(pathFrom, startsWith + "*");
                if (files != null)
                {
                    foreach (string file in files)
                    {
                        if (!Directory.Exists(pathTo))
                        {
                            Directory.CreateDirectory(pathTo);
                        }

                        string fullPathTo = pathTo + "/" + Path.GetFileName(file);
                        if (File.Exists(fullPathTo))
                        {
                            File.Delete(fullPathTo);
                        }
                        File.Move(file, fullPathTo);
                        moved = true;
                    }
                }
            }

            return moved;
        }
#endif

#if UNITY_EDITOR_WIN
        private static string m_javaHome;

        private static bool IsValidJavaHome(string javaHome)
        {
            return !string.IsNullOrEmpty(javaHome) && File.Exists(Path.Combine(javaHome, @"bin\keytool.exe"));
        }

        private static string JavaHome
        {
            get
            {
                if (!IsValidJavaHome(m_javaHome))
                {
                    string home = Path.Combine(EditorApplication.applicationContentsPath, @"PlaybackEngines\AndroidPlayer\Tools\OpenJDK\Windows");
                    if (!IsValidJavaHome(home))
                    {
                        home = EditorPrefs.GetString("JdkPath");
                        if (!IsValidJavaHome(home))
                        {
                            home = System.Environment.GetEnvironmentVariable("JAVA_HOME");
                        }
                    }

                    if (IsValidJavaHome(home))
                    {
                        m_javaHome = home;
                    }
                }
                
                return m_javaHome;
            }
        }
#endif

        private class UpdateMessage
        {
            public UpdateMessage(string version, string text)
            {
                this.version = version;
                this.text = text;
            }
            
            public readonly string version;
            public readonly string text;
        }

#if UNITY_EDITOR
        private readonly UpdateMessage[] m_assetBundleUpdateMessages =
        {
            new UpdateMessage("1.7.0",
@"- (Breaking change!) Android, Windows Store/UWP: No need to URL-Encode push notifications payload anymore (see DemoServer)
- (Breaking change!) Minimal supported version of Android is now 4.1
- (Breaking change!) Base64 iOS tokens encoding is not supported anymore
- Android: Android O Notification Channels are fully supported and integrated with notification profiles (please update any existing profiles)
- Android: Custom notification buttons
- Android: High Priority/Heads-Up notifications (notification profile settings)
- Android: Small notification icon background color customization (notification profile settings)
- Android: UTNotifications.Manager.Instance.NotificationsAllowed() works correctly, like earlier in iOS
- Android: An option to open a URL instead of the app on a notification tap
- Android: An option to offer to update the system Google Play if the installed version is too old for FCM
- Android: Ignoring any misformatted push messages to support AppsFlyer-like services
- Android: Google Play Services Lib and Android Support Library versions can be specified in the asset settings
- Android: Automated patching of the Proguard config to make sure UTNotifications classes are not removed when minifying the .apk
- Android: (Fix) READ_PHONE_STATE permission is not requested anymore when notification profiles are configured
- iOS: A workaround for a rare iOS bug when local notifications could be duplicated
- iOS: HTTP/2 API in DemoServer
- ShortcutBadger and Google Play Services Resolver are updated to the latest versions
- A number of other fixes and improvements"),
            new UpdateMessage("1.7.1",
@"- (Fixed) Android 5: tap on a notification doesn't open the app, if it's minimized
- (Fixed) Android: PushNotificationsEnabled() fixed"),
            new UpdateMessage("1.7.2",
@"- New Sample UI
- Android: Notification Profile color setting is now also used for notification titles in Android 8+
- Android: Importing FCM-related native libraries only if FCM push notifications are enabled
- (Fixed) Android: Cloud build
- (Fixed) Android: Unicode Characters >10000 in local notifications
- (Fixed) Android: Couldn't open the application on a notification click in some devices
- Other fixes and improvements"),
            new UpdateMessage("1.7.3",
@"- Android: Better compatibility with Android versions <= 6
- iOS: Post build script works correctly in Unity 5.6.6"),
            new UpdateMessage("1.8.0",
@"- Unity 2019.2.5b5, Android Studio 3.4, Android SDK 29-rc2, Play Services Resolver 1.2.117, Google Play Services 49, Android Support Library 47.0.0, and other latest components to release date are supported
- List of currently scheduled notifications can be retrieved
- Push notifications initialization failures can be handled
- DemoServer now supports TLS v1.2 to connect to push notifications providers servers
- Android: No more AndroidManifest.xml patching
- Android: FCM Topics support
- Android: Automated notifications grouping on Android 7.0+
- Android: Exact-time scheduled notifications support (see settings)
- Android: Timer type is now configurable (see settings)
- Large number of other fixes and improvements"),
            new UpdateMessage("1.8.1",
@"- Correct handling of disabled 'Patch mainTemplate.gradle' option of Play Services Resolver
- Android: Avoiding java.lang.ClassNotFoundException on first launch when FCM is turned off
- Play Services Resolver v1.2.122.0"),
            new UpdateMessage("1.8.2",
@"- Supporting AndroidX and Jetifier"),
            new UpdateMessage("1.8.3",
@"- AndroidX instead of Android Support Library
- Better compatibility with latest Firebase Unity SDK components
- System Locale changes between the app runs is handled correctly
- Play Services Resolver v1.2.129.0"),
            new UpdateMessage("1.8.4",
@"- Correct validation of AndroidX libraries in the Gradle template file"),
        };
#endif
	}
}

#endif
