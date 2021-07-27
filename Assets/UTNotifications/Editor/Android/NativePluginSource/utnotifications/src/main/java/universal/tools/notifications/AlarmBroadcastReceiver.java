package universal.tools.notifications;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;

import java.net.URLDecoder;
import java.util.List;

public class AlarmBroadcastReceiver extends BroadcastReceiver {
    public void onReceive(final Context context, final Intent intent) {
        try {
            final String notificationJson = intent.getStringExtra(Manager.KEY_NOTIFICATION);
            if (notificationJson != null) {
                Manager.postNotification(context, new UTNotification(notificationJson));
            } else {
                // Pre-1.7 format
                final String KEY_ID = "__UT_ID";
                final String KEY_USER_DATA = "__UT_USER_DATA";
                final String KEY_TITLE = "__UT_TITLE";
                final String KEY_TEXT = "__UT_TEXT";
                final String KEY_NOTIFICATION_PROFILE = "__UT_NOTIFICATION_PROFILE";
                final String KEY_BADGE_NUMBER = "__UT_BADGE_NUMBER";

                Bundle userData = intent.getParcelableExtra(KEY_USER_DATA);
                String title = URLDecoder.decode(userData.getString(KEY_TITLE), "UTF-8");
                String text = URLDecoder.decode(userData.getString(KEY_TEXT), "UTF-8");
                int id = intent.getIntExtra(KEY_ID, 1);
                String profile = userData.getString(KEY_NOTIFICATION_PROFILE);
                int badge = userData.getInt(KEY_BADGE_NUMBER, -1);

                Manager.postNotification(context, new UTNotification(0, 0, title, text, id, Manager.bundleToMap(userData), profile, badge, (List<UTNotification.Button>) null));
            }
        } catch (final Throwable e) {
            e.printStackTrace();
        }
    }
}