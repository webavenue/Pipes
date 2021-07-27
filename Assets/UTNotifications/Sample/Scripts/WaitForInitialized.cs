using UnityEngine;

namespace UTNotifications
{
    /// <summary>
    /// Sample component that deactivates the parent game object until UTNotifications.Manager gets initialized.
    /// </summary>
    public class WaitForInitialized : MonoBehaviour
    {
        private void Start()
        {
            if (!Manager.Instance.Initialized)
            {
                Manager.Instance.OnInitialized += OnInitialized;
                this.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (Manager.Instance != null)
            {
                Manager.Instance.OnInitialized -= OnInitialized;
            }
        }

        private void OnInitialized()
        {
            this.gameObject.SetActive(true);
            Manager.Instance.OnInitialized -= OnInitialized;
        }
    }
}