using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapters : MonoBehaviour
{
    public static Chapters instance;

    public const int NumberOfChapters = 26;
    public const int NumberOfChestsPerChapter = 3;
    private float[] chestIndexesInChapters = { 0.3f, 0.6f, 1f };

    private int[] eachChapterLevelCount = { 20, 20, 20, 20,
                                            25, 25,
                                            30, 30,
                                            35, 35,
                                            40, 40,
                                            45, 45,
                                            50, 50, 50, 50, 50, 50,
                                            50, 50, 50, 50, 50, 50 }; // From now on it'll be always 50 and can't be more than 80 and can't add more than 255 chapters!

    private string[] eachChapterNameKey = { "USEAST", "CANADA", "MEXICO", "BRAZIL",
                                            "UK", "GERMANY",
                                            "FRANCE", "SPAIN",
                                            "ITALY", "GREECE",
                                            "TURKEY", "EGYPT",
                                            "SAUDI", "KENYA",
                                            "IRAN", "INDIA", "THAILAND", "SINGAPORE", "AUSTRALIA", "NEWZEALAND",
                                            "TAIWAN", "CHINA", "KOREA", "JAPAN", "RUSSIA", "USWEST" }; // We need to define chapter names after 26

    private int[] eachChapterStartingShownIndex = new int[NumberOfChapters];
    private int[] eachChapterLastShownIndex = new int[NumberOfChapters];

    public List<ChapterData> ChaptersData = new List<ChapterData>(NumberOfChestsPerChapter);

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            InitChaptersData();
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }

    private void InitChaptersData()
    {
        int lastChapterStartingIndex = 1;
        eachChapterStartingShownIndex[0] = 1;
        for (int i = 1; i < NumberOfChapters; i++)
        {
            eachChapterStartingShownIndex[i] = lastChapterStartingIndex + GetChapterCapacity(i - 1);
            lastChapterStartingIndex = eachChapterStartingShownIndex[i];
            eachChapterLastShownIndex[i - 1] = eachChapterStartingShownIndex[i] - 1;
            //Debug.Log(eachChapterStartingShownIndex[i] + "YYYY" + eachChapterLastShownIndex[i - 1]);
        }
        eachChapterLastShownIndex[NumberOfChapters - 1] = eachChapterLastShownIndex[NumberOfChapters - 2] + GetChapterCapacity(NumberOfChapters - 1);
    }

    public int GetChapterCapacity(int chapterIndex)
    {
        return eachChapterLevelCount[chapterIndex];
    }

    public string GetChapterName(int chapterIndex)
    {
        return LanguageManager.instance.GetTheTextByKey(eachChapterNameKey[chapterIndex]);
    }

    public string GetChapterKey(int chapterIndex)
    {
        return eachChapterNameKey[chapterIndex];
    }

    public Sprite GetChapterThingSprite(int chapterIndex, int thingIndex)
    {
        if (thingIndex == 0)
            return ChaptersData[chapterIndex].Tree;
        else if (thingIndex == 1)
            return ChaptersData[chapterIndex].Flower;
        else
            return ChaptersData[chapterIndex].Fruit;
    }

    public void FillChapterChestIndexes(int chapterIndex, int[] chestIndexes)
    {
        int chapterCapacity = GetChapterCapacity(chapterIndex);
        for (int i = 0; i < 3; i++)
        {
            chestIndexes[i] = eachChapterStartingShownIndex[chapterIndex] + (int)(chestIndexesInChapters[i] * (float)chapterCapacity);
        }
    }

    public int GetChapterIndexOfThisLevel(int levelNum)
    {
        //Debug.Log(Manager.instance.maxSeenLevelNum + "hhh");
        for (int i = 0; i < NumberOfChapters; i++)
        {
            //Debug.Log((i + 1).ToString() + "hhh" + eachChapterStartingShownIndex[i]);
            if (levelNum < eachChapterStartingShownIndex[i])
                return i -1;
        }
        return NumberOfChapters - 1;
    }

    public int GetCurrentChapterIndex()
    {
        int levelNum = Manager.instance.maxSeenLevelNum;
        Debug.Log(Manager.instance.maxSeenLevelNum + "hhh");
        for (int i = 0; i < NumberOfChapters; i++)
        {
            Debug.Log((i + 1).ToString() + "hhh" + eachChapterStartingShownIndex[i]);
            if (levelNum < eachChapterStartingShownIndex[i])
            {
                Debug.Log((i).ToString() + "hhh" + eachChapterStartingShownIndex[i -1]);
                if (i > 1 && levelNum == eachChapterStartingShownIndex[i - 1])
                {
                    //check previous chapters' last chest.
                    short chestKey = (short)((i - 2) * 10 + 2);
                    Debug.Log((chestKey).ToString() + "hhh" + Manager.instance.chestCards);
                    foreach (KeyValuePair<short, bool> kvp in Manager.instance.chestCards)
                        Debug.Log("hhh Key = {" + kvp.Key + "},{ Value = {" + kvp.Value + "}");
                    if (Manager.instance.chestCards.ContainsKey(chestKey) && Manager.instance.chestCards[chestKey])
                    {
                        return i - 1;
                    }
                    else
                    {
                        return i - 2;
                    }
                }
                else
                {
                    return i - 1;
                }
            }
        }
        return NumberOfChapters - 1;
    }

    public bool IsItLastLevelOfChapter(int levelIndex, int chapterIndex)
    {
        return levelIndex == eachChapterLastShownIndex[chapterIndex];
    }

    public int GetStartingShownLevelIndexOfAChapter(int chapterIndex)
    {
        return eachChapterStartingShownIndex[chapterIndex];
    }

    public int GetLastShownLevelIndexOfAChapter(int chapterIndex)
    {
        return eachChapterLastShownIndex[chapterIndex];
    }

    public int GetMaximumNumberOfStarsInAChapter(int chapterIndex)
    {   // the maximum number of stars the player can possibly earn in the levels of this chapter
        return GetChapterCapacity(chapterIndex) * 3;
    }

    public int GetNumberOfStarsInAChapter(int chapterIndex)
    {   
        return Manager.instance.chapterStars[(byte)chapterIndex];
    }

    public bool IsChapterPerfectlyCompleted(int chapterIndex)
    {   
        if (GetNumberOfStarsInAChapter(chapterIndex) == GetMaximumNumberOfStarsInAChapter(chapterIndex)) {
            return true;
        }
        return false;
    }

    public bool IsChapterLocked(int chapterIndex)
    {
        if (chapterIndex == 0)
        {
            return false;
        }
        else if (Manager.instance.maxSeenLevelNum > eachChapterStartingShownIndex[chapterIndex])
        {
            return false;
        }
        else if (Manager.instance.maxSeenLevelNum == eachChapterStartingShownIndex[chapterIndex] &&
            Manager.instance.chestCards.ContainsKey((short)((chapterIndex - 1) * 10 + 2)) &&
            Manager.instance.chestCards[(short)((chapterIndex - 1) * 10 + 2)])
        {
            return false;
        }
        return true;
    }

    [System.Serializable]
    public class ChapterData
    {
        public string ChapterNameKey;
        public Sprite Tree;
        public Sprite Flower;
        public Sprite Fruit;
    }
}
