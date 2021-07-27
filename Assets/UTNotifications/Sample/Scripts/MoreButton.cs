using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UTNotifications
{
    /// <summary>
    /// More button (creates a popup menu for the specified MenuItems).
    /// </summary>
    [RequireComponent(typeof(UnityEngine.UI.Button))]
    public class MoreButton : MonoBehaviour
    {
        /// <summary>
        /// Describes a single item of a popup menu: label + action.
        /// </summary>
        public struct PopupMenuItem
        {
            public PopupMenuItem(string label, UnityAction action)
            {
                this.label = label;
                this.action = action;
            }
            
            public readonly string label;
            public readonly UnityAction action;
        }
        
        public GameObject PopupPrefab;
        public PopupMenuItem[] MenuItems;

        public static MoreButton FindInstance()
        {
            return GameObject.FindObjectOfType<MoreButton>();
        }

        private void Start()
        {
            Debug.Assert(this.PopupPrefab != null, "Please specify PopupPrefab!");

            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            if (this.popup != null)
            {
                GameObject.Destroy(popup);
                this.popup = null;
            }
            else
            {
                this.popup = GameObject.Instantiate(PopupPrefab, this.GetComponentInParent<Canvas>().transform, false);
                Popup popupScript = this.popup.GetComponent<Popup>();
                Debug.Assert(popupScript != null, PopupPrefab.name + " doesn't have UTSamplePopup script!");

                foreach (var it in this.MenuItems)
                {
                    popupScript.AddItem(it.label, it.action);
                }
            }
        }

        private GameObject popup;
    }
}