using UnityEngine;
using UnityEngine.UI;

namespace UTNotifications
{
    /// <summary>
    /// Script managing a sample button interactable/non-interactable appearance.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.UI.Button))]
    public class ButtonHelper : MonoBehaviour
    {
        public Color DisabledColor = new Color(0.462f, 0.482f, 0.494f);
        
        private void Start()
        {
            this.button = GetComponent<UnityEngine.UI.Button>();
            Debug.Assert(this.button != null, "Button component not found in " + this.gameObject.name);
            this.text = transform.GetComponentInChildren<Text>();
            Debug.Assert(this.text != null, "Text not found in button " + this.gameObject.name);
            this.initialColor = this.text.color;
        }
        
        private void Update()
        {
            if (this.lastInteractable != this.button.interactable)
            {
                this.lastInteractable = this.button.interactable;
                this.text.color = this.lastInteractable ? this.initialColor : this.DisabledColor;
            }
        }

        private UnityEngine.UI.Button button;
        private Text text;
        private bool lastInteractable = true;
        private Color initialColor;
    }
}