using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using ArabicSupport;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager instance;

#if UNITY_EDITOR
    public string languageTester;
#endif

    public TextAsset EnglishDictionaryCsv;
    public TextAsset ArabicDictionaryCsv;
    public TextAsset FarsiDictionaryCsv;
    public TextAsset ChineseDictionaryCsv;
    public TextAsset DanishDictionaryCsv;
    public TextAsset DutchDictionaryCsv;
    public TextAsset FinnishDictionaryCsv;
    public TextAsset FrenchDictionaryCsv;
    public TextAsset GermanDictionaryCsv;
    public TextAsset HindiDictionaryCsv;
    public TextAsset IndonesianDictionaryCsv;
    public TextAsset ItalianDictionaryCsv;
    public TextAsset JapaneseDictionaryCsv;
    public TextAsset KoreanDictionaryCsv;
    public TextAsset PortugueseDictionaryCsv;
    public TextAsset PortugueseBRDictionaryCsv;
    public TextAsset RussianeDictionaryCsv;
    public TextAsset SpanishDictionaryCsv;
    public TextAsset TurkishDictionaryCsv;
    public TextAsset ThaiDictionaryCsv;
    public TextAsset VietnameseDictionaryCsv;

    public bool isLanguageRTL { get; private set; } = false;
    public bool isLanguageTokhmi { get; private set; } = false;
    public bool isLanguageSupported { get; private set; } = true;

    private Dictionary<string, string> theTexts = new Dictionary<string, string>();
    private string englishDictionaryFileName = "en_texts.csv";

    private char lineSeperater = '\n'; // It defines line seperate character
    private char fieldSeperator = ','; // It defines field seperate chracter


    public bool isDictionaryInitialized { get; private set; } = false;

    public string userLanguage { get; private set; } = "en";
    public string userRegion { get; private set; }

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

    public void fillTheTranslatedTextsDictionaryFromFile()
    {
        isDictionaryInitialized = false;
        string[] splitLanguageID = PreciseLocale.GetLanguageID().Split('_');
        userLanguage = splitLanguageID[0];
        userRegion = splitLanguageID[1];

#if UNITY_EDITOR
        if(languageTester != "")
            userLanguage = languageTester;
#endif

        Debug.Log("LANGUAGE" + userLanguage);
        Debug.Log("lang1 "+PreciseLocale.GetCurrencyCode());
        Debug.Log("lang2 " + PreciseLocale.GetCurrencySymbol());
        Debug.Log("lang3 " + PreciseLocale.GetLanguage());
        Debug.Log("lang4 " + PreciseLocale.GetLanguageID());
        Debug.Log("lang5 " + PreciseLocale.GetRegion());

        isLanguageRTL = (userLanguage == "ar" || userLanguage == "fa");

        isLanguageTokhmi = (isLanguageRTL || userLanguage == "hi" || userLanguage == "ja" || userLanguage == "ko" || userLanguage == "zh");

        string[] lines;
        if (userLanguage == "en")
        {
            lines = EnglishDictionaryCsv.text.Split(lineSeperater);
        }
        else if (userLanguage == "ar")
        {
            lines = ArabicDictionaryCsv.text.Split(lineSeperater);
        }
        else if (userLanguage == "fa")
        {
            lines = FarsiDictionaryCsv.text.Split(lineSeperater);
        }
        else if (userLanguage == "da")
        {
            lines = DanishDictionaryCsv.text.Split(lineSeperater);
        }
        else if (userLanguage == "de")
        {
            lines = GermanDictionaryCsv.text.Split(lineSeperater);
        }
        else if (userLanguage == "nl")
        {
            lines = DutchDictionaryCsv.text.Split(lineSeperater);
        }
        else if (userLanguage == "es")
        {
            lines = SpanishDictionaryCsv.text.Split(lineSeperater);
        }
        else if (userLanguage == "fi")
        {
            lines = FinnishDictionaryCsv.text.Split(lineSeperater);
        }
        else if (userLanguage == "fr")
        {
            lines = FrenchDictionaryCsv.text.Split(lineSeperater);
        }
        else if (userLanguage == "hi")
        {
            lines = HindiDictionaryCsv.text.Split(lineSeperater);
        }
        else if (userLanguage == "id")
        {
            lines = IndonesianDictionaryCsv.text.Split(lineSeperater);
        }
        else if (userLanguage == "it")
        {
            lines = ItalianDictionaryCsv.text.Split(lineSeperater);
        }
        else if (userLanguage == "ja")
        {
            lines = JapaneseDictionaryCsv.text.Split(lineSeperater);
        }
        else if (userLanguage == "ko")
        {
            lines = KoreanDictionaryCsv.text.Split(lineSeperater);
        }
        else if (userLanguage == "pt")
        {
            if (userRegion == "BR") {
                lines = PortugueseBRDictionaryCsv.text.Split(lineSeperater);
            }
            else {
                lines = PortugueseDictionaryCsv.text.Split(lineSeperater);
            }
        }
        else if (userLanguage == "ru")
        {
            lines = RussianeDictionaryCsv.text.Split(lineSeperater);
        }
        else if (userLanguage == "th")
        {
            lines = ThaiDictionaryCsv.text.Split(lineSeperater);
        }
        else if (userLanguage == "tr")
        {
            lines = TurkishDictionaryCsv.text.Split(lineSeperater);
        }
        else if (userLanguage == "vi")
        {
            lines = VietnameseDictionaryCsv.text.Split(lineSeperater);
        }
        else if (userLanguage == "zh")
        {
            lines = ChineseDictionaryCsv.text.Split(lineSeperater);
        }
        else
        {
            lines = EnglishDictionaryCsv.text.Split(lineSeperater);
            isLanguageSupported = false;//We Don't support the language but we set default to English
        }
        foreach (string line in lines)
        {
            if(line == "")
            {
                continue;
            }
            char[] separators = { fieldSeperator };
            string[] fields = line.Split(separators, 2, System.StringSplitOptions.None);

            fields[1] = fields[1].Replace("\"",string.Empty);

            if (fields.Length != 0)
            {
                handleKeyValueInEnglishDictionary(fields[0], fields[1]);
            }
        }

        isDictionaryInitialized = true;
    }

    //We want the description object to have the default english text if a key didnt exist!
    public void LocalizeTheTextByKey(Text textObject, string key, bool arabicFix)
    {
        if (isDictionaryInitialized && theTexts.ContainsKey(key))
        {
            if (arabicFix && isLanguageRTL)
            {
                textObject.text = ArabicFixer.Fix(theTexts[key]);
            }
            else
                textObject.text = theTexts[key];
            //Debug.Log("LOCALIZED Text: " + theTexts[key]);
        }
    }

    public string GetTheTextByKey(string key, bool debug = false)
    {
        if (isDictionaryInitialized && theTexts.ContainsKey(key))
        {
            if (debug)
            {
                Debug.Log("k='" + key + "'v='" + theTexts[key] + "'");
            }
            if (isLanguageRTL)
            {
                return ArabicFixer.Fix(theTexts[key]);
            }
            else
            {
                return theTexts[key];
            }
        }
        else
        {
            Debug.LogError("Key is :'" + key + "'!");
            return "";
        }
    }

    public string GetDateText()
    {
        //Monday June 7

        DateTime now = DateTime.Now;
        string weekday = GetTheTextByKey(now.DayOfWeek.ToString());
        string monthName = GetTheTextByKey(Enum.GetName(typeof(Menu.Months), (Menu.Months)now.Month));
        string day = now.Day.ToString();
        if (userLanguage == "ar" || userLanguage == "fa")
        {
            return monthName + " " + arabicFix(day) + " " + weekday;
        }
        else if (userLanguage == "da" || userLanguage == "fr" || userLanguage == "hi" || userLanguage == "id" || userLanguage == "ru" || userLanguage == "vi")
        {
            return weekday + ", " + day + " " + monthName;
        }
        else if (userLanguage == "de")
        {
            return weekday + ", " + day + ". " + monthName;
        }
        else if (userLanguage == "fi")
        {
            return weekday + " " + day + ". " + monthName + "ta";
        }
        else if (userLanguage == "nl" || userLanguage == "it")
        {
            return weekday + " " + day + " " + monthName;
        }
        else if (userLanguage == "ja")
        {
            return monthName + day + "日" + weekday;
        }
        else if (userLanguage == "ko")
        {
            return monthName + day + "일" + weekday;
        }
        else if (userLanguage == "pt")
        {
            return weekday + ", dia " + day + " de " + monthName;
        }
        else if (userLanguage == "es")
        {
            return weekday + " " + day + " de " + monthName;
        }
        else if (userLanguage == "th")
        {
            return weekday + "ที่ " + day + " " + monthName;
        }
        else if (userLanguage == "tr")
        {
            return day + " " + monthName + " " + weekday;
        }
        else if (userLanguage == "zh")
        {
            return weekday + ", " + monthName + day + "日";
        }
        else
        {
            return weekday + ", " + monthName + " " + day;
        }
    }

    #region RTL Support
    public static string arabicFix(string text)
    {
        return ArabicFixer.Fix(text);
    }

    public static void makeRTLText2lined(GameObject gameObject)
    {
        Text textObject = gameObject.GetComponent<Text>();
        RectTransform rectObject = gameObject.GetComponent<RectTransform>();
        textObject.resizeTextForBestFit = false;
        InsertEnterInTheMiddle(gameObject, false);
        while (textObject.preferredWidth > rectObject.rect.width)
        {
            textObject.fontSize--;
        }
    }

    public static void InsertEnterInTheMiddle(GameObject gameObject, bool arabicFix)
    {
        Text textObject = gameObject.GetComponent<Text>();
        RectTransform rectObject = gameObject.GetComponent<RectTransform>();
        if (textObject.preferredWidth > rectObject.rect.width)
        {
            int middleIndex = textObject.text.Length / 2;
            int spaceIndex = textObject.text.IndexOf(" ", middleIndex);
            string[] lines = new string[2];
            lines[0] = textObject.text.Substring(0, spaceIndex);
            lines[1] = textObject.text.Substring(spaceIndex + 1);
            if (arabicFix)
                textObject.text = lines[0] + "\n" + lines[1];
            else
                textObject.text = lines[1] + "\n" + lines[0];
        }
        if (arabicFix)
        {
            textObject.text = ArabicFixer.Fix(textObject.text);
        }
    }
    #endregion

    #region fillTheEnglishDictionaryFromLevels
    public void handleKeyValueInEnglishDictionary(string key, string value)
    {
        if (value.Contains("" + lineSeperater + fieldSeperator))
        {
            Debug.LogError(string.Format("Don't use '{0}' and '{1}' in the texts please!", lineSeperater, fieldSeperator));
        }
        if (theTexts.ContainsKey(key))
        {
            theTexts[key] = value;
        }
        else
        {
            theTexts.Add(key, value);
        }
    }

    public void writeEnglishDictionary()
    {
        if (theTexts != null)
        {
            foreach(KeyValuePair<string,string> keyval in theTexts)
            {
                Debug.Log(keyval.Key + "," + keyval.Value);
            }
            // CreateDirectory() checks for existence and 
            // automagically creates the directory if necessary
            string savedDataPath = getPath() + "/Languages";

            Debug.Log(savedDataPath);

            Directory.CreateDirectory(savedDataPath);

            string dictionaryFullName = savedDataPath + "/" + englishDictionaryFileName;

            string fileContent = "";

            foreach (var item in theTexts)
            {
                fileContent += item.Key + fieldSeperator + item.Value + lineSeperater;
            }


            File.WriteAllText(dictionaryFullName, fileContent);
        }
    }

    // Get path for given CSV file
    private static string getPath()
    {
#if UNITY_EDITOR
        return Application.dataPath;
#elif UNITY_ANDROID
        return Application.persistentDataPath;// +fileName;
#elif UNITY_IOS
        return GetiPhoneDocumentsPath();// +"/"+fileName;
#else
        return Application.dataPath;// +"/"+ fileName;
#endif
    }
    // Get the path in iOS device
    private static string GetiPhoneDocumentsPath()
    {
        string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
        path = path.Substring(0, path.LastIndexOf('/'));
        return path + "/Documents";
    }
    #endregion
}
