using UnityEngine;
using UnityEngine.UI;

namespace UTNotifications
{
    /// <summary>
    /// Sample dialog for setting up a new notification.
    /// </summary>
    public class CreateNotificationDialog : MonoBehaviour
    {
        public Text DialogTitle;
        public Text Title;
        public Text Text;
        public Text ID;
        public Text NotificationProfile;
        public Text Badge;
        public Toggle HasImage;
        public Toggle HasButtons;

        public delegate void OnComplete(string title, string text, int id, string notificationProfile, int badge, bool hasImage, bool hasButtons);
        
        public void Show(string dialogTitle, bool showHasImage, bool showHasButtons, OnComplete onComplete)
        {
            if (this.gameObject.activeSelf)
            {
                throw new System.InvalidOperationException();
            }

            if (onComplete == null)
            {
                throw new System.ArgumentNullException("onComplete");
            }

#if UNITY_ANDROID
            HasImage.gameObject.SetActive(showHasImage);
            HasButtons.gameObject.SetActive(showHasButtons);
#else
            HasImage.gameObject.SetActive(false);
            HasButtons.gameObject.SetActive(false);
#endif

            this.DialogTitle.text = dialogTitle;
            this.onComplete = onComplete;

            this.gameObject.SetActive(true);
        }

        public void OK()
        {
            this.onComplete(Title.text,
                            Text.text,
                            string.IsNullOrEmpty(ID.text) ? 1 : int.Parse(ID.text),
                            NotificationProfile.text,
                            string.IsNullOrEmpty(Badge.text) ? Notification.BADGE_NOT_SPECIFIED : int.Parse(Badge.text),
                            HasImage.isOn,
                            HasButtons.isOn);

            Cancel();
        }

        public void Cancel()
        {
            this.onComplete = null;
            this.gameObject.SetActive(false);
        }

        private void Start()
        {
            Debug.Assert(this.DialogTitle != null, "DialogTitle is not set in " + this.gameObject.name);
            Debug.Assert(this.Title != null, "Title is not set in " + this.gameObject.name);
            Debug.Assert(this.Text != null, "Text is not set in " + this.gameObject.name);
            Debug.Assert(this.ID != null, "ID is not set in " + this.gameObject.name);
            Debug.Assert(this.NotificationProfile != null, "NotificationProfile is not set in " + this.gameObject.name);
            Debug.Assert(this.Badge != null, "Badge is not set in " + this.gameObject.name);
            Debug.Assert(this.HasImage != null, "HasImage is not set in " + this.gameObject.name);
            Debug.Assert(this.HasButtons != null, "HasButtons is not set in " + this.gameObject.name);
        }

        private OnComplete onComplete;
    }
}