using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum QuestType
{
    RotateTiles = 0,
    FinishLevels = 1,
    EarnCoins = 2,
    EarnInfiniteHearts = 3, // set infinite heart time in minute.
    EarnHints = 4,
    EarnInfiniteMove = 5,
    WinLevelsContinuously = 6,
    EarnCards = 7,
    EarnStars = 8,
    UsePowerUps = 9,
    WinWithoutUndo = 10,
    FillWells = 11,
    GrowPlants = 12,
    WinWith3Stars = 13,
    FillPipes = 14,
    OpenChests = 15,
    OpenStarChest = 16,
    OpenLevelChest = 17,
    ReachSpecificLevel = 18,
    PlayLevelsForSomeMinutes = 19,
    EarnScore = 20,
    PlayLevelsForSomeMinutesContinuously = 21,
}

public enum GiftType
{
    infiniteHeart = 0,
    coin = 1,
    hint = 2,
    infinteMove = 3,
}

public class Quests : MonoBehaviour
{
    [Serializable]
    public struct QuestItems {
        public string textKey;
        public int easyAmount;
        public int normalAmount;
        public int hardAmount;
        public QuestType questType;
    }

    [Serializable]
    public struct GiftItems {
        public int easyGiftAmount;
        public int normalGiftAmount;
        public int hardGiftAmount;
        public GiftType giftType;
    }

    public List<QuestItems> questList;
    public List<GiftItems> giftList;
    public GameObject easyQuestPanel;
    public GameObject normalQuestPanel;
    public GameObject hardQuestPanel;
    public Text easyText;
    public Text normalText;
    public Text hardText;
    public RectTransform easyGreenBar;
    public RectTransform normalGreenBar;
    public RectTransform hardGreenBar;
    public Text easyProgressText;
    public Text normalProgressText;
    public Text hardProgressText;
    public Text easyRewardText;
    public Text normalRewardText;
    public Text hardRewardText;
    public Image easyRewardImage;
    public Image normalRewardImage;
    public Image hardRewardImage;
    public GameObject easyBadge;
    public GameObject normalBadge;
    public GameObject hardBadge;
    public GameObject easyTick;
    public GameObject normalTick;
    public GameObject hardTick;
    public Button easyClaimButton;
    public Button normalClaimButton;
    public Button hardClaimButton;
    public Sprite simpleQuestSprite;
    public Sprite DoneQuestSprite;
    public Sprite ClaimedQuestSprite;
    public Sprite[] giftSprite;
    public GameObject[] rewardAmountText;
    public GameObject[] rewardTitle;
    public GameObject[] crowns;
    public GameObject[] titleBars;
    public Menu menu;

    public enum QuestLevel {
        easy = 0,
        normal = 1,
        hard = 2
    }

    private int[] questsIndex;
    private bool isQuestIndexDefined;
    private int[] giftIndex;
    private bool isGiftIndexDefined;


    private static readonly string textkey_reward = "Reward";
    private static readonly string textkey_easy = "Easy";
    private static readonly string textkey_medium = "Medium";
    private static readonly string textkey_hard = "Hard";
    private static readonly string textkey_claim = "Claim";
    private static readonly string textkey_claimed = "Claimed";

    private string sad;

    void Awake()
    {
        sad = LanguageManager.instance.isLanguageRTL ? "۱۰۰" : "100";

        questsIndex = new int[questList.Count];
        giftIndex = new int[4];
        easyClaimButton.onClick.AddListener(() => ClaimGifts(QuestLevel.easy));
        normalClaimButton.onClick.AddListener(() => ClaimGifts(QuestLevel.normal));
        hardClaimButton.onClick.AddListener(() => ClaimGifts(QuestLevel.hard));
        rewardTitle[0].GetComponent<Text>().text = LanguageManager.instance.GetTheTextByKey(textkey_reward);
        rewardTitle[1].GetComponent<Text>().text = LanguageManager.instance.GetTheTextByKey(textkey_reward);
        rewardTitle[2].GetComponent<Text>().text = LanguageManager.instance.GetTheTextByKey(textkey_reward);
        easyClaimButton.GetComponentInChildren<Text>().text = LanguageManager.instance.GetTheTextByKey(textkey_claim);
        normalClaimButton.GetComponentInChildren<Text>().text = LanguageManager.instance.GetTheTextByKey(textkey_claim);
        hardClaimButton.GetComponentInChildren<Text>().text = LanguageManager.instance.GetTheTextByKey(textkey_claim);
        titleBars[0].GetComponentInChildren<Text>().text = LanguageManager.instance.GetTheTextByKey(textkey_easy);
        titleBars[1].GetComponentInChildren<Text>().text = LanguageManager.instance.GetTheTextByKey(textkey_medium);
        titleBars[2].GetComponentInChildren<Text>().text = LanguageManager.instance.GetTheTextByKey(textkey_hard);
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        if ((Manager.instance.easyQuestCount < Manager.instance.easyQuestGoal || Manager.instance.isEasyQuestClaimed) &&
           (Manager.instance.normalQuestCount < Manager.instance.normalQuestGoal || Manager.instance.isNormalQuestClaimed) &&
           (Manager.instance.hardQuestCount < Manager.instance.hardQuestGoal || Manager.instance.isHardQuestClaimed))
        {
            Manager.instance.isQuestRedPointOn = false;
            menu.redDot.SetActive(false);
        }
        UpdateQuests();
    }

    public void UpdateQuests() {
        if ((DateTime.Now - Manager.instance.lastQuestStartTime) > TimeSpan.FromDays(1)) {
            ShuffleGiftIndex(true);
            isGiftIndexDefined = true;
            if (Manager.instance.lastQuestStartTime == DateTime.MinValue)
            {
                ShuffleQuestIndex(true);
                Manager.instance.questIndex = 0;
                isQuestIndexDefined = true;
            }
            else {
                if ((Manager.instance.questIndex / 3) < (questList.Count / 3 - 1))
                {
                    Manager.instance.questIndex += 3;
                }
                else
                {
                    ShuffleQuestIndex(true);
                    Manager.instance.questIndex = 0;
                    isQuestIndexDefined = true;
                }
            }
            Manager.instance.lastQuestStartTime = DateTime.Now.Date;
        }
        if (!isQuestIndexDefined) {
            ShuffleQuestIndex(false);
            isQuestIndexDefined = true;
        }
        if (!isGiftIndexDefined) {
            ShuffleGiftIndex(false);
            isGiftIndexDefined = true;
        }
        if ((DateTime.Now - Manager.instance.easyQuestStartTime) > TimeSpan.FromDays(1))
        {
            if (Manager.instance.easyQuestCount < Manager.instance.easyQuestGoal ||
                    Manager.instance.isEasyQuestClaimed ||
                        Manager.instance.easyQuestGoal == 0) {
                SetDBValues(QuestLevel.easy);
                StartCoroutine(Transitions.scaleUp(easyQuestPanel, new Vector3(0.2f, 0.2f, 1), 0.2f));
            }
        }
        if ((DateTime.Now - Manager.instance.normalQuestStartTime) > TimeSpan.FromDays(1))
        {
            if (Manager.instance.normalQuestCount < Manager.instance.normalQuestGoal ||
                    Manager.instance.isNormalQuestClaimed ||
                        Manager.instance.normalQuestGoal == 0)
            {
                SetDBValues(QuestLevel.normal);
                StartCoroutine(Transitions.scaleUp(normalQuestPanel, new Vector3(0.2f, 0.2f, 1), 0.2f));

            }
        }
        if ((DateTime.Now - Manager.instance.hardQuestStartTime) > TimeSpan.FromDays(1))
        {
            if (Manager.instance.hardQuestCount < Manager.instance.hardQuestGoal ||
                    Manager.instance.isHardQuestClaimed ||
                        Manager.instance.hardQuestGoal == 0)
            {
                SetDBValues(QuestLevel.hard);
                StartCoroutine(Transitions.scaleUp(hardQuestPanel, new Vector3(0.2f, 0.2f, 1), 0.2f));
            }
        }
        Manager.instance.Save(false);
        SetVisualValues(QuestLevel.easy);
        SetVisualValues(QuestLevel.normal);
        SetVisualValues(QuestLevel.hard);
    }

    private void ShuffleQuestIndex(bool savingSeed)
    {
        if (savingSeed) {
            Manager.instance.questSeed = UnityEngine.Random.Range(0, int.MaxValue);
        }
        UnityEngine.Random.InitState(Manager.instance.questSeed);
        Essentials.InitAndShuffleArray(questsIndex);
        UnityEngine.Random.InitState(Environment.TickCount);
    }

    private void ShuffleGiftIndex(bool savingSeed)
    {
        if (savingSeed)
        {
            Manager.instance.questGiftSeed = UnityEngine.Random.Range(0, int.MaxValue);
        }
        UnityEngine.Random.InitState(Manager.instance.questGiftSeed);
        Essentials.InitAndShuffleArray(giftIndex);
        UnityEngine.Random.InitState(Environment.TickCount);
    }

    private void SetDBValues(QuestLevel questLevel) {
        switch (questLevel) {
            case QuestLevel.easy:
                Manager.instance.easyQuestStartTime = DateTime.Now.Date;
                Manager.instance.easyQuestCount = 0;
                Manager.instance.easyQuestGoal = questList[questsIndex[Manager.instance.questIndex]].easyAmount;
                Manager.instance.easyQuestType = questList[questsIndex[Manager.instance.questIndex]].questType;
                Manager.instance.easyQuestGiftType = giftList[giftIndex[0]].giftType;
                Manager.instance.easyQuestGiftAmount = giftList[giftIndex[0]].easyGiftAmount;
                Manager.instance.isEasyQuestClaimed = false;
                break;
            case QuestLevel.normal:
                Manager.instance.normalQuestStartTime = DateTime.Now.Date;
                Manager.instance.normalQuestCount = 0;
                Manager.instance.normalQuestGoal = questList[questsIndex[Manager.instance.questIndex + 1]].normalAmount;
                Manager.instance.normalQuestType = questList[questsIndex[Manager.instance.questIndex + 1]].questType;
                Manager.instance.normalQuestGiftType = giftList[giftIndex[1]].giftType;
                Manager.instance.normalQuestGiftAmount = giftList[giftIndex[1]].normalGiftAmount;
                Manager.instance.isNormalQuestClaimed = false;
                break;
            case QuestLevel.hard:
                Manager.instance.hardQuestStartTime = DateTime.Now.Date;
                Manager.instance.hardQuestCount = 0;
                Manager.instance.hardQuestGoal = questList[questsIndex[Manager.instance.questIndex + 2]].hardAmount;
                Manager.instance.hardQuestType = questList[questsIndex[Manager.instance.questIndex + 2]].questType;
                Manager.instance.hardQuestGiftType = giftList[giftIndex[2]].giftType;
                Manager.instance.hardQuestGiftAmount = giftList[giftIndex[2]].hardGiftAmount;
                Manager.instance.isHardQuestClaimed = false;
                break;
        }
    }

    private void SetVisualValues(QuestLevel questLevel)
    {
        switch (questLevel)
        {
            case QuestLevel.easy:
                if (Manager.instance.isEasyQuestClaimed)
                {
                    rewardAmountText[0].SetActive(false);
                    rewardTitle[0].SetActive(false);
                    crowns[0].SetActive(false);
                    easyBadge.GetComponent<Image>().sprite = ClaimedQuestSprite;
                    easyTick.SetActive(true);
                    easyClaimButton.gameObject.SetActive(false);
                    easyGreenBar.transform.parent.gameObject.SetActive(false);
                    easyText.text = LanguageManager.instance.GetTheTextByKey(textkey_claimed);
                }
                else
                {
                    rewardAmountText[0].SetActive(true);
                    rewardTitle[0].SetActive(true);
                    crowns[0].SetActive(true);
                    easyTick.SetActive(false);

                    if (Manager.instance.easyQuestCount > Manager.instance.easyQuestGoal)
                    {
                        Manager.instance.easyQuestCount = Manager.instance.easyQuestGoal;
                    }

                    easyRewardText.text = "+" + Manager.instance.easyQuestGiftAmount;
                    if (LanguageManager.instance.isLanguageRTL)
                        easyRewardText.text = LanguageManager.arabicFix(easyRewardText.text);
                    easyRewardImage.sprite = giftSprite[(int)Manager.instance.easyQuestGiftType];

                    string questNumber = Manager.instance.easyQuestGoal.ToString();
                    if (LanguageManager.instance.isLanguageRTL)
                        questNumber = LanguageManager.arabicFix(questNumber);
                    easyText.text =
                        LanguageManager.instance.GetTheTextByKey(questList[questsIndex[Manager.instance.questIndex]].textKey)
                            .Replace(sad, questNumber);

                    if (Manager.instance.easyQuestCount == Manager.instance.easyQuestGoal)
                    {
                        easyGreenBar.transform.parent.gameObject.SetActive(false);
                        easyClaimButton.gameObject.SetActive(true);
                        easyBadge.GetComponent<Image>().sprite = DoneQuestSprite;
                        DoneAct(QuestLevel.easy);
                    }
                    else
                    {
                        easyGreenBar.localScale =
                            new Vector3((float)Manager.instance.easyQuestCount / Manager.instance.easyQuestGoal, 1, 1);
                        easyProgressText.text = Manager.instance.easyQuestCount + "/" + Manager.instance.easyQuestGoal;
                        if (LanguageManager.instance.isLanguageRTL)
                            easyProgressText.text = LanguageManager.arabicFix(easyProgressText.text);
                        easyBadge.GetComponent<Image>().sprite = simpleQuestSprite;
                        easyClaimButton.gameObject.SetActive(false);
                        easyGreenBar.transform.parent.gameObject.SetActive(true);
                    }
                }
                break;
            case QuestLevel.normal:
                if (Manager.instance.isNormalQuestClaimed)
                {
                    rewardAmountText[1].SetActive(false);
                    rewardTitle[1].SetActive(false);
                    crowns[1].SetActive(false);
                    normalBadge.GetComponent<Image>().sprite = ClaimedQuestSprite;
                    normalTick.SetActive(true);
                    normalClaimButton.gameObject.SetActive(false);
                    normalGreenBar.transform.parent.gameObject.SetActive(false);
                    normalText.text = LanguageManager.instance.GetTheTextByKey(textkey_claimed);
                }
                else
                {
                    rewardAmountText[1].SetActive(true);
                    rewardTitle[1].SetActive(true);
                    crowns[1].SetActive(true);
                    normalTick.SetActive(false);

                    if (Manager.instance.normalQuestCount > Manager.instance.normalQuestGoal)
                    {
                        Manager.instance.normalQuestCount = Manager.instance.normalQuestGoal;
                    }

                    normalRewardText.text = "+" + Manager.instance.normalQuestGiftAmount;
                    if (LanguageManager.instance.isLanguageRTL)
                        normalRewardText.text = LanguageManager.arabicFix(normalRewardText.text);
                    normalRewardImage.sprite = giftSprite[(int)Manager.instance.normalQuestGiftType];

                    string questNumber = Manager.instance.normalQuestGoal.ToString();
                    if (LanguageManager.instance.isLanguageRTL)
                        questNumber = LanguageManager.arabicFix(questNumber);
                    normalText.text =
                        LanguageManager.instance.GetTheTextByKey(questList[questsIndex[Manager.instance.questIndex + 1]].textKey)
                            .Replace(sad, questNumber);

                    if (Manager.instance.normalQuestCount == Manager.instance.normalQuestGoal)
                    {
                        normalGreenBar.transform.parent.gameObject.SetActive(false);
                        normalClaimButton.gameObject.SetActive(true);
                        normalBadge.GetComponent<Image>().sprite = DoneQuestSprite;
                        DoneAct(QuestLevel.normal);
                    }
                    else
                    {
                        normalGreenBar.localScale =
                            new Vector3((float)Manager.instance.normalQuestCount / Manager.instance.normalQuestGoal, 1, 1);
                        normalProgressText.text = Manager.instance.normalQuestCount + "/" + Manager.instance.normalQuestGoal;
                        if (LanguageManager.instance.isLanguageRTL)
                            normalProgressText.text = LanguageManager.arabicFix(normalProgressText.text);
                        normalBadge.GetComponent<Image>().sprite = simpleQuestSprite;
                        normalClaimButton.gameObject.SetActive(false);
                        normalGreenBar.transform.parent.gameObject.SetActive(true);
                    }
                }
                break;
            case QuestLevel.hard:
                if (Manager.instance.isHardQuestClaimed)
                {
                    rewardAmountText[2].SetActive(false);
                    rewardTitle[2].SetActive(false);
                    crowns[2].SetActive(false);
                    hardBadge.GetComponent<Image>().sprite = ClaimedQuestSprite;
                    hardTick.SetActive(true);
                    hardClaimButton.gameObject.SetActive(false);
                    hardGreenBar.transform.parent.gameObject.SetActive(false);
                    hardText.text = LanguageManager.instance.GetTheTextByKey(textkey_claimed);
                }
                else
                {
                    rewardAmountText[2].SetActive(true);
                    rewardTitle[2].SetActive(true);
                    crowns[2].SetActive(true);
                    hardTick.SetActive(false);

                    if (Manager.instance.hardQuestCount > Manager.instance.hardQuestGoal)
                    {
                        Manager.instance.hardQuestCount = Manager.instance.hardQuestGoal;
                    }

                    hardRewardText.text = "+" + Manager.instance.hardQuestGiftAmount;
                    if (LanguageManager.instance.isLanguageRTL)
                        hardRewardText.text = LanguageManager.arabicFix(hardRewardText.text);
                    hardRewardImage.sprite = giftSprite[(int)Manager.instance.hardQuestGiftType];

                    string questNumber = Manager.instance.hardQuestGoal.ToString();
                    if (LanguageManager.instance.isLanguageRTL)
                        questNumber = LanguageManager.arabicFix(questNumber);
                    hardText.text =
                        LanguageManager.instance.GetTheTextByKey(questList[questsIndex[Manager.instance.questIndex + 2]].textKey)
                            .Replace(sad, questNumber);

                    if (Manager.instance.hardQuestCount == Manager.instance.hardQuestGoal)
                    {
                        hardGreenBar.transform.parent.gameObject.SetActive(false);
                        hardClaimButton.gameObject.SetActive(true);
                        hardBadge.GetComponent<Image>().sprite = DoneQuestSprite;
                        DoneAct(QuestLevel.hard);
                    }
                    else
                    {
                        hardGreenBar.localScale =
                            new Vector3((float)Manager.instance.hardQuestCount / Manager.instance.hardQuestGoal, 1, 1);
                        hardProgressText.text = Manager.instance.hardQuestCount + "/" + Manager.instance.hardQuestGoal;
                        if (LanguageManager.instance.isLanguageRTL)
                            hardProgressText.text = LanguageManager.arabicFix(hardProgressText.text);
                        hardBadge.GetComponent<Image>().sprite = simpleQuestSprite;
                        hardClaimButton.gameObject.SetActive(false);
                        hardGreenBar.transform.parent.gameObject.SetActive(true);
                    }
                }
                break;
        }
    }

    public void ClaimGifts(QuestLevel questLevel)
    {
        StartCoroutine(ClaimGiftsIE(questLevel));
    }

    public IEnumerator ClaimGiftsIE(QuestLevel questLevel) {
        GiftType giftType = GiftType.infiniteHeart; // just set a default
        int giftAmount = 0;
        Sounds.instance.PlayChestItems();
        VibrationManager.instance.VibrateSuccess();
        bool shouldResetQuest = true;
        switch (questLevel) {
            case QuestLevel.easy:
                giftType = Manager.instance.easyQuestGiftType;
                giftAmount = Manager.instance.easyQuestGiftAmount;
                if ((DateTime.Now - Manager.instance.easyQuestStartTime) < TimeSpan.FromDays(1))
                {
                    StartCoroutine(Transitions.setScale(easyBadge, new Vector3(0.02f, 0.02f, 1), 0.2f));
                    yield return StartCoroutine(Transitions.setScale(easyClaimButton.gameObject,
                        new Vector3(0.02f, 0.02f, 1), 0.2f));
                    rewardAmountText[0].SetActive(false);
                    rewardTitle[0].SetActive(false);
                    crowns[0].SetActive(false);
                    easyBadge.GetComponent<Image>().sprite = ClaimedQuestSprite;
                    easyTick.SetActive(true);
                    easyClaimButton.gameObject.SetActive(false);
                    StartCoroutine(Transitions.setScale(easyBadge, new Vector3(1, 1, 1), 0.2f));
                    Manager.instance.isEasyQuestClaimed = true;
                    shouldResetQuest = false;
                    AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.quest_easy_claimed);
                }
                break;
            case QuestLevel.normal:
                giftType = Manager.instance.normalQuestGiftType;
                giftAmount = Manager.instance.normalQuestGiftAmount;
                if ((DateTime.Now - Manager.instance.normalQuestStartTime) < TimeSpan.FromDays(1))
                {
                    StartCoroutine(Transitions.setScale(normalBadge, new Vector3(0.02f, 0.02f, 1), 0.2f));
                    yield return StartCoroutine(Transitions.setScale(normalClaimButton.gameObject,
                        new Vector3(0.02f, 0.02f, 1), 0.2f));
                    rewardAmountText[1].SetActive(false);
                    rewardTitle[1].SetActive(false);
                    crowns[1].SetActive(false);
                    normalBadge.GetComponent<Image>().sprite = ClaimedQuestSprite;
                    normalTick.SetActive(true);
                    normalClaimButton.gameObject.SetActive(false);
                    StartCoroutine(Transitions.setScale(normalBadge, new Vector3(1, 1, 1), 0.2f));
                    Manager.instance.isNormalQuestClaimed = true;
                    shouldResetQuest = false;
                    AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.quest_normal_claimed);
                }
                break;
            case QuestLevel.hard:
                giftType = Manager.instance.hardQuestGiftType;
                giftAmount = Manager.instance.hardQuestGiftAmount;
                if ((DateTime.Now - Manager.instance.hardQuestStartTime) < TimeSpan.FromDays(1))
                {
                    StartCoroutine(Transitions.setScale(hardBadge, new Vector3(0.02f, 0.02f, 1), 0.2f));
                    yield return StartCoroutine(Transitions.setScale(hardClaimButton.gameObject,
                        new Vector3(0.02f, 0.02f, 1), 0.2f));
                    rewardAmountText[2].SetActive(false);
                    rewardTitle[2].SetActive(false);
                    crowns[2].SetActive(false);
                    hardBadge.GetComponent<Image>().sprite = ClaimedQuestSprite;
                    hardTick.SetActive(true);
                    hardClaimButton.gameObject.SetActive(false);
                    StartCoroutine(Transitions.setScale(hardBadge, new Vector3(1, 1, 1), 0.2f));
                    Manager.instance.isHardQuestClaimed = true;
                    shouldResetQuest = false;
                    AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.quest_hard_claimed);
                }
                break;
        }
        if ((Manager.instance.easyQuestCount < Manager.instance.easyQuestGoal || Manager.instance.isEasyQuestClaimed) &&
            (Manager.instance.normalQuestCount < Manager.instance.normalQuestGoal || Manager.instance.isNormalQuestClaimed) &&
            (Manager.instance.hardQuestCount < Manager.instance.hardQuestGoal || Manager.instance.isHardQuestClaimed))
        {
            Manager.instance.isQuestRedPointOn = false;
            menu.redDot.SetActive(false);
        }
        switch (giftType)
        {
            case GiftType.infiniteHeart:
                Currencies.instance.UpdateInfiniteHeart(menu.HeartsNumText,menu.HeartsTimerText, menu.heartImage,giftAmount);
                break;
            case GiftType.coin:
                Currencies.instance.UpdateCoins(menu.coins.transform.GetChild(2).GetComponent<Text>(),giftAmount);
                Sounds.instance.PlayEarnCoin();
                break;
            case GiftType.hint:
                Manager.instance.numOfHint += giftAmount;
                CheckQuestType(QuestType.EarnHints, true, giftAmount, false,false);
                break;
            case GiftType.infinteMove:
                Manager.instance.numOfIMItem += giftAmount;
                CheckQuestType(QuestType.EarnInfiniteMove, true, giftAmount, false,false);
                break;
        }
        if (shouldResetQuest) {
            SetDBValues(questLevel);
            switch (questLevel) {
                case QuestLevel.easy:
                    StartCoroutine(Transitions.scaleUp(easyQuestPanel, new Vector3(0.2f, 0.2f, 1), 0.2f));
                    break;
                case QuestLevel.normal:

                    StartCoroutine(Transitions.scaleUp(normalQuestPanel, new Vector3(0.2f, 0.2f, 1), 0.2f));
                    break;
                case QuestLevel.hard:
                    StartCoroutine(Transitions.scaleUp(hardQuestPanel, new Vector3(0.2f, 0.2f, 1), 0.2f));
                    break;
            }
            SetVisualValues(questLevel);
        }
        Manager.instance.Save(false);
        yield return null;
    }

    public static void CheckQuestType(QuestType questType, bool isAdd,int amount,bool isReset,bool saveToDB) {
        if(Manager.instance.easyQuestType == questType){
            if (isReset)
            {
                ResetQuestProgress(QuestLevel.easy,saveToDB);
            }
            else
            {
                ChangeQuestProgress(QuestLevel.easy, isAdd, amount,saveToDB);
            }
        }
        else if (Manager.instance.normalQuestType == questType)
        {
            if (isReset)
            {
                ResetQuestProgress(QuestLevel.normal,saveToDB);
            }
            else
            {
                ChangeQuestProgress(QuestLevel.normal, isAdd, amount,saveToDB);
            }
        }
        else if (Manager.instance.hardQuestType == questType)
        {
            if (isReset)
            {
                ResetQuestProgress(QuestLevel.hard,saveToDB);
            }
            else
            {
                ChangeQuestProgress(QuestLevel.hard, isAdd, amount,saveToDB);
            }
        }
    }

    private static void ChangeQuestProgress(QuestLevel questLevel,bool isAdd,int amount,bool saveToDB) {
        switch (questLevel) {
            case QuestLevel.easy:
                if (Manager.instance.easyQuestCount < Manager.instance.easyQuestGoal) {
                    if (isAdd)
                    {
                        Manager.instance.easyQuestCount += amount;
                        if (Manager.instance.easyQuestCount >= Manager.instance.easyQuestGoal) {
                            AchievementManager.CheckAchievements_QuestCompleted();
                            AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.quest_easy_completed);
                            Manager.instance.isQuestRedPointOn = true;
                        }
                    }
                }
                if (!isAdd) {
                    Manager.instance.easyQuestCount -= amount;
                }
                break;
            case QuestLevel.normal:
                if (Manager.instance.normalQuestCount < Manager.instance.normalQuestGoal)
                {
                    if (isAdd)
                    {
                        Manager.instance.normalQuestCount += amount;
                        if (Manager.instance.normalQuestCount >= Manager.instance.normalQuestGoal) {
                            AchievementManager.CheckAchievements_QuestCompleted();
                            AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.quest_normal_completed);
                            Manager.instance.isQuestRedPointOn = true;
                        }
                    }
                }
                if(!isAdd)
                {
                    Manager.instance.normalQuestCount -= amount;
                }
                break;
            case QuestLevel.hard:
                if (Manager.instance.hardQuestCount < Manager.instance.hardQuestGoal)
                {
                    if (isAdd)
                    {
                        Manager.instance.hardQuestCount += amount;
                        if (Manager.instance.hardQuestCount >= Manager.instance.hardQuestGoal)
                        {
                            AchievementManager.CheckAchievements_QuestCompleted();
                            AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.quest_hard_completed);
                            Manager.instance.isQuestRedPointOn = true;
                        }
                    }
                }
                if(!isAdd)
                {
                    Manager.instance.hardQuestCount -= amount;
                }
                break;
        }
        if (saveToDB)
        {
            Manager.instance.Save(false);
        }
    }

    private static void ResetQuestProgress(QuestLevel questLevel, bool saveToDB) {
        switch (questLevel)
        {
            case QuestLevel.easy:
                if (Manager.instance.easyQuestCount < Manager.instance.easyQuestGoal) {
                    Manager.instance.easyQuestCount = 0;
                }
                break;
            case QuestLevel.normal:
                if (Manager.instance.normalQuestCount < Manager.instance.normalQuestGoal) {
                    Manager.instance.normalQuestCount = 0;
                }
                break;
            case QuestLevel.hard:
                if (Manager.instance.hardQuestCount < Manager.instance.hardQuestGoal) {
                    Manager.instance.hardQuestCount = 0;
                }
                break;
        }
        if (saveToDB) {
            Manager.instance.Save(false);
        }
    }

    private void DoneAct(QuestLevel questLevel) {
        Sounds.instance.PlayDailyChallengeDone();
        VibrationManager.instance.VibrateSuccess();
        switch (questLevel) {
            case QuestLevel.easy:
                StartCoroutine(Transitions.ErrorRotateZ(easyClaimButton.gameObject, 10, true, 0.1f, 20));
                break;
            case QuestLevel.normal:
                StartCoroutine(Transitions.ErrorRotateZ(normalClaimButton.gameObject, 10, true, 0.1f, 20));
                break;
            case QuestLevel.hard:
                StartCoroutine(Transitions.ErrorRotateZ(hardClaimButton.gameObject, 10, true, 0.1f, 20));
                break;
        }
    }

}
