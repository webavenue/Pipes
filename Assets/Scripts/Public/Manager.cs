using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Xml.Serialization;

public class Manager : MonoBehaviour
{
    public int numOfIMItem_init;
    public int numOfHint_init;
    public int starChestCapacity_init;
    public int coins_init;
    public int hearts_init;
    [Header("Prices")]
    public int buyFullHeartsPrice;

    public static Manager instance;

    #region Version0
    [HideInInspector]
    public int maxSeenLevelNum;
    [HideInInspector]
    public bool isMusicEnabled;
    [HideInInspector]
    public bool isSoundEnabled;
    [HideInInspector]
    public bool isHapticEnabled;
    [HideInInspector]
    public bool userConsent_IsGiven;
    [HideInInspector]
    public bool userConsent;
    [HideInInspector]
    public bool userHasBoughtNoAds;
    [HideInInspector]
    public int hearts;
    [HideInInspector]
    public int coins;
    [HideInInspector]
    public DateTime heartsRefilledTime; // or use for infinite heart!
    [HideInInspector]
    public bool isInfiniteHeart;
    [HideInInspector]
    public int infiniteHeartMinute;
    [HideInInspector]
    public Dictionary<short, bool> levelStars = new Dictionary<short, bool>(); // if is one start set to false else if two set to true!
    [HideInInspector]
    public Dictionary<short, bool> chestCards = new Dictionary<short, bool>(); // if it is true, it is claimed!
    [HideInInspector]
    public Dictionary<byte, byte> chapterStars = new Dictionary<byte, byte>(); // can't be more than 80 levels in a chapter and can't add more than 255 chapters!
    [HideInInspector]
    public int numOfIMItem;
    [HideInInspector]
    public int numOfHint;
    [HideInInspector]
    public int starChestProgress;
    [HideInInspector]
    public int starChestCapacity;
    [Header("Quests")]
    [HideInInspector]
    public int questIndex;
    [HideInInspector]
    public int questSeed;
    [HideInInspector]
    public int questGiftSeed;
    [HideInInspector]
    public DateTime lastQuestStartTime;
    [HideInInspector]
    public DateTime easyQuestStartTime;
    [HideInInspector]
    public DateTime normalQuestStartTime;
    [HideInInspector]
    public DateTime hardQuestStartTime;
    [HideInInspector]
    public int easyQuestCount;
    [HideInInspector]
    public int normalQuestCount;
    [HideInInspector]
    public int hardQuestCount;
    [HideInInspector]
    public int easyQuestGoal;
    [HideInInspector]
    public int normalQuestGoal;
    [HideInInspector]
    public int hardQuestGoal;
    [HideInInspector]
    public QuestType easyQuestType;
    [HideInInspector]
    public QuestType normalQuestType;
    [HideInInspector]
    public QuestType hardQuestType;
    [HideInInspector]
    public GiftType easyQuestGiftType;
    [HideInInspector]
    public GiftType normalQuestGiftType;
    [HideInInspector]
    public GiftType hardQuestGiftType;
    [HideInInspector]
    public int easyQuestGiftAmount;
    [HideInInspector]
    public int normalQuestGiftAmount;
    [HideInInspector]
    public int hardQuestGiftAmount;
    [HideInInspector]
    public bool isEasyQuestClaimed;
    [HideInInspector]
    public bool isNormalQuestClaimed;
    [HideInInspector]
    public bool isHardQuestClaimed;
    [Header("Achievements")]
    [HideInInspector]
    public bool firstHintUsed;
    [HideInInspector]
    public bool firstIMUsed;
    [HideInInspector]
    public int numberOfCompletedQuests;
    [HideInInspector]
    public int numberOfConsecutiveDays;
    [HideInInspector]
    public DateTime startOfConsecutiveDays;
    [Header("FTUE")]
    [HideInInspector]
    public bool isFTUEHintShown;
    [HideInInspector]
    public bool isFTUEInfiniteMoveShown;
    [HideInInspector]
    public bool isFTUEQuestShown;
    [HideInInspector]
    public bool isFTUECollectionShown;
    [HideInInspector]
    public bool isQuestRedPointOn;

    private int oldVersion;
    #endregion


    #region Version1
    [HideInInspector]
    public bool ratingIsGiven;
    [HideInInspector]
    public bool isHintGivenBeforeFTUE;
    [HideInInspector]
    public bool isInfiniteMovesGivenBeforeFTUE;
    [HideInInspector]
    public bool isFTUEChapterShown;
    [HideInInspector]
    public DateTime lastFreeHeartTime;
    #endregion

    #region Version2
    [HideInInspector]
    public int AnalyticsNumberOfSessions;
    [HideInInspector]
    public float AnalyticsMinutesOfGamePlay;
    [HideInInspector]
    public int AnalyticsNumberOfInterstitialShown;
    [HideInInspector]
    public int AnalyticsNumberOfInterstitialClicked;
    [HideInInspector]
    public int AnalyticsNumberOfRewardedVideoShown;
    [HideInInspector]
    public int AnalyticsNumberOfRewardedVideoClicked;
    #endregion

    [HideInInspector]
    public bool needToGetDataFromCloud;

    private int newVersion;
    private bool firstTime = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        {
            Application.targetFrameRate = 60;
        }
        newVersion = 2;
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            firstTime = true;
            LoadFromDisk();
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
        if (firstTime && oldVersion < newVersion || !File.Exists(Application.persistentDataPath + "/GameSave.dat")) {
            UpdateFile();
            Save(false);
        }
        if (firstTime)
        {
            LanguageManager.instance.fillTheTranslatedTextsDictionaryFromFile();
        }
    }

    public void Save(bool toCloud)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/GameSave.dat");
        InfoData data = new InfoData();

        data.maxLevelNum = maxSeenLevelNum;
        data.isMusicEnabled = isMusicEnabled;
        data.isSoundEnabled = isSoundEnabled;
        data.isHapticEnabled = isHapticEnabled;
        data.userConsent_IsGiven = userConsent_IsGiven;
        data.userConsent = userConsent;
        data.userHasBoughtNoAds = userHasBoughtNoAds;
        data.oldVersion = oldVersion;
        data.hearts = hearts;
        data.coins = coins;
        data.heartsRefilledTime = heartsRefilledTime;
        data.isInfiniteHeart = isInfiniteHeart;
        data.infiniteHeartMinute = infiniteHeartMinute;
        data.levelStars = levelStars;
        data.chestCards = chestCards;
        data.chapterStars = chapterStars;
        data.numOfIMItem = numOfIMItem;
        data.numOfHint = numOfHint;
        data.starChestProgress = starChestProgress;
        data.starChestCapacity = starChestCapacity;
        data.lastQuestStartTime = lastQuestStartTime;
        data.easyQuestStartTime = easyQuestStartTime;
        data.normalQuestStartTime = normalQuestStartTime;
        data.hardQuestStartTime = hardQuestStartTime;
        data.QuestIndex = questIndex;
        data.questSeed = questSeed;
        data.questGiftSeed = questGiftSeed;
        data.easyQuestCount = easyQuestCount;
        data.normalQuestCount = normalQuestCount;
        data.hardQuestCount = hardQuestCount;
        data.easyQuestGoal = easyQuestGoal;
        data.normalQuestGoal = normalQuestGoal;
        data.hardQuestGoal = hardQuestGoal;
        data.easyQuestType = easyQuestType;
        data.normalQuestType = normalQuestType;
        data.hardQuestType = hardQuestType;
        data.easyQuestGiftType = easyQuestGiftType;
        data.normalQuestGiftType = normalQuestGiftType;
        data.hardQuestGiftType = hardQuestGiftType;
        data.easyQuestGiftAmount = easyQuestGiftAmount;
        data.normalQuestGiftAmount = normalQuestGiftAmount;
        data.hardQuestGiftAmount = hardQuestGiftAmount;
        data.isEasyQuestClaimed = isEasyQuestClaimed;
        data.isNormalQuestClaimed = isNormalQuestClaimed;
        data.isHardQuestClaimed = isHardQuestClaimed;
        data.firstHintUsed = firstHintUsed;
        data.firstIMUsed = firstIMUsed;
        data.numberOfCompletedQuests = numberOfCompletedQuests;
        data.numberOfConsecutiveDays = numberOfConsecutiveDays;
        data.startOfConsecutiveDays = startOfConsecutiveDays;
        data.isFTUEHintShown = isFTUEHintShown;
        data.isFTUEInfiniteMoveShown = isFTUEInfiniteMoveShown;
        data.isFTUEQuestShown = isFTUEQuestShown;
        data.isFTUECollectionShown = isFTUECollectionShown;
        data.isFTUEChapterShown = isFTUEChapterShown;
        data.isQuestRedPointOn = isQuestRedPointOn;
        data.ratingIsGiven = ratingIsGiven;
        data.isHintGivenBeforeFTUE = isHintGivenBeforeFTUE;
        data.isInfiniteMovesGivenBeforeFTUE = isInfiniteMovesGivenBeforeFTUE;
        data.lastFreeHeartTime = lastFreeHeartTime;
        data.AnalyticsNumberOfSessions = AnalyticsNumberOfSessions;
        data.AnalyticsMinutesOfGamePlay = AnalyticsMinutesOfGamePlay;
        data.AnalyticsNumberOfInterstitialShown = AnalyticsNumberOfInterstitialShown;
        data.AnalyticsNumberOfInterstitialClicked = AnalyticsNumberOfInterstitialClicked;
        data.AnalyticsNumberOfRewardedVideoShown = AnalyticsNumberOfRewardedVideoShown;
        data.AnalyticsNumberOfRewardedVideoClicked = AnalyticsNumberOfRewardedVideoClicked;

        bf.Serialize(file, data);
        file.Close();

        if (toCloud)
        {
            Debug.Log("Save To Cloud");

            InfoData2 data2 = data.changeTypeToInfoData2();

            XmlSerializer xmlSerializer = new XmlSerializer(data2.GetType());
            StringWriter stringWriter = new StringWriter();

            xmlSerializer.Serialize(stringWriter, data2);

            string text = stringWriter.ToString();

            Debug.Log(text);
            CloudManager.instance.SaveToCloud(text);
        }
    }

    public void LoadFromDisk()
    {
        if (File.Exists(Application.persistentDataPath + "/GameSave.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/GameSave.dat", FileMode.Open);
            InfoData data = (InfoData)bf.Deserialize(file);
            file.Close();

            #region Version0
            maxSeenLevelNum = data.maxLevelNum;
            isMusicEnabled = data.isMusicEnabled;
            isSoundEnabled = data.isSoundEnabled;
            isHapticEnabled = data.isHapticEnabled;
            userConsent_IsGiven = data.userConsent_IsGiven;
            userConsent = data.userConsent;
            userHasBoughtNoAds = data.userHasBoughtNoAds;
            oldVersion = data.oldVersion;
            hearts = data.hearts;
            coins = data.coins;
            heartsRefilledTime = data.heartsRefilledTime;
            isInfiniteHeart = data.isInfiniteHeart;
            infiniteHeartMinute = data.infiniteHeartMinute;
            levelStars = data.levelStars;
            chestCards = data.chestCards;
            chapterStars = data.chapterStars;
            numOfIMItem = data.numOfIMItem;
            numOfHint = data.numOfHint;
            starChestProgress = data.starChestProgress;
            starChestCapacity = data.starChestCapacity;
            lastQuestStartTime = data.lastQuestStartTime;
            easyQuestStartTime = data.easyQuestStartTime;
            normalQuestStartTime = data.normalQuestStartTime;
            hardQuestStartTime = data.hardQuestStartTime;
            questIndex = data.QuestIndex;
            questSeed = data.questSeed;
            questGiftSeed = data.questGiftSeed;
            easyQuestCount = data.easyQuestCount;
            normalQuestCount = data.normalQuestCount;
            hardQuestCount = data.hardQuestCount;
            easyQuestGoal = data.easyQuestGoal;
            normalQuestGoal = data.normalQuestGoal;
            hardQuestGoal = data.hardQuestGoal;
            easyQuestType = data.easyQuestType;
            normalQuestType = data.normalQuestType;
            hardQuestType = data.hardQuestType;
            easyQuestGiftType = data.easyQuestGiftType;
            normalQuestGiftType = data.normalQuestGiftType;
            hardQuestGiftType = data.hardQuestGiftType;
            easyQuestGiftAmount = data.easyQuestGiftAmount;
            normalQuestGiftAmount = data.normalQuestGiftAmount;
            hardQuestGiftAmount = data.hardQuestGiftAmount;
            isEasyQuestClaimed = data.isEasyQuestClaimed;
            isNormalQuestClaimed = data.isNormalQuestClaimed;
            isHardQuestClaimed = data.isHardQuestClaimed;
            firstHintUsed = data.firstHintUsed;
            firstIMUsed = data.firstIMUsed;
            numberOfCompletedQuests = data.numberOfCompletedQuests;
            numberOfConsecutiveDays = data.numberOfConsecutiveDays;
            startOfConsecutiveDays = data.startOfConsecutiveDays;
            isFTUEHintShown = data.isFTUEHintShown;
            isFTUEInfiniteMoveShown = data.isFTUEInfiniteMoveShown;
            isFTUEQuestShown = data.isFTUEQuestShown;
            isFTUECollectionShown = data.isFTUECollectionShown;
            isQuestRedPointOn = data.isQuestRedPointOn;
            #endregion

            // for each version of database we should write an if and else to get data from database or initialize them.
            #region version1
            if (oldVersion >= 1)
            {
                ratingIsGiven = data.ratingIsGiven;
                isHintGivenBeforeFTUE = data.isHintGivenBeforeFTUE;
                isInfiniteMovesGivenBeforeFTUE = data.isInfiniteMovesGivenBeforeFTUE;
                isFTUEChapterShown = data.isFTUEChapterShown;
                lastFreeHeartTime = data.lastFreeHeartTime;
            }
            #endregion

            #region version2
            if (oldVersion >= 2)
            {
                AnalyticsNumberOfSessions = data.AnalyticsNumberOfSessions;
                AnalyticsMinutesOfGamePlay = data.AnalyticsMinutesOfGamePlay;
                AnalyticsNumberOfInterstitialShown = data.AnalyticsNumberOfInterstitialShown;
                AnalyticsNumberOfInterstitialClicked = data.AnalyticsNumberOfInterstitialClicked;
                AnalyticsNumberOfRewardedVideoShown = data.AnalyticsNumberOfRewardedVideoShown;
                AnalyticsNumberOfRewardedVideoClicked = data.AnalyticsNumberOfRewardedVideoClicked;
            }
            #endregion
        }
        else
        {
            // for initialize parameters
            needToGetDataFromCloud = true;
            maxSeenLevelNum = 1;
            isMusicEnabled = true;
            isSoundEnabled = true;
            isHapticEnabled = true;
            starChestCapacity = starChestCapacity_init;
            coins = coins_init;
            hearts = hearts_init;
            heartsRefilledTime = DateTime.MinValue;
            lastQuestStartTime = DateTime.MinValue;
            easyQuestStartTime = DateTime.MinValue;
            normalQuestStartTime = DateTime.MinValue;
            hardQuestStartTime = DateTime.MinValue;
            startOfConsecutiveDays = DateTime.MinValue;
            lastFreeHeartTime = DateTime.MinValue;
        }
    }

    public void LoadFromCloud(string cloudData)
    {
        InfoData2 data2 = new InfoData2();

        XmlSerializer xmlSerializer = new XmlSerializer(data2.GetType());

        StringReader stringReader = new StringReader(cloudData);

        data2 = (InfoData2)xmlSerializer.Deserialize(stringReader);

        InfoData data = data2.changeTypeToInfoData();

        #region Version0
        maxSeenLevelNum = data.maxLevelNum;
        isMusicEnabled = data.isMusicEnabled;
        isSoundEnabled = data.isSoundEnabled;
        isHapticEnabled = data.isHapticEnabled;
        userConsent_IsGiven = data.userConsent_IsGiven;
        userConsent = data.userConsent;
        userHasBoughtNoAds = data.userHasBoughtNoAds;
        oldVersion = data.oldVersion;
        hearts = data.hearts;
        coins = data.coins;
        heartsRefilledTime = data.heartsRefilledTime;
        isInfiniteHeart = data.isInfiniteHeart;
        infiniteHeartMinute = data.infiniteHeartMinute;
        levelStars = data.levelStars;
        chestCards = data.chestCards;
        chapterStars = data.chapterStars;
        numOfIMItem = data.numOfIMItem;
        numOfHint = data.numOfHint;
        starChestProgress = data.starChestProgress;
        starChestCapacity = data.starChestCapacity;
        lastQuestStartTime = data.lastQuestStartTime;
        easyQuestStartTime = data.easyQuestStartTime;
        normalQuestStartTime = data.normalQuestStartTime;
        hardQuestStartTime = data.hardQuestStartTime;
        questIndex = data.QuestIndex;
        questSeed = data.questSeed;
        questGiftSeed = data.questGiftSeed;
        easyQuestCount = data.easyQuestCount;
        normalQuestCount = data.normalQuestCount;
        hardQuestCount = data.hardQuestCount;
        easyQuestGoal = data.easyQuestGoal;
        normalQuestGoal = data.normalQuestGoal;
        hardQuestGoal = data.hardQuestGoal;
        easyQuestType = data.easyQuestType;
        normalQuestType = data.normalQuestType;
        hardQuestType = data.hardQuestType;
        easyQuestGiftType = data.easyQuestGiftType;
        normalQuestGiftType = data.normalQuestGiftType;
        hardQuestGiftType = data.hardQuestGiftType;
        easyQuestGiftAmount = data.easyQuestGiftAmount;
        normalQuestGiftAmount = data.normalQuestGiftAmount;
        hardQuestGiftAmount = data.hardQuestGiftAmount;
        isEasyQuestClaimed = data.isEasyQuestClaimed;
        isNormalQuestClaimed = data.isNormalQuestClaimed;
        isHardQuestClaimed = data.isHardQuestClaimed;
        firstHintUsed = data.firstHintUsed;
        firstIMUsed = data.firstIMUsed;
        numberOfCompletedQuests = data.numberOfCompletedQuests;
        numberOfConsecutiveDays = data.numberOfConsecutiveDays;
        startOfConsecutiveDays = data.startOfConsecutiveDays;
        isFTUEHintShown = data.isFTUEHintShown;
        isFTUEInfiniteMoveShown = data.isFTUEInfiniteMoveShown;
        isFTUEQuestShown = data.isFTUEQuestShown;
        isFTUECollectionShown = data.isFTUECollectionShown;
        isQuestRedPointOn = data.isQuestRedPointOn;
        #endregion

        // for each version of database we should write an if and else to get data from database or initialize them.
        #region version1
        if (oldVersion >= 1)
        {
            ratingIsGiven = data.ratingIsGiven;
            isHintGivenBeforeFTUE = data.isHintGivenBeforeFTUE;
            isInfiniteMovesGivenBeforeFTUE = data.isInfiniteMovesGivenBeforeFTUE;
            isFTUEChapterShown = data.isFTUEChapterShown;
            lastFreeHeartTime = data.lastFreeHeartTime;
        }
        #endregion

        if (oldVersion < newVersion)
        {
            UpdateFile();
        }

        Save(false);
        instance.needToGetDataFromCloud = false;
        GameScenes.instance.LoadMenu();
    }

    private void UpdateFile()
    {
        if (oldVersion == 0)
        {
            for (byte i = 0; i<26; i++)
            {
                chapterStars.Add(i,0);
            }
            oldVersion++;
        }
    }

}

[System.Serializable]
public class InfoData
{
    #region Version0
    public int maxLevelNum;
    public bool isMusicEnabled;
    public bool isSoundEnabled;
    public bool isHapticEnabled;
    public bool userConsent_IsGiven;
    public bool userConsent;
    public bool userHasBoughtNoAds;
    public int oldVersion;
    public int hearts;
    public int coins;
    public DateTime heartsRefilledTime;
    public bool isInfiniteHeart;
    public int infiniteHeartMinute;
    public Dictionary<short, bool> levelStars = new Dictionary<short, bool>();
    public Dictionary<short, bool> chestCards = new Dictionary<short, bool>();
    public Dictionary<byte, byte> chapterStars = new Dictionary<byte, byte>(); // can't be more than 80 levels in a chapter and can't add more than 255 chapters!
    public int numOfIMItem;
    public int numOfHint;
    public int starChestProgress;
    public int starChestCapacity;
    public DateTime lastQuestStartTime;
    public DateTime easyQuestStartTime;
    public DateTime normalQuestStartTime;
    public DateTime hardQuestStartTime;
    public int QuestIndex;
    public int questSeed;
    public int questGiftSeed;
    public int easyQuestCount;
    public int normalQuestCount;
    public int hardQuestCount;
    public int easyQuestGoal;
    public int normalQuestGoal;
    public int hardQuestGoal;
    public QuestType easyQuestType;
    public QuestType normalQuestType;
    public QuestType hardQuestType;
    public GiftType easyQuestGiftType;
    public GiftType normalQuestGiftType;
    public GiftType hardQuestGiftType;
    public int easyQuestGiftAmount;
    public int normalQuestGiftAmount;
    public int hardQuestGiftAmount;
    public bool isEasyQuestClaimed;
    public bool isNormalQuestClaimed;
    public bool isHardQuestClaimed;
    public bool firstHintUsed;
    public bool firstIMUsed;
    public int numberOfCompletedQuests;
    public int numberOfConsecutiveDays;
    public DateTime startOfConsecutiveDays;
    public bool isFTUEHintShown;
    public bool isFTUEInfiniteMoveShown;
    public bool isFTUEQuestShown;
    public bool isFTUECollectionShown;
    public bool isQuestRedPointOn;
    #endregion

    #region Version1
    public bool ratingIsGiven;
    public bool isHintGivenBeforeFTUE;
    public bool isInfiniteMovesGivenBeforeFTUE;
    public bool isFTUEChapterShown;
    public DateTime lastFreeHeartTime;
    #endregion

    #region Version2
    public int AnalyticsNumberOfSessions;
    public float AnalyticsMinutesOfGamePlay;
    public int AnalyticsNumberOfInterstitialShown;
    public int AnalyticsNumberOfInterstitialClicked;
    public int AnalyticsNumberOfRewardedVideoShown;
    public int AnalyticsNumberOfRewardedVideoClicked;
    #endregion

    public InfoData2 changeTypeToInfoData2()
    {
        InfoData2 infoData2 = new InfoData2();
        infoData2.maxLevelNum = maxLevelNum;
        infoData2.isMusicEnabled = isMusicEnabled;
        infoData2.isSoundEnabled = isSoundEnabled;
        infoData2.isHapticEnabled = isHapticEnabled;
        infoData2.userConsent_IsGiven = userConsent_IsGiven;
        infoData2.userConsent = userConsent;
        infoData2.userHasBoughtNoAds = userHasBoughtNoAds;
        infoData2.oldVersion = oldVersion;
        infoData2.hearts = hearts;
        infoData2.coins = coins;
        infoData2.heartsRefilledTime = heartsRefilledTime;
        infoData2.isInfiniteHeart = isInfiniteHeart;
        infoData2.infiniteHeartMinute = infiniteHeartMinute;
        infoData2.levelStars = DictionaryToString(levelStars);
        infoData2.chestCards = DictionaryToString(chestCards);
        infoData2.chapterStars = DictionaryToString(chapterStars);
        infoData2.numOfIMItem = numOfIMItem;
        infoData2.numOfHint = numOfHint;
        infoData2.starChestProgress = starChestProgress;
        infoData2.starChestCapacity = starChestCapacity;
        infoData2.lastQuestStartTime = lastQuestStartTime;
        infoData2.easyQuestStartTime = easyQuestStartTime;
        infoData2.normalQuestStartTime = normalQuestStartTime;
        infoData2.hardQuestStartTime = hardQuestStartTime;
        infoData2.QuestIndex = QuestIndex;
        infoData2.questSeed = questSeed;
        infoData2.questGiftSeed = questGiftSeed;
        infoData2.easyQuestCount = easyQuestCount;
        infoData2.normalQuestCount = normalQuestCount;
        infoData2.hardQuestCount = hardQuestCount;
        infoData2.easyQuestGoal = easyQuestGoal;
        infoData2.normalQuestGoal = normalQuestGoal;
        infoData2.hardQuestGoal = hardQuestGoal;
        infoData2.easyQuestType = easyQuestType;
        infoData2.normalQuestType = normalQuestType;
        infoData2.hardQuestType = hardQuestType;
        infoData2.easyQuestGiftType = easyQuestGiftType;
        infoData2.normalQuestGiftType = normalQuestGiftType;
        infoData2.hardQuestGiftType = hardQuestGiftType;
        infoData2.easyQuestGiftAmount = easyQuestGiftAmount;
        infoData2.normalQuestGiftAmount = normalQuestGiftAmount;
        infoData2.hardQuestGiftAmount = hardQuestGiftAmount;
        infoData2.isEasyQuestClaimed = isEasyQuestClaimed;
        infoData2.isNormalQuestClaimed = isNormalQuestClaimed;
        infoData2.isHardQuestClaimed = isHardQuestClaimed;
        infoData2.firstHintUsed = firstHintUsed;
        infoData2.firstIMUsed = firstIMUsed;
        infoData2.numberOfCompletedQuests = numberOfCompletedQuests;
        infoData2.numberOfConsecutiveDays = numberOfConsecutiveDays;
        infoData2.startOfConsecutiveDays = startOfConsecutiveDays;
        infoData2.isFTUEHintShown = isFTUEHintShown;
        infoData2.isFTUEInfiniteMoveShown = isFTUEInfiniteMoveShown;
        infoData2.isFTUEQuestShown = isFTUEQuestShown;
        infoData2.isFTUECollectionShown = isFTUECollectionShown;
        infoData2.isFTUEChapterShown = isFTUEChapterShown;
        infoData2.isQuestRedPointOn = isQuestRedPointOn;
        infoData2.ratingIsGiven = ratingIsGiven;
        infoData2.isHintGivenBeforeFTUE = isHintGivenBeforeFTUE;
        infoData2.isInfiniteMovesGivenBeforeFTUE = isInfiniteMovesGivenBeforeFTUE;
        infoData2.lastFreeHeartTime = lastFreeHeartTime;
        return infoData2;
    }

    public string DictionaryToString<T,G>(Dictionary<T, G> dictionary)
    {
        string dictionaryString = "";
        foreach (KeyValuePair<T, G> keyValues in dictionary)
        {
            dictionaryString += keyValues.Key + ":" + keyValues.Value + ",";
        }
        return dictionaryString.TrimEnd(',', ' ') + "";
    }
}

[System.Serializable]
public class InfoData2
{
    #region Version0
    public int maxLevelNum;
    public bool isMusicEnabled;
    public bool isSoundEnabled;
    public bool isHapticEnabled;
    public bool userConsent_IsGiven;
    public bool userConsent;
    public bool userHasBoughtNoAds;
    public int oldVersion;
    public int hearts;
    public int coins;
    public DateTime heartsRefilledTime;
    public bool isInfiniteHeart;
    public int infiniteHeartMinute;
    public string levelStars;
    public string chestCards;
    public string chapterStars;
    public int numOfIMItem;
    public int numOfHint;
    public int starChestProgress;
    public int starChestCapacity;
    public DateTime lastQuestStartTime;
    public DateTime easyQuestStartTime;
    public DateTime normalQuestStartTime;
    public DateTime hardQuestStartTime;
    public int QuestIndex;
    public int questSeed;
    public int questGiftSeed;
    public int easyQuestCount;
    public int normalQuestCount;
    public int hardQuestCount;
    public int easyQuestGoal;
    public int normalQuestGoal;
    public int hardQuestGoal;
    public QuestType easyQuestType;
    public QuestType normalQuestType;
    public QuestType hardQuestType;
    public GiftType easyQuestGiftType;
    public GiftType normalQuestGiftType;
    public GiftType hardQuestGiftType;
    public int easyQuestGiftAmount;
    public int normalQuestGiftAmount;
    public int hardQuestGiftAmount;
    public bool isEasyQuestClaimed;
    public bool isNormalQuestClaimed;
    public bool isHardQuestClaimed;
    public bool firstHintUsed;
    public bool firstIMUsed;
    public int numberOfCompletedQuests;
    public int numberOfConsecutiveDays;
    public DateTime startOfConsecutiveDays;
    public bool isFTUEHintShown;
    public bool isFTUEInfiniteMoveShown;
    public bool isFTUEQuestShown;
    public bool isFTUECollectionShown;
    public bool isQuestRedPointOn;
    #endregion

    #region Version1
    public bool ratingIsGiven;
    public bool isHintGivenBeforeFTUE;
    public bool isInfiniteMovesGivenBeforeFTUE;
    public bool isFTUEChapterShown;
    public DateTime lastFreeHeartTime;
    #endregion

    public InfoData changeTypeToInfoData()
    {
        InfoData infoData1 = new InfoData();

        #region Version0
        infoData1.maxLevelNum = maxLevelNum;
        infoData1.isMusicEnabled = isMusicEnabled;
        infoData1.isSoundEnabled = isSoundEnabled;
        infoData1.isHapticEnabled = isHapticEnabled;
        infoData1.userConsent_IsGiven = userConsent_IsGiven;
        infoData1.userConsent = userConsent;
        infoData1.userHasBoughtNoAds = userHasBoughtNoAds;
        infoData1.oldVersion = oldVersion;
        infoData1.hearts = hearts;
        infoData1.coins = coins;
        infoData1.heartsRefilledTime = heartsRefilledTime;
        infoData1.isInfiniteHeart = isInfiniteHeart;
        infoData1.infiniteHeartMinute = infiniteHeartMinute;
        infoData1.levelStars = StringToDictionaryShortBool(levelStars);
        infoData1.chestCards = StringToDictionaryShortBool(chestCards);
        infoData1.chapterStars = StringToDictionaryByteByte(chapterStars);
        infoData1.numOfIMItem = numOfIMItem;
        infoData1.numOfHint = numOfHint;
        infoData1.starChestProgress = starChestProgress;
        infoData1.starChestCapacity = starChestCapacity;
        infoData1.lastQuestStartTime = lastQuestStartTime;
        infoData1.easyQuestStartTime = easyQuestStartTime;
        infoData1.normalQuestStartTime = normalQuestStartTime;
        infoData1.hardQuestStartTime = hardQuestStartTime;
        infoData1.QuestIndex = QuestIndex;
        infoData1.questSeed = questSeed;
        infoData1.questGiftSeed = questGiftSeed;
        infoData1.easyQuestCount = easyQuestCount;
        infoData1.normalQuestCount = normalQuestCount;
        infoData1.hardQuestCount = hardQuestCount;
        infoData1.easyQuestGoal = easyQuestGoal;
        infoData1.normalQuestGoal = normalQuestGoal;
        infoData1.hardQuestGoal = hardQuestGoal;
        infoData1.easyQuestType = easyQuestType;
        infoData1.normalQuestType = normalQuestType;
        infoData1.hardQuestType = hardQuestType;
        infoData1.easyQuestGiftType = easyQuestGiftType;
        infoData1.normalQuestGiftType = normalQuestGiftType;
        infoData1.hardQuestGiftType = hardQuestGiftType;
        infoData1.easyQuestGiftAmount = easyQuestGiftAmount;
        infoData1.normalQuestGiftAmount = normalQuestGiftAmount;
        infoData1.hardQuestGiftAmount = hardQuestGiftAmount;
        infoData1.isEasyQuestClaimed = isEasyQuestClaimed;
        infoData1.isNormalQuestClaimed = isNormalQuestClaimed;
        infoData1.isHardQuestClaimed = isHardQuestClaimed;
        infoData1.firstHintUsed = firstHintUsed;
        infoData1.firstIMUsed = firstIMUsed;
        infoData1.numberOfCompletedQuests = numberOfCompletedQuests;
        infoData1.numberOfConsecutiveDays = numberOfConsecutiveDays;
        infoData1.startOfConsecutiveDays = startOfConsecutiveDays;
        infoData1.isFTUEHintShown = isFTUEHintShown;
        infoData1.isFTUEInfiniteMoveShown = isFTUEInfiniteMoveShown;
        infoData1.isFTUEQuestShown = isFTUEQuestShown;
        infoData1.isFTUECollectionShown = isFTUECollectionShown;
        infoData1.isQuestRedPointOn = isQuestRedPointOn;
        #endregion

        #region version1
        if (oldVersion >= 1)
        {
            infoData1.ratingIsGiven = ratingIsGiven;
            infoData1.isHintGivenBeforeFTUE = isHintGivenBeforeFTUE;
            infoData1.isInfiniteMovesGivenBeforeFTUE = isInfiniteMovesGivenBeforeFTUE;
            infoData1.isFTUEChapterShown = isFTUEChapterShown;
            infoData1.lastFreeHeartTime = lastFreeHeartTime;
        }
        #endregion
        return infoData1;
    }

    public Dictionary<short, bool> StringToDictionaryShortBool(string theString)
    {
        Dictionary<short, bool> dict = new Dictionary<short, bool>();
        char[] separator = { ',' };
        string[] rows = theString.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string row in rows)
        {
            string[] keyVal = row.Split(':');
            dict.Add(short.Parse(keyVal[0]), bool.Parse(keyVal[1]));
        }
        return dict;
    }

    public Dictionary<byte, byte> StringToDictionaryByteByte(string theString)
    {
        Dictionary<byte, byte> dict = new Dictionary<byte, byte>();
        char[] separator = { ',' };
        string[] rows = theString.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string row in rows)
        {
            string[] keyVal = row.Split(':');
            dict.Add(byte.Parse(keyVal[0]), byte.Parse(keyVal[1]));
        }
        return dict;
    }

}
