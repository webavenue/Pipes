using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UTNotifications
{
    /// <summary>
    /// Sample popup menu dialog.
    /// </summary>
    public class Popup : MonoBehaviour
    {
        public GameObject ItemPrefab;
        
        public void AddItem(string label, UnityAction action)
        {
            Debug.Assert(this.ItemPrefab != null, "Please specify ItemPrefab!");
            
            GameObject item = GameObject.Instantiate(this.ItemPrefab, this.transform, false);
            Text text = item.GetComponentInChildren<Text>();
            Debug.Assert(text != null, ItemPrefab.name + " doesn't have a Text child!");
            text.text = label;

            UnityEngine.UI.Button button = item.GetComponentInChildren<UnityEngine.UI.Button>();
            Debug.Assert(button != null, ItemPrefab.name + " doesn't have a Button child!");
            button.onClick.AddListener(action);
        }
    }
}