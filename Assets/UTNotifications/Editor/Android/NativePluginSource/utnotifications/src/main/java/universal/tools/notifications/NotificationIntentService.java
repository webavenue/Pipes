package universal.tools.notifications;

import android.app.Activity;
import android.app.IntentService;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.net.Uri;
import android.os.Handler;
import android.os.Looper;
import android.util.AndroidRuntimeException;

import com.unity3d.player.UnityPlayer;

public class NotificationIntentService extends IntentService {
    public NotificationIntentService() {
        super("NotificationIntentService");
    }

    @Override
    protected void onHandleIntent(final Intent intent) {
        if (intent == null) {
            return;
        }

        try {
            final Context context = this.getApplicationContext();
            Manager.setIntent(intent);
            if (intent.hasExtra(Manager.KEY_ID) && intent.hasExtra(Manager.KEY_BUTTON_INDEX)) {
                // Clicks on buttons don't hide notifications automatically, we should help it a little.
                final int id = intent.getIntExtra(Manager.KEY_ID, 0);

                // Android notification panel gets REALLY buggy when hiding a notification directly when clicking on its button.
                // So let's inject a pause with Looper.getMainLooper():
                new Handler(Looper.getMainLooper()).post(new Runnable() {
                    public void run() {
                    Manager.hideNotification(context, id);
                    }
                });
            }

            if (intent.getExtras() != null && intent.getExtras().containsKey(Manager.KEY_OPEN_URL)) {
                // Open specified URL on click
                try {
                    String url = intent.getExtras().getString(Manager.KEY_OPEN_URL);
                    assert url != null;
                    if (!url.startsWith("http://") && !url.startsWith("https://")) {
                        url = "http://" + url;
                    }

                    Intent browserIntent = new Intent(Intent.ACTION_VIEW, Uri.parse(url));
                    browserIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                    startActivity(browserIntent);

                    return;
                } catch (Throwable e) {
                    e.printStackTrace();
                }
            }

            final Intent activityIntent;
            final Activity currentActivity = UnityPlayer.currentActivity;
            if (currentActivity != null) {
                // Show the current activity if any
                activityIntent = new Intent(currentActivity, currentActivity.getClass());
                activityIntent.setFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT);
                try {
                    startActivity(activityIntent);
                } catch (AndroidRuntimeException e) {
                    activityIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                    startActivity(activityIntent);
                }
            } else {
                // Show the application main activity otherwise
                final PackageManager packageManager = context.getPackageManager();
                activityIntent = packageManager.getLaunchIntentForPackage(context.getPackageName());
                assert activityIntent != null;
                activityIntent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP
                                        | Intent.FLAG_ACTIVITY_NEW_TASK
                                        | Intent.FLAG_ACTIVITY_RESET_TASK_IF_NEEDED);
                activityIntent.addCategory(Intent.CATEGORY_LAUNCHER);
                startActivity(activityIntent);
            }
        } catch (Throwable e) {
            e.printStackTrace();
        }
    }
}
