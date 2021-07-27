using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AdManager : MonoBehaviour
{
    public static AdManager instance;


    public enum rewardedVideoAdType
    {
        extraMoves,
        extraGifts,
        getHeart
    }

    public enum interestitialAdType
    {
        After_win
    }

    public rewardedVideoAdType TheRewardedAdType { get; private set; }

    public bool AppLovinIsInit { get; private set; }

    [HideInInspector]
    public UIManager uiManager;
    [HideInInspector]
    public StarChest starChest;
    [HideInInspector]
    public Menu menu;

    [HideInInspector]
    public bool bannerAdReady = false;

    [HideInInspector]
    public bool interestitialAdReady = false;

    [HideInInspector]
    public bool rewardedAdReady = false;

    [HideInInspector]
    public bool rewardedVideoShown = false;


    private static float timeOfAdStart;
    private static int lastInterstitialTime;
    private static int thisInterstitialTime;
    private int interstitialRetryAttempt;

    private string applovinString = "applovin";
    private string afterWinString = "after_win";
    private string interstitialPlacementString;
    private bool userIsRewarded;
    private int rewardedRetryAttempt;

#if UNITY_ANDROID
    private string InterstitialAdUnitId = "813d079ace61c86d";

    private string RewardedAdUnitId = "b2176b7eb288f064";

    private string BannerAdUnitId = "92b57503f478b8ad";
#elif UNITY_IOS
    private string InterstitialAdUnitId = "1f2b09ddc970a7f2";

    private string RewardedAdUnitId = "2114c782f8358e9d";

    private string BannerAdUnitId = "3cad9234d9f42069";
#endif

    private Action afterInterstitial;

    void Awake()
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

    public void ApplovinInit(bool userConsent)
    {

        // AdMob Keys
        //Android: ca-app-pub-4062760191449557~8727672726
        //iOS: ca-app-pub-4062760191449557~1954897738

        //Appodeal keys
        //Android : 7444e044a44e8b50b9fe0477ba2c49354710f5ebd6c96c8d
        //iOS : a204a896f817c88b420987e962d697de595ce5dd905c512e

        //Pipes
        //Android
        //Applovin Ad Units
        //Interstitials: 813d079ace61c86d
        //Rewarded: b2176b7eb288f064
        //Banner: 92b57503f478b8ad
        //AdMob App ID: ca - app - pub - 4062760191449557~9384699452
        //iOS
        //Applovin Ad Units
        //Interstitials: 1f2b09ddc970a7f2
        //Rewarded: 2114c782f8358e9d
        //Banner: 3cad9234d9f42069
        //AdMob App ID: ca - app - pub - 4062760191449557~2356256677

        string ApplovinKey = "QNtTl8ZSvEu8tNsG5kODENTcTOHpKyslKkD3tnVn0Nvumm8rTX3hwbJN-aBIX21smFvd0OO1nIvW31MWZPDT_X";
        
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) => {
            // AppLovin SDK is initialized, start loading ads
            AppLovinIsInit = true;

            MaxSdk.SetHasUserConsent(userConsent);

#if (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
                if (MaxSdkUtils.CompareVersions(UnityEngine.iOS.Device.systemVersion, "14.5") != MaxSdkUtils.VersionComparisonResult.Lesser)
                {
                    AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(sdkConfiguration.AppTrackingStatus == MaxSdkBase.AppTrackingStatus.Authorized);
                }
#endif

            InitializeInterstitialAds();
            InitializeRewardedAds();
            InitializeBannerAds();
        };

        MaxSdk.SetSdkKey(ApplovinKey);
        MaxSdk.InitializeSdk();

        lastInterstitialTime = (int)Time.time;
    }

    public void InitializeInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;

        // Load the first interstitial
        LoadInterstitialIfEnabledAndNeeded();
    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(InterstitialAdUnitId);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

        // Reset retry attempt
        interstitialRetryAttempt = 0;

        interestitialAdReady = true;
        AnalyticsManager.instance.LogInterstitialLoaded(applovinString, "interstitial");
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

        interstitialRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));

        Invoke("LoadInterstitial", (float)retryDelay);
        AnalyticsManager.instance.LogInterstitialFailed(applovinString, "interstitial");
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {

    }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        LoadInterstitial();
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        AnalyticsManager.instance.LogInterstitialClicked(applovinString, interstitialPlacementString);
    }

    private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad.
        interestitialAdReady = false;
        ResetInterstitialConditions();
        LoadInterstitialIfEnabledAndNeeded();
        if (afterInterstitial != null)
            afterInterstitial.Invoke();
        AnalyticsManager.instance.LogInterstitialShow(applovinString, interstitialPlacementString);
    }

    public void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        //MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
        //MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        // Load the first rewarded ad
        LoadRewardedIfEnabledAndNeeded();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(RewardedAdUnitId);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

        // Reset retry attempt
        rewardedRetryAttempt = 0;

        rewardedAdReady = true;
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

        rewardedRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, rewardedRetryAttempt));

        Invoke("LoadRewardedAd", (float)retryDelay);
        AnalyticsManager.instance.LogVideoAdFailed(applovinString, TheRewardedAdType.ToString());
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        LoadRewardedAd();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        AnalyticsManager.instance.LogVideoAdClicked(applovinString, TheRewardedAdType.ToString());
    }

    private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        rewardedAdReady = false;
        LoadRewardedIfEnabledAndNeeded();
        ActuallyGiveTheRewardToTheUser();
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // The rewarded ad displayed and the user should receive the reward.

        userIsRewarded = true;
    }

    private void ActuallyGiveTheRewardToTheUser()
    {
        if (TheRewardedAdType == rewardedVideoAdType.extraMoves)
        {
            if (userIsRewarded)
            {
                uiManager.LevelFailContinue();
                AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.extra_moves_earned_with_ads);
            }
            else
            {
                uiManager.LevelFailClose(false);
                AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.extra_moves_with_ads_canceled);
            }
        }
        else if (TheRewardedAdType == rewardedVideoAdType.extraGifts)
        {
            if (userIsRewarded)
            {
                starChest.AfterAdsExtraGifts();
            }
            else
            {
                starChest.ExtraGiftsVideoCanceled();
            }
        }
        else if (TheRewardedAdType == rewardedVideoAdType.getHeart)
        {
            if (userIsRewarded)
            {
                menu.GetOneHeart();
            }
            else
            {
                //menu.CloseBuyHearts();
            }
        }
        long elapsedTime = (long)(Time.unscaledTime - timeOfAdStart);

        AnalyticsManager.instance.LogVideoAdRewarded(applovinString, TheRewardedAdType.ToString(), elapsedTime);
        Music.instance.UnmuteMusic();
    }

    public bool isRewardedVideoAvailable()
    {
#if UNITY_EDITOR
        return true;
#else
        return rewardedAdReady;
#endif
    }

    public void ResetInterstitialConditions()
    {
        lastInterstitialTime = (int)Time.time;
    }

    private void ActuallyShowInterstitialAd(interestitialAdType adType)
    {
        if (MaxSdk.IsInterstitialReady(InterstitialAdUnitId))
        {
            MaxSdk.ShowInterstitial(InterstitialAdUnitId);
        }
    }

    public void LoadInterstitialIfEnabledAndNeeded()
    {
        if (!interestitialAdReady && !Manager.instance.userHasBoughtNoAds && ConfigManager.instance.GetConfig_InterstitalAdEnabled())
        {
            LoadInterstitial();
        }
    }

    public bool IsItTimeToShowInterstitial(int thisLevelActualIndex, interestitialAdType adType)
    {
        if (interestitialAdReady && !Manager.instance.userHasBoughtNoAds && ConfigManager.instance.GetConfig_InterstitalAdEnabled())
        {
            if (adType == interestitialAdType.After_win)
            {
                int everyYSeconds = ConfigManager.instance.GetConfig_InterstitialYSeconds();
                int levelWIndex = ConfigManager.instance.GetConfig_NoInterstitialBeforeLevelW();
                thisInterstitialTime = (int)Time.time;

                //Debug.Log("ADADAD EveryYSeconds:" + everyYSeconds + " LevelWIndex:" + levelWIndex + " ThisIntTime:" + thisInterstitialTime + " LastIntTime:" + lastInterstitialTime);
                return ((thisLevelActualIndex > levelWIndex) && (thisInterstitialTime - lastInterstitialTime > everyYSeconds));
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void ShowInterstitialAd(interestitialAdType adType, Action afterInterstitial)
    {
        if (adType == interestitialAdType.After_win)
        {
            this.afterInterstitial = afterInterstitial;
            thisInterstitialTime = (int)Time.time;
            interstitialPlacementString = afterWinString;
            ActuallyShowInterstitialAd(interestitialAdType.After_win);
        }
    }

    public void LoadRewardedIfEnabledAndNeeded()
    {
        if (!rewardedAdReady && !Manager.instance.userHasBoughtNoAds)
        {
            LoadRewardedAd();
        }
    }

    private void ActuallyShowRewardedAd()
    {
        if (MaxSdk.IsRewardedAdReady(RewardedAdUnitId))
        {
            MaxSdk.ShowRewardedAd(RewardedAdUnitId);
        }
    }

    public void ShowVideoAdIfAvailable(rewardedVideoAdType adType)
    {
        bool available = isRewardedVideoAvailable();
        //StartCoroutine(uiManager.uiLog("SSV av:" + available.ToString()));
        if (available)
        {
            userIsRewarded = false;
            TheRewardedAdType = adType;
            Music.instance.MuteMusic();
            timeOfAdStart = Time.unscaledTime;
            ActuallyShowRewardedAd();
            rewardedVideoShown = true;
            AnalyticsManager.instance.LogVideoAdShow(applovinString, adType.ToString());
        }
    }

    public void InitializeBannerAds()
    {
        // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
        // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
        MaxSdk.CreateBanner(BannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

        // Set background or background color for banners to be fully functional
        MaxSdk.SetBannerBackgroundColor(BannerAdUnitId, Color.white);
    }

    public void ShowBannerIfEnabled(int thisLevelActualIndex)
    {
        int levelWIndex = ConfigManager.instance.GetConfig_NoInterstitialBeforeLevelW();
        if (!Manager.instance.userHasBoughtNoAds && ConfigManager.instance.GetConfig_BannerAdEnabled() && (thisLevelActualIndex > levelWIndex))
        {
            MaxSdk.ShowBanner(BannerAdUnitId);
            uiManager.ChangeUIForBanner(true);
        }
    }

    public void HideBanner()
    {
        MaxSdk.HideBanner(BannerAdUnitId);
        uiManager.ChangeUIForBanner(false);
    }
}
