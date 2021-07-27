package universal.tools.notifications;

import android.content.Context;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.util.Log;

import com.google.firebase.messaging.FirebaseMessagingService;
import com.google.firebase.messaging.RemoteMessage;

import java.util.Map;

public class FCMService extends FirebaseMessagingService {
    @Override
    public void onMessageReceived(final RemoteMessage message) {
        final Bundle bundle = new Bundle();

        final RemoteMessage.Notification fcmNotification = message.getNotification();
        if (fcmNotification != null) {
            Log.w(FCMService.class.getName(), "\"notification\"-FCM message received. Please use \"data\"-only FCM messages with UTNotifications to make sure it works correctly.");

            final SharedPreferences prefs = getApplicationContext().getSharedPreferences(Manager.class.getName(), Context.MODE_PRIVATE);
            if (fcmNotification.getTitle() != null) {
                bundle.putString(Manager.getTitleKey(prefs), fcmNotification.getTitle());
            }

            if (fcmNotification.getBody() != null) {
                bundle.putString(Manager.getTextKey(prefs), fcmNotification.getBody());
            }
        }

        final Map<String, String> data = message.getData();
        if (data != null) {
            for (final Map.Entry<String, String> it : data.entrySet()) {
                bundle.putString(it.getKey(), it.getValue());
            }
        }

        Manager.postPushNotification(this, FCMProvider.Name, bundle);
    }
}
