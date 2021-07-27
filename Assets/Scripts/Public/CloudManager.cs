using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Xml.Serialization;
using CloudOnce;

public class CloudManager : MonoBehaviour
{
    public static CloudManager instance;

    private static string[] jakon;


    private bool mAuthenticating = false;
    public bool Authenticated
    {
        get { return Social.Active.localUser.authenticated; }
    }

    public GameObject LoadCloadDataOverlayPrefab;

    private GameObject theOverlayObj;
    private static readonly string textkey_loadCloudData = "LoadCloudData";
    public bool dataLoadedInSplash;

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

    // Start is called before the first frame update
    void Start()
    {
        Authenticate();

        InfoData2 a = new InfoData2();
        XmlSerializer jakonXml = new XmlSerializer(a.GetType());
        StringWriter stringWriter = new StringWriter();
        jakonXml.Serialize(stringWriter, a);
        string text = stringWriter.ToString();
        jakon = text.Split(new[] { Environment.NewLine }, 3, StringSplitOptions.None);
        Debug.Log("jakon is: " + jakon[0] + "\n" + jakon[1]);
    }

    public void ShowLoadDataPromptOverlay()
    {
        theOverlayObj = Instantiate(LoadCloadDataOverlayPrefab);
        theOverlayObj.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = LanguageManager.instance.GetTheTextByKey(textkey_loadCloudData);
        theOverlayObj.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(ActuallyLoadDataFromCloud);
        theOverlayObj.transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(TurnOffPromptOverlay);
    }

    public void TurnOffPromptOverlay()
    {
        Destroy(theOverlayObj);
    }

    public void ActuallyLoadDataFromCloud()
    {
        Debug.Log("The String data is:'" + CloudVariables.DatabaseString + "'");
        CloudVariables.DatabaseString = jakon[0] + Environment.NewLine + jakon[1] + Environment.NewLine +
                                    CloudVariables.DatabaseString.Split(new[] { Environment.NewLine }, 2, StringSplitOptions.None)[1];
        Debug.Log("The String data after ja kardan is:'" + CloudVariables.DatabaseString + "'");
        ProcessCloudData(CloudVariables.DatabaseString);
        TurnOffPromptOverlay();
    }

    public void Authenticate()
    {
        if (Authenticated || mAuthenticating)
        {
            Debug.LogWarning("Ignoring repeated call to Authenticate().");
            return;
        }

        Cloud.OnInitializeComplete += CloudOnceInitializeComplete;
        Cloud.OnNewCloudValues += CloudOnceNewCloudValues;
        Cloud.OnSignInFailed += CloudOnceSignInFailed;
        Cloud.OnSignedInChanged += CloudOnceSignedInChanged;
        Cloud.Initialize(true, true, true);
        //Social.localUser.Authenticate(success => Debug.Log("authentication success: "+success));
    }

    public void CloudOnceInitializeComplete()
    {
        Cloud.OnInitializeComplete -= CloudOnceInitializeComplete;

        // Do anything that requires CloudOnce to be initialized,
        // for example disabling your loading screen
    }

    public void CloudOnceSignInFailed()
    {
        Debug.Log("CloudOnceLog SignIn Failed");
    }

    public void CloudOnceSignedInChanged(bool SignedInResult)
    {
        Debug.Log("CloudOnceLog SignIn Changed " + SignedInResult);
#if !UNITY_EDITOR
        if (SignedInResult)
        {
            AnalyticsManager.instance.LogUserLogin();
        }
#endif
        //if(SignedInResult && Manager.instance.needToGetDataFromCloud)
        //{
        //    Debug.Log("Loading from cloud");
        //    Cloud.Storage.Load();
        //    Cloud.Achievements.ShowOverlay();
        //    Debug.Log("Achievements Shown");
        //}
    }

    public void CloudOnceNewCloudValues(string[] keyIDs)
    {
        foreach (string keyID in keyIDs)
        {
            Debug.Log(keyID);
            if (keyID == "DatabaseString" && CloudVariables.DatabaseString != null && Manager.instance.needToGetDataFromCloud)
            {
                if (GameScenes.instance.AreWeInSplash())
                {
                    dataLoadedInSplash = true;
                }
                else
                {
                    ShowLoadDataPromptOverlay();
                }
            }
        }
    }

    void ProcessCloudData(string cloudData)
    {
        if (cloudData == null)
        {
            Debug.Log("No data saved to the cloud yet...");
            return;
        }

#if !UNITY_EDITOR
        Debug.Log("Decoding cloud data from bytes.");
        Manager.instance.LoadFromCloud(cloudData);
#endif
    }

    public void SaveToCloud(string saveString)
    {
        if (saveString != "")
        {
            Debug.Log("save called'" + saveString + "'");
            CloudVariables.DatabaseString = saveString;
            Cloud.Storage.Save();
        }
    }
}
