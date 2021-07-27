using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace UTNotifications
{
    /// <summary>
    /// Various utils used by UTNotifications Sample.
    /// </summary>
    public static class SampleUtils
    {
        static public string UniqueName(Transform transform)
        {
            if (transform == null)
            {
                return SceneManager.GetActiveScene().name;
            }
            else
            {
                return UniqueName(transform.parent) + "." + transform.name;
            }
        }

        static public string GenerateDeviceUniqueIdentifier()
        {
#if !UNITY_ANDROID || UNITY_EDITOR
            return SystemInfo.deviceUniqueIdentifier;
#else
            string deviceId = null;
            try
            {
                using (AndroidJavaClass manager = new AndroidJavaClass("universal.tools.notifications.Manager"))
                {
                    deviceId = manager.CallStatic<string>("getDeviceId");
                }
            }
            catch (AndroidJavaException e)
            {
                Debug.LogException(e);
            }

            if (string.IsNullOrEmpty(deviceId)) {
                Debug.LogWarning("Failed to get a device id. Using a default id instead.");
                deviceId = "00000000000000000000000000000000";
            }

            return GetMd5Hash(deviceId);
#endif
        }

#if UNITY_ANDROID
        static private string GetMd5Hash(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                stringBuilder.Append(data[i].ToString("x2"));
            }

            return stringBuilder.ToString();
        }
#endif
    }
}