using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterSelector : MonoBehaviour
{
    public RectTransform mainCanvasTransform;
    public float firstChapterLocalPositionY;
    public float snapTime;
    public ScrollRect theScrollRect;
    public RectTransform contentRect;
    public GameObject bg1;
    public GameObject bg2;
    public GameObject bottomBar;
    public GameObject Coins;
    public GameObject Hearts;
    public GameObject BackToMenu;
    public GameObject HomeContentFather;
    public GameObject ChaptersContentFather;
    public GameObject theChapterPrefab;
    public GameObject NextChapter;
    public GameObject PreviousChapter;
    public Text ChapterSelectText;
    public Menu MenuScript;
    public Sprite[] chaptersSpecificImages = new Sprite[Chapters.NumberOfChapters];

    private int currentChapterIndex = 0;
    private int snappedChapterIndex = 0;
    private bool isSelectorInit = false;
    private float dragStartX;
    private Coroutine snapCo;
    private RectTransform[] theChaptersRects = new RectTransform[Chapters.NumberOfChapters];
    private Vector2 firstChapterLocalPosition;
    private float eachChapterPositionDiffX;
    private float snapDiffX;

    private static readonly string textkey_Enter = "Enter";
    private static readonly string textkey_SelectChapter = "SelectChapter";
    private static readonly string textkey_LevelsXY = "LevelsXY";

    // Start is called before the first frame update
    void Start()
    {

    }

    public void DragStarted()
    {
        //Debug.Log("Drag1:" + Mathf.Abs(contentRect.anchoredPosition.x - firstChapterLocalPosition.x).ToString());
        dragStartX = Mathf.Abs(contentRect.anchoredPosition.x - firstChapterLocalPosition.x);
    }

    public void DragFinished()
    {
        //Debug.Log("Drag2:" + Mathf.Abs(contentRect.anchoredPosition.x - firstChapterLocalPosition.x).ToString());
        float dragFinishX = Mathf.Abs(contentRect.anchoredPosition.x - firstChapterLocalPosition.x);
        bool movedToRight = (dragStartX - dragFinishX > 0);
        if (snappedChapterIndex == 0 && movedToRight)
        {
            SnapToThisChapter(0);
        }
        else if (snappedChapterIndex == Chapters.NumberOfChapters - 1 && !movedToRight)
        {
            SnapToThisChapter(Chapters.NumberOfChapters - 1);
        }
        else
        {
            if (movedToRight)
            {
                if(Mathf.Abs(dragFinishX - theChaptersRects[snappedChapterIndex -1].anchoredPosition.x) <=
                    Mathf.Abs(dragFinishX - theChaptersRects[snappedChapterIndex].anchoredPosition.x))
                {
                    SnapToThisChapter(snappedChapterIndex - 1);
                }
                else
                {
                    SnapToThisChapter(snappedChapterIndex);
                }
            }
            else
            {
                if (Mathf.Abs(dragFinishX - theChaptersRects[snappedChapterIndex + 1].anchoredPosition.x) <=
                    Mathf.Abs(dragFinishX - theChaptersRects[snappedChapterIndex].anchoredPosition.x))
                {
                    SnapToThisChapter(snappedChapterIndex + 1);
                }
                else
                {
                    SnapToThisChapter(snappedChapterIndex);
                }
            }
        }
    }

    public void InitChapters()
    {
        HomeContentFather.SetActive(false);
        bottomBar.SetActive(false);
        bg1.SetActive(false);
        bg2.SetActive(false);
        Coins.SetActive(false);
        Hearts.SetActive(false);
        BackToMenu.SetActive(true);
        if (!isSelectorInit)
        {
            isSelectorInit = true;
            firstChapterLocalPosition = new Vector2(mainCanvasTransform.sizeDelta.x / 2, firstChapterLocalPositionY);
            eachChapterPositionDiffX = mainCanvasTransform.sizeDelta.x;
            snapDiffX = firstChapterLocalPosition.x;
            ChapterSelectText.text = LanguageManager.instance.GetTheTextByKey(textkey_SelectChapter);
            currentChapterIndex = Chapters.instance.GetCurrentChapterIndex();
            float widthOfContent = Chapters.NumberOfChapters * Mathf.Abs(eachChapterPositionDiffX);
            Vector2 thisChapterPosition =
                new Vector2(firstChapterLocalPosition.x, firstChapterLocalPosition.y);
            contentRect.sizeDelta = new Vector2(widthOfContent, contentRect.sizeDelta.y);
            for (int i = 0; i < Chapters.NumberOfChapters; i++)
            {
                GameObject thisChapter = Instantiate(theChapterPrefab, ChaptersContentFather.transform, false);
                theChaptersRects[i] = thisChapter.GetComponent<RectTransform>();
                thisChapter.GetComponent<RectTransform>().anchoredPosition = thisChapterPosition;
                thisChapter.transform.GetChild(0).GetComponent<Text>().text = Chapters.instance.GetChapterName(i);
                GameObject imageBackChild = thisChapter.transform.GetChild(1).gameObject;
                GameObject levelsChild = thisChapter.transform.GetChild(2).gameObject;
                GameObject progressChild = thisChapter.transform.GetChild(3).gameObject;
                GameObject enterChild = thisChapter.transform.GetChild(4).gameObject;
                Text levelsText = levelsChild.GetComponent<Text>();
                levelsText.text = LanguageManager.instance.GetTheTextByKey(textkey_LevelsXY);
                int minIndex = Chapters.instance.GetStartingShownLevelIndexOfAChapter(i);
                int maxIndex = Chapters.instance.GetLastShownLevelIndexOfAChapter(i);
                if (LanguageManager.instance.isLanguageRTL)
                {
                    levelsText.text = levelsText.text.Replace("۱", LanguageManager.arabicFix(minIndex.ToString()));
                    levelsText.text = levelsText.text.Replace("۲۰", LanguageManager.arabicFix(maxIndex.ToString()));
                }
                else
                {
                    levelsText.text = levelsText.text.Replace("1", minIndex.ToString());
                    levelsText.text = levelsText.text.Replace("20", maxIndex.ToString());
                }
                if(currentChapterIndex == i)
                {
                    SnapToThisChapter(i);
                }
                if (Chapters.instance.IsChapterLocked(i))
                {
                    Vector2 levelsPos = levelsChild.transform.position;
                    levelsPos.y = progressChild.transform.position.y;
                    levelsChild.transform.position = levelsPos;
                }
                else
                {
                    int j = i;
                    enterChild.GetComponent<Button>().onClick.AddListener(() => HideChapterSelector(j));
                    enterChild.GetComponentInChildren<Text>().text = LanguageManager.instance.GetTheTextByKey(textkey_Enter);
                    enterChild.SetActive(true);
                    imageBackChild.transform.GetChild(0).GetComponent<Image>().sprite = chaptersSpecificImages[i];
                    if (Chapters.instance.IsChapterPerfectlyCompleted(i))
                    {
                        imageBackChild.transform.GetChild(1).gameObject.SetActive(true);
                        imageBackChild.transform.GetChild(2).gameObject.SetActive(true);
                        imageBackChild.transform.GetChild(3).gameObject.SetActive(true);
                        RectTransform progressBarTransform = progressChild.GetComponent<RectTransform>();
                        RectTransform levelsTransform = levelsChild.GetComponent<RectTransform>();
                        levelsTransform.localPosition = new Vector2(levelsTransform.localPosition.x, progressBarTransform.localPosition.y);
                    }
                    else
                    {
                        RectTransform progressBarTransform = progressChild.transform.GetChild(1).GetComponent<RectTransform>();
                        Text progressText = progressChild.transform.GetChild(2).GetComponent<Text>();
                        int currentStars = Chapters.instance.GetNumberOfStarsInAChapter(i);
                        int maximumStars = Chapters.instance.GetMaximumNumberOfStarsInAChapter(i);
                        progressBarTransform.localScale = new Vector3(progressBarTransform.localScale.x +
                            currentStars * (1 - progressBarTransform.localScale.x) / maximumStars,
                            progressBarTransform.localScale.y, 1);
                        if (LanguageManager.instance.isLanguageRTL)
                        {
                            progressText.text = progressText.text.Replace("3", LanguageManager.arabicFix(currentStars.ToString()));
                            progressText.text = progressText.text.Replace("20", LanguageManager.arabicFix(maximumStars.ToString()));
                        }
                        else
                        {
                            progressText.text = progressText.text.Replace("3", currentStars.ToString());
                            progressText.text = progressText.text.Replace("20", maximumStars.ToString());
                        }
                        progressChild.SetActive(true);
                    }
                }
                thisChapterPosition.x += eachChapterPositionDiffX;
            }
        }
    }

    public void HideChapterSelector(int chapterIndex)
    {
        HomeContentFather.SetActive(true);
        bottomBar.SetActive(true);
        bg1.SetActive(true);
        bg2.SetActive(true);
        Coins.SetActive(true);
        Hearts.SetActive(true);
        BackToMenu.SetActive(false);
        gameObject.SetActive(false);
        if (chapterIndex != -1)
            MenuScript.ShowPage_Home(chapterIndex);
    }

    public void SnapToNextChapter()
    {
        SnapToThisChapter(snappedChapterIndex + 1);
    }

    public void SnapToPreviousChapter()
    {
        SnapToThisChapter(snappedChapterIndex - 1);
    }

    public void SnapToThisChapter(int i)
    {
        if (snapCo != null)
            StopCoroutine(snapCo);
        snapCo = StartCoroutine(SnapToThisChapterIE(i));
    }
    private IEnumerator SnapToThisChapterIE(int i)
    {
        if (i == Chapters.NumberOfChapters - 1)
        {
            NextChapter.SetActive(false);
            PreviousChapter.SetActive(true);
        }
        else if (i == 0)
        {
            NextChapter.SetActive(true);
            PreviousChapter.SetActive(false);
        }
        else
        {
            NextChapter.SetActive(true);
            PreviousChapter.SetActive(true);
        }

        RectTransform snapTarget = theChaptersRects[i];
        snappedChapterIndex = i;

        theScrollRect.StopMovement();

        Canvas.ForceUpdateCanvases();

        float diffX = theScrollRect.transform.InverseTransformPoint(contentRect.position).x -
            theScrollRect.transform.InverseTransformPoint(snapTarget.position).x;
        Vector2 startPos = contentRect.anchoredPosition;
        Vector2 snappedPos = new Vector2(diffX + snapDiffX, contentRect.anchoredPosition.y);

        float currentTime = 0f;
        do
        {
            contentRect.anchoredPosition = Vector2.Lerp(startPos, snappedPos, currentTime / snapTime);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= snapTime);
        contentRect.anchoredPosition = snappedPos;
    }
}
