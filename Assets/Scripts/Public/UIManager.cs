using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UIManager : MonoBehaviour
{
    public float pauseTime;
    public float resumeTime;
    public float starTime;
    public float starWaitTime;
    public float settingsOpenTime;
    public float settingsCloseTime;
    public float settingsRotateDegrees;

    public GameObject PausePanelCircle;
    public Text LevelNumberText;
    public Text LevelEndLevelNumberText;
    public Text LevelEndScoreText;
    public GameObject SettingsButton;
    public GameObject MusicButton;
    public GameObject SoundButton;
    public GameObject VibrateButton;
    public GameObject ShareButton;
    public GameObject ContinueButton;
    public GameObject RetryButton;
    public GameObject BackBanner;
    public float BottomBarYBannerOn;
    public float BottomBarYBannerOff;
    [Header("EndLevel")]
    public GameObject skipButton;
    public Text skipText;
    public GameObject endPanel;
    public GameObject Circle;
    public GameObject[] Stars;
    public Vector3 starsStartPosition;
    [Header("LevelFailed")]
    public GameObject levelFail;
    public GameObject levelFailedBack;
    public GameObject levelFailedContinueButton;
    public GameObject levelFailShortButtonL;
    public GameObject levelFailShortButtonR;
    public GameObject playOnMoney;
    public GameObject levelFailedCross;
    public Text outOfMovesText;
    public Text getXMovesText;
    public Text levelFailedLeftButtonText;
    public Text levelFailedRightButtonText;
    public Text playOnText;
    public Text playOnMoneyText;
    public int firstRevivePrice;
    public GameObject showBoardGO;
    public Text showBoardText;
    public Sprite showBoardPanel;
    public Text plusFiveText;
    [Header("BrokenHeart")]
    public GameObject brokenHeartBack;
    public GameObject fiveHearts;
    public Image brokenHeartImage;
    public Text brokenHeartLevelNumberText;
    public Text brokenHeartText;
    public Text tryAgainText;
    public Sprite infiniteHeartSprite;
    public Sprite brokenHeartSprite;
    [Header("NoHeart")]
    public GameObject noHeartBack;
    public Text noHeartTitleText;
    public Text noHeartTimerTitleText;
    public Text noHeartTimerText;
    public Text noHeartNumText;
    public Text buyFullHeartsPriceText;
    public Text buyFullHeartsText;
    public Text zeroNumtext;
    [Header("BuyPowerUps")]
    public GameObject buyPowerUps;
    public GameObject buyPowerUpsBack;
    public Image buyPowerUpsImage;
    public int numOfBoughtPowerUps;
    public int powerUpsPrice;
    public Text powerUpsDescText;
    public Text powerUpsPriceText;
    public Text numOfBoughtPowerUpsText;
    [Header("InLevel")]
    public GameObject TopBar;
    public GameObject BottomBar;
    public Button undoBtn;
    public Button InfiniteMoveButton;
    public Text InfiniteMoveItemNumber;
    public Button hintBtn;
    public Text hintNumber;
    public Text NumOfMoves;
    public Image undoImage;
    public Image hintImage;
    public Image infiniteMovesImage;
    public Sprite undoSprite;
    public Sprite hintSprite;
    public Sprite infiniteMovesSprite;
    public Sprite lockSprite;
    public GameObject backButton;
    [Header("FTUE")]
    public GameObject BottomBarFTUE;
    public Button undoBtnFTUE;
    public Button InfiniteMoveButtonFTUE;
    public Text InfiniteMoveItemNumberFTUE;
    public Button hintBtnFTUE;
    public Text hintNumberFTUE;
    public GameObject movesNumberFTUE;
    public Text NumOfMovesFTUE;
    public GameObject firstTileFTUE;
    public string[] unlockLevelNum;
    public string[] unlockLevelNumRtl;
    public int lockClickTextTime;
    [Header("Online Progress")]
    public Image[] progressSectors;
    public GameObject offStar;
    public GameObject oneStar;
    public GameObject twoStars;
    public GameObject threeStars;
    [Tooltip("time takes filling hole of a sector")]
    public float fillingProgressTime;
    [Tooltip("time takes for filling hole of a sector")]
    public float fillingProgressTimeAfterWin;
    public float delayBetweenSectorsFilling;
    [Header("ScoreAct")]
    public float totalScoreTime;
    [Tooltip("it should not be round number to create scores steps which are not round!")]
    public int numOfAddingScoreSteps;
    [Header("Star Chest")]
    public GameObject StarChestFather;
    public GameObject addedFirstStar;
    public GameObject addedSecondStar;
    public GameObject addedThirdStar;
    public float addedStarMoveTime;
    public Transform starChestTransform;
    public StarChest starChestScript;
    [Header("No Ads Prompt")]
    public GameObject noAdsBack;
    public Text noAdsTitle;
    public GameObject NoAdsbar2;
    public GameObject NoAdsButton;
    [Header("Back To Menu Prompt")]
    public GameObject BackToMenuPrompt;
    public Text AreYouSureText;
    public Text LoseHeartText;
    public Text QuitText;
    public Image LoseHeartImage;
    [Header("InGameShop")]
    public GameObject inGameShopObj;
    public Shop inGameShopScript;
    [Header("Rating")]
    public GameObject ratingObj;
    public GameObject rateUs;
    public GameObject ratingCross;
    public Text ratingText;
    public Text rateUsText;
    [Header("Miscellaneous")]
    public LevelManger levelManger;
    public Sprite levelEndPanel;
    public Sprite endCircle;
    public Sprite MusicOn;
    public Sprite MusicOff;
    public Sprite SoundOn;
    public Sprite SoundOff;
    public Sprite VibrateOn;
    public Sprite VibrateOff;
    public Vector3 circleFinalScale;

    [HideInInspector]
    public bool isSkipEndGame;
    [HideInInspector]
    public bool NoAdsIsClicked = false;
    [HideInInspector]
    public bool ingameShopIsOpened = false;
    [HideInInspector]
    public Coroutine NoAdsCoroutine;
    [HideInInspector]
    public int buyPowerUpType = -1;

    private Vector3 SettingsPos;
    private Vector3 OpenSettingPos;
    private bool settingsIsOpened = false;
    private float firstSectorFault = 0.8f;
    private float thirdSectorFault = 0.2f;
    private int numOfRevives; // show how many time the player has been revived!
    private int revivePrice;
    private IEnumerator nextHeartTimerIE;
    private GameObject addedStars;
    private bool showBoardMode;
    private Image levelFailSR;
    private Color textRedColor;
    private int powerUpsIndex;

    private bool backToMenuIsOnScreen;
    private bool levelFailedIsOnScreen;
    private bool noAdsPromptIsOnScreen;
    [HideInInspector]
    public bool levelCompleteIsOnScreen;

    private static readonly string textkey_LevelX = "LevelX";
    private static readonly string textkey_NoAdsTitle = "NoAdsTitle";
    private static readonly string textkey_NoAdsButton = "NoAdsButton";
    private static readonly string textkey_TapShowBoard = "TapShowBoard";
    private static readonly string textkey_TapToReturn = "TapToReturn";
    private static readonly string textkey_Continue = "Continue";
    private static readonly string textkey_Retry = "Retry";
    private static readonly string textkey_Share = "Share";
    private static readonly string textkey_OutOfMoves = "OutOfMoves";
    private static readonly string textkey_GetXMoves = "GetXMoves";
    private static readonly string textkey_VideoAd = "VideoAd";
    private static readonly string textkey_PlayOn = "PlayOn";
    private static readonly string textkey_LevelFailed = "LevelFailed";
    private static readonly string textkey_TryAgain = "TryAgain";
    private static readonly string textkey_AreYouSure = "AreYouSure";
    private static readonly string textkey_YouLoseHeart = "YouLoseHeart";
    private static readonly string textkey_YouWontLoseHeart = "YouWontLoseHeart";
    private static readonly string textkey_Buy = "Buy";
    private static readonly string textkey_Get3Hints = "Get3Hints";
    private static readonly string textkey_Get3IMs = "Get3IMs";
    private static readonly string textkey_Quit = "Quit";
    private static readonly string textkey_Rating = "Rating";
    private static readonly string textkey_RateUs = "RateUs";
    private static readonly string textkey_OutOfHearts = "OutOfHearts";
    private static readonly string textkey_TimeToNextHeart = "TimeToNextHeart";
    private static readonly string textkey_RefillHearts = "RefillHearts";

    // Start is called before the first frame update
    void Start()
    {
        if (CloudManager.instance.dataLoadedInSplash)
        {
            CloudManager.instance.ShowLoadDataPromptOverlay();
            CloudManager.instance.dataLoadedInSplash = false;
        }
        SettingsPos = VibrateButton.transform.position;
        OpenSettingPos = undoBtn.transform.position;

        SoundButton.GetComponentsInChildren<Image>()[1].sprite = (Manager.instance.isSoundEnabled ? SoundOn : SoundOff);
        MusicButton.GetComponentsInChildren<Image>()[1].sprite = (Manager.instance.isMusicEnabled ? MusicOn : MusicOff);
        VibrateButton.GetComponentsInChildren<Image>()[1].sprite = (Manager.instance.isHapticEnabled ? VibrateOn : VibrateOff);

        SetLevelNumberInTexts();

        AdManager.instance.uiManager = this;
        AdManager.instance.LoadInterstitialIfEnabledAndNeeded();
        AdManager.instance.ShowBannerIfEnabled(levelManger.levelNum);

        TranslateGameSceneTexts();

        InfiniteMoveItemNumber.text = Manager.instance.numOfIMItem.ToString();
        hintNumber.text = Manager.instance.numOfHint.ToString();
        if (LanguageManager.instance.isLanguageRTL)
        {
            InfiniteMoveItemNumber.text = LanguageManager.arabicFix(InfiniteMoveItemNumber.text);
            hintNumber.text = LanguageManager.arabicFix(hintNumber.text);
        }
        levelFailSR = levelFail.GetComponent<Image>();
        textRedColor = new Color(212 / 255f, 105 / 255f, 111 / 255f, 1f);
        HandleElementsLocksAndFuncs();

        if ( !Manager.instance.ratingIsGiven )
        {
            int a = ConfigManager.instance.GetConfig_RatingPopupAfterWin(1);
            int b = ConfigManager.instance.GetConfig_RatingPopupAfterWin(2);
            if (levelManger.levelNum == a || levelManger.levelNum == b)
            {
                RatingManager.instance.PrepareStoreRating();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (levelCompleteIsOnScreen || levelFailedIsOnScreen || noAdsPromptIsOnScreen)
            {
                return;
            }
            if (ingameShopIsOpened)
            {
                inGameShopScript.CloseInGameShop();
            }
            else if (BackToMenuPrompt.activeInHierarchy)
            {
                HideBackToMenuPrompt();
            }
            else
            {
                ShowBackToMenuPrompt();
            }
        }
    }

#if UNITY_EDITOR
    private string directLevelLoad = "";
    void OnGUI()
    {
        // Make a text field that modifies directLevelLoad.
        directLevelLoad = GUI.TextField(new Rect(0, 0, 100, 20), directLevelLoad, 25);
        if (GUI.Button(new Rect(120, 0, 100, 20), "Load!"))
        {
            GameScenes.instance.LoadSpecificLevel(short.Parse(directLevelLoad));
        }
    }
#endif

    private void TranslateGameSceneTexts()
    {
        ShareButton.GetComponentInChildren<Text>().text = LanguageManager.instance.GetTheTextByKey(textkey_Share);
        ContinueButton.GetComponentInChildren<Text>().text = LanguageManager.instance.GetTheTextByKey(textkey_Continue);
        RetryButton.GetComponentInChildren<Text>().text = LanguageManager.instance.GetTheTextByKey(textkey_Retry);
        levelFailedContinueButton.GetComponentInChildren<Text>().text = LanguageManager.instance.GetTheTextByKey(textkey_Continue);
        AreYouSureText.text = LanguageManager.instance.GetTheTextByKey(textkey_AreYouSure);
        QuitText.text = LanguageManager.instance.GetTheTextByKey(textkey_Quit);
        brokenHeartText.text = LanguageManager.instance.GetTheTextByKey(textkey_LevelFailed);
        tryAgainText.text = LanguageManager.instance.GetTheTextByKey(textkey_TryAgain);
        outOfMovesText.text = LanguageManager.instance.GetTheTextByKey(textkey_OutOfMoves);
        getXMovesText.text = LanguageManager.instance.GetTheTextByKey(textkey_GetXMoves);
        showBoardText.text = LanguageManager.instance.GetTheTextByKey(textkey_TapShowBoard);
        levelFailedRightButtonText.text = LanguageManager.instance.GetTheTextByKey(textkey_VideoAd);
        playOnText.text = LanguageManager.instance.GetTheTextByKey(textkey_PlayOn);
    }

    private void SetLevelNumberInTexts()
    {
        string theLevelX;
        if (LanguageManager.instance.isLanguageRTL)
            theLevelX = LanguageManager.instance.GetTheTextByKey(textkey_LevelX).Replace("۱۲", LanguageManager.arabicFix(levelManger.levelNum.ToString()));
        else
            theLevelX = LanguageManager.instance.GetTheTextByKey(textkey_LevelX).Replace("12", levelManger.levelNum.ToString());
        LevelNumberText.text = theLevelX;
        LevelEndLevelNumberText.text = theLevelX;
        brokenHeartLevelNumberText.text = theLevelX;
    }

    public void ToggleSettings()
    {
        if (settingsIsOpened)
            StartCoroutine(CloseTheSettings());
        else
            StartCoroutine(OpenTheSettings());

        Sounds.instance.PlayUIInteraction();
        VibrationManager.instance.VibrateTouch();
    }

    private IEnumerator OpenTheSettings()
    {
        settingsIsOpened = true;
        hintBtn.gameObject.SetActive(false);
        InfiniteMoveButton.gameObject.SetActive(false);
        undoBtn.gameObject.SetActive(false);
        float settingsMiniRotateTime = settingsOpenTime / 3;
        StartCoroutine(Transitions.rotateZ(SettingsButton, settingsRotateDegrees, true, settingsOpenTime));
        StartCoroutine(Transitions.moveToPosition(SettingsButton, OpenSettingPos, settingsOpenTime));
        yield return StartCoroutine(Transitions.scaleUp(VibrateButton, new Vector3(0.02f, 0.02f, 0.02f), settingsMiniRotateTime));
        yield return StartCoroutine(Transitions.scaleUp(MusicButton, new Vector3(0.02f, 0.02f, 0.02f), settingsMiniRotateTime));
        yield return StartCoroutine(Transitions.scaleUp(SoundButton, new Vector3(0.02f, 0.02f, 0.02f), settingsMiniRotateTime));
        AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.ingame_settings_opened);
    }

    private IEnumerator CloseTheSettings()
    {
        settingsIsOpened = false;
        float settingsMiniRotateTime = settingsCloseTime / 3;
        StartCoroutine(Transitions.rotateZ(SettingsButton, -settingsRotateDegrees, true, settingsCloseTime));
        StartCoroutine(Transitions.moveToPosition(SettingsButton, SettingsPos, settingsCloseTime));
        SoundButton.SetActive(false);
        yield return new WaitForSeconds(settingsMiniRotateTime);
        MusicButton.SetActive(false);
        yield return new WaitForSeconds(settingsMiniRotateTime);
        VibrateButton.SetActive(false);
        yield return new WaitForSeconds(settingsMiniRotateTime);
        hintBtn.gameObject.SetActive(true);
        InfiniteMoveButton.gameObject.SetActive(true);
        undoBtn.gameObject.SetActive(true);
    }

    private IEnumerator PauseAct(GameObject panel, GameObject circle, Vector3 finalScale, float totalTime)
    {
        Color panelColor = panel.GetComponent<Image>().color;
        panel.GetComponent<Image>().color = new Color(panelColor.r, panelColor.g, panelColor.b, 1f);
        yield return StartCoroutine(Transitions.PanelAct(circle, finalScale, totalTime));
        panel.SetActive(true);
    }

    public void ResetTheLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Sounds.instance.PlayUIInteraction();
        VibrationManager.instance.VibrateTouch();
    }

    public void LoadNextLevel()
    {
        Sounds.instance.PlayUIInteraction();
        VibrationManager.instance.VibrateTouch();
        AdManager.instance.HideBanner();
        GameScenes.instance.LoadMenu(Chapters.instance.GetChapterIndexOfThisLevel(levelManger.levelNum), levelManger.levelNum);
    }

    public void setSkipEndGame() {
        isSkipEndGame = true;
        StartCoroutine(Transitions.scaleDownAndDestroy(skipButton, new Vector3(0.9f, 0.9f, 1), 0.1f));
    }

    public void ShowLevelCompleted(int numOfStars,int oldStars)
    {
        StartCoroutine(ShowLevelCompletedIE(numOfStars,oldStars));
    }

    public void ShowLevelFailed() {
        StartCoroutine(ShowLevelFailedIE());
    }

    private IEnumerator ShowLevelCompletedIE(int numOfStars,int oldStars)
    {
        AnalyticsManager.instance.LogLevelCompleted(levelManger.levelChapterNumber - 1,
            levelManger.levelNum, numOfStars, levelManger.score);
        if (skipText) {
            skipText.color = Color.white;
        }
        TopBar.SetActive(false);
        BottomBar.SetActive(false);
        PausePanelCircle.GetComponent<Image>().sprite = endCircle;
        Sounds.instance.PlayLevelComplete();
        VibrationManager.instance.VibrateSuccess();
        if (!isSkipEndGame)
        {
            StartCoroutine(Transitions.PanelAct(PausePanelCircle, circleFinalScale, pauseTime));
            StartCoroutine(Transitions.turnOnImageWithDelay(endPanel, pauseTime - 0.05f));
            yield return new WaitForSeconds(pauseTime);
        }
        else {
            StartCoroutine(Transitions.turnOnImageWithDelay(endPanel, 0.0001f));
        }
        Circle.SetActive(true);
        if (levelManger.levelNum == Manager.instance.maxSeenLevelNum) {
            Quests.CheckQuestType(QuestType.EarnScore, true, levelManger.score, false, true);
        }
        yield return StartCoroutine(ScoreCounter());
        for (int i = 0; i < numOfStars; i++)
        {
            Stars[i].SetActive(true);
            Vector3 finalPos = Stars[i].transform.position;
            Stars[i].transform.position = starsStartPosition;
            if (!isSkipEndGame)
            {
                yield return StartCoroutine(Transitions.moveToPosition(Stars[i], finalPos, starTime));
                Sounds.instance.PlayGiveStar();
                VibrationManager.instance.VibrateHeavy();
                yield return new WaitForSeconds(starWaitTime);
            }
            else {
                Stars[i].transform.position = finalPos;
            }
        }
        if (!isSkipEndGame)
        {
            yield return StartCoroutine(Transitions.scaleUp(starChestScript.gameObject,
                new Vector3(0.02f, 0.02f, 1), 0.5f));
        }
        else {
            starChestScript.gameObject.SetActive(true);
        }
        if (numOfStars >= 1) {
            StartCoroutine(MoveStarToChest(addedFirstStar, addedStarMoveTime));
        }
        if (numOfStars >= 2) {
            StartCoroutine(MoveStarToChest(addedSecondStar, addedStarMoveTime));
        }
        if (numOfStars == 3) {
            StartCoroutine(MoveStarToChest(addedThirdStar, addedStarMoveTime));
        }
        int tempStarChestProgress = Manager.instance.starChestProgress + numOfStars - oldStars;
        yield return StartCoroutine(starChestScript.UpdateChestBar(numOfStars - oldStars));
        if (tempStarChestProgress < Manager.instance.starChestCapacity) {
            if (Manager.instance.maxSeenLevelNum >= 14) 
            { // it is the number of next level!
                StartCoroutine(Transitions.fadeIn(ShareButton, 1f, pauseTime));
            }
            if (Manager.instance.maxSeenLevelNum >= 5)
            { // it is the number of next level!
                StartCoroutine(Transitions.fadeIn(RetryButton, 1f, pauseTime));
                if (Manager.instance.maxSeenLevelNum < 14)
                    RetryButton.GetComponent<Transform>().position = ShareButton.GetComponent<Transform>().position;
            }
            StartCoroutine(Transitions.fadeIn(ContinueButton, 1f, pauseTime));
            AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.level_complete_watched);
        }
        if (skipButton != null) {
            skipButton.GetComponent<Button>().interactable = false;
            StartCoroutine(Transitions.scaleDownAndDestroy(skipButton, new Vector3(0.8f, 0.8f, 1), 0.1f));
        }
    }

    private IEnumerator ScoreCounter() {
        int tempScore = 0;
        do {
            tempScore += levelManger.score / numOfAddingScoreSteps;
            LevelEndScoreText.text = tempScore.ToString();
            if (LanguageManager.instance.isLanguageRTL)
                LevelEndScoreText.text = LanguageManager.arabicFix(LevelEndScoreText.text);
            yield return new WaitForSeconds(totalScoreTime / numOfAddingScoreSteps);
        } while (levelManger.score - tempScore > levelManger.score / numOfAddingScoreSteps && !isSkipEndGame);
        LevelEndScoreText.text = levelManger.score.ToString();
        if (LanguageManager.instance.isLanguageRTL)
            LevelEndScoreText.text = LanguageManager.arabicFix(LevelEndScoreText.text);
        yield return null;
    }

    private IEnumerator ShowLevelFailedIE()
    {
        yield return new WaitForSeconds(1);
        TopBar.SetActive(false);
        BottomBar.SetActive(false);
        showBoardGO.SetActive(true);
        PausePanelCircle.GetComponent<Image>().sprite = endCircle;
        Sounds.instance.PlayError();
        VibrationManager.instance.VibrateFailure();
        StartCoroutine(Transitions.PanelAct(PausePanelCircle, circleFinalScale, pauseTime));
        yield return StartCoroutine(Transitions.turnOnImageWithDelay(levelFail, pauseTime - 0.05f));
        StartCoroutine(Transitions.scaleUp(levelFailedBack, new Vector3(0.2f, 0.2f, 1), 0.2f));
        if (numOfRevives == 0)
        {
            if (Manager.instance.userHasBoughtNoAds)
            {
                levelFailedContinueButton.SetActive(true);
            }
            else {
                levelFailShortButtonL.SetActive(true);
                levelFailShortButtonR.SetActive(true);
                revivePrice = firstRevivePrice;
                levelFailedLeftButtonText.text = "" + firstRevivePrice;
                if (LanguageManager.instance.isLanguageRTL)
                    levelFailedLeftButtonText.text = LanguageManager.arabicFix(levelFailedLeftButtonText.text);
            }
        }
        else {
            ContinueButton.SetActive(false);
            levelFailShortButtonL.SetActive(false);
            levelFailShortButtonR.SetActive(false);
            playOnMoney.SetActive(true);
            revivePrice = (int)Mathf.Pow(2, numOfRevives) * firstRevivePrice;
            playOnMoneyText.text = "" + revivePrice;
            if (LanguageManager.instance.isLanguageRTL)
                playOnMoneyText.text = LanguageManager.arabicFix(playOnMoneyText.text);
        }
        if (LanguageManager.instance.isLanguageRTL)
            plusFiveText.text = LanguageManager.arabicFix(plusFiveText.text);
        numOfRevives++;
    }

    public void LevelFailContinue()
    {
        StartCoroutine(LevelFailContinueIE());
    }

    private IEnumerator LevelFailContinueIE()
    {
        Vector3 pos;
        TopBar.SetActive(true);
        BottomBar.SetActive(true);
        BottomBar.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, BottomBarYBannerOff, 0);
        pos = levelFailedBack.transform.position;
        StartCoroutine(Transitions.moveToPosition(levelFailedBack, new Vector3(6, pos.y, pos.z), 0.5f));
        yield return StartCoroutine(Transitions.fadeOut(levelFail, 1, 0.5f));
        Sounds.instance.PlayAfterVideoAdReward();
        VibrationManager.instance.VibrateSuccess();
        Color tempColor;
        levelFail.SetActive(false);
        tempColor = levelFail.GetComponent<Image>().color;
        levelFail.GetComponent<Image>().color = new Color(tempColor.r, tempColor.g, tempColor.b, 1);
        levelFailedBack.SetActive(false);
        levelFailedBack.transform.position = new Vector3(0, pos.y, pos.z);
        int numberOfExtraMoves = ConfigManager.instance.GetConfig_ExtraMovesAfterLoss();
        levelManger.numOfMoves += numberOfExtraMoves;
        NumOfMoves.text = levelManger.numOfMoves.ToString();
        if (LanguageManager.instance.isLanguageRTL)
            NumOfMoves.text = LanguageManager.arabicFix(NumOfMoves.text);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Transitions.stamp(NumOfMoves.gameObject, new Vector3(3f, 3f, 1), 0.2f));
    }

    public void ShowVideoForExtraMoves()
    {
        AdManager.instance.ShowVideoAdIfAvailable(AdManager.rewardedVideoAdType.extraMoves);
    }

    public void PayCoinsForExtraMoves()
    {
        if (Manager.instance.coins >= revivePrice)
        {
            Manager.instance.coins -= revivePrice;
            Manager.instance.Save(false);
            LevelFailContinue();
            AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.extra_moves_earned_with_coins);
        }
        else
        {
            ShowInGameShop();
        }
    }

    public void ShowBoard() {
        StartCoroutine(ShowBoardIE());
    }

    private IEnumerator ShowBoardIE() {
        if (!showBoardMode)
        {
            yield return StartCoroutine(Transitions.setScale(levelFailedBack, new Vector3(0.9f, 0.9f, 1), 0.1f));
            levelFailedBack.SetActive(false);
            showBoardText.text = LanguageManager.instance.GetTheTextByKey(textkey_TapToReturn);
            showBoardText.color = textRedColor;
            showBoardMode = true;
            levelFailSR.sprite = showBoardPanel;
            AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.level_failed_board_seen);
        }
        else
        {
            levelFailedBack.SetActive(true);
            yield return StartCoroutine(Transitions.setScale(levelFailedBack, new Vector3(1f, 1f, 1), 0.1f));
            showBoardText.text = LanguageManager.instance.GetTheTextByKey(textkey_TapShowBoard);
            showBoardText.color = Color.white;
            showBoardMode = false;
            levelFailSR.sprite = levelEndPanel;
        }
    }

    public void LevelFailClose(bool isSoundEnabled)
    {
        StartCoroutine(LevelFailCloseIE(isSoundEnabled));
    }

    public IEnumerator LevelFailCloseIE(bool isSoundEnabled)
    {
        if (isSoundEnabled) {
            Sounds.instance.PlayUIInteraction();
            VibrationManager.instance.VibrateTouch();
        }
        showBoardGO.SetActive(false);
        Vector3 pos = levelFailedBack.transform.position;
        yield return StartCoroutine(Transitions.moveToPosition(levelFailedBack, new Vector3(6, pos.y, pos.z), 0.5f));
        levelFailedBack.SetActive(false);
        levelFailedBack.transform.position = new Vector3(0, pos.y, pos.z);
        StartCoroutine(Transitions.scaleUp(brokenHeartBack, new Vector3(0.2f, 0.2f, 1), 0.2f));
        if (!Manager.instance.isInfiniteHeart)
        {
            Manager.instance.hearts--;
            if (Manager.instance.hearts == 4)
            {
                Manager.instance.heartsRefilledTime = DateTime.Now;
            }
            Manager.instance.Save(false);
            AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.level_failed_heart_lost);
        }
        else {
            brokenHeartImage.sprite = infiniteHeartSprite;
        }
    }

    public void TryAgain()
    {
        Menu.UpdateHeartsNum();
        if (Manager.instance.hearts > 0 || Manager.instance.isInfiniteHeart)
        {
            StartCoroutine(TryAgainIE());
        }
        else
        {
            StartCoroutine(OpenNoHeartIE());
        }
    }

    public IEnumerator TryAgainIE()
    {
        Vector3 pos = brokenHeartBack.transform.position;
        StartCoroutine(Transitions.moveToPosition(brokenHeartBack, new Vector3(6, pos.y, pos.z), 0.5f));
        yield return StartCoroutine(Transitions.fadeOut(levelFail, 1, 0.5f));
        brokenHeartBack.SetActive(false);
        Sounds.instance.PlayUIInteraction();
        VibrationManager.instance.VibrateTouch();
        GameScenes.instance.LoadSpecificLevel(levelManger.levelNum);
    }

    private IEnumerator OpenNoHeartIE()
    {
        if (LanguageManager.instance.isLanguageRTL)
            zeroNumtext.text = LanguageManager.arabicFix(zeroNumtext.text);
        noHeartTitleText.text = LanguageManager.instance.GetTheTextByKey(textkey_OutOfHearts);
        noHeartTimerTitleText.text = LanguageManager.instance.GetTheTextByKey(textkey_TimeToNextHeart);
        buyFullHeartsText.text = LanguageManager.instance.GetTheTextByKey(textkey_RefillHearts);
        Vector3 pos = brokenHeartBack.transform.position;
        yield return StartCoroutine(Transitions.moveToPosition(brokenHeartBack, new Vector3(6, pos.y, pos.z), 0.5f));
        brokenHeartBack.SetActive(false);
        brokenHeartBack.transform.position = new Vector3(0, pos.y, pos.z);
        StartCoroutine(Transitions.scaleUp(noHeartBack, new Vector3(0.2f, 0.2f, 1), 0.2f));
        buyFullHeartsPriceText.text = Manager.instance.buyFullHeartsPrice.ToString();
        if (LanguageManager.instance.isLanguageRTL)
            buyFullHeartsPriceText.text = LanguageManager.arabicFix(buyFullHeartsPriceText.text);
        nextHeartTimerIE = NextHeartTimer();
        StartCoroutine(nextHeartTimerIE);
    }

    private IEnumerator NextHeartTimer()
    {
        TimeSpan heartTime = DateTime.Now - Manager.instance.heartsRefilledTime;
        heartTime = new TimeSpan(0, 30, 0) - heartTime;
        do
        {
            noHeartTimerText.text = heartTime.ToString(@"hh\:mm\:ss").Substring(3, 5);
            if (LanguageManager.instance.isLanguageRTL)
                noHeartTimerText.text = LanguageManager.arabicFix(noHeartTimerText.text);
            yield return new WaitForSeconds(1);
            heartTime = heartTime.Subtract(new TimeSpan(0, 0, 1));
        } while (heartTime.TotalSeconds >= 0);
        Manager.instance.hearts++;
        noHeartNumText.text = Manager.instance.hearts.ToString();
        if (LanguageManager.instance.isLanguageRTL)
            noHeartNumText.text = LanguageManager.arabicFix(noHeartNumText.text);
        StartCoroutine(Transitions.stamp(noHeartNumText.gameObject, new Vector3(3f, 3f, 1), 0.2f));
        if (Manager.instance.hearts == 5)
        {
            Manager.instance.heartsRefilledTime = DateTime.MinValue;
        }
        else
        {
            Manager.instance.heartsRefilledTime =
                Manager.instance.heartsRefilledTime.Add(new TimeSpan(0, 0, 1800));
            nextHeartTimerIE = NextHeartTimer();
            StartCoroutine(nextHeartTimerIE);
        }
        Manager.instance.Save(false);
    }

    public void BuyFullHearts() {
        StartCoroutine(BuyFullHeartsIE());
    }

    public IEnumerator BuyFullHeartsIE() {
        if (Manager.instance.coins >= Manager.instance.buyFullHeartsPrice)
        {
            Manager.instance.coins -= Manager.instance.buyFullHeartsPrice;
            Manager.instance.hearts = 5;
            Manager.instance.Save(false);
            Vector3 pos = noHeartBack.transform.position;
            yield return StartCoroutine(Transitions.moveToPosition(noHeartBack, new Vector3(6, pos.y, pos.z), 0.5f));
            noHeartBack.SetActive(false);
            noHeartBack.transform.position = new Vector3(0, pos.y, pos.z);
            StartCoroutine(Transitions.scaleUp(brokenHeartBack, new Vector3(0.2f, 0.2f, 1), 0.2f));
            brokenHeartImage.gameObject.SetActive(false);
            fiveHearts.SetActive(true);
            brokenHeartText.gameObject.SetActive(false);
        }
        else {
            ShowInGameShop();
        }
    }

    public void CloseNoHeart()
    {
        StartCoroutine(CloseNoHeartIE());
    }

    public IEnumerator CloseNoHeartIE() {
        Sounds.instance.PlayUIInteraction();
        VibrationManager.instance.VibrateTouch();
        Vector3 pos = noHeartBack.transform.position;
        yield return StartCoroutine(Transitions.moveToPosition(noHeartBack, new Vector3(6, pos.y, pos.z), 0.5f));
        noHeartBack.SetActive(false);
        noHeartBack.transform.position = new Vector3(0, pos.y, pos.z);
        StartCoroutine(Transitions.scaleUp(brokenHeartBack, new Vector3(0.2f, 0.2f, 1), 0.2f));
    }

    public void CloseBrokenHeart()
    {
        Sounds.instance.PlayUIInteraction();
        VibrationManager.instance.VibrateTouch();
        AdManager.instance.HideBanner();
        GameScenes.instance.LoadMenu(Chapters.instance.GetChapterIndexOfThisLevel(levelManger.levelNum), levelManger.levelNum);
    }

    public void ShowBuyPowerUps(int spriteIndex) {
        if (spriteIndex == 1 || (spriteIndex == 0 && levelManger.numOfGivenHint < 3)) {
            StartCoroutine(ShowBuyPowerUpsIE(spriteIndex));
        }
    }

    private IEnumerator ShowBuyPowerUpsIE(int spriteIndex) {
        if (spriteIndex == 0) {
            hintBtn.interactable = false;
            hintBtn.transform.GetChild(2).GetComponent<Button>().interactable = false;
        }
        powerUpsIndex = spriteIndex;
        PausePanelCircle.GetComponent<Image>().sprite = endCircle;
        Sounds.instance.PlayShowPopUp();
        VibrationManager.instance.VibrateTouch();
        StartCoroutine(Transitions.PanelAct(PausePanelCircle, circleFinalScale, pauseTime));
        yield return StartCoroutine(Transitions.turnOnImageWithDelay(buyPowerUps, pauseTime - 0.05f));
        StartCoroutine(Transitions.scaleUp(buyPowerUpsBack, new Vector3(0.2f, 0.2f, 1), 0.2f));
        numOfBoughtPowerUpsText.text =
            "×" + (LanguageManager.instance.isLanguageRTL ? LanguageManager.arabicFix(numOfBoughtPowerUps.ToString()) : numOfBoughtPowerUps.ToString());
        switch (spriteIndex) {
            case 0: {
                    buyPowerUpsImage.sprite = hintSprite;
                    powerUpsDescText.text = LanguageManager.instance.GetTheTextByKey(textkey_Get3Hints);
                    if (LanguageManager.instance.isLanguageRTL)
                    {
                        powerUpsDescText.text = powerUpsDescText.text.Replace("۳", LanguageManager.arabicFix(numOfBoughtPowerUps.ToString()));
                    }
                    else
                    {
                        powerUpsDescText.text = powerUpsDescText.text.Replace("3", numOfBoughtPowerUps.ToString());
                    }
                    break;
                }
            case 1: {
                    buyPowerUpsImage.sprite = infiniteMovesSprite;
                    powerUpsDescText.text = LanguageManager.instance.GetTheTextByKey(textkey_Get3IMs);
                    if (LanguageManager.instance.isLanguageRTL)
                    {
                        powerUpsDescText.text = powerUpsDescText.text.Replace("۳", LanguageManager.arabicFix(numOfBoughtPowerUps.ToString()));
                    }
                    else
                    {
                        powerUpsDescText.text = powerUpsDescText.text.Replace("3", numOfBoughtPowerUps.ToString());
                    }
                    break;
                }
            default:
                throw new Exception("invalid sprite index");
        }
        powerUpsPriceText.text = powerUpsPrice.ToString();
    }

    public void BuyPowerUps() {
        StartCoroutine(BuyPowerUpsIE());
    }

    private IEnumerator BuyPowerUpsIE() {
        if (Manager.instance.coins >= powerUpsPrice)
        {
            switch (powerUpsIndex)
            {
                case 0:
                    {
                        Manager.instance.coins -= powerUpsPrice;
                        Manager.instance.numOfHint += numOfBoughtPowerUps;
                        GameObject hintPlus = hintBtn.transform.GetChild(2).gameObject;
                        hintPlus.SetActive(false);
                        hintNumber.transform.parent.gameObject.SetActive(true);
                        hintNumber.text = numOfBoughtPowerUpsText.text.Substring(1);
                        hintBtn.onClick.RemoveAllListeners();
                        hintBtn.onClick.AddListener(() => levelManger.Hint());
                        Manager.instance.Save(false);
                        break;
                    }
                case 1:
                    {
                        Manager.instance.coins -= powerUpsPrice;
                        Manager.instance.numOfIMItem += numOfBoughtPowerUps;
                        GameObject infiniteMovePlus = InfiniteMoveButton.transform.GetChild(2).gameObject;
                        infiniteMovePlus.SetActive(false);
                        InfiniteMoveItemNumber.transform.parent.gameObject.SetActive(true);
                        InfiniteMoveItemNumber.text = numOfBoughtPowerUpsText.text.Substring(1);
                        InfiniteMoveButton.onClick.RemoveAllListeners();
                        InfiniteMoveButton.onClick.AddListener(() => MakeMovesInfinite());
                        Manager.instance.Save(false);
                        break;
                    }
                default:
                    throw new Exception("invalid sprite index");
            }
            Sounds.instance.PlayChestItems();
            VibrationManager.instance.VibrateSuccess();
            StartCoroutine(CloseBuyPowerUpsIE(false));
            yield return null;
        }
        else {
            buyPowerUpType = powerUpsIndex;
            ShowInGameShop();
        }
    }

    public void CloseBuyPowerUps(bool isSoundEnabled) {
        StartCoroutine(CloseBuyPowerUpsIE(isSoundEnabled));
    }

    public IEnumerator CloseBuyPowerUpsIE(bool isSoundEnabled)
    {
        if (isSoundEnabled)
        {
            Sounds.instance.PlayUIInteraction();
            VibrationManager.instance.VibrateTouch();
        }
        if (powerUpsIndex == 0)
        {
            hintBtn.interactable = true;
            hintBtn.transform.GetChild(2).GetComponent<Button>().interactable = true;
        }
        Vector3 pos;
        pos = buyPowerUpsBack.transform.position;
        StartCoroutine(Transitions.moveToPosition(buyPowerUpsBack, new Vector3(6, pos.y, pos.z), 0.5f));
        yield return StartCoroutine(Transitions.fadeOut(buyPowerUps, 1, 0.5f));
        Color tempColor;
        buyPowerUps.SetActive(false);
        tempColor = buyPowerUps.GetComponent<Image>().color;
        buyPowerUps.GetComponent<Image>().color = new Color(tempColor.r, tempColor.g, tempColor.b, 1);
        buyPowerUpsBack.SetActive(false);
        buyPowerUpsBack.transform.position = new Vector3(0, pos.y, pos.z);
    }

    public void MakeMovesInfinite()
    {
        Sounds.instance.PlayInfiniteMovesUse();
        VibrationManager.instance.VibrateSuccess();
        Quests.CheckQuestType(QuestType.UsePowerUps, true, 1, false, false);
        AchievementManager.UnlockAchievement_FirstInfiniteMoves();
        levelManger.infiniteMove = true;
        NumOfMoves.text = "∞";
        Manager.instance.numOfIMItem--;
        if (Manager.instance.numOfIMItem == 0)
        {
            ToggleInfiniteMovesPlusAndBadge(true);
        }
        else
        {
            InfiniteMoveButton.interactable = false;
        }
        InfiniteMoveItemNumber.text = Manager.instance.numOfIMItem.ToString();
        if (LanguageManager.instance.isLanguageRTL)
            InfiniteMoveItemNumber.text = LanguageManager.arabicFix(InfiniteMoveItemNumber.text);
        Manager.instance.Save(false);
        AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.ingame_IM);
    }

    public void ChangeUIForBanner(bool weHaveBanner)
    {
        BackBanner.SetActive(weHaveBanner);
        RectTransform botBarRect = BottomBar.GetComponent<RectTransform>();
        Vector3 botBarPos = botBarRect.anchoredPosition3D;
        botBarPos.y = weHaveBanner ? BottomBarYBannerOn : BottomBarYBannerOff;

        botBarRect.anchoredPosition = botBarPos;
        BottomBarFTUE.GetComponent<RectTransform>().anchoredPosition = botBarPos;

        SettingsPos = VibrateButton.transform.position;
        OpenSettingPos = undoBtn.transform.position;

    }

    public IEnumerator FillProgressSectors(float addedDegree)
    {
        if (addedDegree > 0)
        {
            levelManger.progressDegree += addedDegree;
            if (levelManger.progressDegree <= 1)
            {
                StartCoroutine(Transitions.FillCircularImage(progressSectors[0], fillingProgressTime, progressSectors[0].fillAmount,
                    levelManger.progressDegree * firstSectorFault));
            }
            else
            {
                if (levelManger.progressDegree - addedDegree > 1)
                {
                    progressSectors[0].fillAmount = 1;
                    StartCoroutine(Transitions.FillCircularImage(progressSectors[1], fillingProgressTime, progressSectors[1].fillAmount,
                        levelManger.progressDegree - 1));
                }
                else
                {
                    yield return StartCoroutine(Transitions.FillCircularImage(progressSectors[0], fillingProgressTime,
                        progressSectors[0].fillAmount, firstSectorFault));
                    offStar.SetActive(false);
                    oneStar.SetActive(true);
                    StartCoroutine(Transitions.stamp(oneStar, new Vector3(1.5f, 1.5f, 1.5f), 0.2f));
                    StartCoroutine(Transitions.FillCircularImage(progressSectors[1], fillingProgressTime, progressSectors[1].fillAmount,
                        levelManger.progressDegree - 1));
                }
            }
        }
        yield return null;
    }

    public IEnumerator FillProgressSectorsAfterWin(int score, int remainedMoves, int extraMoves2, int extraMoves3)
    {
        VibrationManager.instance.VibrateHold();
        switch (score)
        {
            case 1:
                if (levelManger.progressDegree < 1)
                {
                    yield return StartCoroutine(Transitions.FillCircularImage(progressSectors[0], fillingProgressTimeAfterWin,
                        progressSectors[0].fillAmount, firstSectorFault));
                    offStar.SetActive(false);
                    oneStar.SetActive(true);
                    StartCoroutine(Transitions.stamp(oneStar, new Vector3(1.5f, 1.5f, 1.5f), 0.2f));
                    yield return new WaitForSeconds(delayBetweenSectorsFilling);
                    yield return StartCoroutine(Transitions.FillCircularImage(progressSectors[1], fillingProgressTimeAfterWin,
                        progressSectors[1].fillAmount, remainedMoves / extraMoves2));
                }
                else
                {
                    float emptyDegreeOfSecondSector = 2 - levelManger.progressDegree;
                    yield return StartCoroutine(Transitions.FillCircularImage(progressSectors[1], fillingProgressTimeAfterWin,
                       progressSectors[1].fillAmount, (remainedMoves / extraMoves2 * emptyDegreeOfSecondSector) + levelManger.progressDegree - 1));
                }
                break;
            case 2:
                if (levelManger.progressDegree < 1)
                {
                    yield return StartCoroutine(Transitions.FillCircularImage(progressSectors[0], fillingProgressTimeAfterWin,
                        progressSectors[0].fillAmount, firstSectorFault));
                    offStar.SetActive(false);
                    oneStar.SetActive(true);
                    StartCoroutine(Transitions.stamp(oneStar, new Vector3(1.5f, 1.5f, 1.5f), 0.2f));
                    yield return new WaitForSeconds(delayBetweenSectorsFilling);
                    yield return StartCoroutine(Transitions.FillCircularImage(progressSectors[1], fillingProgressTimeAfterWin,
                        progressSectors[1].fillAmount, 1));
                    oneStar.SetActive(false);
                    twoStars.SetActive(true);
                    StartCoroutine(Transitions.stamp(twoStars, new Vector3(1.5f, 1.5f, 1.5f), 0.2f));
                    yield return new WaitForSeconds(delayBetweenSectorsFilling);
                    yield return StartCoroutine(Transitions.FillCircularImage(progressSectors[2], fillingProgressTimeAfterWin,
                        progressSectors[2].fillAmount, firstSectorFault * remainedMoves / extraMoves2 + thirdSectorFault));
                }
                else if (levelManger.progressDegree < 2)
                {
                    yield return StartCoroutine(Transitions.FillCircularImage(progressSectors[1], fillingProgressTimeAfterWin,
                        progressSectors[1].fillAmount, 1));
                    oneStar.SetActive(false);
                    twoStars.SetActive(true);
                    StartCoroutine(Transitions.stamp(twoStars, new Vector3(1.5f, 1.5f, 1.5f), 0.2f));
                    yield return new WaitForSeconds(delayBetweenSectorsFilling);
                    yield return StartCoroutine(Transitions.FillCircularImage(progressSectors[2], fillingProgressTimeAfterWin,
                        progressSectors[2].fillAmount, firstSectorFault * remainedMoves / extraMoves3 + thirdSectorFault));
                }
                break;
            case 3:
                if (levelManger.progressDegree < 1)
                {
                    yield return StartCoroutine(Transitions.FillCircularImage(progressSectors[0], fillingProgressTimeAfterWin,
                        progressSectors[0].fillAmount, firstSectorFault));
                    offStar.SetActive(false);
                    oneStar.SetActive(true);
                    StartCoroutine(Transitions.stamp(oneStar, new Vector3(1.5f, 1.5f, 1.5f), 0.2f));
                    yield return new WaitForSeconds(delayBetweenSectorsFilling);
                    yield return StartCoroutine(Transitions.FillCircularImage(progressSectors[1], fillingProgressTimeAfterWin,
                        progressSectors[1].fillAmount, 1));
                    while (progressSectors[1].fillAmount < 1)
                    {
                        progressSectors[1].fillAmount += Time.deltaTime / fillingProgressTimeAfterWin;
                        yield return null;
                    }
                    progressSectors[1].fillAmount = 1;
                    oneStar.SetActive(false);
                    twoStars.SetActive(true);
                    StartCoroutine(Transitions.stamp(twoStars, new Vector3(1.5f, 1.5f, 1.5f), 0.2f));
                    yield return new WaitForSeconds(delayBetweenSectorsFilling);
                    yield return StartCoroutine(Transitions.FillCircularImage(progressSectors[2], fillingProgressTimeAfterWin,
                        progressSectors[2].fillAmount, firstSectorFault + thirdSectorFault));
                    twoStars.SetActive(false);
                    threeStars.SetActive(true);
                    StartCoroutine(Transitions.stamp(threeStars, new Vector3(1.5f, 1.5f, 1.5f), 0.2f));
                    yield return new WaitForSeconds(delayBetweenSectorsFilling);
                }
                else if (levelManger.progressDegree < 2)
                {
                    yield return StartCoroutine(Transitions.FillCircularImage(progressSectors[1], fillingProgressTimeAfterWin,
                        progressSectors[1].fillAmount, 1));
                    oneStar.SetActive(false);
                    twoStars.SetActive(true);
                    StartCoroutine(Transitions.stamp(twoStars, new Vector3(1.5f, 1.5f, 1.5f), 0.2f));
                    yield return new WaitForSeconds(delayBetweenSectorsFilling);
                    yield return StartCoroutine(Transitions.FillCircularImage(progressSectors[2], fillingProgressTimeAfterWin,
                        progressSectors[2].fillAmount, firstSectorFault + thirdSectorFault));
                    twoStars.SetActive(false);
                    threeStars.SetActive(true);
                    StartCoroutine(Transitions.stamp(threeStars, new Vector3(1.5f, 1.5f, 1.5f), 0.2f));
                    yield return new WaitForSeconds(delayBetweenSectorsFilling);
                }
                break;
        }
        yield return null;
    }

    public void ProcessAfterWinConditions()
    {
        //Process Ratings
        if ( !Manager.instance.ratingIsGiven )
        {
            int a = ConfigManager.instance.GetConfig_RatingPopupAfterWin(1);
            int b = ConfigManager.instance.GetConfig_RatingPopupAfterWin(2);
            if (levelManger.levelNum == a || levelManger.levelNum == b)
            {
                StartCoroutine(ShowRatingPopup());
                return;
            }
        }
        //Process Ads
        if (AdManager.instance.IsItTimeToShowInterstitial(levelManger.levelNum, AdManager.interestitialAdType.After_win))
        {
            NoAdsCoroutine = StartCoroutine(NoAdsPromptIE());
            return;
        }
        //Go To Main Menu
        LoadNextLevel();
    }

    public void ActuallyLaunchStoreRating()
    {
        Manager.instance.ratingIsGiven = true;
        Manager.instance.Save(false);
        RatingManager.instance.LaunchStoreRating();
        //Go To Main Menu anyway
        LoadNextLevel();
    }

    public void HideRatingPopup()
    {
        ratingObj.SetActive(false);
        rateUs.SetActive(false);
        ratingCross.SetActive(false);
        LoadNextLevel();
    }

    private IEnumerator ShowRatingPopup()
    {
        Circle.SetActive(false);
        ShareButton.SetActive(false);
        ContinueButton.SetActive(false);
        RetryButton.SetActive(false);
        StarChestFather.SetActive(false);
        for (int i = 0; i < Stars.Length; i++)
        {
            Stars[i].SetActive(false);
        }
        ratingText.text = LanguageManager.instance.GetTheTextByKey(textkey_Rating);
        rateUsText.text = LanguageManager.instance.GetTheTextByKey(textkey_RateUs);
        yield return StartCoroutine(Transitions.scaleUp(ratingObj, new Vector3(0.02f, 0.02f, 0.02f), 0.2f));
        ratingObj.SetActive(true);
        yield return StartCoroutine(Transitions.scaleUp(rateUs, new Vector3(0.02f, 0.02f, 0.02f), 0.2f));
        rateUs.SetActive(true);
        yield return StartCoroutine(Transitions.fadeIn(ratingCross, 1f, 0.3f));
        ratingCross.SetActive(true);
    }

    public void BuyNoAds()
    {
        if(NoAdsCoroutine != null)
            StopCoroutine(NoAdsCoroutine);
        IAPManager.instance.BuyNoAdsProduct();
        NoAdsIsClicked = true;
    }

    private IEnumerator NoAdsPromptIE()
    {
        IAPManager.instance.uiManagerScript = this;
        NoAdsIsClicked = false;
        noAdsTitle.text = LanguageManager.instance.GetTheTextByKey(textkey_NoAdsTitle);
        NoAdsButton.GetComponentInChildren<Text>().text = LanguageManager.instance.GetTheTextByKey(textkey_NoAdsButton);
        noAdsBack.SetActive(true);
        float promptTime = ConfigManager.instance.GetConfig_NoAdsPromptTimer();
        float beatTime = 0.5f;
        int numOfBeats = (int)(promptTime / beatTime);
        NoAdsbar2.transform.localScale = new Vector3(0f, 1f, 1f);
        StartCoroutine(Transitions.setScale(NoAdsbar2, new Vector3(1f, 1f, 1f), promptTime));
        StartCoroutine(Transitions.beatObject(NoAdsButton, 1.2f, numOfBeats, beatTime));
        yield return new WaitForSeconds(promptTime);
        //noAdsBack.SetActive(false);
        if (!NoAdsIsClicked)
        {
            AdManager.instance.ShowInterstitialAd(AdManager.interestitialAdType.After_win, LoadNextLevel);
        }
    }

    public void AfterNoAdsPromptClicked()
    {
        if (Manager.instance.userHasBoughtNoAds)
        {
            LoadNextLevel();
        }
        else
        {
            AdManager.instance.ShowInterstitialAd(AdManager.interestitialAdType.After_win, LoadNextLevel);
        }
    }

    private IEnumerator ToggleIcon(GameObject gameObject, bool turnOn, Sprite onSprite, Sprite offSprite)
    {
        Sounds.instance.PlayUIInteraction();
        VibrationManager.instance.VibrateTouch();
        gameObject.GetComponent<Button>().interactable = false;
        yield return StartCoroutine(Transitions.rotateZ(gameObject, 180, true, 0.1f));
        gameObject.GetComponentsInChildren<Image>()[1].sprite = (turnOn ? onSprite : offSprite);
        yield return StartCoroutine(Transitions.rotateZ(gameObject, 180, true, 0.1f));
        gameObject.GetComponent<Button>().interactable = true;
    }

    public void ToggleSound()
    {
        Manager.instance.isSoundEnabled = !Manager.instance.isSoundEnabled;
        StartCoroutine(ToggleIcon(SoundButton, Manager.instance.isSoundEnabled, SoundOn, SoundOff));
        AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.ingame_sound_set_to, ":" + Manager.instance.isSoundEnabled);
    }

    public void ToggleMusic()
    {
        Manager.instance.isMusicEnabled = !Manager.instance.isMusicEnabled;
        if (Manager.instance.isMusicEnabled)
            Music.instance.StartMusic(Music.PipesMusics.Gameplay);
        else
            Music.instance.StopMusic();
        StartCoroutine(ToggleIcon(MusicButton, Manager.instance.isMusicEnabled, MusicOn, MusicOff));
        AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.ingame_music_set_to, ":" + Manager.instance.isMusicEnabled);
    }

    public void ToggleVibrate()
    {
        Manager.instance.isHapticEnabled = !Manager.instance.isHapticEnabled;
        StartCoroutine(ToggleIcon(VibrateButton, Manager.instance.isHapticEnabled, VibrateOn, VibrateOff));
        AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.ingame_vibrate_set_to, ":" + Manager.instance.isHapticEnabled);
    }

    public void NumOfMovesWarning() {
        Sounds.instance.PlayError();
        VibrationManager.instance.VibrateWarning();
        StartCoroutine(Transitions.beatObject(NumOfMoves.gameObject,1.2f,5,0.2f));
        AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.ingame_x_moves_left);
    }

    private void HandleElementsLocksAndFuncs() {
        if (GameScenes.instance.levelIndex < 20 &&
            !Manager.instance.isInfiniteMovesGivenBeforeFTUE &&
            !Manager.instance.isFTUEInfiniteMoveShown) {
            InfiniteMoveButton.transform.GetChild(1).gameObject.SetActive(false);
            infiniteMovesImage.sprite = lockSprite;
            infiniteMovesImage.transform.localPosition = new Vector3(0, 5, 0);
            InfiniteMoveButton.onClick.AddListener(() => ClickOnLocks(2));
        }
        else if (GameScenes.instance.levelIndex < 20 &&
                 Manager.instance.isInfiniteMovesGivenBeforeFTUE &&
                 !Manager.instance.isFTUEInfiniteMoveShown) {
            Essentials.MakeTint(InfiniteMoveButton.GetComponent<Image>(), 0.9f);
            Essentials.MakeTint(InfiniteMoveButton.transform.GetChild(1).GetComponent<Image>(), 0.9f);
            InfiniteMoveButton.onClick.AddListener(() => ClickOnLocks(2));
        }
        else if (Manager.instance.numOfIMItem == 0 && Manager.instance.isFTUEInfiniteMoveShown)
        {
            ToggleInfiniteMovesPlusAndBadge(true);
        }
        else
        {
            InfiniteMoveButton.onClick.AddListener(() => MakeMovesInfinite());
        }
        if (GameScenes.instance.levelIndex < 13 &&
            !Manager.instance.isHintGivenBeforeFTUE &&
            !Manager.instance.isFTUEHintShown) {
            hintBtn.transform.GetChild(1).gameObject.SetActive(false);
            hintImage.sprite = lockSprite;
            hintImage.transform.localPosition = new Vector3(0, 5, 0);
            hintBtn.onClick.AddListener(() => ClickOnLocks(1));
        }
        else if (GameScenes.instance.levelIndex < 13 &&
                 Manager.instance.isHintGivenBeforeFTUE &&
                 !Manager.instance.isFTUEHintShown) {
            Essentials.MakeTint(hintBtn.GetComponent<Image>(),0.9f);
            Essentials.MakeTint(hintBtn.transform.GetChild(1).GetComponent<Image>(),0.9f);
            hintBtn.onClick.AddListener(() => ClickOnLocks(1));
        }
        else if (Manager.instance.numOfHint == 0 && Manager.instance.isFTUEHintShown)
        {
            ToggleHintPlusAndBadge(true);
        }
        else
        {
            hintBtn.onClick.AddListener(() => levelManger.Hint());
        }
        if (GameScenes.instance.levelIndex <= 2) {
            undoBtn.interactable = true;
            undoImage.sprite = lockSprite;
            undoImage.transform.localPosition = new Vector3(0, 5, 0);
            undoBtn.onClick.AddListener(() => ClickOnLocks(0));
        }
        else if (GameScenes.instance.levelIndex >= 3) {
            undoBtn.onClick.AddListener(() => levelManger.Undo());
        }
    }

    public void ShowBackToMenuPrompt()
    {
        Sounds.instance.PlayUIInteraction();
        VibrationManager.instance.VibrateTouch();
        if (Manager.instance.isInfiniteHeart)
        {
            LoseHeartImage.sprite = infiniteHeartSprite;
            LoseHeartText.text = LanguageManager.instance.GetTheTextByKey(textkey_YouWontLoseHeart);
        }
        else
        {
            LoseHeartImage.sprite = brokenHeartSprite;
            LoseHeartText.text = LanguageManager.instance.GetTheTextByKey(textkey_YouLoseHeart);
        }
        BackToMenuPrompt.SetActive(true);
    }

    public void HideBackToMenuPrompt()
    {
        Sounds.instance.PlayUIInteraction();
        VibrationManager.instance.VibrateTouch();
        BackToMenuPrompt.SetActive(false);
    }

    public void GoToMenuAndLoseAHeart()
    {
        Sounds.instance.PlayUIInteraction();
        VibrationManager.instance.VibrateTouch();
        if (!Manager.instance.isInfiniteHeart)
        {
            Manager.instance.hearts--;
            if (Manager.instance.hearts == 4)
            {
                Manager.instance.heartsRefilledTime = DateTime.Now;
            }
            Manager.instance.Save(false);
            AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.level_failed_heart_lost);
        }
        AdManager.instance.HideBanner();
        GameScenes.instance.LoadMenu(Chapters.instance.GetChapterIndexOfThisLevel(levelManger.levelNum), levelManger.levelNum);
    }

    private IEnumerator MoveStarToChest(GameObject star,float addedStarMoveTime) {
        StartCoroutine(Transitions.scaleUp(star, new Vector3(0.2f, 0.2f, 1f), addedStarMoveTime / 10));
        StartCoroutine(Transitions.moveToPosition(star, starChestTransform.position, addedStarMoveTime));
        yield return new WaitForSeconds(8 * addedStarMoveTime / 10);
        StartCoroutine(Transitions.setScale(star, new Vector3(0.02f, 0.02f, 1), addedStarMoveTime / 10));
        yield return new WaitForSeconds(addedStarMoveTime / 10);
    }

    public void ShowInGameShop()
    {
        IAPManager.instance.uiManagerScript = this;
        TopBar.SetActive(false);
        BottomBar.SetActive(false);
        inGameShopObj.SetActive(true);
        inGameShopScript.InitInGameShop();
    }

    public void UnlockHint() {
        hintBtn.transform.GetChild(1).gameObject.SetActive(true);
        hintImage.sprite = hintSprite;
        hintImage.transform.localPosition = new Vector3(0, 0, 0);
        hintBtn.onClick.RemoveAllListeners();
        hintBtn.onClick.AddListener(() => levelManger.Hint());
    }

    public void UnlockInfiniteMoves() {
        InfiniteMoveButton.transform.GetChild(1).gameObject.SetActive(true);
        infiniteMovesImage.sprite = infiniteMovesSprite;
        infiniteMovesImage.transform.localPosition = new Vector3(0, 0, 0);
        InfiniteMoveButton.onClick.RemoveAllListeners();
        InfiniteMoveButton.onClick.AddListener(() => MakeMovesInfinite());
    }

    public void CheckPowerUpsLock() {
        if (!Manager.instance.isFTUEHintShown)
        {
            Manager.instance.isHintGivenBeforeFTUE = true;
            UnlockHint();
        }
        if (!Manager.instance.isFTUEInfiniteMoveShown)
        {
            Manager.instance.isInfiniteMovesGivenBeforeFTUE = true;
            UnlockInfiniteMoves();
        }
    }

    public void ClickOnLocks(int lockIndex) {
        StartCoroutine(ClickOnLocksIE(lockIndex));
    }

    public IEnumerator ClickOnLocksIE(int lockIndex) {
        yield return StartCoroutine(levelManger.FTUEScript.IntroduceAnElement(null, 7, false, false));
        if(LanguageManager.instance.isLanguageRTL)
            levelManger.FTUEScript.textPanelText.text =
                levelManger.FTUEScript.textPanelText.text.Replace("۲",unlockLevelNumRtl[lockIndex]);
        else
            levelManger.FTUEScript.textPanelText.text =
                levelManger.FTUEScript.textPanelText.text.Replace("2",unlockLevelNum[lockIndex]);
        yield return new WaitForSeconds(lockClickTextTime);
        levelManger.FTUEScript.CallContinueAfterIntroduction();
    }

    public void ToggleHintPlusAndBadge(bool turnOnPlus) {
        GameObject hintPlus = hintBtn.transform.GetChild(2).gameObject;
        hintPlus.SetActive(turnOnPlus);
        hintNumber.transform.parent.gameObject.SetActive(!turnOnPlus);
        if (turnOnPlus) {
            hintBtn.onClick.RemoveAllListeners();
            hintBtn.onClick.AddListener(() => ShowBuyPowerUps(0));
            hintPlus.GetComponent<Button>().onClick.RemoveAllListeners();
            hintPlus.GetComponent<Button>().onClick.AddListener(() => ShowBuyPowerUps(0));
        }
        else
        {
            hintBtn.onClick.RemoveAllListeners();
            hintBtn.onClick.AddListener(() => levelManger.Hint());
        }
    }

    public void ToggleInfiniteMovesPlusAndBadge(bool turnOnPlus) {
        GameObject infiniteMovePlus = InfiniteMoveButton.transform.GetChild(2).gameObject;
        infiniteMovePlus.SetActive(turnOnPlus);
        InfiniteMoveItemNumber.transform.parent.gameObject.SetActive(!turnOnPlus);
        if (turnOnPlus) {
            InfiniteMoveButton.onClick.RemoveAllListeners();
            InfiniteMoveButton.onClick.AddListener(() => ShowBuyPowerUps(1));
            infiniteMovePlus.GetComponent<Button>().onClick.RemoveAllListeners();
            infiniteMovePlus.GetComponent<Button>().onClick.AddListener(() => ShowBuyPowerUps(1));
        }
        else
        {
            InfiniteMoveButton.onClick.RemoveAllListeners();
            InfiniteMoveButton.onClick.AddListener(() => MakeMovesInfinite());
        }
    }

}

