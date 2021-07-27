using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UTNotifications
{
    /// <summary>
    /// Sample class for a Toggle that remembers its last state before the app was closed last time.
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class RememberValueToggle : MonoBehaviour
    {
        private void Awake()
        {
            this.toggle = GetComponent<Toggle>();
            this.uniqueName = SampleUtils.UniqueName(this.transform);

            toggle.isOn = PlayerPrefs.GetInt(this.uniqueName, toggle.isOn ? 1 : 0) == 0 ? false : true;

            toggle.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(bool value)
        {
            PlayerPrefs.SetInt(this.uniqueName, value ? 1 : 0);
        }

        private Toggle toggle;
        private string uniqueName;
    }
}