using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NotifManager : MonoBehaviour
{
    public static NotifManager instance;


    private int notifEnabledStart = 8;
    private int notifEnabledEnd = 22;

    private static readonly string textkey_notif1Title = "Notif1Title";
    private static readonly string textkey_notif1Text = "Notif1Text";
    private static readonly string textkey_notif2Title = "Notif2Title";
    private static readonly string textkey_notif2Text = "Notif2Text";
    private static readonly string textkey_notif3Title = "Notif3Title";
    private static readonly string textkey_notif3Text = "Notif3Text";
    private static readonly string textkey_notif0Title = "Notif0Title";
    private static readonly string textkey_notif0Text = "Notif0Text";

    private UTNotifications.Manager TheNotificationManager;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }

    public void ScheduleLocalNotifs()
    {
        TheNotificationManager = UTNotifications.Manager.Instance;
        TheNotificationManager.OnNotificationClicked += OnNotificationClicked;
        TheNotificationManager.OnInitialized += OnInitialized;
        bool res = TheNotificationManager.Initialize(false);
        //Debug.Log("UTN Init: " + TheNotificationManager.Initialized + res);
    }

    public void OnInitialized()
    {
        //Debug.Log("UTN Is Init: " + TheNotificationManager.Initialized);
        int theHour = DateTime.Now.Hour;
        int firstNotif_diffDays = ConfigManager.instance.GetConfig_NotificationDiffDays(1);
        int secondNotif_diffDays = ConfigManager.instance.GetConfig_NotificationDiffDays(2);
        int thirdNotif_diffDays = ConfigManager.instance.GetConfig_NotificationDiffDays(3);

        if (TheNotificationManager.Initialized)
        {
            TheNotificationManager.CancelAllNotifications();

            if (theHour < notifEnabledStart || theHour > notifEnabledEnd)
                return;


            ScheduleNotificationWithMessage(1, LanguageManager.instance.GetTheTextByKey(textkey_notif1Title),
                                                    LanguageManager.instance.GetTheTextByKey(textkey_notif1Text), DateTime.Now.AddDays(firstNotif_diffDays));
            ScheduleNotificationWithMessage(2, LanguageManager.instance.GetTheTextByKey(textkey_notif2Title),
                                                    LanguageManager.instance.GetTheTextByKey(textkey_notif2Text), DateTime.Now.AddDays(secondNotif_diffDays));
            ScheduleNotificationWithMessage(3, LanguageManager.instance.GetTheTextByKey(textkey_notif3Title),
                                                    LanguageManager.instance.GetTheTextByKey(textkey_notif3Text), DateTime.Now.AddDays(thirdNotif_diffDays));
        }
    }

    private void ScheduleShortTimeLocalNotifs()
    {
        int theHour = DateTime.Now.Hour;
        if (theHour < notifEnabledStart || theHour > notifEnabledEnd)
            return;

        int afterGameNotif_diffMin = ConfigManager.instance.GetConfig_NotificationZeroDiffMinutes();
        if (afterGameNotif_diffMin > 0)
        {
            ScheduleNotificationWithMessage(0, LanguageManager.instance.GetTheTextByKey(textkey_notif0Title),
                                                    LanguageManager.instance.GetTheTextByKey(textkey_notif0Text), DateTime.Now.AddMinutes(afterGameNotif_diffMin));
        }
    }

    private void CancelShortTimeLocalNotifs()
    {
        if(TheNotificationManager != null)
            TheNotificationManager.CancelNotification(0);
    }

    public void ScheduleNotificationWithMessage(int notifId, string title, string text, DateTime fireDate)
    {
        //Debug.Log("UTN Notif Set: " + notifId + "," + title + text + fireDate);
        if (TheNotificationManager != null)
            TheNotificationManager.ScheduleNotification(fireDate, title, text, notifId);
    }

    public void OnNotificationClicked(UTNotifications.ReceivedNotification receivedNotification)
    {
        AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.game_opened_by_notification, ":" + receivedNotification.id.ToString());
        //Debug.Log("UTN Is Logged!" + receivedNotification.id);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            ScheduleShortTimeLocalNotifs();
        }
        else
        {
            CancelShortTimeLocalNotifs();
        }
        //Debug.Log("UTN set 30 min Notif: " + pause);
    }

}
