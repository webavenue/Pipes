using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.RemoteConfig;

public class ConfigManager : MonoBehaviour
{
    public static ConfigManager instance;

    private bool itsSafeToFetch = false;
    private Dictionary<string, object> defaults = new Dictionary<string, object>();
    private Dictionary<string, ConfigValue> fetchedValues = new Dictionary<string, ConfigValue>();

    // ****************** PLEASE DON'T CHANGE THESE VALUES WITHOUT PERMISSION, THEY ARE THE SAME STRINGS THAT IS USED IN FIREBASE CONSOLE.
    private const string bannerAdEnabledKey = "banner_ad_enabled";
    private const string interstitalAdEnabledKey = "interstitial_ad_enabled";
    private const string interstitialAdYSecondsKey = "interstitial_ad_y_seconds";
    private const string noInterstitialBeforeLevelWKey = "no_interstitial_before_level_w";
    private const string noAdsPromptTimerKey = "no_ads_prompt_timer";
    private const string notificationZerothDiffMinutesKey = "notification_zeroth_diff_minutes";
    private const string notificationFirstDiffDaysKey = "notification_first_diff_days";
    private const string notificationSecondDiffDaysKey = "notification_second_diff_days";
    private const string notificationThirdDiffDaysKey = "notification_third_diff_days";
    private const string extraMovesAfterLossKey = "extra_moves_after_loss";
    private const string ratingPopupAfterLevelAKey = "rating_popup_after_level_a";
    private const string ratingPopupAfterLevelBKey = "rating_popup_after_level_b";
    // ****************** PLEASE DON'T CHANGE THESE VALUES WITHOUT PERMISSION, THEY ARE THE SAME STRINGS THAT IS USED IN FIREBASE CONSOLE.

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            // These are the values that are used if we haven't fetched data from the
            // backend yet, or if we ask for values that the backend doesn't have:
            //defaults.Add("propertyname_string", "default local string");
            //defaults.Add("propertyname_int", 1);
            //defaults.Add("propertyname_float", 1.0f);
            //defaults.Add("propertyname_bool", false);
            defaults.Add(bannerAdEnabledKey, false);
            defaults.Add(interstitalAdEnabledKey, false);
            defaults.Add(interstitialAdYSecondsKey, 60);
            defaults.Add(noInterstitialBeforeLevelWKey, 4);
            defaults.Add(noAdsPromptTimerKey, 2f);
            defaults.Add(notificationZerothDiffMinutesKey, 30);
            defaults.Add(notificationFirstDiffDaysKey, 1);
            defaults.Add(notificationSecondDiffDaysKey, 3);
            defaults.Add(notificationThirdDiffDaysKey, 7);
            defaults.Add(extraMovesAfterLossKey, 4);
            defaults.Add(ratingPopupAfterLevelAKey, 10);
            defaults.Add(ratingPopupAfterLevelBKey, 100);
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    async public void Async_initConfigs()
    {
        if (AnalyticsManager.instance.FirebaseIsInit)
        {
            FirebaseRemoteConfig TheRemoteConfig = FirebaseRemoteConfig.DefaultInstance;
            //Debug.Log("fbfbfb Remote Config Started!");
            await TheRemoteConfig.SetDefaultsAsync(defaults).ContinueWith(
                task1 =>
                {
                    // it's now safe to Fetch
                    TheRemoteConfig.FetchAndActivateAsync().ContinueWith(
                        task2 =>
                        {
                            // handle completion
                            fetchedValues = (Dictionary<string, ConfigValue>)TheRemoteConfig.AllValues;
                            itsSafeToFetch = true;
                            DebugLogNewValuesAfterActivation();
                        });
                });
        }
    }

    private void DebugLogNewValuesAfterActivation()
    {
        Debug.Log(bannerAdEnabledKey + " new value: '" + GetConfig_BannerAdEnabled() + "'");
        Debug.Log(interstitalAdEnabledKey + " new value: '" + GetConfig_InterstitalAdEnabled() + "'");
        Debug.Log(interstitialAdYSecondsKey + " new value: '" + GetConfig_InterstitialYSeconds() + "'");
        Debug.Log(noInterstitialBeforeLevelWKey + " new value: '" + GetConfig_NoInterstitialBeforeLevelW() + "'");
        Debug.Log(noAdsPromptTimerKey + " new value: '" + GetConfig_NoAdsPromptTimer() + "'");
        Debug.Log(notificationZerothDiffMinutesKey + " new value: '" + GetConfig_NotificationZeroDiffMinutes() + "'");
        Debug.Log(notificationFirstDiffDaysKey + " new value: '" + GetConfig_NotificationDiffDays(1) + "'");
        Debug.Log(notificationSecondDiffDaysKey + " new value: '" + GetConfig_NotificationDiffDays(2) + "'");
        Debug.Log(notificationThirdDiffDaysKey + " new value: '" + GetConfig_NotificationDiffDays(3) + "'");
        Debug.Log(extraMovesAfterLossKey + " new value: '" + GetConfig_ExtraMovesAfterLoss() + "'");
        Debug.Log(ratingPopupAfterLevelAKey + " new value: '" + GetConfig_RatingPopupAfterWin(1) + "'");
        Debug.Log(ratingPopupAfterLevelBKey + " new value: '" + GetConfig_RatingPopupAfterWin(2) + "'");

    }

    public bool GetConfig_BannerAdEnabled()
    {
        if (AnalyticsManager.instance.FirebaseIsInit && itsSafeToFetch)
        {
            return fetchedValues[bannerAdEnabledKey].BooleanValue;
        }
        else
        {
            return (bool)defaults[bannerAdEnabledKey];
        }
    }

    public bool GetConfig_InterstitalAdEnabled()
    {
        if (AnalyticsManager.instance.FirebaseIsInit && itsSafeToFetch)
        {
            return fetchedValues[interstitalAdEnabledKey].BooleanValue;
        }
        else
        {
            return (bool)defaults[interstitalAdEnabledKey];
        }
    }

    public int GetConfig_InterstitialYSeconds()
    {
        if (AnalyticsManager.instance.FirebaseIsInit && itsSafeToFetch)
        {
            return (int)fetchedValues[interstitialAdYSecondsKey].LongValue;
        }
        else
        {
            return (int)defaults[interstitialAdYSecondsKey];
        }
    }

    public int GetConfig_NoInterstitialBeforeLevelW()
    {
        if (AnalyticsManager.instance.FirebaseIsInit && itsSafeToFetch)
        {
            return (int)fetchedValues[noInterstitialBeforeLevelWKey].LongValue;
        }
        else
        {
            return (int)defaults[noInterstitialBeforeLevelWKey];
        }
    }

    public float GetConfig_NoAdsPromptTimer()
    {
        if (AnalyticsManager.instance.FirebaseIsInit && itsSafeToFetch)
        {
            return (float)fetchedValues[noAdsPromptTimerKey].DoubleValue;
        }
        else
        {
            return (float)defaults[noAdsPromptTimerKey];
        }
    }

    public int GetConfig_NotificationZeroDiffMinutes()
    {
        if (AnalyticsManager.instance.FirebaseIsInit && itsSafeToFetch)
        {
            return (int)fetchedValues[notificationZerothDiffMinutesKey].LongValue;
        }
        else
        {
            return (int)defaults[notificationZerothDiffMinutesKey];
        }
    }

    public int GetConfig_NotificationDiffDays(int index1Or2Or3)
    {
        string key;
        if (index1Or2Or3 == 1)
        {
            key = notificationFirstDiffDaysKey;
        }
        else if (index1Or2Or3 == 2)
        {
            key = notificationSecondDiffDaysKey;
        }
        else if (index1Or2Or3 == 3)
        {
            key = notificationThirdDiffDaysKey;
        }
        else
        {
            return 0;
        }
        if (AnalyticsManager.instance.FirebaseIsInit && itsSafeToFetch)
        {
            return (int)fetchedValues[key].LongValue;
        }
        else
        {
            return (int)defaults[key];
        }
    }

    public int GetConfig_ExtraMovesAfterLoss()
    {
        if (AnalyticsManager.instance.FirebaseIsInit && itsSafeToFetch)
        {
            return (int)fetchedValues[extraMovesAfterLossKey].LongValue;
        }
        else
        {
            return (int)defaults[extraMovesAfterLossKey];
        }
    }

    public int GetConfig_RatingPopupAfterWin(int index)//1 or 2
    {
        string key;
        if (index == 1)
            key = ratingPopupAfterLevelAKey;
        else if (index == 2)
            key = ratingPopupAfterLevelBKey;
        else
            return 0;
        if (AnalyticsManager.instance.FirebaseIsInit && itsSafeToFetch)
        {
            return (int)fetchedValues[key].LongValue;
        }
        else
        {
            return (int)defaults[key];
        }
    }
}
