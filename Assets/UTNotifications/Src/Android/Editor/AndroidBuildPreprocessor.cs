#if UNITY_ANDROID

using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif

namespace UTNotifications
{
    class AndroidBuildPreprocessor
#if UNITY_2018_1_OR_NEWER
        : IPreprocessBuildWithReport, IPostprocessBuildWithReport
#else
        : IPreprocessBuild, IPostprocessBuild
#endif
    {
        public int callbackOrder
        {
            get
            {
                return 0;
            }
        }

#if UNITY_2018_1_OR_NEWER
        public void OnPreprocessBuild(BuildReport report)
        {
            BuildTarget target = report.summary.platform;
#else
        public void OnPreprocessBuild(BuildTarget target, string path)
        {
#endif
            this.resFileName = null;
            if (target == BuildTarget.Android)
            {
                if (Settings.Instance.PushNotificationsEnabledFirebase)
                {
                    GenerateFCMValues();
                }

                ValidateNativeLibraries();
            }
        }

#if UNITY_2018_1_OR_NEWER
        public void OnPostprocessBuild(BuildReport report)
        {
            BuildTarget target = report.summary.platform;
#else
        public void OnPostprocessBuild(BuildTarget target, string path)
        {
#endif
            if (target == BuildTarget.Android && !string.IsNullOrEmpty(this.resFileName))
            {
                File.Delete(resFileName);
                AssetDatabase.Refresh();
            }
        }

        private void GenerateFCMValues()
        {
            string googleServicesFileName = Settings.GoogleServicesJsonFilePath;

            if (!File.Exists(googleServicesFileName))
            {
                Debug.LogError("UTNotifications: Firebase Cloud Messaging is enabled, but \"" + googleServicesFileName + "\" doesn't exist. Please make sure to complete the FCM configuration.");
                return;
            }

            try
            {
                JSONNode json = JSON.Parse(File.ReadAllText(googleServicesFileName));
                JSONNode client = FindActiveClient(json);
                if (client == null)
                {
                    Debug.LogError("UTNotifications: \"" + googleServicesFileName + "\" doesn't contain the configuration for " + Settings.AndroidBundleId);
                    return;
                }

                string googleAppId = client["client_info"]["mobilesdk_app_id"].Value;
                string gcmDefaultSenderId = json["project_info"]["project_number"].Value;
                string defaultWebClientId = FindDefaultWebClient(client);
                string firebaseDatabaseUrl = json["project_info"]["firebase_url"].Value;
                string googleApiKey = client["api_key"].AsArray[0]["current_key"].Value;
                string googleCrashReportingApiKey = googleApiKey;
                string projectId = json["project_info"]["project_id"].Value;

                if (string.IsNullOrEmpty(googleAppId)
                    || string.IsNullOrEmpty(gcmDefaultSenderId)
                    || string.IsNullOrEmpty(defaultWebClientId)
                    || string.IsNullOrEmpty(firebaseDatabaseUrl)
                    || string.IsNullOrEmpty(googleApiKey)
                    || string.IsNullOrEmpty(googleCrashReportingApiKey)
                    || string.IsNullOrEmpty(projectId))
                {
                    Debug.LogError("UTNotifications: \"" + googleServicesFileName + "\" doesn't contain some of the required values for " + Settings.AndroidBundleId);
                    return;
                }

                this.resFileName = Path.Combine(Settings.GetAndroidResourceFolder("values", true), "fcm_values.xml");
                File.WriteAllText(this.resFileName, string.Format(resFileTemplate,
                    googleAppId,
                    gcmDefaultSenderId,
                    defaultWebClientId,
                    firebaseDatabaseUrl,
                    googleApiKey,
                    googleCrashReportingApiKey,
                    projectId));
                AssetDatabase.Refresh();
            }
            catch (System.Exception e)
            {
                Debug.LogError("UTNotifications: failed to parse \"" + googleServicesFileName + "\": " + e);
            }
        }

        private static JSONNode FindActiveClient(JSONNode json)
        {
            string bundleId = Settings.AndroidBundleId;
            foreach (JSONNode client in json["client"].AsArray)
            {
                if (client["client_info"]["android_client_info"]["package_name"].Value == bundleId)
                {
                    return client;
                }
            }

            return null;
        }

        private static string FindDefaultWebClient(JSONNode client)
        {
            foreach (JSONNode oauthClient in client["oauth_client"].AsArray)
            {
                if (oauthClient["client_type"].AsInt == 3)
                {
                    return oauthClient["client_id"].Value;
                }
            }

            return null;
        }

        private static bool RequiredLibrariesMissing()
        {
            string mainTemplatePath = Path.Combine(Settings.PluginsFullPath, "Android/mainTemplate.gradle");
			string mainTemplate = File.Exists(mainTemplatePath) ? File.ReadAllText(mainTemplatePath) : "";

			if (!mainTemplate.Contains("com.android.support:support-v4:") &&
                !mainTemplate.Contains("androidx.legacy:legacy-support-v4:") &&
                !PluginExists("com.android.support.support-v4-*.aar") &&
                !PluginExists("androidx.legacy.legacy-support-v4-*.aar"))
			{
				return true;
			}

            if (Settings.Instance.PushNotificationsEnabledFirebase &&
                !mainTemplate.Contains("com.google.firebase:firebase-messaging:") &&
                !PluginExists("com.google.firebase.firebase-messaging-*.aar"))
            {
                return true;
            }

            return false;
        }

        private static bool PluginExists(string wildcard)
        {
            try
            {
                string androidPluginsFolder = Path.Combine(Settings.PluginsFullPath, "Android");
                string[] files = Directory.GetFiles(androidPluginsFolder, wildcard);
                return files != null && files.Length > 0;
            }
            catch
            {
                return false;
            }
        }

        private static void ValidateNativeLibraries()
        {
            if (RequiredLibrariesMissing())
            {
                string errorMessage = "Required Android libraries are missing.\nPlease make sure Play Services Resolver is successfully invoked: Edit -> Assets -> Play Services Resolver -> Android Resolver -> Force Resolve. It's also recommended to turn auto-resolution on.\n\nUTNotificions can not function properly.";
#if !UNITY_CLOUD_BUILD
                if (!EditorUserBuildSettings.enableHeadlessMode && SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Null)
                {
                    EditorUtility.DisplayDialog("Error: Android Native Libraries Missing", errorMessage, "OK");
                }
#endif

                throw new BuildFailedException("UTNotifications: " + errorMessage);
            }
        }

        private string resFileName;

        private static readonly string resFileTemplate =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<resources>
    <string name=""google_app_id"" translatable=""false"">{0}</string>
    <string name=""gcm_defaultSenderId"" translatable=""false"">{1}</string>
    <string name=""default_web_client_id"" translatable=""false"">{2}</string>
    <string name=""firebase_database_url"" translatable=""false"">{3}</string>
    <string name=""google_api_key"" translatable=""false"">{4}</string>
    <string name=""google_crash_reporting_api_key"" translatable=""false"">{5}</string>
    <string name=""project_id"" translatable=""false"">{6}</string>
</resources>";
    }
}

#endif  // UNITY_ANDROID