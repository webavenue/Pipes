#if UNITY_EDITOR && UNITY_ANDROID

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System;

namespace UTNotifications
{
    public static class AndroidManifestManager
    {
    //public
        public static void Update()
        {
            if (Settings.ExportMode)
            {
                return;
            }
            
            string manifestFile = Settings.MainAndroidManifestFullPath;

            //If AndroidManifest.xml doesn't exist in project copy default one
            if (!File.Exists(manifestFile))
            {
                return;
            }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                try
                {
                    xmlDocument.Load(manifestFile);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return;
                }

                XmlElement manifestNode = XmlUtils.FindChildNode(xmlDocument, "manifest") as XmlElement;
                XmlNode applicationNode = XmlUtils.FindChildNode(manifestNode, "application");

                if (applicationNode == null)
                {
                    return;
                }

                string ns = applicationNode.GetNamespaceOfPrefix("android");

                bool modified = false;
                modified |= UpdateUTNotificationsCommon(xmlDocument, manifestNode, applicationNode, ns);
                modified |= UpdateFirebaseCloudMessaging(xmlDocument, manifestNode, applicationNode, ns);
                modified |= UpdateAmazonDeviceMessaging(xmlDocument, manifestNode, applicationNode, ns);

                if (modified)
                {
                    xmlDocument.Save(manifestFile);
                }
            }
            catch
            {
                // Ignore: manifest patching is just taking care of legacy entries
            }
        }

    //private
        private static bool UpdateUTNotificationsCommon(XmlDocument xmlDocument, XmlNode manifestNode, XmlNode applicationNode, string ns)
        {
            bool modified = false;
            modified |= XmlUtils.RemoveElement(xmlDocument, applicationNode, "receiver", "name", ns, "universal.tools.notifications.AlarmBroadcastReceiver", "UTNotifications common");
            modified |= XmlUtils.RemoveElement(xmlDocument, applicationNode, "service", "name", ns, "universal.tools.notifications.NotificationIntentService");
            modified |= XmlUtils.RemoveElement(xmlDocument, manifestNode, "uses-permission", "name", ns, "android.permission.VIBRATE");
            modified |= XmlUtils.RemoveElement(xmlDocument, manifestNode, "uses-permission", "name", ns, "android.permission.WAKE_LOCK");

            modified |= UpdateRestoreScheduledOnReboot(xmlDocument, manifestNode, applicationNode, ns);
            modified |= UpdateAndroid4CompatibilityMode(xmlDocument, manifestNode, applicationNode, ns);

            return modified;
        }

        // Not supported anymore
        private static bool UpdateAndroid4CompatibilityMode(XmlDocument xmlDocument, XmlNode manifestNode, XmlNode applicationNode, string ns)
        {
            string comment = "Android 4.4 Compatibility Mode";
            return XmlUtils.RemoveElement(xmlDocument, manifestNode, "uses-sdk", "targetSdkVersion", ns, "20", comment);
        }

        private static bool UpdateFirebaseCloudMessaging(XmlDocument xmlDocument, XmlNode manifestNode, XmlNode applicationNode, string ns)
        {
            string comment = "Firebase Cloud Messaging";

            bool modified = false;
            modified |= XmlUtils.RemoveElement(xmlDocument, applicationNode, "receiver", "name", ns, "universal.tools.notifications.GcmBroadcastReceiver", comment);
            modified |= XmlUtils.RemoveElement(xmlDocument, applicationNode, "receiver", "name", ns, "com.google.android.gms.gcm.GcmReceiver", comment);
            modified |= XmlUtils.RemoveElement(xmlDocument, applicationNode, "service", "name", ns, "universal.tools.notifications.GcmIntentService");
            modified |= XmlUtils.RemoveElement(xmlDocument, applicationNode, "service", "name", ns, "universal.tools.notifications.GcmInstanceIDListenerService");
            modified |= XmlUtils.RemoveElement(xmlDocument, manifestNode, "permission", "name", ns, Settings.AndroidBundleId + ".permission.C2D_MESSAGE", comment);
            modified |= XmlUtils.RemoveElement(xmlDocument, manifestNode, "uses-permission", "name", ns, Settings.AndroidBundleId + ".permission.C2D_MESSAGE");
            modified |= XmlUtils.RemoveElement(xmlDocument, manifestNode, "uses-permission", "name", ns, "com.google.android.c2dm.permission.RECEIVE");

            return modified;
        }

        private static bool UpdateAmazonDeviceMessaging(XmlDocument xmlDocument, XmlElement manifestNode, XmlNode applicationNode, string ns)
        {
            string amazonNs = "http://schemas.amazon.com/apk/res/android";
            string comment = "Amazon Device Messaging";

            bool modified = false;
            modified |= XmlUtils.RemoveElement(xmlDocument, applicationNode, "amazon:enable-feature", "name", ns, "com.amazon.device.messaging", comment, amazonNs);
            modified |= XmlUtils.RemoveElement(xmlDocument, applicationNode, "receiver", "name", ns, "universal.tools.notifications.AdmBroadcastReceiver");
            modified |= XmlUtils.RemoveElement(xmlDocument, applicationNode, "service", "name", ns, "universal.tools.notifications.AdmIntentService");
            modified |= XmlUtils.RemoveElement(xmlDocument, manifestNode, "permission", "name", ns, Settings.AndroidBundleId + ".permission.RECEIVE_ADM_MESSAGE", comment);
            modified |= XmlUtils.RemoveElement(xmlDocument, manifestNode, "uses-permission", "name", ns, Settings.AndroidBundleId + ".permission.RECEIVE_ADM_MESSAGE");
            modified |= XmlUtils.RemoveElement(xmlDocument, manifestNode, "uses-permission", "name", ns, "com.amazon.device.messaging.permission.RECEIVE");

            return modified;
        }

        private static bool UpdateRestoreScheduledOnReboot(XmlDocument xmlDocument, XmlNode manifestNode, XmlNode applicationNode, string ns)
        {
            string comment = "Restore Scheduled Notifications On Reboot";

            bool modified = false;
            modified |= XmlUtils.RemoveElement(xmlDocument, applicationNode, "receiver", "name", ns, "universal.tools.notifications.ScheduledNotificationsRestorer", comment);
            modified |= XmlUtils.RemoveElement(xmlDocument, manifestNode, "uses-permission", "name", ns, "android.permission.RECEIVE_BOOT_COMPLETED", comment);

            return modified;
        }
    }
}

#endif //UNITY_EDITOR