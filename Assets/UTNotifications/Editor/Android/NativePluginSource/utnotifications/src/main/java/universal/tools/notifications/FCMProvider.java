package universal.tools.notifications;

import android.content.Context;
import android.util.Log;

import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.GoogleApiAvailability;
import com.google.android.gms.tasks.OnFailureListener;
import com.google.android.gms.tasks.OnSuccessListener;
import com.google.android.gms.tasks.Task;
import com.google.firebase.iid.FirebaseInstanceId;
import com.google.firebase.iid.InstanceIdResult;
import com.google.firebase.messaging.FirebaseMessaging;
import com.unity3d.player.UnityPlayer;

import java.io.IOException;

public class FCMProvider implements IPushNotificationsProvider {
    static final String Name = "FCM";

    static boolean isAvailable(Context context, boolean allowUpdatingGooglePlayIfRequired) {
        try {
            final GoogleApiAvailability googleApiAvailability = GoogleApiAvailability.getInstance();

            final int result = googleApiAvailability.isGooglePlayServicesAvailable(context);
            if (result != ConnectionResult.SUCCESS && allowUpdatingGooglePlayIfRequired) {
                if (googleApiAvailability.isUserResolvableError(result) && result != ConnectionResult.SERVICE_MISSING && result != ConnectionResult.SERVICE_INVALID) {
                    googleApiAvailability.getErrorDialog(UnityPlayer.currentActivity, result, 0).show();
                }

                // May get available on next run as a result.
                return false;
            }

            return true;
        } catch (final Throwable e) {
            return false;
        }
    }

    @Override
    public void enable() {
        FirebaseMessaging.getInstance().setAutoInitEnabled(true);

        final Task<InstanceIdResult> task = FirebaseInstanceId.getInstance().getInstanceId();
        task.addOnSuccessListener(
                new OnSuccessListener<InstanceIdResult>() {
                    @Override
                    public void onSuccess(final InstanceIdResult instanceIdResult) {
                        Manager.onPushRegistered(FCMProvider.Name, instanceIdResult.getToken());
                    }
                }
        );

        task.addOnFailureListener(new OnFailureListener() {
            @Override
            public void onFailure(final Exception e) {
                Manager.onPushRegistrationFailed(e.toString());
            }
        });
    }

    @Override
    public void disable() {
        FirebaseMessaging.getInstance().setAutoInitEnabled(false);
        new Thread(new Runnable() {
            @Override
            public void run() {
                try {
                    FirebaseInstanceId.getInstance().deleteInstanceId();
                } catch (final IOException e) {
                    Log.e(FCMProvider.class.getName(), "Failed to disable", e);
                }
            }
        }).start();
    }

    @Override
    public void subscribeToTopic(final String topic) {
        FirebaseMessaging.getInstance().subscribeToTopic(topic);
    }

    @Override
    public void unsubscribeFromTopic(final String topic) {
        FirebaseMessaging.getInstance().unsubscribeFromTopic(topic);
    }
}
