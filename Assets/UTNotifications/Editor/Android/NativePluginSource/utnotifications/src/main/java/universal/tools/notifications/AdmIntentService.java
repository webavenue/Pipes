package universal.tools.notifications;

import android.content.Intent;

import com.amazon.device.messaging.ADMMessageHandlerBase;

public class AdmIntentService extends ADMMessageHandlerBase {
    public AdmIntentService() {
        super("AdmIntentService");
    }

    @Override
    protected void onRegistered(final String newRegistrationId) {
        Manager.onPushRegistered(ADMProvider.Name, newRegistrationId);
    }

    @Override
    protected void onUnregistered(final String registrationId) {
    }

    @Override
    protected void onRegistrationError(final String errorId) {
        Manager.onPushRegistrationFailed(errorId);
    }

    @Override
    protected void onMessage(final Intent intent) {
        Manager.postPushNotification(getApplicationContext(), ADMProvider.Name, intent.getExtras());
    }
}
