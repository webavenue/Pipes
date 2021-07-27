using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class IAPManager : MonoBehaviour, IStoreListener
{
    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    // Product identifiers for all products capable of being purchased: 
    // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
    // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
    // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

    // General product identifiers for the consumable, non-consumable, and subscription products.
    // Use these handles in the code to reference which product to purchase. Also use these values 
    // when defining the Product Identifiers on the store. Except, for illustration purposes, the 
    // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
    // specific mapping to Unity Purchasing's AddProduct, below.
    public static string product_pipes_no_ads = "pipes_no_ads";

    public static string product_pipes_coins_1 = "pipes_coins_1";
    public static string product_pipes_coins_2 = "pipes_coins_2";
    public static string product_pipes_coins_3 = "pipes_coins_3";
    public static string product_pipes_coins_4 = "pipes_coins_4";
    public static string product_pipes_coins_5 = "pipes_coins_5";
    public static string product_pipes_coins_6 = "pipes_coins_6";

    public static string product_pipes_bundle_1 = "pipes_bundle_1";
    public static string product_pipes_bundle_2 = "pipes_bundle_2";
    public static string product_pipes_bundle_3 = "pipes_bundle_3";
    public static string product_pipes_bundle_4 = "pipes_bundle_4";
    public static string product_pipes_bundle_5 = "pipes_bundle_5";
    public static string product_pipes_bundle_6 = "pipes_bundle_6";

    [HideInInspector]
    public Menu menuScript;
    [HideInInspector]
    public UIManager uiManagerScript;

    public static IAPManager instance;

    private AnalyticsManager.NoAdsPlaces noAdsPlace;

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
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }
    }

    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());


        // Add a product to sell / restore by way of its identifier, associating the general identifier
        // with its store-specific identifiers.
        builder.AddProduct(product_pipes_coins_1, ProductType.Consumable);
        builder.AddProduct(product_pipes_coins_2, ProductType.Consumable);
        builder.AddProduct(product_pipes_coins_3, ProductType.Consumable);
        builder.AddProduct(product_pipes_coins_4, ProductType.Consumable);
        builder.AddProduct(product_pipes_coins_5, ProductType.Consumable);
        builder.AddProduct(product_pipes_coins_6, ProductType.Consumable);

        builder.AddProduct(product_pipes_bundle_1, ProductType.Consumable);
        builder.AddProduct(product_pipes_bundle_2, ProductType.Consumable);
        builder.AddProduct(product_pipes_bundle_3, ProductType.Consumable);
        builder.AddProduct(product_pipes_bundle_4, ProductType.Consumable);
        builder.AddProduct(product_pipes_bundle_5, ProductType.Consumable);
        builder.AddProduct(product_pipes_bundle_6, ProductType.Consumable);

        // Continue adding the non-consumable product.
        builder.AddProduct(product_pipes_no_ads, ProductType.NonConsumable);
        // And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
        // if the Product ID was configured differently between Apple and Google stores. Also note that
        // one uses the general kProductIDSubscription handle inside the game - the store-specific IDs 
        // must only be referenced here. 

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }


    public bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void BuyNoAdsProduct()
    {
        // Buy the non-consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        if (Manager.instance.userHasBoughtNoAds)
            return;
        Sounds.instance.PlayUIInteraction();
        VibrationManager.instance.VibrateTouch();
        if (GameScenes.instance.AreWeInMenu())
        {
            noAdsPlace = AnalyticsManager.NoAdsPlaces.GameStore;
        }
        else
        {
            noAdsPlace = AnalyticsManager.NoAdsPlaces.NoAdsPrompt;
        }
        BuyProductID(product_pipes_no_ads);
    }

    public void BuyBundle(int index)
    {
        // Buy the non-consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        Sounds.instance.PlayUIInteraction();
        VibrationManager.instance.VibrateTouch();
        string bundle_product_id;
        switch (index)
        {
            case 1:
                bundle_product_id = product_pipes_bundle_1;
                break;
            case 2:
                bundle_product_id = product_pipes_bundle_2;
                break;
            case 3:
                bundle_product_id = product_pipes_bundle_3;
                break;
            case 4:
                bundle_product_id = product_pipes_bundle_4;
                break;
            case 5:
                bundle_product_id = product_pipes_bundle_5;
                break;
            case 6:
                bundle_product_id = product_pipes_bundle_6;
                break;
            default:
                return;
        }
        BuyProductID(bundle_product_id);
    }

    public void BuyCoins(int index)
    {
        // Buy the non-consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        Sounds.instance.PlayUIInteraction();
        VibrationManager.instance.VibrateTouch();
        string coins_product_id;
        switch (index)
        {
            case 1:
                coins_product_id = product_pipes_coins_1;
                break;
            case 2:
                coins_product_id = product_pipes_coins_2;
                break;
            case 3:
                coins_product_id = product_pipes_coins_3;
                break;
            case 4:
                coins_product_id = product_pipes_coins_4;
                break;
            case 5:
                coins_product_id = product_pipes_coins_5;
                break;
            case 6:
                coins_product_id = product_pipes_coins_6;
                break;
            default:
                return;
        }
        BuyProductID(coins_product_id);
    }


    void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    // this func is only for apple.
    public void RestorePurchases()
    {
        Sounds.instance.PlayUIInteraction();
        VibrationManager.instance.VibrateTouch();
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // ... begin restoring purchases
        Debug.Log("RestorePurchases started ...");

        // Fetch the Apple store-specific subsystem.
        var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
        // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
        // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
        apple.RestoreTransactions((result) => {
            // The first phase of restoration. If no more responses are received on ProcessPurchase then 
            // no purchases are available to be restored.
            Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
        });
    }


    //  
    // --- IStoreListener
    //

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        // A consumable product has been purchased by this user.
        if (string.Equals(args.purchasedProduct.definition.id, product_pipes_bundle_1, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            Manager.instance.numOfHint += 1;
            Manager.instance.numOfIMItem += 1;
            Sounds.instance.PlayAfterVideoAdReward();
            VibrationManager.instance.VibrateSuccess();
            if (GameScenes.instance.AreWeInMenu())
            {
                Currencies.instance.UpdateCoins(menuScript.coins.transform.GetChild(2).GetComponent<Text>(), 200);
                if (!Manager.instance.isFTUEHintShown) {
                    Manager.instance.isHintGivenBeforeFTUE = true;
                }
                if (!Manager.instance.isFTUEInfiniteMoveShown)
                {
                    Manager.instance.isInfiniteMovesGivenBeforeFTUE = true;
                }
            }
            else
            {
                GameObject hintPlus = uiManagerScript.hintBtn.transform.GetChild(2).gameObject;
                GameObject infiniteMovePlus = uiManagerScript.InfiniteMoveButton.transform.GetChild(2).gameObject;
                Currencies.instance.UpdateCoins(null, 200);
                uiManagerScript.CheckPowerUpsLock();
                if (hintPlus.activeSelf) {
                    uiManagerScript.ToggleHintPlusAndBadge(false);
                }
                if (infiniteMovePlus.activeSelf) {
                    uiManagerScript.ToggleInfiniteMovesPlusAndBadge(false);
                }
                uiManagerScript.hintNumber.text = Manager.instance.numOfHint.ToString();
                uiManagerScript.InfiniteMoveItemNumber.text = Manager.instance.numOfIMItem.ToString();
            }
            Quests.CheckQuestType(QuestType.EarnHints, true, 1, false, false);
            Quests.CheckQuestType(QuestType.EarnInfiniteMove, true, 1, false, false);
            Manager.instance.Save(true);
            AnalyticsManager.instance.LogBundlePurchased(1, args.purchasedProduct.metadata.isoCurrencyCode, args.purchasedProduct.metadata.localizedPrice,
                args.purchasedProduct.definition.id, args.purchasedProduct.receipt, args.purchasedProduct.transactionID);
        }
        // A consumable product has been purchased by this user.
        else if (string.Equals(args.purchasedProduct.definition.id, product_pipes_bundle_2, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            Manager.instance.numOfHint += 1;
            Manager.instance.numOfIMItem += 1;
            Sounds.instance.PlayAfterVideoAdReward();
            VibrationManager.instance.VibrateSuccess();
            if (GameScenes.instance.AreWeInMenu())
            {
                Currencies.instance.UpdateCoins(menuScript.coins.transform.GetChild(2).GetComponent<Text>(), 500);
                if (!Manager.instance.isFTUEHintShown)
                {
                    Manager.instance.isHintGivenBeforeFTUE = true;
                }
                if (!Manager.instance.isFTUEInfiniteMoveShown)
                {
                    Manager.instance.isInfiniteMovesGivenBeforeFTUE = true;
                }
            }
            else
            {
                GameObject hintPlus = uiManagerScript.hintBtn.transform.GetChild(2).gameObject;
                GameObject infiniteMovePlus = uiManagerScript.InfiniteMoveButton.transform.GetChild(2).gameObject;
                Currencies.instance.UpdateCoins(null, 500);
                uiManagerScript.CheckPowerUpsLock();
                if (hintPlus.activeSelf)
                {
                    uiManagerScript.ToggleHintPlusAndBadge(false);
                }
                if (infiniteMovePlus.activeSelf)
                {
                    uiManagerScript.ToggleInfiniteMovesPlusAndBadge(false);
                }
                uiManagerScript.hintNumber.text = Manager.instance.numOfHint.ToString();
                uiManagerScript.InfiniteMoveItemNumber.text = Manager.instance.numOfIMItem.ToString();
            }
            Quests.CheckQuestType(QuestType.EarnHints, true, 1, false, false);
            Quests.CheckQuestType(QuestType.EarnInfiniteMove, true, 1, false, false);
            Manager.instance.Save(true);
            AnalyticsManager.instance.LogBundlePurchased(2, args.purchasedProduct.metadata.isoCurrencyCode, args.purchasedProduct.metadata.localizedPrice,
                args.purchasedProduct.definition.id, args.purchasedProduct.receipt, args.purchasedProduct.transactionID);
        }
        // A consumable product has been purchased by this user.
        else if (string.Equals(args.purchasedProduct.definition.id, product_pipes_bundle_3, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            Manager.instance.numOfHint += 3;
            Manager.instance.numOfIMItem += 3;
            Sounds.instance.PlayAfterVideoAdReward();
            VibrationManager.instance.VibrateSuccess();
            if (GameScenes.instance.AreWeInMenu())
            {
                Currencies.instance.UpdateCoins(menuScript.coins.transform.GetChild(2).GetComponent<Text>(), 1000);
                if (!Manager.instance.isFTUEHintShown)
                {
                    Manager.instance.isHintGivenBeforeFTUE = true;
                }
                if (!Manager.instance.isFTUEInfiniteMoveShown)
                {
                    Manager.instance.isInfiniteMovesGivenBeforeFTUE = true;
                }
            }
            else
            {
                GameObject hintPlus = uiManagerScript.hintBtn.transform.GetChild(2).gameObject;
                GameObject infiniteMovePlus = uiManagerScript.InfiniteMoveButton.transform.GetChild(2).gameObject;
                Currencies.instance.UpdateCoins(null, 1000);
                uiManagerScript.CheckPowerUpsLock();
                if (hintPlus.activeSelf)
                {
                    uiManagerScript.ToggleHintPlusAndBadge(false);
                }
                if (infiniteMovePlus.activeSelf)
                {
                    uiManagerScript.ToggleInfiniteMovesPlusAndBadge(false);
                }
                uiManagerScript.hintNumber.text = Manager.instance.numOfHint.ToString();
                uiManagerScript.InfiniteMoveItemNumber.text = Manager.instance.numOfIMItem.ToString();
            }
            Quests.CheckQuestType(QuestType.EarnHints, true, 3, false, false);
            Quests.CheckQuestType(QuestType.EarnInfiniteMove, true, 3, false, false);
            Manager.instance.Save(true);
            AnalyticsManager.instance.LogBundlePurchased(3, args.purchasedProduct.metadata.isoCurrencyCode, args.purchasedProduct.metadata.localizedPrice,
                args.purchasedProduct.definition.id, args.purchasedProduct.receipt, args.purchasedProduct.transactionID);
        }
        // A consumable product has been purchased by this user.
        else if (string.Equals(args.purchasedProduct.definition.id, product_pipes_bundle_4, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            Manager.instance.numOfHint += 5;
            Manager.instance.numOfIMItem += 5;
            Sounds.instance.PlayAfterVideoAdReward();
            VibrationManager.instance.VibrateSuccess();
            if (GameScenes.instance.AreWeInMenu())
            {
                Currencies.instance.UpdateCoins(menuScript.coins.transform.GetChild(2).GetComponent<Text>(), 2000);
                Currencies.instance.UpdateInfiniteHeart(menuScript.HeartsNumText, menuScript.HeartsTimerText, menuScript.heartImage, 360);
                if (!Manager.instance.isFTUEHintShown)
                {
                    Manager.instance.isHintGivenBeforeFTUE = true;
                }
                if (!Manager.instance.isFTUEInfiniteMoveShown)
                {
                    Manager.instance.isInfiniteMovesGivenBeforeFTUE = true;
                }
            }
            else
            {
                GameObject hintPlus = uiManagerScript.hintBtn.transform.GetChild(2).gameObject;
                GameObject infiniteMovePlus = uiManagerScript.InfiniteMoveButton.transform.GetChild(2).gameObject;
                Currencies.instance.UpdateCoins(null, 2000);
                Currencies.instance.UpdateInfiniteHeart(null, null, null, 360);
                uiManagerScript.CheckPowerUpsLock();
                if (hintPlus.activeSelf)
                {
                    uiManagerScript.ToggleHintPlusAndBadge(false);
                }
                if (infiniteMovePlus.activeSelf)
                {
                    uiManagerScript.ToggleInfiniteMovesPlusAndBadge(false);
                }
                uiManagerScript.hintNumber.text = Manager.instance.numOfHint.ToString();
                uiManagerScript.InfiniteMoveItemNumber.text = Manager.instance.numOfIMItem.ToString();
            }
            Quests.CheckQuestType(QuestType.EarnHints, true, 5, false, false);
            Quests.CheckQuestType(QuestType.EarnInfiniteMove, true, 5, false, false);
            Manager.instance.Save(true);
            AnalyticsManager.instance.LogBundlePurchased(4, args.purchasedProduct.metadata.isoCurrencyCode, args.purchasedProduct.metadata.localizedPrice,
                args.purchasedProduct.definition.id, args.purchasedProduct.receipt, args.purchasedProduct.transactionID);
        }
        // A consumable product has been purchased by this user.
        else if (string.Equals(args.purchasedProduct.definition.id, product_pipes_bundle_5, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            Manager.instance.numOfHint += 12;
            Manager.instance.numOfIMItem += 12;
            Sounds.instance.PlayAfterVideoAdReward();
            VibrationManager.instance.VibrateSuccess();
            if (GameScenes.instance.AreWeInMenu())
            {
                Currencies.instance.UpdateCoins(menuScript.coins.transform.GetChild(2).GetComponent<Text>(), 4000);
                Currencies.instance.UpdateInfiniteHeart(menuScript.HeartsNumText, menuScript.HeartsTimerText, menuScript.heartImage, 720);
                if (!Manager.instance.isFTUEHintShown)
                {
                    Manager.instance.isHintGivenBeforeFTUE = true;
                }
                if (!Manager.instance.isFTUEInfiniteMoveShown)
                {
                    Manager.instance.isInfiniteMovesGivenBeforeFTUE = true;
                }
            }
            else
            {
                GameObject hintPlus = uiManagerScript.hintBtn.transform.GetChild(2).gameObject;
                GameObject infiniteMovePlus = uiManagerScript.InfiniteMoveButton.transform.GetChild(2).gameObject;
                Currencies.instance.UpdateCoins(null, 4000);
                Currencies.instance.UpdateInfiniteHeart(null, null, null, 720);
                uiManagerScript.CheckPowerUpsLock();
                if (hintPlus.activeSelf)
                {
                    uiManagerScript.ToggleHintPlusAndBadge(false);
                }
                if (infiniteMovePlus.activeSelf)
                {
                    uiManagerScript.ToggleInfiniteMovesPlusAndBadge(false);
                }
                uiManagerScript.hintNumber.text = Manager.instance.numOfHint.ToString();
                uiManagerScript.InfiniteMoveItemNumber.text = Manager.instance.numOfIMItem.ToString();
            }
            Quests.CheckQuestType(QuestType.EarnHints, true, 12, false, false);
            Quests.CheckQuestType(QuestType.EarnInfiniteMove, true, 12, false, false);
            Manager.instance.Save(true);
            AnalyticsManager.instance.LogBundlePurchased(5, args.purchasedProduct.metadata.isoCurrencyCode, args.purchasedProduct.metadata.localizedPrice,
                args.purchasedProduct.definition.id, args.purchasedProduct.receipt, args.purchasedProduct.transactionID);
        }
        // A consumable product has been purchased by this user.
        else if (string.Equals(args.purchasedProduct.definition.id, product_pipes_bundle_6, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            Manager.instance.numOfHint += 25;
            Manager.instance.numOfIMItem += 25;
            Sounds.instance.PlayAfterVideoAdReward();
            VibrationManager.instance.VibrateSuccess();
            if (GameScenes.instance.AreWeInMenu())
            {
                Currencies.instance.UpdateCoins(menuScript.coins.transform.GetChild(2).GetComponent<Text>(), 8000);
                Currencies.instance.UpdateInfiniteHeart(menuScript.HeartsNumText, menuScript.HeartsTimerText, menuScript.heartImage, 1440);
                if (!Manager.instance.isFTUEHintShown)
                {
                    Manager.instance.isHintGivenBeforeFTUE = true;
                }
                if (!Manager.instance.isFTUEInfiniteMoveShown)
                {
                    Manager.instance.isInfiniteMovesGivenBeforeFTUE = true;
                }
            }
            else
            {
                GameObject hintPlus = uiManagerScript.hintBtn.transform.GetChild(2).gameObject;
                GameObject infiniteMovePlus = uiManagerScript.InfiniteMoveButton.transform.GetChild(2).gameObject;
                Currencies.instance.UpdateCoins(null, 8000);
                Currencies.instance.UpdateInfiniteHeart(null, null, null, 1440);
                uiManagerScript.CheckPowerUpsLock();
                if (hintPlus.activeSelf)
                {
                    uiManagerScript.ToggleHintPlusAndBadge(false);
                }
                if (infiniteMovePlus.activeSelf)
                {
                    uiManagerScript.ToggleInfiniteMovesPlusAndBadge(false);
                }
                uiManagerScript.hintNumber.text = Manager.instance.numOfHint.ToString();
                uiManagerScript.InfiniteMoveItemNumber.text = Manager.instance.numOfIMItem.ToString();
            }
            Quests.CheckQuestType(QuestType.EarnHints, true, 25, false, false);
            Quests.CheckQuestType(QuestType.EarnInfiniteMove, true, 25, false, false);
            Manager.instance.Save(true);
            AnalyticsManager.instance.LogBundlePurchased(6, args.purchasedProduct.metadata.isoCurrencyCode, args.purchasedProduct.metadata.localizedPrice,
                args.purchasedProduct.definition.id, args.purchasedProduct.receipt, args.purchasedProduct.transactionID);
        }
        // A consumable product has been purchased by this user.
        else if (string.Equals(args.purchasedProduct.definition.id, product_pipes_coins_1, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            Sounds.instance.PlayAfterVideoAdReward();
            VibrationManager.instance.VibrateSuccess();
            if (GameScenes.instance.AreWeInMenu())
            {
                Currencies.instance.UpdateCoins(menuScript.coins.transform.GetChild(2).GetComponent<Text>(), 150);
            }
            else
            {
                Currencies.instance.UpdateCoins(null, 150);
            }
            Manager.instance.Save(true);
            AnalyticsManager.instance.LogCoinsPurchased(1, args.purchasedProduct.metadata.isoCurrencyCode, args.purchasedProduct.metadata.localizedPrice,
                args.purchasedProduct.definition.id, args.purchasedProduct.receipt, args.purchasedProduct.transactionID);
        }
        // A consumable product has been purchased by this user.
        else if (string.Equals(args.purchasedProduct.definition.id, product_pipes_coins_2, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            Sounds.instance.PlayAfterVideoAdReward();
            VibrationManager.instance.VibrateSuccess();
            if (GameScenes.instance.AreWeInMenu())
            {
                Currencies.instance.UpdateCoins(menuScript.coins.transform.GetChild(2).GetComponent<Text>(), 500);
            }
            else
            {
                Currencies.instance.UpdateCoins(null, 500);
            }
            Manager.instance.Save(true);
            AnalyticsManager.instance.LogCoinsPurchased(2, args.purchasedProduct.metadata.isoCurrencyCode, args.purchasedProduct.metadata.localizedPrice,
                args.purchasedProduct.definition.id, args.purchasedProduct.receipt, args.purchasedProduct.transactionID);
        }
        // A consumable product has been purchased by this user.
        else if (string.Equals(args.purchasedProduct.definition.id, product_pipes_coins_3, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            Sounds.instance.PlayAfterVideoAdReward();
            VibrationManager.instance.VibrateSuccess();
            if (GameScenes.instance.AreWeInMenu())
            {
                Currencies.instance.UpdateCoins(menuScript.coins.transform.GetChild(2).GetComponent<Text>(), 1000);
            }
            else
            {
                Currencies.instance.UpdateCoins(null, 1000);
            }
            Manager.instance.Save(true);
            AnalyticsManager.instance.LogCoinsPurchased(3, args.purchasedProduct.metadata.isoCurrencyCode, args.purchasedProduct.metadata.localizedPrice,
                args.purchasedProduct.definition.id, args.purchasedProduct.receipt, args.purchasedProduct.transactionID);
        }
        // A consumable product has been purchased by this user.
        else if (string.Equals(args.purchasedProduct.definition.id, product_pipes_coins_4, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            Sounds.instance.PlayAfterVideoAdReward();
            VibrationManager.instance.VibrateSuccess();
            if (GameScenes.instance.AreWeInMenu())
            {
                Currencies.instance.UpdateCoins(menuScript.coins.transform.GetChild(2).GetComponent<Text>(), 2000);
            }
            else
            {
                Currencies.instance.UpdateCoins(null, 2000);
            }
            Manager.instance.Save(true);
            AnalyticsManager.instance.LogCoinsPurchased(4, args.purchasedProduct.metadata.isoCurrencyCode, args.purchasedProduct.metadata.localizedPrice,
                args.purchasedProduct.definition.id, args.purchasedProduct.receipt, args.purchasedProduct.transactionID);
        }
        // A consumable product has been purchased by this user.
        else if (string.Equals(args.purchasedProduct.definition.id, product_pipes_coins_5, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            Sounds.instance.PlayAfterVideoAdReward();
            VibrationManager.instance.VibrateSuccess();
            if (GameScenes.instance.AreWeInMenu())
            {
                Currencies.instance.UpdateCoins(menuScript.coins.transform.GetChild(2).GetComponent<Text>(), 4000);
            }
            else
            {
                Currencies.instance.UpdateCoins(null, 4000);
            }
            Manager.instance.Save(true);
            AnalyticsManager.instance.LogCoinsPurchased(5, args.purchasedProduct.metadata.isoCurrencyCode, args.purchasedProduct.metadata.localizedPrice,
                args.purchasedProduct.definition.id, args.purchasedProduct.receipt, args.purchasedProduct.transactionID);
        }
        // A consumable product has been purchased by this user.
        else if (string.Equals(args.purchasedProduct.definition.id, product_pipes_coins_6, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            Sounds.instance.PlayAfterVideoAdReward();
            VibrationManager.instance.VibrateSuccess();
            if (GameScenes.instance.AreWeInMenu())
            {
                Currencies.instance.UpdateCoins(menuScript.coins.transform.GetChild(2).GetComponent<Text>(), 8000);
            }
            else
            {
                Currencies.instance.UpdateCoins(null, 8000);
            }
            Manager.instance.Save(true);
            AnalyticsManager.instance.LogCoinsPurchased(6, args.purchasedProduct.metadata.isoCurrencyCode, args.purchasedProduct.metadata.localizedPrice,
                args.purchasedProduct.definition.id, args.purchasedProduct.receipt, args.purchasedProduct.transactionID);
        }
        // Or ... a non-consumable product has been purchased by this user.
        else if (string.Equals(args.purchasedProduct.definition.id, product_pipes_no_ads, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // The non-consumable item has been successfully purchased, grant this item to the player.
            Manager.instance.userHasBoughtNoAds = true;
            Manager.instance.Save(true);
            Sounds.instance.PlayAfterVideoAdReward();
            VibrationManager.instance.VibrateSuccess();
            if(GameScenes.instance.AreWeInGame())
            {
                uiManagerScript.AfterNoAdsPromptClicked();
            }
            AnalyticsManager.instance.LogNoAdsPurchased(noAdsPlace, args.purchasedProduct.metadata.isoCurrencyCode, args.purchasedProduct.metadata.localizedPrice,
                args.purchasedProduct.definition.id, args.purchasedProduct.receipt, args.purchasedProduct.transactionID);
        }
        // Or ... an unknown product has been purchased by this user. Fill in additional products here....
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
            // The subscription item has been successfully purchased, grant this to the player.
        }

        TenjinManager.instance.ProcessPurchaseWithTenjin(args);

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        //Debug.Log("OnPurchaseFailed : 1" + uiManagerScript != null);
        //Debug.Log("OnPurchaseFailed : 2" + uiManagerScript.NoAdsIsClicked);
        //Debug.Log("OnPurchaseFailed : 3" + GameScenes.instance.AreWeInGame());
 
        if (uiManagerScript.NoAdsIsClicked && GameScenes.instance.AreWeInGame())
        {
            uiManagerScript.AfterNoAdsPromptClicked();
        }
    }

    public string GetPrice_Product_NoAds()
    {
        return m_StoreController.products.WithID(product_pipes_no_ads).metadata.localizedPrice.ToString();
    }

    public string GetCurrency_Product_NoAds()
    {
        return m_StoreController.products.WithID(product_pipes_no_ads).metadata.localizedPriceString[0].ToString();
    }
    
    public string GetPrice_Product_Coins(int coinsID)
    {
        if (coinsID == 1)
            return m_StoreController.products.WithID(product_pipes_coins_1).metadata.localizedPrice.ToString();
        else if (coinsID == 2)
            return m_StoreController.products.WithID(product_pipes_coins_2).metadata.localizedPrice.ToString();
        else if (coinsID == 3)
            return m_StoreController.products.WithID(product_pipes_coins_3).metadata.localizedPrice.ToString();
        else if (coinsID == 4)
            return m_StoreController.products.WithID(product_pipes_coins_4).metadata.localizedPrice.ToString();
        else if (coinsID == 5)
            return m_StoreController.products.WithID(product_pipes_coins_5).metadata.localizedPrice.ToString();
        else if (coinsID == 6)
            return m_StoreController.products.WithID(product_pipes_coins_6).metadata.localizedPrice.ToString();
        else
            return "";
    }
    
    public string GetCurrency_Product_Coins(int coinsID)
    {
        if (coinsID == 1)
            return m_StoreController.products.WithID(product_pipes_coins_1).metadata.localizedPriceString[0].ToString();
        else if (coinsID == 2)
            return m_StoreController.products.WithID(product_pipes_coins_2).metadata.localizedPriceString[0].ToString();
        else if (coinsID == 3)
            return m_StoreController.products.WithID(product_pipes_coins_3).metadata.localizedPriceString[0].ToString();
        else if (coinsID == 4)
            return m_StoreController.products.WithID(product_pipes_coins_4).metadata.localizedPriceString[0].ToString();
        else if (coinsID == 5)
            return m_StoreController.products.WithID(product_pipes_coins_5).metadata.localizedPriceString[0].ToString();
        else if (coinsID == 6)
            return m_StoreController.products.WithID(product_pipes_coins_6).metadata.localizedPriceString[0].ToString();
        else
            return "";
    }
    
    public string GetPrice_Product_Bundle(int bundleID)
    {
        if (bundleID == 1)
            return m_StoreController.products.WithID(product_pipes_bundle_1).metadata.localizedPrice.ToString();
        else if (bundleID == 2)
            return m_StoreController.products.WithID(product_pipes_bundle_2).metadata.localizedPrice.ToString();
        else if (bundleID == 3)
            return m_StoreController.products.WithID(product_pipes_bundle_3).metadata.localizedPrice.ToString();
        else if (bundleID == 4)
            return m_StoreController.products.WithID(product_pipes_bundle_4).metadata.localizedPrice.ToString();
        else if (bundleID == 5)
            return m_StoreController.products.WithID(product_pipes_bundle_5).metadata.localizedPrice.ToString();
        else if (bundleID == 6)
            return m_StoreController.products.WithID(product_pipes_bundle_6).metadata.localizedPrice.ToString();
        else
            return "";
    }
    
    public string GetCurrency_Product_Bundle(int bundleID)
    {
        if (bundleID == 1)
            return m_StoreController.products.WithID(product_pipes_bundle_1).metadata.localizedPriceString[0].ToString();
        else if (bundleID == 2)
            return m_StoreController.products.WithID(product_pipes_bundle_2).metadata.localizedPriceString[0].ToString();
        else if (bundleID == 3)
            return m_StoreController.products.WithID(product_pipes_bundle_3).metadata.localizedPriceString[0].ToString();
        else if (bundleID == 4)
            return m_StoreController.products.WithID(product_pipes_bundle_4).metadata.localizedPriceString[0].ToString();
        else if (bundleID == 5)
            return m_StoreController.products.WithID(product_pipes_bundle_5).metadata.localizedPriceString[0].ToString();
        else if (bundleID == 6)
            return m_StoreController.products.WithID(product_pipes_bundle_6).metadata.localizedPriceString[0].ToString();
        else
            return "";
    }
}
