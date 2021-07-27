using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public Text StoreTitle;
    public GameObject NoAds;
    public GameObject RestorePurchases;
    public GameObject[] Bundles = new GameObject[6];
    public GameObject[] Coins = new GameObject[6];
    public Text[] coinAmountTexts;
    public Text[] hintAmountTexts;
    public Text[] infiniteMovesTexts;

    [Header("InGameShop")] 
    public bool ingame;
    public float miniShopCoinsDiffY;
    public GameObject MoreOffersObj;
    public ScrollRect theShopScroll;
    public RectTransform theShopScrollRectTransform;
    public RectTransform theShopContentRectTransform;
    public UIManager uimanager;

    private float theDiffY;
    private bool smallShop;

    public static readonly string textkey_Restore = "RestorePurchases";//Also used in Menu Script
    private static readonly string textkey_ShopTitle = "Store";
    private static readonly string textkey_NoAds = "NoAds";
    private static readonly string textkey_XHours = "XHours";
    private static readonly string textkey_MoreOffers = "MoreOffers";
    private static readonly string[] textkey_BundleNames = { "Beginner", "Super", "Mega", "Giant", "Champion", "Legendary" };

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdatePricesAndCurrenciesIE());
    }

    private IEnumerator UpdatePricesAndCurrenciesIE()
    {
        for (int i = 0; i < 6; i++)
        {
            Bundles[i].transform.GetChild(0).GetComponentInChildren<Text>().text = LanguageManager.instance.GetTheTextByKey(textkey_BundleNames[i]);
        }
        for (int i = 3; i < 6; i++)
        {
            //Infinite Hearts
            string IHNumber = (6 * (Mathf.Pow(2, (i - 3)))).ToString();
            if (LanguageManager.instance.isLanguageRTL)
            {
                Bundles[i].transform.GetChild(2).GetComponentInChildren<Text>().text = LanguageManager.instance.GetTheTextByKey(textkey_XHours)
                    .Replace("۱۲", LanguageManager.arabicFix(IHNumber));
            }
            else
            {
                Bundles[i].transform.GetChild(2).GetComponentInChildren<Text>().text = LanguageManager.instance.GetTheTextByKey(textkey_XHours)
                    .Replace("12", IHNumber);
            }
        }
        StoreTitle.text = LanguageManager.instance.GetTheTextByKey(textkey_ShopTitle);
        if (!ingame)
        {
            Text RestoreText = RestorePurchases.GetComponentInChildren<Text>();
            RestoreText.text = LanguageManager.instance.GetTheTextByKey(textkey_Restore);
#if UNITY_ANDROID
            RestorePurchases.SetActive(false);
            Vector3 NoAdsPos = NoAds.transform.position;
            NoAdsPos.x = 0;
            NoAds.transform.position = NoAdsPos;
#endif
        }
        // PRICES :
        for (int j = 0; j < 20; j++)//Num Of Retries
        {
            if (IAPManager.instance.IsInitialized())
            {
                if (!ingame)
                {
                    Text noAdsText;
                    noAdsText = NoAds.transform.GetChild(1).GetChild(0).GetComponentInChildren<Text>();
                    noAdsText.text = IAPManager.instance.GetPrice_Product_NoAds();
                    if (LanguageManager.instance.isLanguageRTL) {
                        noAdsText.text = LanguageManager.arabicFix(noAdsText.text);
                    }
                    NoAds.transform.GetChild(1).GetChild(1).GetComponentInChildren<Text>().text = IAPManager.instance.GetCurrency_Product_NoAds();
                }
                Text bundleText;
                Text coinText;
                for (int i = 0; i < 6; i++)
                {
                    bundleText = Bundles[i].transform.GetChild(1).GetChild(0).GetComponentInChildren<Text>();
                    bundleText.text = IAPManager.instance.GetPrice_Product_Bundle(i + 1);
                    if (LanguageManager.instance.isLanguageRTL) {
                        bundleText.text = LanguageManager.arabicFix(bundleText.text);
                        coinAmountTexts[i].text = LanguageManager.arabicFix(coinAmountTexts[i].text);
                        hintAmountTexts[i].text = LanguageManager.arabicFix(hintAmountTexts[i].text);
                        infiniteMovesTexts[i].text = LanguageManager.arabicFix(infiniteMovesTexts[i].text);
                    }
                    Bundles[i].transform.GetChild(1).GetChild(1).GetComponentInChildren<Text>().text = IAPManager.instance.GetCurrency_Product_Bundle(i + 1);
                    coinText = Coins[i].transform.GetChild(1).GetChild(0).GetComponentInChildren<Text>();
                    coinText.text = IAPManager.instance.GetPrice_Product_Coins(i + 1);
                    if (LanguageManager.instance.isLanguageRTL) {
                        coinText.text = LanguageManager.arabicFix(coinText.text);
                        coinAmountTexts[i + 6].text = LanguageManager.arabicFix(coinAmountTexts[i+6].text); // for coins' bundles
                    }
                    Coins[i].transform.GetChild(1).GetChild(1).GetComponentInChildren<Text>().text = IAPManager.instance.GetCurrency_Product_Coins(i + 1);
                }

                yield break;
            }
            else
            {
                yield return new WaitForSeconds(1);
            }
        }
    }

    public void InitInGameShop()
    {
        if (ingame && !smallShop)
        {
            smallShop = true;
            uimanager.ingameShopIsOpened = true;
            theShopScroll.enabled = false;

            Vector2 coins0Pos = Coins[0].transform.localPosition;
            Vector2 coins1Pos = Coins[1].transform.localPosition;
            Vector2 moreOffersPos = MoreOffersObj.transform.localPosition;

            theDiffY = Coins[2].transform.localPosition.y - Coins[1].transform.localPosition.y + miniShopCoinsDiffY;
            coins0Pos.y -= theDiffY;
            coins1Pos.y -= theDiffY;
            moreOffersPos.y -= theDiffY;

            Coins[0].transform.localPosition = coins0Pos;
            Coins[1].transform.localPosition = coins1Pos;
            MoreOffersObj.transform.localPosition = moreOffersPos;


            MoreOffersObj.SetActive(true);
            for (int i = 2; i < 6; i++)
            {
                Coins[i].SetActive(false);
                Bundles[i].SetActive(false);
            }
        }
    }

    public void BuyNoAds()
    {
        IAPManager.instance.BuyNoAdsProduct();
    }

    public void Restore()
    {
        IAPManager.instance.RestorePurchases();
    }

    public void BuyBundle(int index)
    {
        IAPManager.instance.BuyBundle(index);
    }

    public void BuyCoins(int index)
    {
        IAPManager.instance.BuyCoins(index);
    }

    public void ShowMoreOffers()
    {
        if (ingame && smallShop)
        {
            smallShop = false;

            Vector2 coins0Pos = Coins[0].transform.localPosition;
            Vector2 coins1Pos = Coins[1].transform.localPosition;
            Vector2 moreOffersPos = MoreOffersObj.transform.localPosition;

            coins0Pos.y += theDiffY;
            coins1Pos.y += theDiffY;
            moreOffersPos.y += theDiffY;

            Coins[0].transform.localPosition = coins0Pos;
            Coins[1].transform.localPosition = coins1Pos;
            MoreOffersObj.transform.localPosition = moreOffersPos;

            MoreOffersObj.GetComponentInChildren<Text>().text = LanguageManager.instance.GetTheTextByKey(textkey_MoreOffers);

            MoreOffersObj.SetActive(false);
            for (int i = 2; i < 6; i++)
            {
                Coins[i].SetActive(true);
                Bundles[i].SetActive(true);
            }

            theShopScroll.enabled = true;
        }
    }

    public void CloseInGameShop()
    {
        if (uimanager.buyPowerUpType == 0 && Manager.instance.numOfHint != 0)
        {
            uimanager.CloseBuyPowerUps(false);
            uimanager.buyPowerUpType = -1;
        }
        else if(uimanager.buyPowerUpType == 1 && Manager.instance.numOfIMItem != 0) {
            uimanager.CloseBuyPowerUps(false);
            uimanager.buyPowerUpType = -1;
        }
        if (ingame)
        {
            uimanager.ingameShopIsOpened = false;
            uimanager.TopBar.SetActive(true);
            uimanager.BottomBar.SetActive(true);
            if (smallShop)
            {
                ShowMoreOffers();
                transform.parent.gameObject.SetActive(false);
            }
            else
            {
                theShopScroll.StopMovement();
                Canvas.ForceUpdateCanvases();
                Vector2 contentPos = theShopContentRectTransform.anchoredPosition;
                contentPos.y = theShopScrollRectTransform.sizeDelta.y;
                theShopContentRectTransform.anchoredPosition = contentPos;
                transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
