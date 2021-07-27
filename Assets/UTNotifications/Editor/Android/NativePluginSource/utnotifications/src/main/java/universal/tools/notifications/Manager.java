package universal.tools.notifications;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.app.AlarmManager;
import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.res.Resources;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.AsyncTask;
import android.os.Build;
import android.os.Bundle;
import android.os.SystemClock;
import android.provider.Settings;
import android.security.NetworkSecurityPolicy;
import android.service.notification.StatusBarNotification;
import androidx.annotation.RequiresApi;
import androidx.core.app.NotificationManagerCompat;
import android.telephony.TelephonyManager;
import android.text.TextUtils;
import android.util.Base64;
import android.util.Log;

import com.unity3d.player.UnityPlayer;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.lang.ref.WeakReference;
import java.net.URL;
import java.net.URLConnection;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;

import me.leolin.shortcutbadger.ShortcutBadger;

public class Manager {
    static final String KEY_NOTIFICATION = "universal.tools.notifications.__notification";
    static final String KEY_OPEN_URL = "universal.tools.notifications.__open_url";
    static final String KEY_BADGE_NUMBER = "universal.tools.notifications.__badge_number";
    static final String KEY_BUTTON_INDEX = "universal.tools.notifications.__button_index";
    static final String KEY_ID = "universal.tools.notifications.__id";
    private static final String WILL_HANDLE_RECEIVED_NOTIFICATIONS = "WILL_HANDLE_RECEIVED_NOTIFICATIONS";
    private static final String INCREMENTAL_ID = "INCREMENTAL_ID";
    private static final String START_ID = "START_ID";
    private static final String SHOW_NOTIFICATIONS_MODE = "SHOW_NOTIFICATIONS_MODE";
    private static final String NOTIFICATIONS_GROUPING_MODE = "NOTIFICATIONS_GROUPING_MODE";
    private static final String SCHEDULE_TIMER_TYPE = "SCHEDULE_TIMER_TYPE";
    private static final String SHOW_LATEST_NOTIFICATIONS_ONLY = "SHOW_LATEST_NOTIFICATIONS_ONLY";
    private static final String SCHEDULE_EXACT = "SCHEDULE_EXACT";
    private static final String NOTIFICATIONS_ENABLED = "NOTIFICATIONS_ENABLED";
    private static final String PUSH_NOTIFICATIONS_ENABLED = "PUSH_NOTIFICATIONS_ENABLED";
    private static final String RECEIVED_NOTIFICATIONS = "RECEIVED_NOTIFICATIONS";
    private static final String SCHEDULED_NOTIFICATION_IDS = "SCHEDULED_NOTIFICATION_IDS";
    private static final String TITLE_FIELD_NAME = "TITLE_FIELD_NAME";
    private static final String TEXT_FIELD_NAME = "TEXT_FIELD_NAME";
    private static final String USER_DATA_FIELD_NAME = "USER_DATA_FIELD_NAME";
    private static final String NOTIFICATION_PROFILE_FIELD_NAME = "NOTIFICATION_PROFILE_FIELD_NAME";
    private static final String ID_FIELD_NAME = "ID_FIELD_NAME";
    private static final String BADGE_FIELD_NAME = "BADGE_FIELD_NAME";
    private static final String BUTTONS_FIELD_NAME = "BUTTONS_FIELD_NAME";
    private static final String ALERT = "alert";
    private static final String BODY = "body";
    private static final String IMAGE_URL = "image_url";
    private static final String OPEN_URL = "open_url";
    @SuppressWarnings("unused")
    private static IPushNotificationsProvider m_provider;
    private static Intent intent;
    private static int m_nextPushNotificationId = -1;
    private static WeakReference<Activity> m_closedActivity = null;

    //public
    public static void initialize(final boolean firebasePushNotificationsEnabled,
                                     final boolean amazonPushNotificationsEnabled,
                                     final boolean willHandleReceivedNotifications,
                                     final int startId,
                                     final boolean incrementalId,
                                     final int showNotificationsMode,
                                     final boolean restoreScheduledOnReboot,
                                     final int notificationsGroupingMode,
                                     final int scheduleTimerType,
                                     final boolean showLatestNotificationOnly,
                                     final boolean scheduleExact,
                                     final String titleFieldName,
                                     final String textFieldName,
                                     final String userDataFieldName,
                                     final String notificationProfileFieldName,
                                     final String idFieldName,
                                     final String badgeFieldName,
                                     final String buttonsFieldName,
                                     final String profilesSettingsJson,
                                     final boolean allowUpdatingGooglePlayIfRequired) {
        m_nextPushNotificationId = startId;

        final Context context = UnityPlayer.currentActivity.getApplicationContext();

        final SharedPreferences prefs = context.getSharedPreferences(Manager.class.getName(), Context.MODE_PRIVATE);
        final SharedPreferences.Editor editor = prefs.edit();
        editor.putBoolean(WILL_HANDLE_RECEIVED_NOTIFICATIONS, willHandleReceivedNotifications);
        editor.putBoolean(INCREMENTAL_ID, incrementalId);
        editor.putInt(START_ID, startId);
        editor.putInt(SHOW_NOTIFICATIONS_MODE, showNotificationsMode);
        editor.putInt(NOTIFICATIONS_GROUPING_MODE, notificationsGroupingMode);
        editor.putInt(SCHEDULE_TIMER_TYPE, scheduleTimerType);
        editor.putBoolean(SHOW_LATEST_NOTIFICATIONS_ONLY, showLatestNotificationOnly);
        editor.putBoolean(SCHEDULE_EXACT, scheduleExact);
        editor.putString(TITLE_FIELD_NAME, titleFieldName);
        editor.putString(TEXT_FIELD_NAME, textFieldName);
        editor.putString(USER_DATA_FIELD_NAME, userDataFieldName);
        editor.putString(NOTIFICATION_PROFILE_FIELD_NAME, notificationProfileFieldName);
        editor.putString(ID_FIELD_NAME, idFieldName);
        editor.putString(BADGE_FIELD_NAME, badgeFieldName);
        editor.putString(BUTTONS_FIELD_NAME, buttonsFieldName);
        registerProfile(context, editor, profilesSettingsJson);
        editor.apply();

        ScheduledNotificationsRestorer.setRestoreScheduledOnReboot(context, restoreScheduledOnReboot);

        try {
            if (firebasePushNotificationsEnabled) {
                if (fcmProviderAvailable(allowUpdatingGooglePlayIfRequired)) {
                    m_provider = new FCMProvider();
                }
            }
            if (amazonPushNotificationsEnabled) {
                if (admProviderAvailable()) {
                    m_provider = new ADMProvider(context);
                }
            }

            if (m_provider != null) {
                if (pushNotificationsEnabled(context)) {
                    m_provider.enable();
                } else {
                    m_provider.disable();
                }
            } else if (firebasePushNotificationsEnabled || amazonPushNotificationsEnabled) {
                onPushRegistrationFailed("None of the enabled push notifications APIs is available in the device or required libraries are missing in Assets/Plugins/Android.");
            }
        } catch (final Throwable e) {
            if (pushNotificationsEnabled(context)) {
                onPushRegistrationFailed(e.toString());
            } else {
                Log.e(Manager.class.getName(), "Failed to disable push notifications", e);
            }
        }
    }

    public static boolean fcmProviderAvailable(final boolean allowUpdatingGooglePlayIfRequired) {
        return FCMProvider.isAvailable(UnityPlayer.currentActivity.getApplicationContext(), allowUpdatingGooglePlayIfRequired);
    }

    public static boolean admProviderAvailable() {
        return ADMProvider.isAvailable();
    }

    public static void postNotification(final String title, final String text, final int id, final String userDataJson, final String notificationProfile, final int badgeNumber, final String buttonsJson) throws JSONException {
        postNotification(UnityPlayer.currentActivity, new UTNotification(0L, 0, decodeBase64(title), decodeBase64(text), id, decodeBase64(userDataJson), notificationProfile, badgeNumber, decodeBase64(buttonsJson)));
    }

    public static void scheduleNotification(final int triggerInSeconds, final String title, final String text, final int id, final String userDataJson, final String notificationProfile, final int badgeNumber, final String buttonsJson) throws JSONException {
        scheduleNotificationCommon(UnityPlayer.currentActivity, new UTNotification(triggerInSecondsToTriggerAtSystemTimeMillis(triggerInSeconds), 0, decodeBase64(title), decodeBase64(text), id, decodeBase64(userDataJson), notificationProfile, badgeNumber, decodeBase64(buttonsJson)));
    }

    public static void scheduleNotificationRepeating(final int firstTriggerInSeconds, final int intervalSeconds, final String title, final String text, final int id, final String userDataJson, final String notificationProfile, final int badgeNumber, final String buttonsJson) throws JSONException {
        scheduleNotificationCommon(UnityPlayer.currentActivity, new UTNotification(triggerInSecondsToTriggerAtSystemTimeMillis(firstTriggerInSeconds), intervalSeconds, decodeBase64(title), decodeBase64(text), id, decodeBase64(userDataJson), notificationProfile, badgeNumber, decodeBase64(buttonsJson)));
    }

    public static boolean notificationsEnabled() {
        return notificationsEnabled(UnityPlayer.currentActivity.getApplicationContext());
    }

    public static boolean notificationsAllowed() {
        return NotificationManagerCompat.from(UnityPlayer.currentActivity.getApplicationContext()).areNotificationsEnabled();
    }

    public static void setNotificationsEnabled(final boolean enabled) {
        final Context context = UnityPlayer.currentActivity.getApplicationContext();

        final SharedPreferences prefs = context.getSharedPreferences(Manager.class.getName(), Context.MODE_PRIVATE);
        final SharedPreferences.Editor editor = prefs.edit();
        editor.putBoolean(NOTIFICATIONS_ENABLED, enabled);
        editor.apply();
    }

    public static boolean pushNotificationsEnabled() {
        final Context context = UnityPlayer.currentActivity.getApplicationContext();
        return m_provider != null && notificationsEnabled(context) && pushNotificationsEnabled(context);
    }

    public static void setPushNotificationsEnabled(final boolean enabled) {
        final Context context = UnityPlayer.currentActivity.getApplicationContext();

        if (pushNotificationsEnabled(context) != enabled) {
            final SharedPreferences prefs = context.getSharedPreferences(Manager.class.getName(), Context.MODE_PRIVATE);
            final SharedPreferences.Editor editor = prefs.edit();
            editor.putBoolean(PUSH_NOTIFICATIONS_ENABLED, enabled);
            editor.apply();

            if (m_provider != null) {
                if (enabled) {
                    m_provider.enable();
                } else {
                    m_provider.disable();
                }
            }
        }
    }

    public static void hideNotification(final int id) {
        hideNotification(UnityPlayer.currentActivity, id);
    }

    public static void cancelNotification(final int id) {
        hideNotification(id);

        final Context context = UnityPlayer.currentActivity.getApplicationContext();

        // Only id really matters to find the notification
        final PendingIntent pendingIntent = buildPendingIntentForScheduledNotification(context, new UTNotification(id));
        final AlarmManager alarmManager = (AlarmManager) context.getSystemService(Context.ALARM_SERVICE);
        alarmManager.cancel(pendingIntent);

        clearSheduledNotificationId(context, id);
    }

    public static void hideAllNotifications() {
        hideAllNotifications(UnityPlayer.currentActivity);
    }

    public static void cancelAllNotifications() {
        hideAllNotifications();
        for (int id : getStoredScheduledNotificationIds(UnityPlayer.currentActivity.getApplicationContext())) {
            cancelNotification(id);
        }
    }

    public static String getClickedNotificationPacked() throws JSONException {
        final Intent activityIntent = getIntent();
        if (activityIntent != null) {
            final String result = activityIntent.getStringExtra(KEY_NOTIFICATION);
            setIntent(null);

            if (result != null) {
                if (activityIntent.hasExtra(KEY_BUTTON_INDEX)) {
                    // Add button index to the json
                    JSONObject json = new JSONObject(result);
                    json.put("buttonIndex", activityIntent.getIntExtra(KEY_BUTTON_INDEX, -1));
                    return json.toString();
                } else {
                    return result;
                }
            }
        }

        return "";
    }

    public static String getReceivedNotificationsPacked() {
        final Context context = UnityPlayer.currentActivity.getApplicationContext();
        final String res = readReceivedNotificationsPacked(context);

        final SharedPreferences prefs = context.getSharedPreferences(Manager.class.getName(), Context.MODE_PRIVATE);
        final SharedPreferences.Editor editor = prefs.edit();
        editor.remove(RECEIVED_NOTIFICATIONS);
        editor.apply();

        return "[" + res + "]";
    }

    public static int getBadge() {
        return getBadge(UnityPlayer.currentActivity.getApplicationContext());
    }

    public static void setBadge(final int badgeNumber) {
        setBadge(UnityPlayer.currentActivity.getApplicationContext(), badgeNumber);
    }

    public static void subscribeToTopic(final String topic) {
        if (m_provider != null) {
            m_provider.subscribeToTopic(topic);
        }
    }

    public static void unsubscribeFromTopic(final String topic) {
        if (m_provider != null) {
            m_provider.unsubscribeFromTopic(topic);
        }
    }

    @SuppressLint({"HardwareIds", "MissingPermission"})
    public static String getDeviceId() {
        final Context context = UnityPlayer.currentActivity;
        String deviceId = "";
        if (context != null) {
            try {
                final TelephonyManager telephonyManager = (TelephonyManager) context.getSystemService(Context.TELEPHONY_SERVICE);
                if (telephonyManager != null) {
                    deviceId = telephonyManager.getDeviceId();
                }
            } catch (final Throwable ignored) {
                // No permission. Ignore.
            }

            if (deviceId == null || deviceId.isEmpty()) {
                try {
                    deviceId = Settings.Secure.getString(context.getContentResolver(), Settings.Secure.ANDROID_ID);
                } catch (final Throwable ignored) {
                }
            }

            if (deviceId == null || deviceId.isEmpty()) {
                InputStream stream = null;
                BufferedReader reader = null;
                try {
                    stream = new FileInputStream("/sys/class/net/wlan0/address");
                    reader = new BufferedReader(new InputStreamReader(stream));
                    deviceId = reader.readLine();
                } catch (final IOException ignored) {
                } finally {
                    if (reader != null) {
                        try {
                            reader.close();
                        } catch (final IOException ignored) {
                        }
                    }

                    if (stream != null) {
                        try {
                            stream.close();
                        } catch (final IOException ignored) {
                        }
                    }
                }
            }

            if (deviceId == null) {
                deviceId = "";
            }
        }

        return deviceId;
    }

    private static boolean isCleartextPermitted(final String host) {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            final NetworkSecurityPolicy policy = NetworkSecurityPolicy.getInstance();
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
                return policy.isCleartextTrafficPermitted(host);
            } else {
                return policy.isCleartextTrafficPermitted();
            }
        }

        return true;
    }

    static String getTitleKey(final SharedPreferences prefs) {
        return prefs.getString(TITLE_FIELD_NAME, "title");
    }

    static String getTextKey(final SharedPreferences prefs) {
        return prefs.getString(TEXT_FIELD_NAME, "text");
    }

    static String getIdKey(final SharedPreferences prefs) {
        return prefs.getString(ID_FIELD_NAME, "id");
    }

    static String getUserDataKey(final SharedPreferences prefs) {
        return prefs.getString(USER_DATA_FIELD_NAME, "");
    }

    static String getNotificationProfileKey(final SharedPreferences prefs) {
        return prefs.getString(NOTIFICATION_PROFILE_FIELD_NAME, "notification_profile");
    }

    static String getBadgeNumberKey(final SharedPreferences prefs) {
        return prefs.getString(BADGE_FIELD_NAME, "badge_number");
    }

    static String getButtonsKey(final SharedPreferences prefs) {
        return prefs.getString(BUTTONS_FIELD_NAME, "buttons");
    }

    static void postPushNotification(Context context, final String provider, final Bundle extras) {
        context = context.getApplicationContext();

        if (!pushNotificationsEnabled(context)) {
            return;
        }

        final SharedPreferences prefs = context.getSharedPreferences(Manager.class.getName(), Context.MODE_PRIVATE);
        final String titleKey = getTitleKey(prefs);
        final String textKey = getTextKey(prefs);
        final String userDataKey = getUserDataKey(prefs);
        final String notificationProfileKey = getNotificationProfileKey(prefs);
        final String idKey = getIdKey(prefs);
        final String badgeKey = getBadgeNumberKey(prefs);
        final String buttonsKey = getButtonsKey(prefs);

        String title = extractStringFromBundle(extras, titleKey);
        String text = extractStringFromBundle(extras, textKey);
        if (text == null) {
            // OneSignal format support
            text = extras.getString(ALERT);
            extras.remove(ALERT);
        }
        if (text == null) {
            // "body" format support
            text = extras.getString(BODY);
            extras.remove(BODY);
        }

        if (title == null && text == null) {
            // Message format is invalid. It can be a service message from AppsFlyer or similar: ignore.
            Log.e(Manager.class.getName(), "Push notification message is received but malformed: " +
                    "\"data/" + titleKey + " & " + "\"data/" + textKey + " not present!\nIt can be just a service message, then you can ignore it. Otherwise configure Advanced->Push Payload Format in the UTNotifications settings");
            return;
        }

        if (title == null) {
            title = "\"data/" + titleKey + "\" not present! Configure Advanced->Push Payload Format in the UTNotifications settings";
        }
        if (text == null) {
            text = "\"data/" + textKey + "\" not present! Configure Advanced->Push Payload Format in the UTNotifications settings";
        }

        final String notificationProfile = extractStringFromBundle(extras, notificationProfileKey);
        final int badgeNumber = extractIntFromBundle(extras, badgeKey, -1);
        final int id = extractIntFromBundle(extras, idKey, -1);

        // Try parsing buttons data
        String buttonsJsonString = extractStringFromBundle(extras, buttonsKey);
        if (buttonsJsonString == null) {
            // Try OneSignal format
            buttonsJsonString = extractStringFromBundle(extras, "o");
        }
        List<UTNotification.Button> buttons = null;
        if (buttonsJsonString != null) {
            try {
                JSONArray buttonsArray = new JSONArray(buttonsJsonString);
                List<UTNotification.Button> parsedButtons = new ArrayList<>(buttonsArray.length());
                for (int i = 0; i < buttonsArray.length(); ++i) {
                    JSONObject buttonJson = buttonsArray.getJSONObject(i);
                    String buttonTitle;
                    if (buttonJson.has("title")) {
                        buttonTitle = buttonJson.getString("title");
                        buttonJson.remove("title");
                    } else if (buttonJson.has("n")) {
                        buttonTitle = buttonJson.getString("n");
                        buttonJson.remove("n");
                    } else {
                        Log.e(Manager.class.getName(), "Skipping button without a title: " + buttonJson);
                        continue;
                    }

                    parsedButtons.add(new UTNotification.Button(buttonTitle, buttonJson));
                }

                buttons = parsedButtons;
            } catch (Throwable e) {
                e.printStackTrace();
            }
        }

        Bundle userData = getSubBundle(extras, userDataKey.split("\\/"), false);
        if (userData == null) {
            userData = extras;
        }

        Manager.postNotification(context, new UTNotification(provider, 0L, 0, title, text, (id != -1) ? id : Manager.getNextPushNotificationId(context), bundleToMap(userData), notificationProfile, badgeNumber, buttons));
    }

    static void postNotification(final Context context_, final UTNotification notification) {
        final Context context = context_.getApplicationContext();

        if (notification.userData != null && notification.userData.containsKey(IMAGE_URL)) {
            final AsyncTask<Void, Void, Bitmap> downloadImageAsyncTask = new AsyncTask<Void, Void, Bitmap>() {
                @Override
                protected Bitmap doInBackground(final Void... params) {
                    InputStream in = null;

                    try {
                        final URL url = new URL(notification.userData.get(IMAGE_URL));
                        final URLConnection connection = url.openConnection();
                        connection.setDoInput(true);
                        connection.connect();
                        in = connection.getInputStream();
                        return BitmapFactory.decodeStream(in);
                    } catch (final Throwable e) {
                        e.printStackTrace();
                        return null;
                    } finally {
                        if (in != null) {
                            try {
                                in.close();
                            } catch (final Throwable e) {
                                e.printStackTrace();
                            }
                        }
                    }
                }

                @Override
                protected void onPostExecute(final Bitmap result) {
                    super.onPostExecute(result);

                    showNotification(context, notification, result);
                }
            };

            downloadImageAsyncTask.execute();
        } else {
            showNotification(context, notification, null);
        }
    }

    @SuppressLint("NewApi")
    public static void hideNotification(final Context context, final int id) {
        try {
            final NotificationManager notificationManager = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);

            String groupKey = null;
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
                final StatusBarNotification[] activeNotifications = notificationManager.getActiveNotifications();
                for (StatusBarNotification notification : activeNotifications)
                {
                    if (notification.getId() == id && notification.isGroup()) {
                        groupKey = notification.getNotification().getGroup();
                        break;
                    }
                }
            }

            notificationManager.cancel(id);

            if (groupKey != null) {
                final int groupSummary = getGroupNotificationId(groupKey);
                boolean emptyGroup = true;
                final StatusBarNotification[] activeNotifications = notificationManager.getActiveNotifications();
                for (StatusBarNotification notification : activeNotifications) {
                    if (notification.getId() != id && notification.getId() != groupSummary && notification.isGroup() && groupKey.equals(notification.getNotification().getGroup())) {
                        emptyGroup = false;
                        break;
                    }
                }

                if (emptyGroup) {
                    notificationManager.cancel(groupSummary);
                }
            }
        } catch (final Throwable e) {
            e.printStackTrace();
        }
    }

    public static void onApplicationQuit() {
        final Activity currentActivity = UnityPlayer.currentActivity;
        if (currentActivity != null) {
            m_closedActivity = new WeakReference<>(currentActivity);
        } else {
            m_closedActivity = null;
        }
    }

    public static String getStoredScheduledNotificationIdsJson() {
        final Context context = UnityPlayer.currentActivity;
        if (context == null) {
            return "[]";
        } else {
            final String idsString = getStoredScheduledNotificationIdsString(context.getApplicationContext());
            return TextUtils.isEmpty(idsString) ? "[]" : "[" + idsString + "]";
        }
    }

    static void onPushRegistered(final String providerName, final String id) {
        if (UnityPlayer.currentActivity != null) {
            final JSONArray json = new JSONArray();
            json.put(providerName);
            json.put(id);

            UnityPlayer.UnitySendMessage("UTNotificationsManager", "_OnAndroidIdReceived", json.toString());
        }
    }

    static void onPushRegistrationFailed(final String error) {
        if (UnityPlayer.currentActivity != null) {
            UnityPlayer.UnitySendMessage("UTNotificationsManager", "_OnAndroidPushRegistrationFailed", error);
        }
    }

    static boolean notificationsEnabled(final Context context) {
        SharedPreferences prefs = context.getSharedPreferences(Manager.class.getName(), Context.MODE_PRIVATE);
        return prefs.getBoolean(NOTIFICATIONS_ENABLED, true);
    }

    static boolean pushNotificationsEnabled(final Context context) {
        SharedPreferences prefs = context.getSharedPreferences(Manager.class.getName(), Context.MODE_PRIVATE);
        return prefs.getBoolean(PUSH_NOTIFICATIONS_ENABLED, true);
    }

    static int getNextPushNotificationId(Context context) {
        context = context.getApplicationContext();

        if (m_nextPushNotificationId == -1) {
            m_nextPushNotificationId = context.getSharedPreferences(Manager.class.getName(), Context.MODE_PRIVATE).getInt(START_ID, 0);
        }

        if (context.getSharedPreferences(Manager.class.getName(), Context.MODE_PRIVATE).getBoolean(INCREMENTAL_ID, false)) {
            return m_nextPushNotificationId++;
        } else {
            return m_nextPushNotificationId;
        }
    }

    static int[] getStoredScheduledNotificationIds(final Context context) {
        String scheduledNotificationIdsString = getStoredScheduledNotificationIdsString(context);

        if (scheduledNotificationIdsString == null || scheduledNotificationIdsString.isEmpty()) {
            return new int[0];
        }

        String[] split = scheduledNotificationIdsString.split(",");
        int[] res = new int[split.length];
        for (int i = 0; i < split.length; ++i) {
            res[i] = Integer.parseInt(split[i]);
        }

        return res;
    }

    static void scheduleNotificationCommon(Context context, final UTNotification notification) {
        context = context.getApplicationContext();

        final PendingIntent pendingIntent = buildPendingIntentForScheduledNotification(context, notification);

        long triggerAtSystemTimeMillis = notification.triggerAtSystemTimeMillis;
        final long currentSystemTimeMillis = System.currentTimeMillis();
        final AlarmManager alarmManager = (AlarmManager) context.getSystemService(Context.ALARM_SERVICE);
        final int timerType = context.getSharedPreferences(Manager.class.getName(), Context.MODE_PRIVATE).getInt(SCHEDULE_TIMER_TYPE, AlarmManager.ELAPSED_REALTIME_WAKEUP);
        if (notification.isRepeated()) {
            if (triggerAtSystemTimeMillis < currentSystemTimeMillis) {
                triggerAtSystemTimeMillis = currentSystemTimeMillis + ((triggerAtSystemTimeMillis - currentSystemTimeMillis) % notification.intervalSeconds);
            }

            // http://developer.android.com/reference/android/app/AlarmManager.html#setInexactRepeating(int, long, long, android.app.PendingIntent)
            // As of API 19, all repeating alarms are inexact. Because this method has been available since API 3, your application can safely call
            // it and be assured that it will get similar behavior on both current and older versions of Android.

            alarmManager.setInexactRepeating(timerType, triggerAtSystemTimeMillisToTimerTime(timerType, triggerAtSystemTimeMillis, currentSystemTimeMillis), (long) notification.intervalSeconds * 1000L, pendingIntent);
        } else {
            if (triggerAtSystemTimeMillis < currentSystemTimeMillis) {
                triggerAtSystemTimeMillis = currentSystemTimeMillis + 1000L;
            }
            final long triggerAtMillis = triggerAtSystemTimeMillisToTimerTime(timerType, triggerAtSystemTimeMillis, currentSystemTimeMillis);

            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.KITKAT && context.getSharedPreferences(Manager.class.getName(), Context.MODE_PRIVATE).getBoolean(SCHEDULE_EXACT, false)) {
                alarmManager.setExact(timerType, triggerAtMillis, pendingIntent);
            } else {
                alarmManager.set(timerType, triggerAtMillis, pendingIntent);
            }
        }

        storeScheduledNotificationId(context, notification);
    }

    private static Intent getIntent() {
        return intent;
    }

    static void setIntent(final Intent intent_) {
        intent = intent_;
    }

    static HashMap<String, String> bundleToMap(Bundle bundle) {
        if (bundle == null || bundle.isEmpty()) {
            return null;
        }

        final HashMap<String, String> map = new HashMap<>();
        for (String key : bundle.keySet()) {
            try {
                final Object value = bundle.get(key);
                map.put(key, value.toString());
            } catch (Throwable e) {
                e.printStackTrace();
            }
        }

        return map;
    }

    private static void hideAllNotifications(final Context context) {
        try {
            NotificationManager notificationManager = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);
            notificationManager.cancelAll();
        } catch (Throwable e) {
            e.printStackTrace();
        }
    }

    private static void showNotification(Context context, final UTNotification notification, final Bitmap bitmap) {
        try {
            context = context.getApplicationContext();

            if (notificationsEnabled(context)) {
                if (checkShowMode(context)) {
                    final android.app.Notification androidNotification = buildNotification(context, notification, bitmap);

                    final boolean onlyLatestNotification = context.getSharedPreferences(Manager.class.getName(), Context.MODE_PRIVATE).getBoolean(SHOW_LATEST_NOTIFICATIONS_ONLY, false);
                    if (onlyLatestNotification) {
                        hideAllNotifications(context);
                    }

                    if (notification.badgeNumber >= 0) {
                        // Required for Xiaomi devices, see https://github.com/leolin310148/ShortcutBadger/wiki/Xiaomi-Device-Support
                        ShortcutBadger.applyNotification(context, androidNotification, notification.badgeNumber);
                        setBadge(context, notification.badgeNumber);
                    }

                    NotificationManager notificationManager = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);
                    notificationManager.notify(notification.id, androidNotification);

                    // Automatically create/update a group notification
                    if (!onlyLatestNotification && Build.VERSION.SDK_INT >= Build.VERSION_CODES.N && !TextUtils.isEmpty(androidNotification.getGroup())) {
                        notificationManager.notify(getGroupNotificationId(androidNotification.getGroup()), buildGroupNotification(context, androidNotification));
                    }
                }

                notificationReceived(context, notification);
            }

            if (!isRepeatedOrNotScheduled(notification)) {
                clearSheduledNotificationId(context, notification.id);
            }
        } catch (Throwable e) {
            Log.e(Manager.class.getName(), e.toString());
            e.printStackTrace();
        }
    }

    private static int getGroupNotificationId(final String group) {
        return -Math.abs(group.hashCode());
    }

    private static PendingIntent buildPendingIntentForScheduledNotification(final Context context, final UTNotification notification) {
        final Intent notificationIntent = new Intent(context, AlarmBroadcastReceiver.class);

        notificationIntent.setData(Uri.parse("custom://ut." + notification.id));
        notificationIntent.putExtra(KEY_NOTIFICATION, notification.toString());

        return PendingIntent.getBroadcast(context, notification.id, notificationIntent, PendingIntent.FLAG_UPDATE_CURRENT);
    }

    private static int setSmallIcon(final Context context, final Resources res, final String packageName, final Notification.Builder builder, Profile profile, final Profile defaultProfile) {
        int icon = profile.getSmallIcon(context, res, packageName);
        if (icon == 0 && profile != defaultProfile) {
            icon = defaultProfile.getSmallIcon(context, res, packageName);
        }

        builder.setSmallIcon(icon);

        return icon;
    }

    private static void setColor(final Notification.Builder builder, Profile profile) {
        // Set the icon bg color, if specified
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP && profile.color != null) {
            builder.setColor(profile.color);
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
                builder.setColorized(true);
            }
        }
    }

    private static int getLargeIcon(final Resources res, final String packageName, final Profile profile, final Profile defaultProfile) {
        int largeIcon = profile.getLargeIcon(res, packageName);
        if (largeIcon == 0 && profile != defaultProfile) {
            largeIcon = defaultProfile.getLargeIcon(res, packageName);
        }

        return largeIcon;
    }

    private static String getSoundUri(final Resources res, final String packageName, Profile profile, final Profile defaultProfile) {
        int soundId = profile.getSoundId(res, packageName);
        if (soundId == 0 && profile != defaultProfile) {
            soundId = defaultProfile.getSoundId(res, packageName);
            profile = defaultProfile;
        }

        return Profile.getSoundUri(packageName, profile.id, soundId);
    }

    private static PendingIntent buildPendingIntent(final Context context, final UTNotification notification, final int buttonIndex) {
        final Intent notificationIntent = new Intent(context, NotificationIntentService.class);
        notificationIntent.putExtra(KEY_NOTIFICATION, notification.toString());
        notificationIntent.putExtra(KEY_ID, notification.id);

        final Map<String, String> userData =
                buttonIndex >= 0 && buttonIndex < notification.buttons.size()
                        ? notification.buttons.get(buttonIndex).userData
                        : notification.userData;
        if (userData != null && userData.containsKey(OPEN_URL)) {
            final String url = userData.get(OPEN_URL);
            if (url != null) {
                notificationIntent.putExtra(KEY_OPEN_URL, url);
            }
        }
        int id;
        if (buttonIndex >= 0) {
            notificationIntent.putExtra(KEY_BUTTON_INDEX, buttonIndex);
            final int BUTTON_ID_BASE = 0x80800000;
            final int BUTTONS_PER_NOTIFICATION_MAX = 64; // Even than enough
            final int PRIME_NUMBER = 104729;
            // Let's generate the button id as least likely as possible to be in use
            id = BUTTON_ID_BASE + (notification.id % PRIME_NUMBER) * BUTTONS_PER_NOTIFICATION_MAX + buttonIndex;
        } else {
            id = notification.id;
        }
        return PendingIntent.getService(context, id, notificationIntent, PendingIntent.FLAG_UPDATE_CURRENT);
    }

    @SuppressWarnings("deprecation")
    @SuppressLint("NewApi")
    private static android.app.Notification buildNotification(final Context context, final UTNotification notification, final Bitmap bitmap) {
        final Resources res = context.getResources();

        final SharedPreferences prefs = context.getSharedPreferences(Manager.class.getName(), Context.MODE_PRIVATE);
        final Profile defaultProfile = new Profile(Profile.DEFAULT_ID, prefs);
        final Profile profile = (notification.notificationProfile == null || Profile.DEFAULT_ID.equals(notification.notificationProfile))
                ? defaultProfile
                : new Profile(notification.notificationProfile, prefs);
        final String packageName = context.getPackageName();

        final boolean notificationChannels = Build.VERSION.SDK_INT >= Build.VERSION_CODES.O;

        //Find a custom large icon if specified
        int largeIcon = getLargeIcon(res, packageName, profile, defaultProfile);

        //Find a custom sound if specified and not controlled by notification channels
        final String soundUri = notificationChannels ? null : getSoundUri(res, packageName, profile, defaultProfile);

        final PendingIntent contentIntent = buildPendingIntent(context, notification, -1);

        final Notification.Builder builder = notificationChannels
                ? new Notification.Builder(context, profile.id)
                : new Notification.Builder(context);

        builder.setContentTitle(notification.title)
                .setContentText(notification.text)
                .setContentIntent(contentIntent)
                .setAutoCancel(true);

        if (!notificationChannels) { // Controlled by notification channels otherwise
            builder.setDefaults((soundUri == null)
                    ? Notification.DEFAULT_ALL
                    : Notification.DEFAULT_LIGHTS | Notification.DEFAULT_VIBRATE);
        }

        // Find an set small icon
        int icon = setSmallIcon(context, res, packageName, builder, profile, defaultProfile);

        // Set color, if required
        setColor(builder, profile);

        // Add buttons if any and if supported
        if (notification.buttons != null &&
                !notification.buttons.isEmpty() &&
                Build.VERSION.SDK_INT > Build.VERSION_CODES.KITKAT) {

            for (int i = 0; i < notification.buttons.size(); ++i) {
                final UTNotification.Button it = notification.buttons.get(i);
                final PendingIntent intent = buildPendingIntent(context, notification, i);

                final Notification.Action action = new Notification.Action.Builder(0, it.title, intent).build();
                builder.addAction(action);
            }
        }

        if (!notificationChannels && profile.highPriority) {
            builder.setPriority(android.app.Notification.PRIORITY_HIGH);
        }

        // Android version prior 5 crop oversized icons instead of scaling them. Let's help it.
        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.LOLLIPOP) {
            if (largeIcon == 0) {
                largeIcon = icon;
            }

            int targetWidth = res.getDimensionPixelSize(android.R.dimen.notification_large_icon_width);
            int targetHeight = res.getDimensionPixelSize(android.R.dimen.notification_large_icon_height);

            builder.setLargeIcon(Bitmap.createScaledBitmap(BitmapFactory.decodeResource(res, largeIcon), targetWidth, targetHeight, false));
        } else if (largeIcon != 0) {
            builder.setLargeIcon(BitmapFactory.decodeResource(res, largeIcon));
        }

        if (soundUri != null) {
            builder.setSound(Uri.parse(soundUri));
        }

        if (bitmap != null) {
            builder.setStyle(new Notification.BigPictureStyle().bigPicture(bitmap).setBigContentTitle(notification.title).setSummaryText(notification.text));
        } else {
            builder.setStyle(new Notification.BigTextStyle().bigText(notification.text));
        }

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
            final int groupingMode = context.getSharedPreferences(Manager.class.getName(), Context.MODE_PRIVATE).getInt(NOTIFICATIONS_GROUPING_MODE, 0);
            switch (groupingMode) {
                //NONE
                case 0:
                    break;

                //BY_NOTIFICATION_PROFILES
                case 1:
                    builder.setGroup(profile.id);
                    break;

                //FROM_USER_DATA
                case 2:
                    if (notification.userData != null && notification.userData.containsKey("notification_group")) {
                        builder.setGroup(notification.userData.get("notification_group"));
                    }
                    break;

                //ALL_IN_A_SINGLE_GROUP
                case 3:
                    builder.setGroup("__ALL");
                    break;
            }
        }

        return builder.build();
    }

    @SuppressLint("NewApi")
    @RequiresApi(api = Build.VERSION_CODES.N)
    private static android.app.Notification buildGroupNotification(final Context context, final android.app.Notification notification) {
        final Notification.Builder builder = Build.VERSION.SDK_INT >= Build.VERSION_CODES.O
                ? new Notification.Builder(context, notification.getChannelId())
                : new Notification.Builder(context);

        int smallIcon;
        try {
            smallIcon = notification.getSmallIcon().getResId();
        } catch (final Throwable e) {
            smallIcon = android.R.drawable.btn_plus;
        }

        return builder
                .setSmallIcon(smallIcon)
                .setAutoCancel(true)
                .setGroup(notification.getGroup())
                .setGroupSummary(true)
                .build();
    }

    private static void notificationReceived(final Context context, final UTNotification notification) {
        boolean willHandleReceivedNotifications = context.getSharedPreferences(Manager.class.getName(), Context.MODE_PRIVATE).getBoolean(WILL_HANDLE_RECEIVED_NOTIFICATIONS, false);

        if (willHandleReceivedNotifications) {
            String receivedNotificationsPacked = readReceivedNotificationsPacked(context);
            final String receivedPacked = notification.toString();

            if (!TextUtils.isEmpty(receivedPacked)) {
                if (!TextUtils.isEmpty(receivedNotificationsPacked)) {
                    receivedNotificationsPacked += ',' + receivedPacked;
                } else {
                    receivedNotificationsPacked = receivedPacked;
                }

                final SharedPreferences prefs = context.getSharedPreferences(Manager.class.getName(), Context.MODE_PRIVATE);
                final SharedPreferences.Editor editor = prefs.edit();
                editor.putString(RECEIVED_NOTIFICATIONS, receivedNotificationsPacked);
                editor.apply();
            }
        }
    }

    private static String readReceivedNotificationsPacked(final Context context) {
        return context.getSharedPreferences(Manager.class.getName(), Context.MODE_PRIVATE).getString(RECEIVED_NOTIFICATIONS, "");
    }

    private static String getStoredScheduledNotificationIdsString(final Context context) {
        return context.getSharedPreferences(Manager.class.getName(), Context.MODE_PRIVATE).getString(SCHEDULED_NOTIFICATION_IDS, null);
    }

    private static void storeScheduledNotificationId(final Context context, final UTNotification notification) {
        ScheduledNotificationsRestorer.register(context, notification);

        final int[] ids = getStoredScheduledNotificationIds(context);
        for (int storedId : ids) {
            if (storedId == notification.id) {
                return;
            }
        }

        String scheduledNotificationIdsString = getStoredScheduledNotificationIdsString(context);
        if (scheduledNotificationIdsString == null || scheduledNotificationIdsString.isEmpty()) {
            scheduledNotificationIdsString = "" + notification.id;
        } else {
            scheduledNotificationIdsString += "," + notification.id;
        }

        final SharedPreferences prefs = context.getSharedPreferences(Manager.class.getName(), Context.MODE_PRIVATE);
        final SharedPreferences.Editor editor = prefs.edit();
        editor.putString(SCHEDULED_NOTIFICATION_IDS, scheduledNotificationIdsString);
        editor.apply();
    }

    private static void clearSheduledNotificationId(final Context context, final int id) {
        final String idAsString = Integer.toString(id);

        String scheduledNotificationIdsString = getStoredScheduledNotificationIdsString(context);
        if (scheduledNotificationIdsString != null && !scheduledNotificationIdsString.isEmpty()) {
            boolean found = false;
            if (scheduledNotificationIdsString.equals(idAsString)) {
                scheduledNotificationIdsString = "";
                found = true;
            } else {
                if (scheduledNotificationIdsString.contains("," + idAsString + ",")) {
                    scheduledNotificationIdsString = scheduledNotificationIdsString.replace("," + idAsString + ",", ",");
                    found = true;
                } else if (scheduledNotificationIdsString.startsWith(idAsString + ",")) {
                    scheduledNotificationIdsString = scheduledNotificationIdsString.substring(idAsString.length() + 1);
                    found = true;
                } else if (scheduledNotificationIdsString.endsWith("," + idAsString)) {
                    scheduledNotificationIdsString = scheduledNotificationIdsString.substring(0, scheduledNotificationIdsString.length() - (idAsString.length() + 1));
                    found = true;
                }
            }

            if (found) {
                SharedPreferences prefs = context.getSharedPreferences(Manager.class.getName(), Context.MODE_PRIVATE);
                SharedPreferences.Editor editor = prefs.edit();
                editor.putString(SCHEDULED_NOTIFICATION_IDS, scheduledNotificationIdsString);
                editor.apply();
            }
        }

        ScheduledNotificationsRestorer.cancel(context, id);
    }

    private static boolean checkShowMode(final Context context) {
        final int showNotificationsMode = context.getApplicationContext().getSharedPreferences(Manager.class.getName(), Context.MODE_PRIVATE).getInt(SHOW_NOTIFICATIONS_MODE, 0);

        switch (showNotificationsMode) {
            //WHEN_CLOSED_OR_IN_BACKGROUND
            case 0:
                return UnityPlayer.currentActivity == null || !UnityPlayer.currentActivity.hasWindowFocus();

            //WHEN_CLOSED
            case 1:
                final Activity currentActivity = UnityPlayer.currentActivity;
                return currentActivity == null
                        || currentActivity.getWindow() == null
                        || (m_closedActivity != null && currentActivity.equals(m_closedActivity.get()));

            //ALWAYS
            default:
                return true;
        }
    }

    private static void setBadge(final Context context, final int badgeNumber) {
        if (badgeNumber != 0) {
            ShortcutBadger.applyCount(context, badgeNumber);
        } else {
            ShortcutBadger.removeCount(context);
        }

        final SharedPreferences prefs = context.getSharedPreferences(Manager.class.getName(), Context.MODE_PRIVATE);
        final SharedPreferences.Editor editor = prefs.edit();
        editor.putInt(KEY_BADGE_NUMBER, badgeNumber);
        editor.apply();
    }

    private static int getBadge(final Context context) {
        final SharedPreferences prefs = context.getSharedPreferences(Manager.class.getName(), Context.MODE_PRIVATE);
        return prefs.getInt(KEY_BADGE_NUMBER, 0);
    }

    private static Bundle getSubBundle(Bundle bundle, final String[] path, final boolean exceptOfTheLast) {
        if (path == null || path.length == 0 || (path.length == 1 && (path[0] == null || path[0].isEmpty()))) {
            return bundle;
        }

        int end = (exceptOfTheLast ? path.length - 1 : path.length);
        if (end > 0) {
            final Object obj = bundle.get(path[0]);
            if (obj instanceof String) {
                Bundle asBundle = jsonToBundle((String) obj);
                if (asBundle != null) {
                    bundle.putBundle(path[0], asBundle);
                }
            }
        } else {
            return bundle;
        }

        for (int i = 0; i < end; ++i) {
            try {
                bundle = bundle.getBundle(path[i]);
            } catch (Throwable e) {
                return null;
            }

            if (bundle == null) {
                return null;
            }
        }

        return bundle;
    }

    private static String extractStringFromBundle(Bundle bundle, String key) {
        final String[] path = key.split("/");
        bundle = getSubBundle(bundle, path, true);
        if (bundle != null) {
            try {
                key = path[path.length - 1];
                final String value = bundle.getString(key);
                bundle.remove(key);
                return value;
            } catch (Throwable e) {
                return null;
            }
        } else {
            return null;
        }
    }

    private static int extractIntFromBundle(Bundle bundle, String key, final int defaultValue) {
        int value = defaultValue;

        final String[] path = key.split("/");
        bundle = getSubBundle(bundle, path, true);
        if (bundle != null) {
            key = path[path.length - 1];

            try {
                value = bundle.getInt(key, defaultValue);
            } catch (Throwable ignored) {
            }

            if (value == defaultValue) {
                try {
                    final String valueAsString = bundle.getString(key);
                    value = Integer.parseInt(valueAsString);
                } catch (Throwable ignored) {
                }
            }

            bundle.remove(key);
        }

        return value;
    }

    private static Bundle jsonToBundle(final String json) {
        try {
            final JSONObject jsonObject = new JSONObject(json);
            return jsonToBundle(jsonObject);
        } catch (JSONException e) {
            e.printStackTrace();
            return null;
        }
    }

    private static Bundle jsonToBundle(final JSONObject json) throws JSONException {
        final Bundle bundle = new Bundle();

        final Iterator<String> keys = json.keys();
        while (keys.hasNext()) {
            final String key = keys.next();
            final Object value = json.get(key);

            if (value instanceof String) {
                bundle.putString(key, (String) value);
            } else if (value instanceof Integer) {
                bundle.putInt(key, (Integer) value);
            } else if (value instanceof Long) {
                bundle.putInt(key, (int) (long) (Long) value);
            } else if (value instanceof JSONObject) {
                bundle.putBundle(key, jsonToBundle((JSONObject) value));
            } else {
                bundle.putString(key, value.toString());
            }
        }
        return bundle;
    }

    private static void registerProfile(final Context context, final SharedPreferences.Editor editor, final String profilesSettingsJson) {
        try {
            final JSONArray profiles = new JSONArray(profilesSettingsJson);

            for (int i = 0; i < profiles.length(); ++i) {
                final JSONObject profile = profiles.getJSONObject(i);

                final String id = profile.getString("id");
                final String name = profile.getString("name");
                final String description = profile.getString("description");
                final boolean highPriority = profile.getBoolean("high_priority");
                final Integer color = profile.has("color") ? profile.getInt("color") : null;

                new Profile(id, name, description, highPriority, color)
                        .saveInSharedPreferences(editor)
                        .registerChannel(context);
            }
        } catch (JSONException e) {
            e.printStackTrace();
        }
    }

    private static boolean isRepeatedOrNotScheduled(UTNotification notification) {
        return !notification.isScheduled() || notification.isRepeated();
    }

    private static long triggerInSecondsToTriggerAtSystemTimeMillis(final int triggerInSeconds) {
        if (triggerInSeconds == 0) {
            return 0L;
        } else {
            return System.currentTimeMillis() + (long) triggerInSeconds * 1000L;
        }
    }

    private static long triggerAtSystemTimeMillisToElapsedRealtime(final long triggerAtSystemTimeMillis, final long currentSystemTimeMillis) {
        return SystemClock.elapsedRealtime() + triggerAtSystemTimeMillis - currentSystemTimeMillis;
    }

    private static long triggerAtSystemTimeMillisToTimerTime(final int timerType, final long triggerAtSystemTimeMillis, final long currentSystemTimeMillis) {
        switch (timerType)
        {
            case AlarmManager.ELAPSED_REALTIME:
            case AlarmManager.ELAPSED_REALTIME_WAKEUP:
                return triggerAtSystemTimeMillisToElapsedRealtime(triggerAtSystemTimeMillis, currentSystemTimeMillis);

            case AlarmManager.RTC:
            case AlarmManager.RTC_WAKEUP:
                return triggerAtSystemTimeMillis;

            default:
                Log.e(Manager.class.getName(), "Unexpected timer type: " + timerType);
                return triggerAtSystemTimeMillis;
        }
    }

    private static String decodeBase64(final String base64Encoded) {
        if (base64Encoded == null || base64Encoded.isEmpty()) {
            return base64Encoded;
        }

        try {
            return new String(Base64.decode(base64Encoded, Base64.DEFAULT));
        } catch (Throwable e) {
            e.printStackTrace();
            return base64Encoded;
        }
    }
}
