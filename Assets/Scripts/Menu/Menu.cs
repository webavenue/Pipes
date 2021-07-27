using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject pausePanelCircle;
    public Vector3 circleFinalScale;
    public float pauseTime;

    [Header("Bottom Bar")]
    public Button HomeIconButton;
    public Button SettingsIconButton;
    public Button ShopIconButton;
    public Button QuestsIconButton;
    public Button CollectionIconButton;
    public CanvasGroup HomeIconCG;
    public CanvasGroup SettingsIconCG;
    public CanvasGroup ShopIconCG;
    public CanvasGroup QuestsIconCG;
    public CanvasGroup CollectionIconCG;
    public GameObject HomeIcon;
    public GameObject SettingsIcon;
    public GameObject ShopIcon;
    public GameObject QuestsIcon;
    public GameObject CollectionIcon;
    public GameObject HomeFather;
    public GameObject CollectionFather;
    public GameObject ShopFather;
    public GameObject QuestsFather;
    public GameObject SettingsFather;
    public Sprite SelectedTab;
    public Sprite NotSelectedTab;

    [Header("FTUE")]
    public FTUE FTUEScript;
    public GameObject QuestsIconFTUE;
    public GameObject CollectionIconFTUE;
    public GameObject chapterSelectFTUE;

    [Header("Top Bar")]
    public GameObject coins;
    public GameObject hearts;
    public Image heartImage;
    public Sprite heartSprite;
    public Sprite infiniteHeartSprite;
    public Text HeartsNumText;
    public Text HeartsTimerText;
    public Button heartPlus;

    [Header("Home")]
    public Vector2 firstLevelButtonLocalPosition;
    public float currentSizeMultiplier;
    public float eachLevelPositionDiffY;
    public float eachLevelPositionDiffX;
    public float addedYForTheCurrentLevelSize;
    public float snapDiffY;
    public RectTransform contentRect;
    public RectTransform scrollRect;
    public GameObject LevelObjectPool;
    public GameObject ChestObjectPool;
    public GameObject LevelsFather;
    public GameObject ChestsFather;
    public GameObject LevelPrefab;
    public GameObject LevelChestPrefab;
    public GameObject CloudPrefab;
    public GameObject ChapterName;
    public GameObject HomeContentFather;
    public GameObject MiniGame;
    public GameObject BackParticle1;
    public GameObject BackParticle2;
    public GameObject chapterSelectorObj;
    public ChapterSelector chapterSelectorScript;
    public ScrollRect srect;
    public Sprite LevelBackDefaultSprite;
    public Sprite CurrentLevelSprite;
    public Sprite ChestButtonWhiteBack;
    public Sprite ChestButtonGoldBack;
    public Sprite ChestButtonOrangeBack;
    public Sprite ChestButtonWhite;
    public Sprite ChestButtonGold;
    public Sprite ChestButtonGoldOpened;
    public GameObject CurrentLevelLight;
    public GameObject ChestStarLight;

    [Header("Collection")]
    public Text CollectionTitle;
    public Vector2 firstChapterPosition;
    public float eachChapterPositionDiffY;
    public GameObject CollectionChapterPrefab;
    public GameObject CollectionContentFather;
    public Sprite CollectionWhiteBackCard;

    [Header("Settings")]
    public float togglePosX;
    public Text SettingsTitle;
    public GameObject AchievementsObj;
    public GameObject RestoreObjInSettings;
    public GameObject SoundIcon;
    public GameObject MusicIcon;
    public GameObject VibrateIcon;
    public Sprite ToggleBackOn;
    public Sprite ToggleBackOff;
    public Sprite SoundOn;
    public Sprite SoundOff;
    public Sprite MusicOn;
    public Sprite MusicOff;
    public Sprite VibrateOn;
    public Sprite VibrateOff;
    public GameObject SoundToggle;
    public GameObject MusicToggle;
    public GameObject VibrateToggle;
    public Text SoundText;
    public Text MusicText;
    public Text VibrateText;
    public Text AchivementsText;
    public Text RestoreText;

    [Header("Shop")]
    public Shop shopScript;
    public Text ShopTitle;
    public Text InfiniteHeart4;
    public Text InfiniteHeart5;
    public Text InfiniteHeart6;
    public Text Tag1;
    public Text Tag3;
    public Text Tag6;

    [Header("Daily Quests")]
    public Text QuestsTitle;
    public Text TimerDate;
    public Text TimerHour;
    public Text TimerMinute;
    public Text TimerSecond;
    public GameObject redDot;

    [Header("Buy hearts")]
    public GameObject buyHeart;
    public GameObject buyHeartBack;
    public GameObject heartZeroNum;
    public Text refill;
    public Text fullHeartsPriceText;
    public Text adsPlusOneText;
    public int freeHeartTime;
    public Image freeHeartImage;
    public Button buyOneHeartBtn;

    [HideInInspector]
    public static int manuallyShowChapterX = -1;
    [HideInInspector]
    public static int manuallyShowLevelY = -1;

    private int homeLastChapterIndex;
    private int[] xIndex = new int[4];
    private GameObject activePageIcon;
    private GameObject activePageFather;
    private RectTransform snapTarget;
    private GameObject theLevelAfterCurrentChest;
    private float activeTabIconSize;
    private float otherTabsIconSize;
    private int updateThisChapterProgressInCollection = -1;
    private bool isHomeInit;
    private bool isCollectionInit;
    private bool buyHeartIsOnTheScreen;
    private Coroutine timerDecreaserCoroutine;
    private Coroutine HeartTimerCoroutine;
    private MenuTabs currentMenuTab;

    public enum MenuTabs
    {
        Home = 0,
        Collection = 1,
        Shop = 2,
        Quests = 3,
        Settings = 4
    };// IF YOU CHANGE THESE NUMBERS, CHANGE THE CORRESPONDING ARGUMENTS IN FUNCTION CALLS IN UNITY TOO!!!

    private static readonly string textkey_finishLevelX = "FinishLevelX";
    private static readonly string textkey_collection = "Collection";
    private static readonly string textkey_shop = "Store";
    private static readonly string textkey_settings = "Settings";
    private static readonly string textkey_dailyQuests = "DailyQuests";
    private static readonly string textkey_sounds = "Sounds";
    private static readonly string textkey_music = "Music";
    private static readonly string textkey_vibration = "Vibration";
    private static readonly string textkey_achievements = "Achievements";
    private static readonly string textkey_full = "Full";
    private static readonly string textkey_xHours = "XHours";
    private static readonly string textkey_sale = "Sale";
    private static readonly string textkey_mostpopular = "MostPopular";
    private static readonly string textkey_bestvalue = "BestValue";
    private static readonly string textkey_Refill = "Refill";

    public enum Months
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }

    // Start is called before the first frame update
    void Start()
    {
        if (CloudManager.instance.dataLoadedInSplash)
        {
            CloudManager.instance.ShowLoadDataPromptOverlay();
            CloudManager.instance.dataLoadedInSplash = false;
        }
        HandleFTUEEnabling();
        InitMenuVisuals();
        InitBottomBar();
        ShowPage_Home(GetChapterIndexToShow());
        //srect.onValueChanged.AddListener(ListenerMethod);
        FTUEAfterStartMenu();

        AdManager.instance.menu = this;

        StartCoroutine(Transitions.PanelAct(FTUEScript.panelCircle, circleFinalScale, FTUEScript.panelTime));
    }
    
    public void ListenerMethod(Vector2 value)
    {
        Debug.Log("ListenerMethod: " + contentRect.anchoredPosition.y);
    }

    public void InitMenuVisuals()
    {
        InitTopBar();

        QuestsIcon.GetComponentInChildren<Text>().text = System.DateTime.Now.ToString("MMMdd");

        if (QuestsIconButton.interactable &&
            (DateTime.Now - Manager.instance.lastQuestStartTime) > TimeSpan.FromDays(1))
        {
            Manager.instance.isQuestRedPointOn = true;
            redDot.SetActive(true);
        }
        if (Manager.instance.hearts == 5)
        {
            heartPlus.interactable = false;
            Essentials.MakeTint(heartPlus.GetComponent<Image>(), 0.8f);
        }
    }

    public void InitTopBar()
    {
        Text coinsText = coins.GetComponentInChildren<Text>();
        coinsText.text = Manager.instance.coins.ToString();
        if (LanguageManager.instance.isLanguageRTL)
            coinsText.text = LanguageManager.arabicFix(coinsText.text);
        UpdateHeartsNum();
        HeartsNumText.text = Manager.instance.hearts.ToString();
        if (LanguageManager.instance.isLanguageRTL)
            HeartsNumText.text = LanguageManager.arabicFix(HeartsNumText.text);
        int defaultHearts = 5;
        if (!Manager.instance.isInfiniteHeart && Manager.instance.hearts == defaultHearts)
        {
            //heartImage.sprite = heartSprite;
            HeartsNumText.gameObject.SetActive(true);
            HeartsNumText.text = defaultHearts.ToString();
            if (LanguageManager.instance.isLanguageRTL)
                HeartsNumText.text = LanguageManager.arabicFix(HeartsNumText.text);
            HeartsTimerText.text = LanguageManager.instance.GetTheTextByKey(textkey_full);
        }
        else
        {
            if (HeartTimerCoroutine != null)
                StopCoroutine(HeartTimerCoroutine);
            HeartTimerCoroutine = StartCoroutine(NextHeartTimer());
        }
    }

    private void InitBottomBar()
    {
        activeTabIconSize = HomeIcon.GetComponent<RectTransform>().sizeDelta.x;
        otherTabsIconSize = CollectionIcon.GetComponent<RectTransform>().sizeDelta.x;
        QuestsIcon.transform.position = (HomeIcon.transform.position + ShopIcon.transform.position) / 2;
        CollectionIcon.transform.position = (HomeIcon.transform.position + SettingsIcon.transform.position) / 2;
        activePageIcon = HomeIcon;
        activePageFather = HomeFather;
    }

    private int GetChapterIndexToShow()
    {
        if (manuallyShowChapterX != -1)
            return manuallyShowChapterX;
        else
            return Chapters.instance.GetCurrentChapterIndex();
    }

    private int GetLevelIndexToShow()
    {
        if (manuallyShowLevelY != -1)
            return manuallyShowLevelY;
        else
            return Manager.instance.maxSeenLevelNum;
    }

    public void ShowPage_Home(int chapterIndex)
    {
        currentMenuTab = MenuTabs.Home;
        HomeFather.SetActive(true);
        BackParticle1.SetActive(true);
        BackParticle2.SetActive(true);
        coins.SetActive(true);
        hearts.SetActive(true);
        int snapTargetIndex = GetLevelIndexToShow();
        if (!isHomeInit || chapterIndex != homeLastChapterIndex)
        {
            isHomeInit = true;
            homeLastChapterIndex = chapterIndex;
            for (int i = 0; i < xIndex.Length; i++)
            {
                xIndex[i] = i;
            }
            int currentLevel = Manager.instance.maxSeenLevelNum;
            //TODO: Change the min and max according to maxSeenLevelNum
            ChapterName.GetComponentInChildren<Text>().text = Chapters.instance.GetChapterName(chapterIndex);
            int totalNumOfLevels = Chapters.instance.GetChapterCapacity(chapterIndex);
            int totalNumOfLevelsAndChests = Chapters.NumberOfChestsPerChapter + Chapters.instance.GetChapterCapacity(chapterIndex);
            short levelIndex = (short)Chapters.instance.GetStartingShownLevelIndexOfAChapter(chapterIndex);
            int[] chestIndexes = new int[Chapters.NumberOfChestsPerChapter];
            bool moveRight = true;
            int buttonPositionIndex = -1; //-1 for left , 0 for middle and 1 for right
            bool itsAChest;
            bool currentIsAChest = false;
            bool lastWasACurrentChest = false;
            int thisChestIndex;
            float heightOfContent = totalNumOfLevelsAndChests * Mathf.Abs(eachLevelPositionDiffY) + addedYForTheCurrentLevelSize;
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, heightOfContent);
            Vector2 thisLevelButtonPosition =
                new Vector2(firstLevelButtonLocalPosition.x + (-eachLevelPositionDiffX / 2), firstLevelButtonLocalPosition.y + heightOfContent);
            Chapters.instance.FillChapterChestIndexes(chapterIndex, chestIndexes);
            int maxLevelIndexOfThisChapter = Chapters.instance.GetLastShownLevelIndexOfAChapter(chapterIndex);
            itsAChest = false;
            thisChestIndex = 0;

            //Init the Pools
            if(LevelObjectPool.transform.childCount != totalNumOfLevels)
            {
                int totalChildCount = LevelsFather.transform.childCount;
                for (int i = 0; i < totalChildCount; i++)
                {
                    GameObject theChild = LevelsFather.transform.GetChild(0).gameObject;
                    theChild.transform.SetParent(LevelObjectPool.transform);
                    ResetThisLevelButton(theChild);
                    theChild.SetActive(false);
                }
                int missingNumOfLevels = totalNumOfLevels - LevelObjectPool.transform.childCount;
                for (int i = 0; i < missingNumOfLevels; i++)
                {
                    Instantiate(LevelPrefab, LevelObjectPool.transform, false);
                }
            }
            if(ChestObjectPool.transform.childCount != Chapters.NumberOfChestsPerChapter)
            {
                int totalChildCount = ChestsFather.transform.childCount;
                for (int i = 0; i < totalChildCount; i++)
                {
                    GameObject theChild = ChestsFather.transform.GetChild(0).gameObject;
                    theChild.transform.SetParent(ChestObjectPool.transform);
                    ResetThisChestButton(theChild);
                    theChild.SetActive(false);
                }
                int missingNumOfChests = Chapters.NumberOfChestsPerChapter - ChestObjectPool.transform.childCount;
                for (int i = 0; i < missingNumOfChests; i++)
                {
                    Instantiate(LevelChestPrefab, ChestObjectPool.transform, false);
                }
            }
            
            while (levelIndex <= maxLevelIndexOfThisChapter)
            {
                if (itsAChest)
                {
                    GameObject thisChestButton = ChestObjectPool.transform.GetChild(0).gameObject;
                    thisChestButton.transform.SetParent(ChestsFather.transform);
                    thisChestButton.transform.localScale = new Vector3(1f, 1f, 1f);
                    thisChestButton.GetComponent<RectTransform>().anchoredPosition = thisLevelButtonPosition;
                    thisChestButton.SetActive(true);
                    int thisChestIndexIndex = thisChestIndex;
                    int thisChapterIndex = chapterIndex;
                    short chestKey = (short)(thisChapterIndex * 10 + thisChestIndexIndex);
                    currentIsAChest = false;
                    if (Manager.instance.chestCards.ContainsKey(chestKey) && Manager.instance.chestCards[chestKey])
                    {
                        MakeChestGold(thisChestButton, Manager.instance.chestCards[chestKey]);
                    }
                    else
                    {
                        Instantiate(ChestStarLight, thisChestButton.transform.position, Quaternion.identity, thisChestButton.transform);
                        if (Manager.instance.levelStars.ContainsKey(levelIndex) || levelIndex < currentLevel)
                        {
                            Debug.Log("HHHHHHHHHappened");
                            currentIsAChest = true;
                            lastWasACurrentChest = true;
                            Vector2 currentScale = thisChestButton.transform.localScale;
                            currentScale.x *= currentSizeMultiplier;
                            currentScale.y *= currentSizeMultiplier;
                            thisChestButton.transform.localScale = currentScale;
                            Manager.instance.chestCards[chestKey] = false;
                            snapTarget = thisChestButton.GetComponent<RectTransform>();
                            Instantiate(CurrentLevelLight, thisChestButton.transform.position,
                                Quaternion.identity, thisChestButton.transform);
                            MakeChestGold(thisChestButton, Manager.instance.chestCards[chestKey]);
                        }
                    }
                    thisChestButton.GetComponent<Button>().onClick.AddListener(() => LoadChest(thisChapterIndex, thisChestIndexIndex, thisChestButton));
                    thisChestIndex++;
                    if (thisChestIndex == Chapters.NumberOfChestsPerChapter)
                        thisChestIndex = 0;
                }
                else
                {
                    GameObject thisLevelButton = LevelObjectPool.transform.GetChild(0).gameObject;
                    thisLevelButton.transform.SetParent(LevelsFather.transform);
                    thisLevelButton.transform.localScale = new Vector3(1f, 1f, 1f);
                    thisLevelButton.GetComponent<RectTransform>().anchoredPosition = thisLevelButtonPosition;
                    thisLevelButton.SetActive(true);
                    Button thisLevelButtonBtn = thisLevelButton.GetComponent<Button>();

                    if(snapTargetIndex != -1 && snapTargetIndex == levelIndex)
                    {
                        snapTarget = thisLevelButton.GetComponent<RectTransform>();
                    }
                    if (levelIndex == (short)Chapters.instance.GetStartingShownLevelIndexOfAChapter(chapterIndex) && 
                        (currentLevel < Chapters.instance.GetStartingShownLevelIndexOfAChapter(chapterIndex) || currentLevel > Chapters.instance.GetLastShownLevelIndexOfAChapter(chapterIndex))) 
                    {
                        snapTarget = thisLevelButton.GetComponent<RectTransform>();
                    }
                    if (lastWasACurrentChest)
                    {
                        theLevelAfterCurrentChest = thisLevelButton;
                        lastWasACurrentChest = false;
                    }
                    short levelIndexIndex = levelIndex;
                    thisLevelButtonBtn.onClick.AddListener(() => RoadMapLevelClick(levelIndexIndex));
                    if (levelIndex >= Manager.instance.maxSeenLevelNum) {
                        // make current level true when make it gold.
                        thisLevelButtonBtn.interactable = false;
                    }
                    else
                    {
                        thisLevelButtonBtn.interactable = true;
                    }

                    Text thisLevelText = thisLevelButton.GetComponentInChildren<Text>();
                    thisLevelText.text = levelIndex.ToString();
                    if (LanguageManager.instance.isLanguageRTL)
                        thisLevelText.text = LanguageManager.arabicFix(thisLevelText.text);
                    int numOfStars = 0;
                    // We only store levels that the player has earned 1 or 2 stars; other levels stars can be calculated by the maxLevelNumber the player has reached.
                    if (Manager.instance.levelStars.ContainsKey(levelIndex))
                    {
                        if (Manager.instance.levelStars[levelIndex])
                        {
                            numOfStars = 2;
                        }
                        else
                        {
                            numOfStars = 1;
                        }
                    }
                    else
                    {
                        numOfStars = (levelIndex >= currentLevel ? 0 : 3);
                    }
                    if (numOfStars >= 1)
                    {
                        //PASSED LEVEL
                        thisLevelButton.transform.GetChild(4).gameObject.SetActive(true);
                        thisLevelButton.transform.GetChild(13).gameObject.SetActive(true);
                        if (numOfStars >= 2)
                        {
                            thisLevelButton.transform.GetChild(5).gameObject.SetActive(true);
                            thisLevelButton.transform.GetChild(14).gameObject.SetActive(true);
                            if (numOfStars == 3)
                            {
                                thisLevelButton.transform.GetChild(6).gameObject.SetActive(true);
                                thisLevelButton.transform.GetChild(15).gameObject.SetActive(true);
                            }
                        }
                    }
                    else
                    {
                        if (levelIndex != currentLevel || currentIsAChest)
                        {
                            //LOCKED LEVEL
                            //TODO : Make the Button beat when its clicked, dont load the level!
                            thisLevelButton.transform.GetChild(7).gameObject.SetActive(false);
                            thisLevelButton.transform.GetChild(8).gameObject.SetActive(false);
                            thisLevelButton.transform.GetChild(9).gameObject.SetActive(false);
                            thisLevelButton.transform.GetChild(10).gameObject.SetActive(false);
                            thisLevelButton.transform.GetChild(11).gameObject.SetActive(false);
                            thisLevelButton.transform.GetChild(12).gameObject.SetActive(false);
                        }
                        else
                        {
                            //CURRENT LEVEL
                            MakeThisLevelCurrent(thisLevelButton);
                        }
                    }
                }
                thisLevelButtonPosition.y += eachLevelPositionDiffY;
                /*if (moveRight)
                {
                    thisLevelButtonPosition.x += eachLevelPositionDiffX;
                    moveRight = !moveRight;
                }
                else
                {
                    thisLevelButtonPosition.x -= eachLevelPositionDiffX;
                    moveRight = !moveRight;
                }*/
                if (moveRight && buttonPositionIndex==-1)
                {
                    buttonPositionIndex = 0;
                    thisLevelButtonPosition.x += eachLevelPositionDiffX;
                }
                else if (moveRight && buttonPositionIndex==0)
                {
                    buttonPositionIndex = 1;
                    thisLevelButtonPosition.x += eachLevelPositionDiffX;
                    moveRight = false;
                }
                else if (!moveRight && buttonPositionIndex==1)
                {
                    buttonPositionIndex = 0;
                    thisLevelButtonPosition.x -= eachLevelPositionDiffX;
                }
                else if (!moveRight && buttonPositionIndex==0)
                {
                    buttonPositionIndex = -1;
                    thisLevelButtonPosition.x -= eachLevelPositionDiffX;
                    moveRight = true;
                }
                if (!itsAChest && levelIndex + 1 == chestIndexes[thisChestIndex])
                {
                    itsAChest = true;
                }
                else
                {
                    itsAChest = false;
                    levelIndex++;
                }
            }
            //theCloud = Instantiate(CloudPrefab, HomeContentFather.transform, false);
        }

        RectTransform LevelsRect = LevelsFather.GetComponent<RectTransform>();
        LevelsRect.anchoredPosition = new Vector2(LevelsRect.anchoredPosition.x, -contentRect.sizeDelta.y);
        RectTransform ChestsRect = ChestsFather.GetComponent<RectTransform>();
        ChestsRect.anchoredPosition = new Vector2(ChestsRect.anchoredPosition.x, -contentRect.sizeDelta.y);
        SnapToCurrentRect();
    }

    private void ShowPage_Collection()
    {
        currentMenuTab = MenuTabs.Collection;
        CollectionFather.SetActive(true);
        BackParticle1.SetActive(false);
        BackParticle2.SetActive(false);
        coins.SetActive(false);
        hearts.SetActive(false);
        if (!isCollectionInit)
        {
            isCollectionInit = true;
            CollectionTitle.text = LanguageManager.instance.GetTheTextByKey(textkey_collection);
            int[] chestIndexes = new int[Chapters.NumberOfChestsPerChapter];
            RectTransform contentRect = CollectionContentFather.GetComponent<RectTransform>();
            float heightOfContent = Chapters.NumberOfChapters * Mathf.Abs(eachChapterPositionDiffY);
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, heightOfContent);
            //contentRect.anchoredPosition =
            //    new Vector2(contentRect.anchoredPosition.x, contentRect.anchoredPosition.y + (firstshownchapterindex - 1) * Mathf.Abs(eachChapterPositionDiffY));
            Vector2 thisChapterCollectionPosition =
                new Vector2(firstChapterPosition.x, firstChapterPosition.y);
            for (int c = 0; c < Chapters.NumberOfChapters; c++)
            {
                GameObject thisChapterCollection = Instantiate(CollectionChapterPrefab, CollectionContentFather.transform, false);
                Chapters.instance.FillChapterChestIndexes(c, chestIndexes);
                thisChapterCollection.GetComponent<RectTransform>().localPosition = thisChapterCollectionPosition;
                thisChapterCollectionPosition.y -= eachChapterPositionDiffY;
                thisChapterCollection.transform.GetChild(0).GetComponentInChildren<Text>().text = Chapters.instance.GetChapterName(c);
                float chapterProgress = 0f;
                for (int i = 0; i < Chapters.NumberOfChestsPerChapter; i++)
                {
                    int childIndex = i + 2;
                    GameObject thisChest = thisChapterCollection.transform.GetChild(childIndex).gameObject;
                    short chestKey = (short)(c * 10 + i);
                    if (Manager.instance.chestCards.ContainsKey(chestKey) && Manager.instance.chestCards[chestKey])
                    {
                        thisChest.GetComponent<Image>().sprite = CollectionWhiteBackCard;
                        thisChest.transform.GetChild(0).gameObject.SetActive(false);
                        thisChest.transform.GetChild(1).gameObject.SetActive(false);
                        GameObject theImage = thisChest.transform.GetChild(2).gameObject;
                        theImage.SetActive(true);
                        chapterProgress += 1f / (float)Chapters.NumberOfChestsPerChapter;
                        theImage.GetComponent<Image>().sprite = Chapters.instance.GetChapterThingSprite(c, i);
                        //thisChest.transform.GetChild(1).GetComponentInChildren<Text>().text = "Avocado";
                    }
                    else
                    {
                        if (LanguageManager.instance.isLanguageRTL)
                        {
                            thisChest.transform.GetChild(1).GetComponent<Text>().text =
                                LanguageManager.instance.GetTheTextByKey(textkey_finishLevelX)
                                    .Replace("۱۲", "<b>" + LanguageManager.arabicFix((chestIndexes[i] - 1).ToString()) + "</b>");
                        }
                        else
                        {
                            thisChest.transform.GetChild(1).GetComponent<Text>().text =
                                LanguageManager.instance.GetTheTextByKey(textkey_finishLevelX).Replace("12", "<b>" + (chestIndexes[i] - 1).ToString() + "</b>");
                        }
                    }
                }
                thisChapterCollection.transform.GetChild(1).GetChild(0).GetComponent<Image>().fillAmount = chapterProgress;
            }
            updateThisChapterProgressInCollection = -1; // there is no need to update!
        }
        else
        {
            if (updateThisChapterProgressInCollection != -1)
            {
                CollectionPageUpdateChapterProgress(updateThisChapterProgressInCollection);
                updateThisChapterProgressInCollection = -1;
            }
        }
    }

    private void ShowPage_Shop()
    {
        currentMenuTab = MenuTabs.Shop;
        int package4_inifiteHeartHours = 6;
        int package5_inifiteHeartHours = 12;
        int package6_inifiteHeartHours = 24;
        IAPManager.instance.menuScript = this;
        ShopFather.SetActive(true);
        BackParticle1.SetActive(false);
        BackParticle2.SetActive(false);
        coins.SetActive(true);
        hearts.SetActive(true);
        ShopTitle.text = LanguageManager.instance.GetTheTextByKey(textkey_shop);
        Tag1.text = LanguageManager.instance.GetTheTextByKey(textkey_sale);
        Tag3.text = LanguageManager.instance.GetTheTextByKey(textkey_mostpopular);
        Tag6.text = LanguageManager.instance.GetTheTextByKey(textkey_bestvalue);
        if (LanguageManager.instance.isLanguageRTL)
        {
            InfiniteHeart4.text = LanguageManager.instance.GetTheTextByKey(textkey_xHours)
                .Replace("۱۲", LanguageManager.arabicFix(package4_inifiteHeartHours.ToString()));
            InfiniteHeart5.text = LanguageManager.instance.GetTheTextByKey(textkey_xHours)
                .Replace("۱۲", LanguageManager.arabicFix(package5_inifiteHeartHours.ToString()));
            InfiniteHeart6.text = LanguageManager.instance.GetTheTextByKey(textkey_xHours)
                .Replace("۱۲", LanguageManager.arabicFix(package6_inifiteHeartHours.ToString()));
        }
        else
        {
            InfiniteHeart4.text = LanguageManager.instance.GetTheTextByKey(textkey_xHours).Replace("12", package4_inifiteHeartHours.ToString());
            InfiniteHeart5.text = LanguageManager.instance.GetTheTextByKey(textkey_xHours).Replace("12", package5_inifiteHeartHours.ToString());
            InfiniteHeart6.text = LanguageManager.instance.GetTheTextByKey(textkey_xHours).Replace("12", package6_inifiteHeartHours.ToString());
        }
    }

    private void ShowPage_Quests()
    {
        currentMenuTab = MenuTabs.Quests;
        QuestsFather.SetActive(true);
        BackParticle1.SetActive(false);
        BackParticle2.SetActive(false);
        coins.SetActive(true);
        hearts.SetActive(true);
        QuestsTitle.text = LanguageManager.instance.GetTheTextByKey(textkey_dailyQuests);
        
        TimerDate.text = LanguageManager.instance.GetDateText();
        if (timerDecreaserCoroutine == null)
            timerDecreaserCoroutine = StartCoroutine(TimerDecreaser());
    }

    private void ShowPage_Settings()
    {
        currentMenuTab = MenuTabs.Settings;
        SettingsFather.SetActive(true);
        BackParticle1.SetActive(false);
        BackParticle2.SetActive(false);
        coins.SetActive(true);
        hearts.SetActive(true);
#if UNITY_ANDROID
        RestoreObjInSettings.SetActive(false);
        Vector3 AchievePos = AchievementsObj.transform.position;
        AchievePos.x = 0;
        AchievementsObj.transform.position = AchievePos;
#endif
        RestoreText.text = LanguageManager.instance.GetTheTextByKey(Shop.textkey_Restore);
        SettingsTitle.text = LanguageManager.instance.GetTheTextByKey(textkey_settings);
        SoundText.text = LanguageManager.instance.GetTheTextByKey(textkey_sounds);
        MusicText.text = LanguageManager.instance.GetTheTextByKey(textkey_music);
        VibrateText.text = LanguageManager.instance.GetTheTextByKey(textkey_vibration);
        AchivementsText.text = LanguageManager.instance.GetTheTextByKey(textkey_achievements);
        SetToggleCirclePos(SoundToggle.transform.GetChild(0).gameObject, Manager.instance.isSoundEnabled);
        SetToggleCirclePos(MusicToggle.transform.GetChild(0).gameObject, Manager.instance.isMusicEnabled);
        SetToggleCirclePos(VibrateToggle.transform.GetChild(0).gameObject, Manager.instance.isHapticEnabled);
        if (!Manager.instance.isSoundEnabled)
        {
            SoundToggle.GetComponentsInChildren<Image>()[0].sprite = ToggleBackOff;
            SoundIcon.GetComponent<Image>().sprite = SoundOff;
        }
        else
        {
            SoundToggle.GetComponentsInChildren<Image>()[0].sprite = ToggleBackOn;
            SoundIcon.GetComponent<Image>().sprite = SoundOn;
        }
        if (!Manager.instance.isMusicEnabled)
        {
            MusicToggle.GetComponentsInChildren<Image>()[0].sprite = ToggleBackOff;
            MusicIcon.GetComponent<Image>().sprite = MusicOff;
        }
        else
        {
            MusicToggle.GetComponentsInChildren<Image>()[0].sprite = ToggleBackOn;
            MusicIcon.GetComponent<Image>().sprite = MusicOn;
        }
        if (!Manager.instance.isHapticEnabled)
        {
            VibrateToggle.GetComponentsInChildren<Image>()[0].sprite = ToggleBackOff;
            VibrateIcon.GetComponent<Image>().sprite = VibrateOff;
        }
        else
        {
            VibrateToggle.GetComponentsInChildren<Image>()[0].sprite = ToggleBackOn;
            VibrateIcon.GetComponent<Image>().sprite = VibrateOn;
        }
    }

    private void SetToggleCirclePos(GameObject toggleCircle, bool enabled)
    {
        RectTransform toggleRect = toggleCircle.GetComponent<RectTransform>();
        Vector3 toggleFinalPos = toggleRect.anchoredPosition;
        toggleFinalPos.x = (enabled ? togglePosX : -togglePosX);
        toggleRect.anchoredPosition = toggleFinalPos;
    }

    public void LoadSpecificTab(int theTabToLoad)
    {
        StartCoroutine(LoadSpecificTabIE(theTabToLoad));
    }

    private IEnumerator LoadSpecificTabIE(int theTabToLoad)
    {
        if ((MenuTabs)theTabToLoad == currentMenuTab)
            yield break;
        HomeIconButton.interactable = false;
        CollectionIconButton.interactable = false;
        ShopIconButton.interactable = false;
        QuestsIconButton.interactable = false;
        SettingsIconButton.interactable = false;
        if (currentMenuTab == MenuTabs.Settings)
            Manager.instance.Save(false);
        float iconChangeTime = 0.2f;
        GameObject theTabIcon;
        GameObject theTabFather;
        activePageFather.SetActive(false);
        activePageIcon.GetComponent<Image>().sprite = NotSelectedTab;
        StartCoroutine(Transitions.ChangeScaleImage(activePageIcon, otherTabsIconSize, otherTabsIconSize, iconChangeTime));
        switch ((MenuTabs)theTabToLoad)
        {
            case MenuTabs.Home:
                theTabIcon = HomeIcon;
                theTabFather = HomeFather;
                AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.roadmap_opened);
                ShowPage_Home(GetChapterIndexToShow());
                break;
            case MenuTabs.Collection:
                theTabIcon = CollectionIcon;
                theTabFather = CollectionFather;
                AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.collection_opened);
                ShowPage_Collection();
                break;
            case MenuTabs.Shop:
                theTabIcon = ShopIcon;
                theTabFather = ShopFather;
                AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.shop_opened);
                ShowPage_Shop();
                break;
            case MenuTabs.Quests:
                theTabIcon = QuestsIcon;
                theTabFather = QuestsFather;
                AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.quests_opened);
                ShowPage_Quests();
                break;
            case MenuTabs.Settings:
                theTabIcon = SettingsIcon;
                theTabFather = SettingsFather;
                AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.settings_opened);
                ShowPage_Settings();
                break;
            default:
                theTabIcon = HomeIcon;
                theTabFather = HomeFather;
                ShowPage_Home(GetChapterIndexToShow());
                break;
        }
        theTabIcon.GetComponent<Image>().sprite = SelectedTab;
        yield return StartCoroutine(Transitions.ChangeScaleImage(theTabIcon, activeTabIconSize, activeTabIconSize, iconChangeTime));
        activePageIcon = theTabIcon;
        activePageFather = theTabFather;
        HomeIconButton.interactable = true;
        CollectionIconButton.interactable = true;
        ShopIconButton.interactable = true;
        QuestsIconButton.interactable = true;
        SettingsIconButton.interactable = true;
        HandleFTUEEnabling();
    }

    public void LoadLevel(short index)
    {
        Debug.Log("Level is: " + index);
        GameScenes.instance.LoadSpecificLevel(index);
    }

    public void LoadChest(int chapterIndex, int chestIndex, GameObject chestButton)
    {
        Sounds.instance.PlayChestOpening();
        VibrationManager.instance.VibrateSuccess();
        Quests.CheckQuestType(QuestType.OpenChests, true, 1, false, true);
        Quests.CheckQuestType(QuestType.OpenLevelChest, true, 1, false, true);
        //Debug.Log("Chapter Index is: " + chapterIndex + " Chest Index is: " + chestIndex);
        MiniGame miniGame = MiniGame.GetComponent<MiniGame>();
        miniGame.chestButton = chestButton;
        MiniGame.SetActive(true);
        miniGame.GenerateObjects(chapterIndex, chestIndex);
    }

    public void MakeChestGold(GameObject chestButton, bool opened, bool chestIsJustOpened = false, bool ItsThirdChestOfChapter = false)
    {
        chestButton.GetComponentsInChildren<Image>()[0].sprite = opened ? ChestButtonGoldBack : ChestButtonOrangeBack;
        chestButton.GetComponentsInChildren<Image>()[1].sprite = opened ? ChestButtonGoldOpened : ChestButtonGold;
        chestButton.GetComponent<Button>().interactable = !opened;
        if (chestIsJustOpened)
        {
            if(theLevelAfterCurrentChest)
                MakeThisLevelCurrent(theLevelAfterCurrentChest);
            if (chestButton.transform.childCount == 3)
            {
                Vector2 defualtScale = LevelChestPrefab.transform.localScale;
                chestButton.transform.localScale = defualtScale;
                Destroy(chestButton.transform.GetChild(2).gameObject);
                Destroy(chestButton.transform.GetChild(1).gameObject);
            }
            if (isCollectionInit)
            {
                isCollectionInit = false;
                for (int i = 0; i < CollectionContentFather.transform.childCount; i++)
                {
                    Destroy(CollectionContentFather.transform.GetChild(i).gameObject);
                }
            }
            if (ItsThirdChestOfChapter)
            {
                ShowPage_Home(homeLastChapterIndex + 1);
                if (Manager.instance.maxSeenLevelNum == 21 && !Manager.instance.isFTUEChapterShown)
                {
                    StartCoroutine(FTUEScript.IntroduceAnElement(chapterSelectFTUE, 8, true, true));
                    chapterSelectFTUE.SetActive(true);
                    chapterSelectFTUE.transform.position = ChapterName.transform.position;
                    Text chapterSelectorTextFTUE = chapterSelectFTUE.transform.GetComponentInChildren<Text>();
                    chapterSelectorTextFTUE.text = ChapterName.GetComponentInChildren<Text>().text;
                    if (LanguageManager.instance.isLanguageRTL)
                        chapterSelectorTextFTUE.text = LanguageManager.arabicFix(chapterSelectorTextFTUE.text);
                }
            }
        }
    }
    
    public void ResetThisChestButton(GameObject chestButton)
    {
        Vector2 defualtScale = LevelChestPrefab.transform.localScale;
        chestButton.GetComponentsInChildren<Image>()[0].sprite = ChestButtonWhiteBack;
        chestButton.GetComponentsInChildren<Image>()[1].sprite = ChestButtonWhite;
        chestButton.transform.localScale = defualtScale;
        if (chestButton.transform.childCount == 3)
        {
            Destroy(chestButton.transform.GetChild(2).gameObject);
            Destroy(chestButton.transform.GetChild(1).gameObject);
        }
        else if (chestButton.transform.childCount == 2)
        {
            Destroy(chestButton.transform.GetChild(1).gameObject);
        }
    }

    public void UpdateProgressOfThisChapter(int chapterIndex)
    {
        updateThisChapterProgressInCollection = chapterIndex;
    }

    private void CollectionPageUpdateChapterProgress(int chapterIndex)
    {
        float chapterProgress = 0f;

        for (int i = 0; i < Chapters.NumberOfChestsPerChapter; i++)
        {
            short chestKey = (short)(chapterIndex * 10 + i);
            if (Manager.instance.chestCards.ContainsKey(chestKey))
            {
                if (Manager.instance.chestCards[chestKey])
                    chapterProgress += 1f / (float)Chapters.NumberOfChestsPerChapter;
            }
        }
        Debug.Log(chapterIndex + "PPPPP");
        CollectionFather.transform.GetChild(chapterIndex).GetChild(1).GetChild(0).GetComponent<Image>().fillAmount = chapterProgress;
    }

    public void SnapToCurrentRect()
    {
        if (Manager.instance.maxSeenLevelNum > 4)
        {
            Canvas.ForceUpdateCanvases();

            float diffY = scrollRect.transform.InverseTransformPoint(contentRect.position).y -
                scrollRect.transform.InverseTransformPoint(snapTarget.position).y;
            Vector2 snappedPos = new Vector2(contentRect.anchoredPosition.x, diffY + snapDiffY);
            contentRect.anchoredPosition = snappedPos;
        }
    }

    private void MakeThisLevelCurrent(GameObject thisLevelButton)
    {
        thisLevelButton.GetComponent<Button>().interactable = true;
        Vector2 currentScale = thisLevelButton.transform.localScale;
        thisLevelButton.GetComponent<Image>().sprite = CurrentLevelSprite;
        thisLevelButton.transform.GetChild(1).gameObject.SetActive(false);
        thisLevelButton.transform.GetChild(2).gameObject.SetActive(false);
        thisLevelButton.transform.GetChild(3).gameObject.SetActive(false);
        thisLevelButton.transform.GetChild(7).gameObject.SetActive(false);
        thisLevelButton.transform.GetChild(8).gameObject.SetActive(false);
        thisLevelButton.transform.GetChild(9).gameObject.SetActive(false);
        thisLevelButton.transform.GetChild(10).gameObject.SetActive(false);
        thisLevelButton.transform.GetChild(11).gameObject.SetActive(false);
        thisLevelButton.transform.GetChild(12).gameObject.SetActive(false);
        if (currentScale.x == 1) {
            currentScale.x *= currentSizeMultiplier;
            currentScale.y *= currentSizeMultiplier;
        }
        thisLevelButton.transform.localScale = currentScale;
        Instantiate(CurrentLevelLight, thisLevelButton.transform.position,
            Quaternion.identity, thisLevelButton.transform);
        snapTarget = thisLevelButton.GetComponent<RectTransform>();
    }

    private void ResetThisLevelButton(GameObject thisLevelButton)
    {
        Vector2 defualtScale = LevelPrefab.transform.localScale;
        thisLevelButton.GetComponent<Image>().sprite = LevelBackDefaultSprite;
        thisLevelButton.transform.GetChild(1).gameObject.SetActive(true);
        thisLevelButton.transform.GetChild(2).gameObject.SetActive(true);
        thisLevelButton.transform.GetChild(3).gameObject.SetActive(true);
        thisLevelButton.transform.GetChild(4).gameObject.SetActive(false);
        thisLevelButton.transform.GetChild(5).gameObject.SetActive(false);
        thisLevelButton.transform.GetChild(6).gameObject.SetActive(false);
        thisLevelButton.transform.GetChild(7).gameObject.SetActive(true);
        thisLevelButton.transform.GetChild(8).gameObject.SetActive(true);
        thisLevelButton.transform.GetChild(9).gameObject.SetActive(true);
        thisLevelButton.transform.GetChild(10).gameObject.SetActive(true);
        thisLevelButton.transform.GetChild(11).gameObject.SetActive(true);
        thisLevelButton.transform.GetChild(12).gameObject.SetActive(true);
        thisLevelButton.transform.GetChild(13).gameObject.SetActive(false);
        thisLevelButton.transform.GetChild(14).gameObject.SetActive(false);
        thisLevelButton.transform.GetChild(15).gameObject.SetActive(false);
        if (thisLevelButton.transform.childCount == 17)
            Destroy(thisLevelButton.transform.GetChild(16).gameObject);
        thisLevelButton.transform.localScale = defualtScale;
    }

    public static void UpdateHeartsNum()
    {
        if (Manager.instance.isInfiniteHeart)
        {
            int infiniteHeartSeconds = (int)(DateTime.Now - Manager.instance.heartsRefilledTime).TotalSeconds;
            if (infiniteHeartSeconds >= Manager.instance.infiniteHeartMinute * 60)
            {
                Manager.instance.isInfiniteHeart = false;
                Manager.instance.hearts = 5;
                Manager.instance.heartsRefilledTime = DateTime.MinValue;
                Manager.instance.Save(false);
            }
        }
        else
        {
            if (Manager.instance.heartsRefilledTime == DateTime.MinValue)
            {
                Manager.instance.hearts = 5;
                Manager.instance.Save(false);
            }
            else
            {
                // calculate 30 min for each new heart.
                int addedHearts = (int)(DateTime.Now - Manager.instance.heartsRefilledTime).TotalSeconds / 1800;
                Manager.instance.hearts += addedHearts;
                if (Manager.instance.hearts >= 5)
                {
                    Manager.instance.hearts = 5;
                    Manager.instance.heartsRefilledTime = DateTime.MinValue;
                }
                else
                {
                    Manager.instance.heartsRefilledTime =
                        Manager.instance.heartsRefilledTime.Add(new TimeSpan(0, 0, 1800 * addedHearts));
                }
                Manager.instance.Save(false);
            }
        }
    }

    public IEnumerator NextHeartTimer()
    {
        int infiniteHeartDays = 0;
        TimeSpan heartTime = DateTime.Now - Manager.instance.heartsRefilledTime;
        if (Manager.instance.isInfiniteHeart)
        {
            HeartsNumText.gameObject.SetActive(false);
            heartImage.sprite = infiniteHeartSprite;
            //HeartsNumText.text = "∞";
            heartTime = new TimeSpan(0, Manager.instance.infiniteHeartMinute, 0) - heartTime;
            infiniteHeartDays = heartTime.Days;
        }
        else
        {
            HeartsNumText.gameObject.SetActive(true);
            heartImage.sprite = heartSprite;
            heartTime = new TimeSpan(0, 30, 0) - heartTime;
            HeartsNumText.text = Manager.instance.hearts.ToString();
            if (LanguageManager.instance.isLanguageRTL)
                HeartsNumText.text = LanguageManager.arabicFix(HeartsNumText.text);
        }
        do
        {
            if (Manager.instance.isInfiniteHeart && Manager.instance.infiniteHeartMinute > 60)
            {
                HeartsTimerText.text = heartTime.ToString(@"hh\:mm\:ss");
                HeartsTimerText.text =
                    (int.Parse(HeartsTimerText.text.Substring(0, 2)) + infiniteHeartDays * 24).ToString() +
                        HeartsTimerText.text.Substring(2, 6);
                if (LanguageManager.instance.isLanguageRTL)
                    HeartsTimerText.text = LanguageManager.arabicFix(HeartsTimerText.text);
            }
            else
            {
                HeartsTimerText.text = heartTime.ToString(@"hh\:mm\:ss").Substring(3, 5);
                if (LanguageManager.instance.isLanguageRTL)
                    HeartsTimerText.text = LanguageManager.arabicFix(HeartsTimerText.text);
            }
            yield return new WaitForSeconds(1);
            heartTime = heartTime.Subtract(new TimeSpan(0, 0, 1));
        } while (heartTime.TotalSeconds >= 0);
        if (Manager.instance.isInfiniteHeart)
        {
            Manager.instance.isInfiniteHeart = false;
            Manager.instance.hearts = 5;
            heartImage.sprite = heartSprite;
            HeartsNumText.gameObject.SetActive(true);
            HeartsNumText.text = Manager.instance.hearts.ToString();
            if (LanguageManager.instance.isLanguageRTL)
                HeartsNumText.text = LanguageManager.arabicFix(HeartsNumText.text);
            StartCoroutine(Transitions.stamp(HeartsNumText.gameObject, new Vector3(3f, 3f, 1), 0.2f));
            HeartsTimerText.text = LanguageManager.instance.GetTheTextByKey(textkey_full);
            Manager.instance.heartsRefilledTime = DateTime.MinValue;
            Manager.instance.Save(false);
        }
        else
        {
            Sounds.instance.PlayChestItems();
            Manager.instance.hearts++;
            HeartsNumText.text = Manager.instance.hearts.ToString();
            if (LanguageManager.instance.isLanguageRTL)
                HeartsNumText.text = LanguageManager.arabicFix(HeartsNumText.text);
            StartCoroutine(Transitions.stamp(HeartsNumText.gameObject, new Vector3(3f, 3f, 1), 0.2f));
            if (Manager.instance.hearts == 5)
            {
                HeartsTimerText.text = LanguageManager.instance.GetTheTextByKey(textkey_full);
                Manager.instance.heartsRefilledTime = DateTime.MinValue;
                Manager.instance.Save(false);
            }
            else
            {
                Manager.instance.heartsRefilledTime =
                    Manager.instance.heartsRefilledTime.Add(new TimeSpan(0, 0, 1800));
                Manager.instance.Save(false);
                HeartTimerCoroutine = StartCoroutine(NextHeartTimer());
            }
        }
    }

    private IEnumerator TimerDecreaser()
    {
        int hour;
        int minute;
        int second;

        do
        {
            DateTime now = DateTime.Now;
            hour = 23 - now.Hour;
            minute = 59 - now.Minute;
            second = 59 - now.Second;
            TimerHour.text = hour.ToString("D2");
            TimerMinute.text = minute.ToString("D2");
            TimerSecond.text = second.ToString("D2");
            if (LanguageManager.instance.isLanguageRTL)
            {
                TimerHour.text = LanguageManager.arabicFix(TimerHour.text);
                TimerMinute.text = LanguageManager.arabicFix(TimerMinute.text);
                TimerSecond.text = LanguageManager.arabicFix(TimerSecond.text);
            }
            yield return new WaitForSeconds(1);
        } while (hour > 0 || minute > 0 || second > 0);
        if (currentMenuTab != MenuTabs.Quests)
        {
            redDot.SetActive(true);
            Manager.instance.isQuestRedPointOn = true;
        }
        else {
            QuestsFather.GetComponent<Quests>().UpdateQuests();
        }
        timerDecreaserCoroutine = StartCoroutine(TimerDecreaser());
    }

    public void LaunchChapterSelector()
    {
        chapterSelectorObj.SetActive(true);
        chapterSelectorScript.InitChapters();
    }

    private IEnumerator ToggleIcon(GameObject icon, GameObject toggle, bool turnOn, Sprite onSprite, Sprite offSprite)
    {
        Sounds.instance.PlaySettingButtonUI();
        VibrationManager.instance.VibrateTouch();
        GameObject toggleCircle = toggle.transform.GetChild(0).gameObject;
        Vector3 toggleFinalPos = toggleCircle.GetComponent<RectTransform>().anchoredPosition;
        toggleFinalPos.x = -toggleFinalPos.x;
        StartCoroutine(Transitions.moveToPositionUI(toggleCircle, toggleFinalPos, 0.2f));
        toggle.GetComponent<Button>().interactable = false;
        yield return StartCoroutine(Transitions.rotateZ(icon, 180, true, 0.1f));
        icon.GetComponent<Image>().sprite = (turnOn ? onSprite : offSprite);
        toggle.GetComponentsInChildren<Image>()[0].sprite = (turnOn ? ToggleBackOn : ToggleBackOff);
        yield return StartCoroutine(Transitions.rotateZ(icon, 180, true, 0.1f));
        toggle.GetComponent<Button>().interactable = true;
    }

    public void ToggleSound()
    {
        Manager.instance.isSoundEnabled = !Manager.instance.isSoundEnabled;
        StartCoroutine(ToggleIcon(SoundIcon, SoundToggle, Manager.instance.isSoundEnabled, SoundOn, SoundOff));
        AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.sound_set_to, ":" + Manager.instance.isSoundEnabled);
    }

    public void ToggleMusic()
    {
        Manager.instance.isMusicEnabled = !Manager.instance.isMusicEnabled;
        if (Manager.instance.isMusicEnabled)
            Music.instance.StartMusic(Music.PipesMusics.Menu);
        else
            Music.instance.StopMusic();
        StartCoroutine(ToggleIcon(MusicIcon, MusicToggle, Manager.instance.isMusicEnabled, MusicOn, MusicOff));
        AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.music_set_to, ":" + Manager.instance.isMusicEnabled);
    }

    public void ToggleVibrate()
    {
        Manager.instance.isHapticEnabled = !Manager.instance.isHapticEnabled;
        StartCoroutine(ToggleIcon(VibrateIcon, VibrateToggle, Manager.instance.isHapticEnabled, VibrateOn, VibrateOff));
        AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.vibrate_set_to, ":" + Manager.instance.isHapticEnabled);
    }

    public void RestorePurchases()
    {
        IAPManager.instance.RestorePurchases();
    }

    public void HandleFTUEEnabling()
    {
        if (!Manager.instance.isFTUEQuestShown) {
            QuestsIconButton.interactable = false;
            QuestsIconCG.alpha = 0.5f;
        }
        if (!Manager.instance.isFTUECollectionShown)
        {
            CollectionIconButton.interactable = false;
            CollectionIconCG.alpha = 0.5f;
        }
    }

    private void FTUEAfterStartMenu()
    {
        if (Manager.instance.maxSeenLevelNum == 17 && !Manager.instance.isFTUEQuestShown)
        {
            StartCoroutine(FTUEScript.IntroduceAnElement(QuestsIconFTUE, 4, true,true));
            QuestsIconFTUE.SetActive(true);
            QuestsIconFTUE.transform.position = QuestsIcon.transform.position;
        }
    }

    public void QuestsClickFTUE()
    {
        QuestsIconCG.alpha = 1f;
        Manager.instance.isFTUEQuestShown = true;
        Manager.instance.Save(false);
        FTUEScript.CallContinueAfterIntroduction();
        LoadSpecificTab(3);
        AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.ftue_daily_quests);
    }

    public void CollectionClickFTUE()
    {
        CollectionIconCG.alpha = 1f;
        Manager.instance.isFTUECollectionShown = true;
        Manager.instance.Save(false);
        FTUEScript.CallContinueAfterIntroduction();
        LoadSpecificTab(1);
        CollectionIcon.GetComponent<Button>().interactable = true;
        CollectionIcon.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
        AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.ftue_collections);
    }

    public void ChapterSelectorClickFTUE() {
        Manager.instance.isFTUEChapterShown = true;
        Manager.instance.Save(false);
        FTUEScript.CallContinueAfterIntroduction();
        LaunchChapterSelector();
        AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.ftue_chapter_selector);
        AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.ftue_finish);
    }

    public void ShowBuyHeart(bool zeroNum) {
        if (!buyHeartIsOnTheScreen)
        {
            buyHeartIsOnTheScreen = true;
            StartCoroutine(ShowBuyHeartIE(zeroNum));
        }
    }

    private IEnumerator ShowBuyHeartIE(bool zeroNum) {
        hearts.transform.SetAsLastSibling();
        coins.transform.SetAsLastSibling();
        if (zeroNum)
        {
            heartZeroNum.SetActive(true);
        }
        else {
            heartZeroNum.SetActive(false);
        }
        Text heartZeroNumText = heartZeroNum.GetComponent<Text>();
        refill.text = LanguageManager.instance.GetTheTextByKey(textkey_Refill);
        if (LanguageManager.instance.isLanguageRTL) {
            fullHeartsPriceText.text = LanguageManager.arabicFix(fullHeartsPriceText.text);
            heartZeroNumText.text = LanguageManager.arabicFix(heartZeroNumText.text);
            adsPlusOneText.text = LanguageManager.arabicFix(adsPlusOneText.text);
        }
        Sounds.instance.PlayShowPopUp();
        VibrationManager.instance.VibrateTouch();
        //Manager.instance.userHasBoughtNoAds = true; // for test!
        if (Manager.instance.userHasBoughtNoAds)
        {
            freeHeartImage.sprite = heartSprite;
            freeHeartImage.rectTransform.sizeDelta = new Vector2(100,100);
            if (DateTime.Now.Subtract(Manager.instance.lastFreeHeartTime).TotalSeconds > freeHeartTime * 60)
            {
                buyOneHeartBtn.interactable = true;
                buyOneHeartBtn.onClick.RemoveAllListeners();
                buyOneHeartBtn.onClick.AddListener(GetOneHeart);
            }
            else
            {
                buyOneHeartBtn.interactable = false;
                StartCoroutine(NextFreeHeartTimer());
            }
        }
        else {
            buyOneHeartBtn.onClick.AddListener(ShowVideoForOneHeart);
        }
        StartCoroutine(Transitions.PanelAct(pausePanelCircle, circleFinalScale, pauseTime));
        yield return StartCoroutine(Transitions.turnOnImageWithDelay(buyHeart, pauseTime - 0.05f));
        StartCoroutine(Transitions.scaleUp(buyHeartBack, new Vector3(0.2f, 0.2f, 1), 0.2f));
    }

    public IEnumerator NextFreeHeartTimer() {
        TimeSpan nextFreeHeartTime = new TimeSpan(0, 5, 0) - DateTime.Now.Subtract(Manager.instance.lastFreeHeartTime);
        do {
            adsPlusOneText.text = nextFreeHeartTime.ToString(@"hh\:mm\:ss").Substring(3,5);
            if (LanguageManager.instance.isLanguageRTL)
                adsPlusOneText.text = LanguageManager.arabicFix(adsPlusOneText.text);
            yield return new WaitForSeconds(1);
            nextFreeHeartTime = nextFreeHeartTime.Subtract(new TimeSpan(0,0,1));
        } while (nextFreeHeartTime.TotalSeconds > 0);
        adsPlusOneText.text = "+1";
        if (LanguageManager.instance.isLanguageRTL)
            adsPlusOneText.text = LanguageManager.arabicFix(adsPlusOneText.text);
        buyOneHeartBtn.interactable = true;
        buyOneHeartBtn.onClick.RemoveAllListeners();
        buyOneHeartBtn.onClick.AddListener(GetOneHeart);
    }

    public void BuyFullHearts() {
        StartCoroutine(BuyFullHeartsIE());
    }

    private IEnumerator BuyFullHeartsIE() {
        if (Manager.instance.coins >= Manager.instance.buyFullHeartsPrice)
        {
            Manager.instance.coins -= Manager.instance.buyFullHeartsPrice;
            Manager.instance.hearts = 5;
            Manager.instance.heartsRefilledTime = DateTime.MinValue;
            Manager.instance.Save(false);
            HeartsTimerText.text = LanguageManager.instance.GetTheTextByKey(textkey_full);
            HeartsNumText.text = Manager.instance.hearts.ToString();
            Text coinsText = coins.GetComponentInChildren<Text>();
            coinsText.text = Manager.instance.coins.ToString();
            if (LanguageManager.instance.isLanguageRTL)
            {
                HeartsNumText.text = LanguageManager.arabicFix(HeartsNumText.text);
                coinsText.text = LanguageManager.arabicFix(coinsText.text);
            }
            if (HeartTimerCoroutine != null)
                StopCoroutine(HeartTimerCoroutine);
            heartPlus.interactable = false;
            Essentials.MakeTint(heartPlus.GetComponent<Image>(), 0.8f);
            StartCoroutine(Transitions.stamp(hearts, new Vector3(1.5f, 1.5f, 1.5f), 0.2f));
            var particleMain = Instantiate(Currencies.instance.updateParticle, heartImage.transform.position,
                Quaternion.identity, hearts.transform).GetComponent<ParticleSystem>().main;
            particleMain.loop = false;
        }
        else
        {
            LoadSpecificTab((int) MenuTabs.Shop);
        }
        Sounds.instance.PlayChestItems();
        VibrationManager.instance.VibrateSuccess();
        yield return StartCoroutine(CloseBuyHeartsIE(false));
    }

    public void ShowVideoForOneHeart()
    {
        AdManager.instance.ShowVideoAdIfAvailable(AdManager.rewardedVideoAdType.getHeart);
    }


    public void GetOneHeart()
    {
        StartCoroutine(GetOneHeartIE());
    }

    private IEnumerator GetOneHeartIE()
    {
        if (Manager.instance.userHasBoughtNoAds) {
            Manager.instance.lastFreeHeartTime = DateTime.Now;
        }
        if (Manager.instance.hearts < 4)
        {
            Manager.instance.hearts++;
            Manager.instance.Save(false);
            HeartsNumText.text = Manager.instance.hearts.ToString();
            if (LanguageManager.instance.isLanguageRTL)
                HeartsNumText.text = LanguageManager.arabicFix(HeartsNumText.text);
        }
        else if (Manager.instance.hearts == 4)
        {
            Manager.instance.hearts = 5;
            Manager.instance.heartsRefilledTime = DateTime.MinValue;
            Manager.instance.Save(false);
            HeartsTimerText.text = LanguageManager.instance.GetTheTextByKey(textkey_full);
            HeartsNumText.text = Manager.instance.hearts.ToString();
            if (LanguageManager.instance.isLanguageRTL)
                HeartsNumText.text = LanguageManager.arabicFix(HeartsNumText.text);
            if (HeartTimerCoroutine != null)
                StopCoroutine(HeartTimerCoroutine);
            heartPlus.interactable = false;
            Essentials.MakeTint(heartPlus.GetComponent<Image>(), 0.8f);
        }
        StartCoroutine(Transitions.stamp(hearts, new Vector3(1.5f, 1.5f, 1.5f), 0.2f));
        var particleMain = Instantiate(Currencies.instance.updateParticle, heartImage.transform.position,
            Quaternion.identity, hearts.transform).GetComponent<ParticleSystem>().main;
        particleMain.loop = false;
        Sounds.instance.PlayChestItems();
        VibrationManager.instance.VibrateSuccess();
        yield return StartCoroutine(CloseBuyHeartsIE(false));
    }

    public void CloseBuyHearts(bool isSoundEnabled)
    {
        StartCoroutine(CloseBuyHeartsIE(isSoundEnabled));
    }

    private IEnumerator CloseBuyHeartsIE(bool isSoundEnabled)
    {
        buyHeartIsOnTheScreen = false;
        if (isSoundEnabled)
        {
            Sounds.instance.PlayUIInteraction();
            VibrationManager.instance.VibrateTouch();
        }
        coins.transform.SetAsFirstSibling();
        hearts.transform.SetAsFirstSibling();
        Vector3 pos;
        pos = buyHeartBack.transform.position;
        StartCoroutine(Transitions.moveToPosition(buyHeartBack, new Vector3(6, pos.y, pos.z), 0.5f));
        yield return StartCoroutine(Transitions.fadeOut(buyHeart, 1, 0.5f));
        Color tempColor;
        buyHeart.SetActive(false);
        tempColor = buyHeart.GetComponent<Image>().color;
        buyHeart.GetComponent<Image>().color = new Color(tempColor.r, tempColor.g, tempColor.b, 1);
        buyHeartBack.SetActive(false);
        buyHeartBack.transform.position = new Vector3(0, pos.y, pos.z);
    }

    public void RoadMapLevelClick(short levelIndexIndex)
    {
        if (Manager.instance.hearts == 0)
        {
            ShowBuyHeart(true);
        }
        else
        {
            LoadLevel(levelIndexIndex);
        }
    }

    public void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if (HeartTimerCoroutine != null)
                StopCoroutine(HeartTimerCoroutine);
        }
        else
        {
            HandleFTUEEnabling();
            InitMenuVisuals();
        }
    }

}
