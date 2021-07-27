using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace UTNotifications
{
    /// <summary>
    /// Sample component to restrict an InputField's valid content by a regular expression.
    /// </summary>
    [RequireComponent(typeof(InputField))]
    public class ValidatedInputField : MonoBehaviour
    {
        public string RequiredPattern;

        public bool IsValid()
        {
            return this.regex.IsMatch(this.inputField.text);
        }

        public string text
        {
            get
            {
                return this.inputField.text;
            }

            set
            {
                this.inputField.text = value;
            }
        }

        private void Awake()
        {
            this.regex = new Regex(this.RequiredPattern);

            this.inputField = GetComponent<InputField>();
            Debug.Assert(this.inputField != null, "InputField component not found in " + this.gameObject.name);
            this.incorrect = this.transform.Find("Incorrect").gameObject;
            Debug.Assert(this.incorrect != null, "Child \"Incorrect\" not found in " + this.gameObject.name);
            this.inputField.onValueChanged.AddListener(OnValueChanged);
        }

        private void Start()
        {
            OnValueChanged(this.inputField.text);
        }

        private void OnValueChanged(string value)
        {
            this.incorrect.SetActive(!IsValid());
        }

        private Regex regex;

        private InputField inputField;
        private GameObject incorrect;
    }
}