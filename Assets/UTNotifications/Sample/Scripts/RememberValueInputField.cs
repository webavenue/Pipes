using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UTNotifications
{
    /// <summary>
    /// Sample class for a InputField that remembers its last state before the app was closed last time.
    /// </summary>
    [RequireComponent(typeof(InputField))]
    public class RememberValueInputField : MonoBehaviour
    {
        private void Awake()
        {
            this.inputField = GetComponent<InputField>();
            this.uniqueName = SampleUtils.UniqueName(this.transform);

            inputField.text = PlayerPrefs.GetString(this.uniqueName, inputField.text);

            this.inputField.onEndEdit.AddListener(OnEndEdit);
        }

        private void OnEndEdit(string value)
        {
            PlayerPrefs.SetString(this.uniqueName, value);
        }

        private InputField inputField;
        private string uniqueName;
    }
}