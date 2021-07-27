using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;
using Firebase;
using Firebase.Analytics;
using Facebook.Unity;


public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager instance;

    public GameObject GAPrefab;

    [HideInInspector] public FirebaseApp FirbaseInstance;
    [HideInInspector] public bool FirebaseIsInit = false;


    int updatesBeforeException = 0;

    float levelStartTime;
    float levelChestStartTime;

    public enum NoAdsPlaces
    {
        GameStore,
        NoAdsPrompt
    }

    public class DesignKeys
    {
        public static readonly string user_login = "UserLoggedIn";
        public static readonly string user_language = "UserLanguage";
        public static readonly string user_language_not_supported = "UserLanguageNotSupported";
        public static readonly string game_opened_time = "GameOpenedTime";
        public static readonly string game_opened_by_notification = "GameOpenedByNotification";

        public static readonly string splash_loading_time = "SplashLoadingTime";

        public static readonly string consent_shown = "ConsentShownToUser";
        public static readonly string consent_accepted = "ConsentAcceptedByUser";
        public static readonly string consent_rejected = "ConsentRejectedByUser";
        public static readonly string consent_link_opened = "ConsentLinkOpenedByUser";

        public static readonly string ftue_start = "FtueStart";
        public static readonly string ftue_tap_on_pipes = "FtueTapOnPipes";
        public static readonly string ftue_use_all_pipes = "FtueUseAllPipes";
        public static readonly string ftue_limited_moves = "FtueLimitedMoves";
        public static readonly string ftue_undo = "FtueUndo";
        public static readonly string ftue_hint = "FtueHint";
        public static readonly string ftue_infinite_moves = "FtueInfiniteMoves";
        public static readonly string ftue_daily_quests = "FtueDailyQuests";
        public static readonly string ftue_collections = "FtueCollections";
        public static readonly string ftue_chapter_selector = "FtueChapterSelector";
        public static readonly string ftue_finish = "FtueFinish";

        public static readonly string chapter_completed = "ChapterCompleted";
        //public static readonly string level_started = "LevelStarted";
        //public static readonly string level_completed = "LevelCompleted";
        public static readonly string level_complete_score = "LevelCompleteScore";
        public static readonly string level_complete_time = "LevelCompleteTime";//Amount of time spent in level
        public static readonly string level_complete_watched = "LevelCompleteWatched";
        public static readonly string level_complete_skipped = "LevelCompleteSkipped";//Amount of time the user has watched Level Complete
        //public static readonly string level_failed = "LevelFailed";
        public static readonly string level_failed_heart_lost = "LevelFailedHeartLost";
        public static readonly string level_failed_board_seen = "LevelFailedBoardSeen";

        public static readonly string extra_moves_earned_with_ads = "ExtraMovesEarnedWithAds";
        public static readonly string extra_moves_with_ads_canceled = "ExtraMovesWithAdsCanceled";
        public static readonly string extra_moves_earned_with_coins = "ExtraMovesEarnedWithCoins";

        public static readonly string ingame_hint = "InGameHint";//Hint Index
        public static readonly string ingame_IM = "InGameIM";
        public static readonly string ingame_undo = "InGameUndo";
        public static readonly string ingame_x_moves_left = "InGameXMovesLeft";
        public static readonly string ingame_settings_opened = "InGameSettingsOpened";
        public static readonly string ingame_music_set_to = "InGameMusicSetTo";
        public static readonly string ingame_sound_set_to = "InGameSoundSetTo";
        public static readonly string ingame_vibrate_set_to = "InGameVibrateSetTo";

        public static readonly string star_chest_opened = "StarChestOpened";
        public static readonly string star_chest_extra_gifts_earned = "StarChestExtraGiftsEarned";
        public static readonly string star_chest_extra_gifts_canceled = "StarChestExtraGiftsCanceled";

        //public static readonly string level_chest_game_started = "LevelChestGameStarted";
        //public static readonly string level_chest_game_finished = "LevelChestGameFinished";
        public static readonly string level_chest_game_time = "LevelChestGameTime";//Amount of time spent in level chest
        public static readonly string level_chest_card_claimed = "LevelChestCardClaimed";

        public static readonly string shop_opened = "ShopOpened";
        public static readonly string no_ads_purchased = "NoAdsPurchased";
        public static readonly string bundleX_purchased = "BundleXPurchased";
        public static readonly string coinsX_purchased = "CoinsXPurchased";

        public static readonly string roadmap_opened = "RoadmapOpened";

        public static readonly string collection_opened = "CollectionOpened";

        public static readonly string quests_opened = "QuestsOpened";
        public static readonly string quest_easy_completed = "QuestsEasyCompleted";
        public static readonly string quest_easy_claimed = "QuestsEasyClaimed";
        public static readonly string quest_normal_completed = "QuestsNormalCompleted";
        public static readonly string quest_normal_claimed = "QuestsNormalClaimed";
        public static readonly string quest_hard_completed = "QuestsHardCompleted";
        public static readonly string quest_hard_claimed = "QuestsHardClaimed";

        public static readonly string settings_opened = "SettingsOpened";
        public static readonly string settings_achievements_opened = "SettingsAchievementsOpened";
        public static readonly string music_set_to = "MusicSetTo";
        public static readonly string sound_set_to = "SoundSetTo";
        public static readonly string vibrate_set_to = "VibrateSetTo";

        public static readonly string interstitial_shown = "InterstitialShown";
        public static readonly string interstitial_clicked = "InterstitialClicked";
        public static readonly string rewarded_shown = "RewardedShown";
        public static readonly string rewarded_clicked = "RewardedClicked";
        public static readonly string rewarded_completed = "RewardedCompleted";

        public static readonly string back_number = "back_";
        public static readonly string spent_minutes_amount = "spent_minutes_";
        public static readonly string clicked_interstetial_number = "clicked_interstetial_";
        public static readonly string view_interstetial_number = "view_interstetial_";
        public static readonly string view_rewarded_number = "view_rewarded_";
        public static readonly string clicked_rewarded_number = "clicked_rewarded_";
    }

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            GameAnalytics.Initialize();
            GameObject ga = Instantiate(GAPrefab);
            ga.name = "GameAnalytics";

            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                //Handle FB.Init
                FB.Init(() =>
                {
                    FB.ActivateApp();
                });
            }


            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    // This is also used for CrashLytics
                    FirbaseInstance = FirebaseApp.DefaultInstance;

                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                    FirebaseIsInit = true;

                    // Initialize The configs too, they also work with Firebase
                    ConfigManager.instance.Async_initConfigs();

                    // Add Firebase Callbacks
                    Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
                    Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
                    // we handle ios uninstalls in notifmanager script!
                }
                else
                {
                    Debug.LogError(string.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            });
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if(FirebaseIsInit)
        //    throwExceptionEvery60Updates();

    }


    // A method that tests your Crashlytics implementation by throwing an
    // exception every 60 frame updates. You should see non-fatal errors in the
    // Firebase console a few minutes after running your app with this method.
    void throwExceptionEvery60Updates()
    {
        if (updatesBeforeException > 0)
        {
            updatesBeforeException--;
        }
        else
        {
            // Set the counter to 60 updates
            updatesBeforeException = 60;

            // Throw an exception to test your Crashlytics implementation
            throw new System.Exception("test exception please ignore");
        }
    }

    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        Debug.Log("Received Registration Token: " + token.Token);
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        Debug.Log("Received a new message from: " + e.Message.From);
    }


    public void LogInterstitialLoaded(string sdkName, string placementName)
    {
        GameAnalytics.NewAdEvent(GAAdAction.Loaded, GAAdType.Interstitial, sdkName, placementName);
    }

    public void LogInterstitialShow(string sdkName, string placementName)
    {
        GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.Interstitial, sdkName, placementName);
        LogDesignEventByKey(DesignKeys.interstitial_shown, ":" + placementName);
        LogNumberOfInterstitialViews();
    }

    public void LogInterstitialClicked(string sdkName, string placementName)
    {
        GameAnalytics.NewAdEvent(GAAdAction.Clicked, GAAdType.Interstitial, sdkName, placementName);
        LogDesignEventByKey(DesignKeys.interstitial_clicked, ":" + placementName);
        LogNumberOfInterstitialClicks();
    }

    public void LogInterstitialFailed(string sdkName, string placementName)
    {
        GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.Interstitial, sdkName, placementName, GAAdError.Unknown);
    }

    public void LogVideoAdShow(string sdkName, string placementName)
    {
        GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.RewardedVideo, sdkName, placementName);
        LogDesignEventByKey(DesignKeys.rewarded_shown, ":" + placementName);
        LogNumberOfRewardedViews();
    }

    public void LogVideoAdRewarded(string sdkName, string placementName, long timeSpent)
    {
        GameAnalytics.NewAdEvent(GAAdAction.RewardReceived, GAAdType.RewardedVideo, sdkName, placementName, timeSpent);
        LogDesignEventByKey(DesignKeys.rewarded_completed, ":" + placementName);
    }

    public void LogVideoAdClicked(string sdkName, string placementName)
    {
        GameAnalytics.NewAdEvent(GAAdAction.Clicked, GAAdType.RewardedVideo, sdkName, placementName);
        LogDesignEventByKey(DesignKeys.rewarded_clicked, ":" + placementName);
        LogNumberOfRewardedClicks();
    }

    public void LogVideoAdFailed(string sdkName, string placementName)
    {
        GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.RewardedVideo, sdkName, placementName, GAAdError.Unknown);
    }

    public void LogNoAdsPurchased(NoAdsPlaces placeOfPurchase, string currency, decimal amountInCents, string productId, string receipt, string transactionID)
    {
#if UNITY_ANDROID
        GooglePurchaseData GPDdata = new GooglePurchaseData(receipt);
        GameAnalytics.NewBusinessEventGooglePlay(currency, (int)amountInCents, "NoAds", productId, placeOfPurchase.ToString(), receipt, GPDdata.inAppDataSignature);
#elif UNITY_IOS
        GameAnalytics.NewBusinessEvent(currency, (int)amountInCents, "NoAds", productId, placeOfPurchase.ToString());
#endif
        if (FirebaseIsInit)
        {
            // Log an event with multiple parameters, passed as a struct:
            Parameter[] PurchaseParameters = {
            new Parameter( FirebaseAnalytics.ParameterCurrency, currency),
            new Parameter( FirebaseAnalytics.ParameterItemId, productId),
            new Parameter( FirebaseAnalytics.ParameterPrice, amountInCents.ToString())
            //,new Parameter( "hit_accuracy", 3.14f)
            };
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventEcommercePurchase, PurchaseParameters);
        }

        LogDesignEventByKey(DesignKeys.no_ads_purchased);
    }

    public void LogBundlePurchased(int bundleIndex, string currency, decimal amountInCents, string productId, string receipt, string transactionID)
    {
#if UNITY_ANDROID
        GooglePurchaseData GPDdata = new GooglePurchaseData(receipt);
        GameAnalytics.NewBusinessEventGooglePlay(currency, (int)amountInCents, "Bundle", productId, "GameStore", receipt, GPDdata.inAppDataSignature);
#elif UNITY_IOS
        GameAnalytics.NewBusinessEvent(currency, (int)amountInCents, "Bundle", productId, "GameStore");
#endif
        if (FirebaseIsInit)
        {
            // Log an event with multiple parameters, passed as a struct:
            Parameter[] PurchaseParameters = {
            new Parameter( FirebaseAnalytics.ParameterCurrency, currency),
            new Parameter( FirebaseAnalytics.ParameterItemId, productId),
            new Parameter( FirebaseAnalytics.ParameterPrice, amountInCents.ToString())
            //,new Parameter( "hit_accuracy", 3.14f)
            };
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventEcommercePurchase, PurchaseParameters);
        }
        GameAnalytics.NewDesignEvent(DesignKeys.bundleX_purchased + ":" + bundleIndex.ToString());
    }

    public void LogCoinsPurchased(int coinsIndex, string currency, decimal amountInCents, string productId, string receipt, string transactionID)
    {
#if UNITY_ANDROID
        GooglePurchaseData GPDdata = new GooglePurchaseData(receipt);
        GameAnalytics.NewBusinessEventGooglePlay(currency, (int)amountInCents, "Coins", productId, "GameStore", receipt, GPDdata.inAppDataSignature);
#elif UNITY_IOS
        GameAnalytics.NewBusinessEvent(currency, (int)amountInCents, "Coins", productId, "GameStore");
#endif
        if (FirebaseIsInit)
        {
            // Log an event with multiple parameters, passed as a struct:
            Parameter[] PurchaseParameters = {
            new Parameter( FirebaseAnalytics.ParameterCurrency, currency),
            new Parameter( FirebaseAnalytics.ParameterItemId, productId),
            new Parameter( FirebaseAnalytics.ParameterPrice, amountInCents.ToString())
            //,new Parameter( "hit_accuracy", 3.14f)
            };
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventEcommercePurchase, PurchaseParameters);
        }
        GameAnalytics.NewDesignEvent(DesignKeys.coinsX_purchased + ":" + coinsIndex.ToString());
    }

    public void LogUserLanguage(bool supported)
    {
        if (supported)
            GameAnalytics.NewDesignEvent(DesignKeys.user_language + ":" + LanguageManager.instance.userLanguage);
        else
            GameAnalytics.NewDesignEvent(DesignKeys.user_language_not_supported + ":" + LanguageManager.instance.userLanguage);
    }

    public void LogUserLogin()
    {
        GameAnalytics.NewDesignEvent(DesignKeys.user_login);
        if (FirebaseIsInit)
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
    }

    public void LogNumberOfSessions()
    {
        Manager.instance.AnalyticsNumberOfSessions++;
        int[] toLogNumbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 15, 20, 25, 30, 40, 50, 60, 70, 80, 90, 100 };
        for (int i = 0; i < toLogNumbers.Length; i++)
        {
            if (Manager.instance.AnalyticsNumberOfSessions == toLogNumbers[i])
            {
                LogDesignEventByKey(DesignKeys.back_number + ":" + Manager.instance.AnalyticsNumberOfSessions);
                return;
            }
        }
        if (Manager.instance.AnalyticsNumberOfSessions % 100 == 0)
        {
            LogDesignEventByKey(DesignKeys.back_number + ":" + Manager.instance.AnalyticsNumberOfSessions);
        }
    }

    public void LogMinutesOfGamePlay(float AddedMinutes)
    {
        Manager.instance.AnalyticsMinutesOfGamePlay += AddedMinutes;
        int RoundedMinutes = (int)Manager.instance.AnalyticsMinutesOfGamePlay;

        int[] toLogNumbers = { 2, 5, 10, 15, 20, 25, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000 };
        if ((RoundedMinutes > 1000) && (RoundedMinutes % 500 == 0))
        {
            LogDesignEventByKey(DesignKeys.spent_minutes_amount + ":" + RoundedMinutes);
        }
        else
        {
            for (int i = toLogNumbers.Length -1; i >= 0; i--)
            {
                if (RoundedMinutes == toLogNumbers[i])
                {
                    LogDesignEventByKey(DesignKeys.spent_minutes_amount + ":" + RoundedMinutes);
                    return;
                }
            }
        }
    }

    public void LogNumberOfInterstitialViews()
    {
        Manager.instance.AnalyticsNumberOfInterstitialShown++;
        int[] toLogNumbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
        for (int i = 0; i < toLogNumbers.Length; i++)
        {
            if (Manager.instance.AnalyticsNumberOfInterstitialShown == toLogNumbers[i])
            {
                LogDesignEventByKey(DesignKeys.view_interstetial_number + ":" + Manager.instance.AnalyticsNumberOfInterstitialShown);
                return;
            }
        }
        if (Manager.instance.AnalyticsNumberOfInterstitialShown % 50 == 0)
        {
            LogDesignEventByKey(DesignKeys.view_interstetial_number + ":" + Manager.instance.AnalyticsNumberOfInterstitialShown);
        }
    }

    public void LogNumberOfInterstitialClicks()
    {
        Manager.instance.AnalyticsNumberOfInterstitialClicked++;
        int[] toLogNumbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
        for (int i = 0; i < toLogNumbers.Length; i++)
        {
            if (Manager.instance.AnalyticsNumberOfInterstitialClicked == toLogNumbers[i])
            {
                LogDesignEventByKey(DesignKeys.clicked_interstetial_number + ":" + Manager.instance.AnalyticsNumberOfInterstitialClicked);
                return;
            }
        }
        if (Manager.instance.AnalyticsNumberOfInterstitialClicked % 50 == 0)
        {
            LogDesignEventByKey(DesignKeys.clicked_interstetial_number + ":" + Manager.instance.AnalyticsNumberOfInterstitialClicked);
        }
    }

    public void LogNumberOfRewardedViews()
    {
        Manager.instance.AnalyticsNumberOfRewardedVideoShown++;
        int[] toLogNumbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 15, 20, 25, 30, 40, 50, 60, 70, 80, 90, 100, 150 };
        for (int i = 0; i < toLogNumbers.Length; i++)
        {
            if (Manager.instance.AnalyticsNumberOfRewardedVideoShown == toLogNumbers[i])
            {
                LogDesignEventByKey(DesignKeys.view_rewarded_number + ":" + Manager.instance.AnalyticsNumberOfRewardedVideoShown);
                return;
            }
        }
        if (Manager.instance.AnalyticsNumberOfRewardedVideoShown % 100 == 0)
        {
            LogDesignEventByKey(DesignKeys.view_rewarded_number + ":" + Manager.instance.AnalyticsNumberOfRewardedVideoShown);
        }
    }

    public void LogNumberOfRewardedClicks()
    {
        Manager.instance.AnalyticsNumberOfRewardedVideoClicked++;
        int[] toLogNumbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 15, 20, 25, 30, 40, 50, 60, 70, 80, 90, 100, 150 };
        for (int i = 0; i < toLogNumbers.Length; i++)
        {
            if (Manager.instance.AnalyticsNumberOfRewardedVideoClicked == toLogNumbers[i])
            {
                LogDesignEventByKey(DesignKeys.clicked_rewarded_number + ":" + Manager.instance.AnalyticsNumberOfRewardedVideoClicked);
                return;
            }
        }
        if (Manager.instance.AnalyticsNumberOfRewardedVideoClicked % 100 == 0)
        {
            LogDesignEventByKey(DesignKeys.clicked_rewarded_number + ":" + Manager.instance.AnalyticsNumberOfRewardedVideoClicked);
        }
    }

    public void LogLevelStarted(int chapterIndex, int levelIndex)
    {
        string chapterKey = Chapters.instance.GetChapterKey(chapterIndex);
        string theLevelIndex = levelIndex.ToString();
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, chapterKey, theLevelIndex);
        levelStartTime = Time.time;
    }

    public void LogLevelCompleted(int chapterIndex, int levelIndex, int stars, int score)
    {
        string chapterKey = Chapters.instance.GetChapterKey(chapterIndex);
        string theLevelIndex = levelIndex.ToString();
        float timeInSeconds = Time.time - levelStartTime;
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, chapterKey, theLevelIndex, stars);

        if (FirebaseIsInit)
        {
            // Log an event with multiple parameters, passed as a struct:
            Parameter[] LevelCompleteParameters = {
                new Parameter( FirebaseAnalytics.ParameterOrigin, chapterKey),
                new Parameter( FirebaseAnalytics.ParameterLevelName, theLevelIndex),
                new Parameter( FirebaseAnalytics.ParameterScore, stars)
            //,new Parameter( "hit_accuracy", 3.14f)
            };
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelEnd, LevelCompleteParameters);
        }
        LogDesignEventByKey(DesignKeys.level_complete_score, "", score);
        LogDesignEventByKey(DesignKeys.level_complete_time, "", timeInSeconds);
        LogMinutesOfGamePlay(timeInSeconds / 60f);
    }

    public void LogLevelFailed(int chapterIndex, int levelIndex)
    {
        string chapterKey = Chapters.instance.GetChapterKey(chapterIndex);
        string theLevelIndex = levelIndex.ToString();
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, chapterKey, theLevelIndex);
    }

    public void LogChestGameStarted(int chapterIndex, int chestIndex)
    {
        string chapterKey = Chapters.instance.GetChapterKey(chapterIndex);
        string thechapterIndex = "c" + chapterIndex.ToString() + chestIndex.ToString();
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, chapterKey, thechapterIndex);
        levelChestStartTime = Time.time;
    }

    public void LogChestGameCompleted(int chapterIndex, int chestIndex)
    {
        string chapterKey = Chapters.instance.GetChapterKey(chapterIndex);
        string thechapterIndex = "c" + chapterIndex.ToString() + chestIndex.ToString();
        int stars = 3;
        float timeInSeconds = Time.time - levelChestStartTime;
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, chapterKey, thechapterIndex, stars);

        if (FirebaseIsInit)
        {
            // Log an event with multiple parameters, passed as a struct:
            Parameter[] LevelCompleteParameters = {
                new Parameter( FirebaseAnalytics.ParameterOrigin, chapterKey),
                new Parameter( FirebaseAnalytics.ParameterLevelName, thechapterIndex),
                new Parameter( FirebaseAnalytics.ParameterScore, stars)
            //,new Parameter( "hit_accuracy", 3.14f)
            };
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelEnd, LevelCompleteParameters);
        }
        LogDesignEventByKey(DesignKeys.level_chest_game_time, "", timeInSeconds);
        if(chestIndex == 2)
            GameAnalytics.NewDesignEvent(DesignKeys.chapter_completed + ":" + chapterKey);
    }

    public void LogDesignEventByKey(string key, string GAParams = "", float GAValue = -1f)
    {
        if (GAValue != -1f)
            GameAnalytics.NewDesignEvent(key + GAParams, GAValue);
        else
            GameAnalytics.NewDesignEvent(key + GAParams);
        if (FirebaseIsInit)
            FirebaseAnalytics.LogEvent(key);
        //Debug.Log("AnalyticsKey=" + key + ",GAparams=" + GAParams + "Value=" + GAValue);
    }

    private class GooglePurchaseData
    {
        // INAPP_PURCHASE_DATA
        public string inAppPurchaseData;
        // INAPP_DATA_SIGNATURE
        public string inAppDataSignature;

        [System.Serializable]
        private struct GooglePurchaseReceipt
        {
            public string Payload;
        }
        [System.Serializable]
        private struct GooglePurchasePayload
        {
            public string json;
            public string signature;
        }

        public GooglePurchaseData(string receipt)
        {
            try
            {
                var purchaseReceipt = JsonUtility.FromJson<GooglePurchaseReceipt>(receipt);
                var purchasePayload = JsonUtility.FromJson<GooglePurchasePayload>(purchaseReceipt.Payload);
                inAppPurchaseData = purchasePayload.json;
                inAppDataSignature = purchasePayload.signature;

                Debug.Log("receipt json: " + purchasePayload.json);
                Debug.Log("receipt signature: " + purchasePayload.signature);
            }
            catch
            {
                Debug.Log("Could not parse receipt: " + receipt);
                inAppPurchaseData = "";
                inAppDataSignature = "";
            }
        }
    }
}
