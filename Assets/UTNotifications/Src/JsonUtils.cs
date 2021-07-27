using System.Collections.Generic;

namespace UTNotifications
{
    public sealed class JsonUtils
    {
        public static JSONArray ToJson(ICollection<Button> buttons)
        {
            if (buttons == null || buttons.Count == 0)
            {
                return null;
            }

            JSONArray json = new JSONArray();

            foreach (Button it in buttons)
            {
                JSONClass button = new JSONClass();
                button.Add("title", it.title);

                JSONNode userData = ToJson(it.userData);
                if (userData != null)
                {
                    button.Add("userData", userData);
                }

                json.Add(button);
            }

            return json;
        }

        public static ICollection<Button> ToButtons(JSONNode json)
        {
            if (json == null || !(json is JSONArray) || json.Count <= 0)
            {
                return null;
            }

            List<Button> buttons = new List<Button>(json.Count);
            for (int i = 0; i < json.Count; ++i)
            {
                JSONClass buttonJson = json[i] as JSONClass;
                if (buttonJson != null)
                {
                    buttons.Add(new Button(buttonJson["title"].Value, ToUserData(buttonJson["userData"])));
                }
            }

            return buttons;
        }

        public static JSONNode ToJson(IDictionary<string, string> userData)
        {
            if (userData == null || userData.Count == 0)
            {
                return null;
            }

            JSONClass json = new JSONClass();
            foreach (KeyValuePair<string, string> it in userData)
            {
                json.Add(it.Key, new JSONData(it.Value));
            }

            return json;
        }

        public static IDictionary<string, string> ToUserData(JSONNode json)
        {
            if (json == null || !(json is JSONClass) || json.Count <= 0)
            {
                return null;
            }

            Dictionary<string, string> userData = new Dictionary<string, string>();
            foreach (KeyValuePair<string, JSONNode> it in (JSONClass)json)
            {
                userData.Add(it.Key, it.Value.Value);
            }

            return userData;
        }
    }
}