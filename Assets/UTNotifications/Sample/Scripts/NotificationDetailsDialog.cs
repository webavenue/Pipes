using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace UTNotifications
{
    /// <summary>
    /// Sample dialogs that displays details of a clicked/received notification.
    /// </summary>
	public class NotificationDetailsDialog : MonoBehaviour
    {
		public Text DialogTitle;
		public Text ID;
		public Text Title;
		public Text Text;
		public Text Profile;
		public Text UserData;
		public Text Badge;

		public ReceivedNotification Current
		{
			get
			{
				if (this.clicked != null)
				{
					return this.clicked;
				}
				else if (this.received.Count > 0)
				{
					return this.received[0];
				}
				else
				{
					return null;
				}
			}
		}

		public void OnReceived(ReceivedNotification received)
		{
			this.received.Add(received);
			if (this.clicked == null && this.received.Count == 1)
			{
				UpdateContents();
			}
		}

		public void OnClicked(ReceivedNotification clicked)
		{
			this.clicked = clicked;
			UpdateContents();
		}

		public void Hide()
		{
			ReceivedNotification current = Current;
			Debug.Assert(current != null);
			UTNotificationsSample.Instance.Hide(current.id);
		}

		public void Hide(int id)
		{
			if (this.clicked != null && this.clicked.id == id)
			{
				this.clicked = null;
			}
			this.received.RemoveAll(it => it.id == id);
			
			UpdateContents();
		}

		public void Cancel()
		{
			ReceivedNotification current = Current;
			Debug.Assert(current != null);
			UTNotificationsSample.Instance.Cancel(current.id);
		}

		public void CancelAll()
		{
			this.clicked = null;
			this.received.Clear();
			UpdateContents();
		}

		private void Start()
        {
            Debug.Assert(this.DialogTitle != null, "DialogTitle is not set in " + this.gameObject.name);
            Debug.Assert(this.Title != null, "Title is not set in " + this.gameObject.name);
            Debug.Assert(this.Text != null, "Text is not set in " + this.gameObject.name);
            Debug.Assert(this.ID != null, "ID is not set in " + this.gameObject.name);
            Debug.Assert(this.Profile != null, "NotificationProfile is not set in " + this.gameObject.name);
            Debug.Assert(this.Badge != null, "Badge is not set in " + this.gameObject.name);
            Debug.Assert(this.UserData != null, "UserData is not set in " + this.gameObject.name);
        }

		private void UpdateContents()
		{
			ReceivedNotification current = Current;
			if (current == null)
			{
				this.gameObject.SetActive(false);
			}
			else
			{
                string type;
                if (current.notification is PushNotification)
                {
                    type = "Push Notification";
                }
                else if (current.notification is ScheduledRepeatingNotification)
                {
                    type = "Scheduled Repeating Notification";
                }
                else if (current.notification is ScheduledNotification)
                {
                    type = "Scheduled Notification";
                }
                else if (current.notification is LocalNotification)
                {
                    type = "Immediate Local Notification";
                }
                else
                {
                    type = current.notification.GetType().ToString();
                }

				this.DialogTitle.text = type + (current == this.clicked ? " Clicked" : " Received");
				this.ID.text = current.id.ToString();
				this.Title.text = current.title;
				this.Text.text = current.text;
				this.Profile.text = current.notificationProfile ?? "";
				this.UserData.text = UserDataString(current.userData);
				this.Badge.text = current.badgeNumber != Notification.BADGE_NOT_SPECIFIED ? current.badgeNumber.ToString() : "";

				this.gameObject.SetActive(true);
			}
		}

		private string UserDataString(IDictionary<string, string> userData)
		{
			if (userData == null)
			{
				return "{}";
			}
			else
			{
				return JsonUtils.ToJson(userData).ToString();
			}
		}
		
		private readonly List<ReceivedNotification> received = new List<ReceivedNotification>();
		private ReceivedNotification clicked;
	}
}