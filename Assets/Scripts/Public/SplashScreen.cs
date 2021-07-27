using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{
    public float underLineHeight;

    public GameObject PipesLogo;
    public GameObject infinityLogo;
    public GameObject hezartooLogo;

    public GameObject[] Lozis = new GameObject[3];

    public GameObject consentObj;
    public GameObject consentAcceptButton;

    public GameObject afterYes;
    public GameObject afterNo;

    public Text consentTitle;
    public Text consentText;
    public Text consentTos;
    public Text consentPP;
    public Text consentReject;
    public Text consentTitleYes;
    public Text consentTextYes;
    public GameObject consentCloseYes;
    public Text consentTitleNo;
    public Text consentTextNo;
    public GameObject consentCloseNo;

    public static readonly string textkey_Title = "ConsentTitle";
    public static readonly string textkey_Text = "ConsentMainText";
    public static readonly string textkey_Tos = "ConsentToS";
    public static readonly string textkey_PP = "ConsentPP";
    public static readonly string textkey_TitleYes = "ConsentTitleYes";
    public static readonly string textkey_TextYes = "ConsentTextYes";
    public static readonly string textkey_TitleNo = "ConsentTitleNo";
    public static readonly string textkey_TextNo = "ConsentTextNo";
    public static readonly string textkey_Close = "Close";
    public static readonly string textkey_Reject = "Reject";
    public static readonly string textkey_Accept = "Accept";
    public static readonly string textkey_Continue = "Continue";

    private readonly string ToS_URL = "https://www.infinitygames.io/tos/";
    private readonly string PP_URL = "https://www.infinitygames.io/privacy-policy/";
    private float ConsentStartTime;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartLoadingAct(3f, 8f, 1.3f, 0.2f, 0.25f));
    }

    private IEnumerator StartLoadingAct(float minimumLoadingTime, float maximumLoadingTimePermitted, float multiplier, float beatTime, float interval)
    {
        StartCoroutine(Transitions.fadeIn(PipesLogo, 1f, 0.4f));
        StartCoroutine(Transitions.fadeIn(infinityLogo, 1f, 0.4f));
        //StartCoroutine(Transitions.fadeIn(hezartooLogo, 1f, 0.4f));
        StartCoroutine(LoadingAct(interval, beatTime));
        while (Time.timeSinceLevelLoad < maximumLoadingTimePermitted)
        {
            yield return new WaitForSeconds(interval);
            if (Time.timeSinceLevelLoad > minimumLoadingTime && IsEveryThingLoaded())
            {
                break;
            }
        }

        //ConfigManager.instance.ActivateFetchedDataIfAny();

        AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.splash_loading_time, "", (int)Time.timeSinceLevelLoad);

        AnalyticsManager.instance.LogUserLanguage(LanguageManager.instance.isLanguageSupported);

        //AnalyticsManager.instance.HandleAndLogRetentionEvents();

        AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.game_opened_time, ":" + DateTime.Now.ToString("HH"));

        AnalyticsManager.instance.LogNumberOfSessions();

        if (!Manager.instance.userConsent_IsGiven)
        {
            infinityLogo.SetActive(false);
            hezartooLogo.SetActive(false);
            PipesLogo.SetActive(false);
            foreach (GameObject lozi in Lozis)
            {
                lozi.SetActive(false);
            }
            ConsentStartTime = Time.time;

            consentTitle.text = LanguageManager.instance.GetTheTextByKey(textkey_Title);
            consentText.text = LanguageManager.instance.GetTheTextByKey(textkey_Text);
            consentTos.text = LanguageManager.instance.GetTheTextByKey(textkey_Tos);
            consentPP.text = LanguageManager.instance.GetTheTextByKey(textkey_PP);
            consentReject.text = LanguageManager.instance.GetTheTextByKey(textkey_Reject);
            consentAcceptButton.GetComponentInChildren<Text>().text = LanguageManager.instance.GetTheTextByKey(textkey_Accept);

            consentTitleYes.text = LanguageManager.instance.GetTheTextByKey(textkey_TitleYes);
            consentTextYes.text = LanguageManager.instance.GetTheTextByKey(textkey_TextYes);
            consentCloseYes.GetComponentInChildren<Text>().text = LanguageManager.instance.GetTheTextByKey(textkey_Continue);

            consentTitleNo.text = LanguageManager.instance.GetTheTextByKey(textkey_TitleNo);
            consentTextNo.text = LanguageManager.instance.GetTheTextByKey(textkey_TextNo);
            consentCloseNo.GetComponentInChildren<Text>().text = LanguageManager.instance.GetTheTextByKey(textkey_Continue);

            if (LanguageManager.instance.isLanguageRTL)
            {
                LanguageManager.makeRTLText2lined(consentTitle.gameObject);
                LanguageManager.makeRTLText2lined(consentText.gameObject);
                LanguageManager.makeRTLText2lined(consentTextYes.gameObject);
                LanguageManager.makeRTLText2lined(consentTitleYes.gameObject);
                LanguageManager.makeRTLText2lined(consentTextNo.gameObject);
                LanguageManager.makeRTLText2lined(consentTitleNo.gameObject);
            }

            SetUnderlineLength(consentTos);
            SetUnderlineLength(consentPP);
            SetUnderlineLength(consentReject);

            consentObj.SetActive(true);
            StartCoroutine(Transitions.scaleUp(consentAcceptButton, new Vector3(.05f, .0128f, 1f), .3f));
            AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.consent_shown);
        }
        else
        {
            InitSDKsWithConsent();
            if (Manager.instance.maxSeenLevelNum == 1)
            {
                LoadFirstLevel(true);
            }
            else
            {
                LoadMenu(true);
            }
        }
    }

    private bool IsEveryThingLoaded()
    {
        //Debug.Log("SPLASH " + CloudManager.instance.Authenticated + !Manager.instance.needToGetDataFromCloud + AnalyticsManager.instance.FirebaseIsInit + LanguageManager.instance.isDictionaryInitialized);
        return ( //!Manager.instance.needToGetDataFromCloud &&
                 //CloudManager.instance.Authenticated &&
                 AnalyticsManager.instance.FirebaseIsInit &&
                 LanguageManager.instance.isDictionaryInitialized);
        // check remote config is init and data is read
        // check data is updated on the local variables
        // put this after other codes in script execution

    }

    private IEnumerator LoadingAct(float interval, float rotateTime)
    {
        int loziIndex = 0;
        int totalLozis = Lozis.Length;
        while (true)
        {
            StartCoroutine(Transitions.rotateZ(Lozis[loziIndex], 180, true, rotateTime));
            if (loziIndex < totalLozis - 1)
            {
                loziIndex++;
            }
            else
            {
                loziIndex = 0;
            }
            yield return new WaitForSeconds(interval);
        }
    }

    private void SetUnderlineLength(Text fatherText)
    {
        GameObject father = fatherText.gameObject;
        GameObject child = father.transform.GetChild(0).gameObject;
        RectTransform childRect = child.GetComponent<RectTransform>();
        childRect.sizeDelta = new Vector2(father.GetComponent<Text>().preferredWidth, underLineHeight);
        Debug.Log(father.GetComponent<Text>().preferredWidth);
    }

    public void setConsent(bool userConsent)
    {
        Manager.instance.userConsent_IsGiven = true;
        Manager.instance.userConsent = userConsent;
        Manager.instance.Save(false);
        Sounds.instance.PlayUIInteraction();
        VibrationManager.instance.VibrateTouch();
        InitSDKsWithConsent();
        consentObj.SetActive(false);
        if (userConsent)
        {
            afterYes.SetActive(true);
            StartCoroutine(Transitions.scaleUp(consentCloseYes, new Vector3(.05f, .0128f, 1f), .3f));

            AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.consent_accepted, "", (float)(Time.time - ConsentStartTime));
        }
        else
        {
            afterNo.SetActive(true);
            StartCoroutine(Transitions.scaleUp(consentCloseNo, new Vector3(.05f, .0128f, 1f), .3f));

            AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.consent_rejected, "", (float)(Time.time - ConsentStartTime));
        }
    }

    public void OpenToS_URL()
    {
        AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.consent_link_opened + ":ToS");
        Sounds.instance.PlayUIInteraction();
        VibrationManager.instance.VibrateTouch();
        Application.OpenURL(ToS_URL);
    }
    public void OpenPP_URL()
    {
        AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.consent_link_opened + ":PP");
        Sounds.instance.PlayUIInteraction();
        VibrationManager.instance.VibrateTouch();
        Application.OpenURL(PP_URL);
    }

    public void LoadMenu(bool mute)
    {
        if (!mute)
        {
            Sounds.instance.PlayUIInteraction();
            VibrationManager.instance.VibrateTouch();
        }
        AchievementManager.CheckAchievements_PlayedForXdays();
        GameScenes.instance.LoadMenu();
    }

    public void LoadFirstLevel(bool mute)
    {
        if (!mute)
        {
            Sounds.instance.PlayUIInteraction();
            VibrationManager.instance.VibrateTouch();
        }
        AchievementManager.CheckAchievements_PlayedForXdays();
        GameScenes.instance.LoadSpecificLevel(1);
    }
    
    private void InitSDKsWithConsent()
    {
        AdManager.instance.ApplovinInit(Manager.instance.userConsent);
        TenjinManager.instance.TenjinConnect();
        NotifManager.instance.ScheduleLocalNotifs();
    }
}
