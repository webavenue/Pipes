using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StarChest : MonoBehaviour
{
    public GameObject starChest;
    //public GameObject starChestLight;
    public GameObject endPanel;
    public GameObject endGame;
    public GameObject endGameShare;
    public GameObject endGameContinue;
    public GameObject endGameRetry;
    public GameObject giftPanel;
    public GameObject bars;
    public GameObject stars;
    public GameObject crossButton;
    public GameObject moreGiftButton;
    public GameObject continueButton;
    public GameObject BottomBar;
    public float delayBetweenStarChestFilling;
    public Text starChestProgressText;
    public Text chooseTwoGiftsText;
    public Text twoMoreGiftsText;
    public RectTransform starChestBarTransform;
    public Sprite frontSprite;
    public Sprite[] giftSprite; // fill in order of GiftTypes enum
    public int[] unitOfGift; // the amount of first level of each gift!
    public List<GameObject> giftCards;
    public GameObject EndGameHints;
    public GameObject EndGameIMs;
    public GameObject CoinsPrefab;
    public GameObject HeartsPrefab;

    private IEnumerator fillingBarIE;
    private Vector3 endGamePos;
    private Vector3 starPos;
    private int choiceCount;
    private int[] giftIndexOfChoices;
    private int[] giftLevel = { 1, 1, 2, 2 };
    private IEnumerator closePanelIE;
    private Vector3 starChestPos;
    private Vector3 giftPanelPos;
    private GameObject theHearts;
    private GameObject theCoins;
    private Image theHeartImage;
    private Text theHeartsNumText;
    private Text theHeartsTimeText;
    private Text theCoinsText;
    private Text theHintsText;
    private Text theIMsText;

    private static readonly string textkey_ChooseTwoGifts = "ChooseTwoGifts";
    private static readonly string textkey_TwoMoreGifts = "TwoMoreGifts";

    void Awake()
    {
        giftIndexOfChoices = new int[4];
        starChestBarTransform.localScale = new Vector3(starChestBarTransform.localScale.x +
            Manager.instance.starChestProgress * (1 - 0.07f) / Manager.instance.starChestCapacity,
                starChestBarTransform.localScale.y, 1);
    }

    void Start()
    {
        starChestProgressText.text = "" + Manager.instance.starChestProgress + "/" + Manager.instance.starChestCapacity;
        if (LanguageManager.instance.isLanguageRTL)
            starChestProgressText.text = LanguageManager.arabicFix(starChestProgressText.text);
        endGamePos = endGame.transform.position;
        starPos = stars.transform.position;
        AdManager.instance.starChest = this;

        chooseTwoGiftsText.text = LanguageManager.instance.GetTheTextByKey(textkey_ChooseTwoGifts);
        if (Manager.instance.userHasBoughtNoAds)
        {
            if (LanguageManager.instance.isLanguageRTL)
            {
                chooseTwoGiftsText.text = chooseTwoGiftsText.text.Replace("۲", "۴");
            }
            else
                chooseTwoGiftsText.text = chooseTwoGiftsText.text.Replace("2", "4");
            choiceCount = 4;
        }
        else{
            choiceCount = 2;
        }
        twoMoreGiftsText.text = LanguageManager.instance.GetTheTextByKey(textkey_TwoMoreGifts);
    }

    public IEnumerator UpdateChestBar(int numOfStars)
    {
        Sounds.instance.PlayStarProgressBar();
        if (numOfStars > 0)
        {
            Quests.CheckQuestType(QuestType.EarnStars, true, numOfStars, false, false);
            fillingBarIE = Transitions.FillBarByScale(starChestBarTransform,
               numOfStars * (1 - 0.07f) / Manager.instance.starChestCapacity, delayBetweenStarChestFilling * numOfStars);
            StartCoroutine(fillingBarIE);
            for (int i = 0; i < numOfStars; i++)
            {
                Manager.instance.starChestProgress++;
                starChestProgressText.text = "" + Manager.instance.starChestProgress + "/" + Manager.instance.starChestCapacity;
                if (LanguageManager.instance.isLanguageRTL)
                    starChestProgressText.text = LanguageManager.arabicFix(starChestProgressText.text);
                yield return new WaitForSeconds(delayBetweenStarChestFilling);
                if (Manager.instance.starChestProgress >= Manager.instance.starChestCapacity)
                {
                    StartCoroutine(OpenStarChest(numOfStars - i - 1));
                    break;
                }
            }
            Manager.instance.Save(false);
        }
        else {
            yield return null;
        }
    }

    public void UpdateChestBarAtOnce(int numOfStars) {
        if (numOfStars > 0)
        {
            Quests.CheckQuestType(QuestType.EarnStars, true, numOfStars, false, false);
            for (int i = 0; i < numOfStars; i++)
            {
                Manager.instance.starChestProgress++;
                starChestProgressText.text =
                    "" + Manager.instance.starChestProgress + "/" + Manager.instance.starChestCapacity;
                if (LanguageManager.instance.isLanguageRTL)
                    starChestProgressText.text = LanguageManager.arabicFix(starChestProgressText.text);
                starChestBarTransform.localScale =
                    new Vector3(starChestBarTransform.localScale.x + (1 - 0.07f) / Manager.instance.starChestCapacity,
                        starChestBarTransform.localScale.y, 1);
                if (Manager.instance.starChestProgress >= Manager.instance.starChestCapacity)
                {
                    StartCoroutine(OpenStarChest(numOfStars - i - 1));
                    break;
                }
            }
            Manager.instance.Save(false);
        }
    }

    public IEnumerator OpenStarChest(int restOfStar)
    {
        AdManager.instance.HideBanner();
        Essentials.InitAndShuffleArray(giftIndexOfChoices);
        Essentials.ShuffleArray(giftLevel);
        Quests.CheckQuestType(QuestType.OpenChests,true,1,false,false);
        Quests.CheckQuestType(QuestType.OpenStarChest,true,1,false,false);
        if (fillingBarIE != null) {
            StopCoroutine(fillingBarIE);
        }
        Manager.instance.starChestProgress = restOfStar;
        Manager.instance.starChestCapacity = 20;
        Manager.instance.Save(false);
        Sounds.instance.PlayChestPrepare();
        VibrationManager.instance.VibrateHold();
        yield return StartCoroutine(Transitions.ErrorRotateZ(starChest, 15, false, 0.1f, 10));
        starChestPos = starChest.transform.position;
        StartCoroutine(Transitions.moveToPosition(starChest, giftPanel.transform.position, 1));
        StartCoroutine(Transitions.setScale(bars, new Vector3(0.02f, 0.02f, 1), 0.2f));
        bars.SetActive(false);
        StartCoroutine(Transitions.moveToPosition(stars, new Vector3(6, starPos.y, starPos.z), 0.5f));
        yield return StartCoroutine(Transitions.moveToPosition(endGame, new Vector3(6, endGamePos.y, endGamePos.z), 0.5f));
        stars.SetActive(false);
        endGame.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(1);
        Sounds.instance.PlayChestOpening();
        VibrationManager.instance.VibrateSuccess();
        StartCoroutine(Transitions.scaleUp(giftPanel, new Vector3(0.02f, 0.02f, 1), 0.2f));

        InitTheGiftDestinations();

        starChestBarTransform.localScale = new Vector3(restOfStar * (1 - 0.07f) / Manager.instance.starChestCapacity + 0.07f, starChestBarTransform.localScale.y, 1);
        starChestProgressText.text = "" + Manager.instance.starChestProgress + "/" + Manager.instance.starChestCapacity;
        if (LanguageManager.instance.isLanguageRTL)
            starChestProgressText.text = LanguageManager.arabicFix(starChestProgressText.text);
        AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.star_chest_opened);
    }

    private void InitTheGiftDestinations()
    {
        theHearts = Instantiate(HeartsPrefab, endPanel.transform);
        theHearts.transform.GetChild(1).gameObject.SetActive(false);
        theHeartsNumText = theHearts.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        theHeartsTimeText = theHearts.transform.GetChild(2).GetComponent<Text>();
        theHeartImage = theHearts.transform.GetChild(0).GetComponent<Image>();
        Currencies.instance.UpdateHeartsNumInDatabase();
        Currencies.instance.InitHeartValues(theHeartsNumText, theHeartsTimeText, theHeartImage);
        theHearts.SetActive(true);

        theCoins = Instantiate(CoinsPrefab, endPanel.transform);
        theCoins.transform.GetChild(1).gameObject.SetActive(false);
        theCoinsText = theCoins.transform.GetChild(2).GetComponent<Text>();
        theCoinsText.text = Manager.instance.coins.ToString();
        theCoins.SetActive(true);

        theHintsText = EndGameHints.transform.GetChild(1).GetChild(0).GetComponent<Text>();
        theHintsText.text = Manager.instance.numOfHint.ToString();
        EndGameHints.SetActive(true);

        theIMsText = EndGameIMs.transform.GetChild(1).GetChild(0).GetComponent<Text>();
        theIMsText.text = Manager.instance.numOfIMItem.ToString();
        EndGameIMs.SetActive(true);

    }

    public void GiftClick() {
        Sounds.instance.PlayChestItems();
        VibrationManager.instance.VibrateSuccess();
        GameObject selectedGift = EventSystem.current.currentSelectedGameObject;
        Image giftImage = selectedGift.transform.GetChild(0).gameObject.GetComponent<Image>();
        Text giftText = selectedGift.transform.GetChild(1).gameObject.GetComponent<Text>();
        GameObject giftBoxImage = selectedGift.transform.GetChild(2).gameObject;
        giftCards.Remove(selectedGift);
        StartCoroutine(FlipCard(selectedGift, giftImage, giftText,giftBoxImage));
        selectedGift.GetComponent<Button>().interactable = false;
    }

    private IEnumerator FlipCard(GameObject selectedGift, Image giftImage, Text giftText, GameObject giftBox)
    {
        choiceCount--;
        int tempGiftIndex = choiceCount;
        if (choiceCount == 0)
        {
            for (int i = 0; i < giftCards.Count; i++)
            {
                giftCards[i].GetComponent<Button>().interactable = false;
            }
        }
        yield return StartCoroutine(Transitions.RotateY(selectedGift, 90, true, 0.2f));
        giftBox.SetActive(false);
        selectedGift.GetComponent<Image>().sprite = frontSprite;
        yield return StartCoroutine(Transitions.RotateY(selectedGift, 90, false, 0.2f));
        GiveAGift(giftImage,giftText,false,tempGiftIndex);
        if (tempGiftIndex == 0) {
            StartCoroutine(Transitions.scaleUp(crossButton, new Vector3(0.02f, 0.02f, 1), 0.2f));
            if (!Manager.instance.userHasBoughtNoAds) {
                StartCoroutine(Transitions.scaleUp(moreGiftButton, new Vector3(0.02f, 0.02f, 1), 0.2f));
            }
        }
    }

    public void ShowVideoAdForExtraGifts()
    {
        if (AdManager.instance.isRewardedVideoAvailable())
        {
            crossButton.SetActive(false);
            moreGiftButton.SetActive(false);
        }
        AdManager.instance.ShowVideoAdIfAvailable(AdManager.rewardedVideoAdType.extraGifts);
    }

    public void ExtraGiftsVideoCanceled()
    {
        crossButton.SetActive(true);
        moreGiftButton.SetActive(true);
    }
    
    public void AfterAdsExtraGifts()
    {
        StartCoroutine(AfterMoreGiftADS());
    }

    private IEnumerator AfterMoreGiftADS()
    {
        Sounds.instance.PlayAfterVideoAdReward();
        VibrationManager.instance.VibrateSuccess();
        for (int i = 0; i < giftCards.Count; i++)
        {
            yield return StartCoroutine(Transitions.RotateY(giftCards[i], 90, true, 0.2f));
            giftCards[i].transform.GetChild(2).gameObject.SetActive(false);
            giftCards[i].GetComponent<Image>().sprite = frontSprite;
            yield return StartCoroutine(Transitions.RotateY(giftCards[i], 90, false, 0.2f));
            Image giftImage = giftCards[i].transform.GetChild(0).gameObject.GetComponent<Image>();
            Text giftText = giftCards[i].transform.GetChild(1).gameObject.GetComponent<Text>();
            GiveAGift(giftImage, giftText, true, i);
            yield return new WaitForSeconds(0.5f);
        }
        StartCoroutine(Transitions.scaleUp(continueButton, new Vector3(0.02f, 0.02f, 1), 0.2f));
        AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.star_chest_extra_gifts_earned);
    }

    public void ClosePanel()
    {
        closePanelIE = ClosePanelIE();
        StartCoroutine(closePanelIE);
    }

    private IEnumerator ClosePanelIE()
    {
        starChest.transform.position = starChestPos;
        starChest.SetActive(false);
        giftPanelPos = giftPanel.transform.position;
        yield return StartCoroutine(Transitions.moveToPosition(giftPanel, new Vector3(6, giftPanelPos.y, giftPanelPos.z), 0.5f));
        giftPanel.SetActive(false);
        Currencies.instance.StopHeartTimer();
        EndGameHints.SetActive(false);
        EndGameIMs.SetActive(false);
        Destroy(theHearts);
        Destroy(theCoins);
        BackToEndGame();
        AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.star_chest_extra_gifts_canceled);
    }

    public void BackToEndGame() {
        StartCoroutine(Transitions.scaleUp(starChest, new Vector3(0.2f, 0.2f, 1), 0.2f));
        bars.SetActive(true);
        StartCoroutine(Transitions.setScale(bars, new Vector3(1, 1, 1), 0.2f));
        endGame.transform.position = endGamePos;
        StartCoroutine(Transitions.scaleUp(endGame, new Vector3(0.2f, 0.2f, 1), 0.2f));
        stars.transform.position = starPos;
        StartCoroutine(Transitions.scaleUp(stars, new Vector3(0.2f, 0.2f, 1), 0.2f));
        endGameShare.SetActive(true);
        endGameContinue.SetActive(true);
        endGameRetry.SetActive(true);
    }

    private void GiveAGift(Image giftImage,Text giftText,bool isAfterADS,int index)
    {
        if (isAfterADS) {
            index += 2;
        }
        giftImage.gameObject.SetActive(true);
        giftText.gameObject.SetActive(true);
        giftImage.sprite = giftSprite[giftIndexOfChoices[index]];
        giftText.text = "+" + giftLevel[giftIndexOfChoices[index]] * unitOfGift[giftIndexOfChoices[index]];
        if (giftIndexOfChoices[index] == (int)GiftType.infiniteHeart) {
            giftText.text += ":00";
        }
        if (LanguageManager.instance.isLanguageRTL)
            giftText.text = LanguageManager.arabicFix(giftText.text);
        StartCoroutine(Transitions.scaleUp(giftImage.gameObject, new Vector3(0.02f, 0.02f, 0.02f), 0.1f));
        StartCoroutine(Transitions.scaleUp(giftText.gameObject, new Vector3(0.02f, 0.02f, 0.02f), 0.1f));
        switch ((GiftType)giftIndexOfChoices[index])
        {
            case GiftType.infiniteHeart:
                Currencies.instance.UpdateInfiniteHeart(theHeartsNumText, theHeartsTimeText, theHeartImage, giftLevel[0] * unitOfGift[0]);
                break;
            case GiftType.coin:
                Currencies.instance.UpdateCoins(theCoinsText,giftLevel[1] * unitOfGift[1]);
                Sounds.instance.PlayEarnCoin();
                break;
            case GiftType.hint:
                Manager.instance.numOfHint += giftLevel[2] * unitOfGift[2];
                Quests.CheckQuestType(QuestType.EarnHints, true, giftLevel[2] * unitOfGift[2], false,false);
                theHintsText.text = Manager.instance.numOfHint.ToString();
                StartCoroutine(Transitions.stamp(EndGameHints, new Vector3(2f, 2f, 2f), 0.2f));
                var particleMain = Instantiate(Currencies.instance.updateParticle, EndGameHints.transform.position,
                    Quaternion.identity, EndGameHints.transform).GetComponent<ParticleSystem>().main;
                particleMain.loop = false;
                if (!Manager.instance.isFTUEHintShown) {
                    Manager.instance.isHintGivenBeforeFTUE = true;
                }
                break;
            case GiftType.infinteMove:
                Manager.instance.numOfIMItem += giftLevel[3] * unitOfGift[3];
                Quests.CheckQuestType(QuestType.EarnInfiniteMove, true, giftLevel[3] * unitOfGift[3], false,false);
                theIMsText.text = Manager.instance.numOfIMItem.ToString();
                StartCoroutine(Transitions.stamp(EndGameIMs, new Vector3(2f, 2f, 2f), 0.2f));
                particleMain = Instantiate(Currencies.instance.updateParticle, EndGameIMs.transform.position,
                    Quaternion.identity, EndGameIMs.transform).GetComponent<ParticleSystem>().main;
                particleMain.loop = false;
                if (!Manager.instance.isFTUEInfiniteMoveShown) {
                    Manager.instance.isInfiniteMovesGivenBeforeFTUE = true;
                }
                break;
        }
        Manager.instance.Save(false);
    }

}
