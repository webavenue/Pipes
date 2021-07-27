package universal.tools.notifications;

interface IPushNotificationsProvider {
    void enable();

    void disable();

    void subscribeToTopic(final String topic);

    void unsubscribeFromTopic(final String topic);
}
