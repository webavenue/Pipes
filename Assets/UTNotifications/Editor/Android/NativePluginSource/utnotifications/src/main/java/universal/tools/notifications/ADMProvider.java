package universal.tools.notifications;

import android.content.Context;

import com.amazon.device.messaging.ADM;

import java.lang.ref.WeakReference;

public class ADMProvider implements IPushNotificationsProvider {
    static final String Name = "ADM";
    private final WeakReference<Context> context;

    ADMProvider(final Context context) {
        this.context = new WeakReference<>(context);
    }

    static boolean isAvailable() {
        try {
            Class.forName("com.amazon.device.messaging.ADM");
            return true;
        } catch (ClassNotFoundException e) {
            return false;
        }
    }

    @Override
    public void enable() {
        final Context context = this.context.get();
        if (context != null) {
            try {
                final ADM adm = new ADM(context);
                final String registrationId = adm.getRegistrationId();

                if (registrationId == null) {
                    adm.startRegister();
                } else {
                    Manager.onPushRegistered(Name, registrationId);
                }
            } catch (final SecurityException e) {
                Manager.onPushRegistrationFailed(e.toString());
            }
        }
    }

    @Override
    public void disable() {
        final Context context = this.context.get();
        if (context != null) {
            new ADM(context).startUnregister();
        }
    }

    @Override
    public void subscribeToTopic(final String topic) {
        // Not supported
    }

    @Override
    public void unsubscribeFromTopic(final String topic) {
        // Not supported
    }
}
