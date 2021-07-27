using UnityEngine;
using UnityEngine.UI;

namespace UTNotifications
{
    [RequireComponent(typeof(Text))]
    public class ScheduledNotificationsList : MonoBehaviour
    {
        public bool Repeated = false;

        private void Awake()
        {
            this.text = GetComponent<Text>();
            this.originalText = text.text;
        }

        private void Update()
        {
            Manager manager = Manager.Instance;
            if (!manager.Initialized)
            {
                return;
            }

            var list = manager.ScheduledNotifications;
            if (list == null || list.Count == 0)
            {
                this.text.text = this.originalText;
            }
            else
            {
                string listString = "";
                bool empty = true;

                foreach (var it in list)
                {
                    if (it is ScheduledRepeatingNotification == this.Repeated)
                    {
                        if (empty)
                        {
                            empty = false;
                        }
                        else
                        {
                            listString += ", ";
                        }

                        listString += it.id.ToString();
                    }
                }

                if (empty)
                {
                    this.text.text = this.originalText;
                }
                else
                {
                    this.text.text = this.originalText + "\nIDs: [" + listString + "]";
                }
            }
        }

        private string originalText;
        private Text text;
    }
}