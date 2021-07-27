package universal.tools.notifications;

import android.text.TextUtils;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;

public class UTNotification {
    final String provider;
    final long triggerAtSystemTimeMillis;
    final int intervalSeconds;
    final String title;
    final String text;
    final int id;
    final Map<String, String> userData;
    final String notificationProfile;
    final int badgeNumber;
    final List<Button> buttons;

    UTNotification(final int id) {
        this.provider = null;
        this.triggerAtSystemTimeMillis = 0;
        this.intervalSeconds = 0;
        this.title = null;
        this.text = null;
        this.id = id;
        this.userData = null;
        this.notificationProfile = null;
        this.badgeNumber = -1;
        this.buttons = null;
    }

    UTNotification(final String jsonString) throws JSONException {
        final JSONObject json = new JSONObject(jsonString);

        this.provider = json.has("provider") ? json.getString("provider") : null;
        this.triggerAtSystemTimeMillis = json.has("triggerAtSystemTimeMillis") ? json.getLong("triggerAtSystemTimeMillis") : 0L;
        this.intervalSeconds = json.has("intervalSeconds") ? json.getInt("intervalSeconds") : 0;
        this.title = json.has("title") ? json.getString("title") : null;
        this.text = json.has("text") ? json.getString("text") : null;
        this.id = json.has("id") ? json.getInt("id") : 0;
        this.userData = json.has("userData") ? userDataJsonToHashMap(json.getJSONObject("userData")) : null;
        this.notificationProfile = json.has("notificationProfile") ? json.getString("notificationProfile") : null;
        this.badgeNumber = json.has("badgeNumber") ? json.getInt("badgeNumber") : -1;
        this.buttons = json.has("buttons") ? buttonsJsonToListOfButtons(json.getJSONArray("buttons")) : null;
    }

    UTNotification(final long triggerAtSystemTimeMillis, final int intervalSeconds, final String title, final String text, final int id, final String userDataJson, final String notificationProfile, final int badgeNumber, final String buttonsJson) throws JSONException {
        this(triggerAtSystemTimeMillis, intervalSeconds, title, text, id, userDataJsonToHashMap(userDataJson), notificationProfile, badgeNumber, buttonsJson);
    }

    private UTNotification(final long triggerAtSystemTimeMillis, final int intervalSeconds, final String title, final String text, final int id, final Map<String, String> userData, final String notificationProfile, final int badgeNumber, final String buttonsJson) throws JSONException {
        this(null, triggerAtSystemTimeMillis, intervalSeconds, title, text, id, userData, notificationProfile, badgeNumber, buttonsJsonToListOfButtons(buttonsJson));
    }

    UTNotification(final long triggerAtSystemTimeMillis, final int intervalSeconds, final String title, final String text, final int id, final Map<String, String> userData, final String notificationProfile, final int badgeNumber, final List<Button> buttons) {
        this(null,  triggerAtSystemTimeMillis, intervalSeconds, title, text, id, userData, notificationProfile, badgeNumber, buttons);
    }

    UTNotification(final String provider, final long triggerAtSystemTimeMillis, final int intervalSeconds, final String title, final String text, final int id, final Map<String, String> userData, final String notificationProfile, final int badgeNumber, final List<Button> buttons) {
        this.provider = provider;
        this.title = title;
        this.text = text;
        this.id = id;
        this.userData = userData;
        this.notificationProfile = notificationProfile;
        this.badgeNumber = badgeNumber;
        this.triggerAtSystemTimeMillis = triggerAtSystemTimeMillis;
        this.intervalSeconds = intervalSeconds;
        this.buttons = buttons;
    }

    private static JSONObject userDataToJson(Map<String, String> userData) throws JSONException {
        final JSONObject json = new JSONObject();

        for (Map.Entry<String, String> it : userData.entrySet()) {
            json.put(it.getKey(), it.getValue());
        }

        return json;
    }

    private static HashMap<String, String> userDataJsonToHashMap(final String jsonString) throws JSONException {
        if (jsonString != null && !jsonString.isEmpty()) {
            return userDataJsonToHashMap(new JSONObject(jsonString));
        } else {
            return null;
        }
    }

    private static HashMap<String, String> userDataJsonToHashMap(final JSONObject json) throws JSONException {
        final HashMap<String, String> userData = new HashMap<>();

        Iterator<String> keys = json.keys();
        while (keys.hasNext()) {
            final String key = keys.next();
            userData.put(key, json.getString(key));
        }

        return userData;
    }

    private static List<Button> buttonsJsonToListOfButtons(String buttonsJson) throws JSONException {
        if (buttonsJson != null && !buttonsJson.isEmpty()) {
            return buttonsJsonToListOfButtons(new JSONArray(buttonsJson));
        } else {
            return null;
        }
    }

    private static List<Button> buttonsJsonToListOfButtons(JSONArray array) throws JSONException {
        final ArrayList<Button> list = new ArrayList<>(array.length());

        for (int i = 0; i < array.length(); ++i) {
            final JSONObject it = array.getJSONObject(i);
            final Map<String, String> userData = it.has("userData") ? userDataJsonToHashMap(it.getJSONObject("userData")) : null;
            list.add(new Button(it.getString("title"), userData));
        }

        return list;
    }

    @Override
    public String toString() {
        try {
            final JSONObject json = new JSONObject();

            if (isScheduled()) {
                json.put("triggerAtSystemTimeMillis", this.triggerAtSystemTimeMillis);
            }
            if (isRepeated()) {
                json.put("intervalSeconds", this.intervalSeconds);
            }
            if (isPush()) {
                json.put("provider", this.provider);
            }
            if (this.title != null) {
                json.put("title", this.title);
            }
            if (this.text != null) {
                json.put("text", this.text);
            }
            json.put("id", this.id);
            if (this.userData != null && !this.userData.isEmpty()) {
                json.put("userData", userDataToJson(this.userData));
            }
            if (this.notificationProfile != null) {
                json.put("notificationProfile", this.notificationProfile);
            }
            json.put("badgeNumber", this.badgeNumber);
            if (hasButtons()) {
                json.put("buttons", buttonsJson());
            }

            return json.toString();
        } catch (final JSONException e) {
            e.printStackTrace();
            return "";
        }
    }

    boolean isScheduled() {
        return this.triggerAtSystemTimeMillis > 0;
    }

    boolean isRepeated() {
        return this.intervalSeconds > 0;
    }

    private boolean isPush() {
        return !TextUtils.isEmpty(this.provider);
    }

    private boolean hasButtons() {
        return this.buttons != null && !this.buttons.isEmpty();
    }

    private JSONArray buttonsJson() throws JSONException {
        if (!hasButtons()) {
            return null;
        }

        final JSONArray array = new JSONArray();
        for (Button it : this.buttons) {
            final JSONObject json = new JSONObject();
            json.put("title", it.title);
            if (it.userData != null) {
                json.put("userData", userDataToJson(it.userData));
            }

            array.put(json);
        }

        return array;
    }

    static class Button {
        final String title;
        final Map<String, String> userData;

        Button(final String title, Map<String, String> userData) {
            this.title = title;
            this.userData = userData;
        }

        Button(final String title, final JSONObject userData) throws JSONException {
            this(title, userDataJsonToHashMap(userData));
        }
    }
}
