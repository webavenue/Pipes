using UnityEngine;
using UnityEngine.UI;

namespace UTNotifications
{
    /// <summary>
    /// Sample component which requires some input fields to have valid values to able to enable a button.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.UI.Button))]
    public class ValidatedInputDependent : MonoBehaviour
    {
        public bool AllowWhenPushDisabled = false;
        public ValidatedInputField[] ValidatedInputFields;

        private void Start()
        {
            Debug.Assert(this.ValidatedInputFields != null && this.ValidatedInputFields.Length > 0, "ValidatedInputFields not set in " + this.gameObject.name);

            if (AllowWhenPushDisabled && !PushNotificationsEnabled())
            {
                this.enabled = false;
            }
            else
            {
                this.button = GetComponent<UnityEngine.UI.Button>();
                Debug.Assert(this.button != null, "Button component not found in " + this.gameObject.name);
            }
        }

        private void Update()
        {
            bool allValid = true;
            foreach (var it in ValidatedInputFields)
            {
                if (!it.IsValid())
                {
                    allValid = false;
                    break;
                }
            }
            this.button.interactable = allValid;
        }

        // Can't use UTNotifications.Manager.Instance.PushNotificationsEnabled() directly for the sample purposes as it returns false when not yet initialized
        private bool PushNotificationsEnabled()
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_ANDROID
            return Settings.Instance.PushNotificationsEnabledFirebase || Settings.Instance.PushNotificationsEnabledAmazon;
#elif UNITY_IOS
            return Settings.Instance.PushNotificationsEnabledIOS;
#elif UNITY_WSA || UNITY_METRO
            return Settings.Instance.PushNotificationsEnabledWindows;
#else
            return false;
#endif
        }

        private UnityEngine.UI.Button button;
    }
}