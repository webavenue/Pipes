package universal.tools.notifications;

import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.content.ContentResolver;
import android.content.Context;
import android.content.SharedPreferences;
import android.content.res.Resources;
import android.media.AudioAttributes;
import android.net.Uri;
import android.os.Build;
import android.util.Log;

public class Profile {
    static final String DEFAULT_ID = "__default_profile";
    private static final String PROFILE = "PROFILE";
    private static final String HIGH_PRIORITY = "HIGH_PRIORITY";
    private static final String COLOR = "COLOR";
    final String id;
    final String name;
    final String description;
    final boolean highPriority;
    final Integer color;

    Profile(String id, final SharedPreferences prefs) {
        if (!prefs.contains(highPrioritySettingName(id))) {
            Log.w(Profile.class.getName(), "Notification Profile " + id + " is not registered! Using default profile instead.");
            id = DEFAULT_ID;
        }

        final String colorKey = colorSettingName(id);

        this.id = id;
        this.name = null;
        this.description = null;
        this.highPriority = prefs.getBoolean(highPrioritySettingName(id), false);
        this.color = prefs.contains(colorKey) ? prefs.getInt(colorKey, 0) : null;
    }

    Profile(final String id, final String name, final String description, final boolean highPriority, final Integer color) {
        this.id = id;
        this.name = name;
        this.description = description;
        this.highPriority = highPriority;
        this.color = color;
    }

    private static int getSoundId(final Resources res, final String packageName, final String profileId) {
        return res.getIdentifier(profileId, "raw", packageName);
    }

    private static String highPrioritySettingName(final String profileId) {
        return PROFILE + "." + profileId + "." + HIGH_PRIORITY;
    }

    private static String colorSettingName(final String profileId) {
        return PROFILE + "." + profileId + "." + COLOR;
    }

    Profile saveInSharedPreferences(final SharedPreferences.Editor editor) {
        editor.putBoolean(highPrioritySettingName(this.id), this.highPriority);
        if (this.color != null) {
            editor.putInt(colorSettingName(this.id), this.color);
        } else {
            editor.remove(colorSettingName(this.id));
        }

        return this;
    }

    Profile registerChannel(final Context context) {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            try {
                final NotificationChannel channel = new NotificationChannel(this.id,
                        this.name,
                        this.highPriority ? NotificationManager.IMPORTANCE_HIGH : NotificationManager.IMPORTANCE_DEFAULT);

                channel.setDescription(this.description != null ? this.description : "");

                // Sound
                final Resources res = context.getResources();
                final String packageName = context.getPackageName();
                int soundId = getSoundId(res, packageName);
                String soundProifileId;
                if (soundId == 0 && !DEFAULT_ID.equals(this.id)) {
                    soundId = getSoundId(res, packageName, soundProifileId = DEFAULT_ID);
                } else {
                    soundProifileId = this.id;
                }
                if (soundId != 0) {
                    channel.setSound(Uri.parse(getSoundUri(packageName, soundProifileId, soundId)),
                            new AudioAttributes.Builder()
                                    .setContentType(AudioAttributes.CONTENT_TYPE_SONIFICATION)
                                    .setUsage(AudioAttributes.USAGE_NOTIFICATION)
                                    .build());
                }

                final NotificationManager notificationManager = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);
                notificationManager.createNotificationChannel(channel);
            } catch (Throwable e) {
                Log.w(Profile.class.getName(), "Unable to register notification channel " + this.id + ": " + e.getMessage());
            }
        }

        return this;
    }

    int getSmallIcon(final Context context, final Resources res, final String packageName) {
        int icon = 0;

        //Check if need to use Android 5.0+ version of the icon
        if (Build.VERSION.SDK_INT >= 21) {
            icon = res.getIdentifier(this.id + "_android5plus", "drawable", packageName);
        }

        if (icon == 0) {
            icon = res.getIdentifier(this.id, "drawable", packageName);
        }

        if (icon == 0 && DEFAULT_ID.equals(this.id)) {
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
                // Starting with Build.VERSION_CODES.M, specifying a small notification icon is mandatory.
                // Besides, starting with Build.VERSION_CODES.LOLLIPOP, the icon should be
                // white+transparent only icon. So let's use a system standard icon by default.
                icon = android.R.drawable.ic_popup_reminder;
            } else {
                // In Android 4.4 and older, colorful icons are allowed.
                // So let's use the application main icon by default.
                icon = context.getApplicationInfo().icon;
            }
        }

        return icon;
    }

    int getLargeIcon(final Resources res, final String packageName) {
        return res.getIdentifier(this.id + "_large", "drawable", packageName);
    }

    int getSoundId(final Resources res, final String packageName) {
        return getSoundId(res, packageName, this.id);
    }

    static String getSoundUri(final String packageName, final String profileId, final int soundId) {
        if (soundId != 0) {
            return ContentResolver.SCHEME_ANDROID_RESOURCE + "://" + packageName + "/raw/" + profileId;
        } else {
            return null;
        }
    }
}
