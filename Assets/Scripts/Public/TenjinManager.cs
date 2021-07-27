using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
using UnityEngine.Purchasing;

public class TenjinManager : MonoBehaviour
{
    public static TenjinManager instance;

    private string api_key = "VQTMHS6LGXK6YWNAPLPVZD7LWCGQBNYY";

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

    public void TenjinConnect()
    {
        BaseTenjin instance = Tenjin.getInstance(api_key);

        bool userOptIn = Manager.instance.userConsent;

        if (userOptIn)
        {
            instance.OptIn();
        }
        else
        {
            instance.OptOut();
        }
#if UNITY_IOS && !UNITY_EDITOR
        print("ios version: "+ Device.systemVersion.Split('.')[0]);
        if (int.Parse(Device.systemVersion.Split('.')[0]) >= 14)
        {
            // Tenjin wrapper for requestTrackingAuthorization
            instance.RequestTrackingAuthorizationWithCompletionHandler((status) =>
            {
                Debug.Log("===> App Tracking Transparency Authorization Status: " + status);

                // Sends install/open event to Tenjin
                instance.Connect();

            });
        }

        else {
            instance.Connect();
        }

#elif UNITY_ANDROID || UNITY_EDITOR

        // Sends install/open event to Tenjin
        instance.Connect();

#endif
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            //do nothing
        }
        else
        {
            TenjinConnect();
        }
    }

    public void ProcessPurchaseWithTenjin(PurchaseEventArgs purchaseEventArgs)
    {
        var price = purchaseEventArgs.purchasedProduct.metadata.localizedPrice;
        double lPrice = decimal.ToDouble(price);
        var currencyCode = purchaseEventArgs.purchasedProduct.metadata.isoCurrencyCode;

        var wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(purchaseEventArgs.purchasedProduct.receipt);
        if (null == wrapper)
        {
            return;
        }
        foreach (KeyValuePair<string, object> KVP in wrapper)
        {
            Debug.Log(KVP.Key + "------" + KVP.Value);
        }

        var payload = (string)wrapper["Payload"]; // For Apple this will be the base64 encoded ASN.1 receipt
        var productId = purchaseEventArgs.purchasedProduct.definition.id;

#if UNITY_ANDROID

        var gpDetails = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
        var gpJson = (string)gpDetails["json"];
        var gpSig = (string)gpDetails["signature"];

        CompletedAndroidPurchase(productId, currencyCode, 1, lPrice, gpJson, gpSig);

#elif UNITY_IOS

            var transactionId = purchaseEventArgs.purchasedProduct.transactionID;

            CompletedIosPurchase(productId, currencyCode, 1, lPrice, transactionId, payload);

#endif
    }

    private void CompletedAndroidPurchase(string ProductId, string CurrencyCode, int Quantity, double UnitPrice, string Receipt, string Signature)
    {
        BaseTenjin instance = Tenjin.getInstance(api_key);
        instance.Transaction(ProductId, CurrencyCode, Quantity, UnitPrice, null, Receipt, Signature);
    }

    private void CompletedIosPurchase(string ProductId, string CurrencyCode, int Quantity, double UnitPrice, string TransactionId, string Receipt)
    {
        BaseTenjin instance = Tenjin.getInstance(api_key);
        instance.Transaction(ProductId, CurrencyCode, Quantity, UnitPrice, TransactionId, Receipt, null);
    }
}
