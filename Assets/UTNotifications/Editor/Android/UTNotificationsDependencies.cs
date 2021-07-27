#if UNITY_EDITOR && UNITY_ANDROID && !UNITY_CLOUD_BUILD

using System.IO;
using UnityEditor;

namespace UTNotifications
{
    [InitializeOnLoad]
    public class UTNotificationsDependencies
    {
        static UTNotificationsDependencies()
        {
            EditorApplication.update += Update;
        }

        public static void RegisterDependencies()
        {
            RegisterAndroidDependencies();
        }

        public static void RegisterAndroidDependencies()
        {
            if (Settings.ExportMode)
            {
                return;
            }

            // Apply user settings.
            File.WriteAllText(Settings.AndroidDependenciesXmlFilePath, GenerateDependencies(Settings.Instance.AndroidLegacySupportLibVersion, Settings.Instance.GooglePlayServicesLibVersion, Settings.Instance.ShortcutBadgerVersion));

            // Make sure regardless of user settings, at least minimal required versions are requested.
            File.WriteAllText(Settings.AndroidMinDependenciesXmlFilePath, GenerateDependencies(Settings.Instance.AndroidLegacySupportLibVersionMin, Settings.Instance.GooglePlayServicesLibVersionMin, Settings.Instance.ShortcutBadgerVersionMin));

            AssetDatabase.Refresh();
        }

        public static void ResolveDependencies()
        {
            if (Settings.ExportMode)
            {
                return;
            }

            RegisterAndroidDependencies();

            Google.VersionHandler.UpdateCompleteMethods = new[]
            {
                typeof(UTNotificationsDependencies).Assembly.FullName + ":UTNotifications.UTNotificationsDependencies:FinishResolveDependencies"
            };
            Google.VersionHandler.UpdateNow();
        }

        public static void FinishResolveDependencies()
        {
            Google.VersionHandler.UpdateCompleteMethods = null;
            Google.VersionHandler.InvokeStaticMethod(Google.VersionHandler.FindClass("Google.JarResolver", "GooglePlayServices.PlayServicesResolver"), "Resolve", null, null);
        }

        private static string GenerateDependencies(string legacySupportLibraryVersion, string playServicesVersion, string shortcutBadgerVersion)
        {
            return string.Format(
                dependenciesXmlTemplate,
                legacySupportLibraryVersion,
                Settings.Instance.PushNotificationsEnabledFirebase
                    ? string.Format(fcmDependenciesTemplate, playServicesVersion)
                    : "",
                shortcutBadgerVersion);
        }

        private static void Update()
        {
            RegisterDependencies();
            EditorApplication.update -= Update;
        }

        private static readonly string dependenciesXmlTemplate =
@"<dependencies>
  <androidPackages>
    <androidPackage spec=""androidx.legacy:legacy-support-v4:{0}"">
      <androidSdkPackageIds>
        <androidSdkPackageId>extra-android-m2repository</androidSdkPackageId>
      </androidSdkPackageIds>
    </androidPackage>
    {1}
    <androidPackage spec=""me.leolin:ShortcutBadger:{2}"" />
  </androidPackages>
</dependencies>";
        private static readonly string fcmDependenciesTemplate =
@"<androidPackage spec=""com.google.firebase:firebase-messaging:{0}"">
      <androidSdkPackageIds>
        <androidSdkPackageId>extra-google-m2repository</androidSdkPackageId>
      </androidSdkPackageIds>
    </androidPackage>";
    }
}

#endif