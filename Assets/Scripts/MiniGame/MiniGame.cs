using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGame : MonoBehaviour
{
    public Menu MenuFather;
    public GameObject MiniGameFather;
    public GameObject backCard;
    public GameObject claimButton;
    public Text gameDescription;

    public Sprite cardCreamBack;
    public Sprite cardWhiteBack;

    public int numOfShapes;
    public int numOfRows;
    public int numOfColumns;
    public float objectScale;
    public float rewardScale;
    public float xOfFirstGrid;
    public float yOfFirstGrid;
    public float xDiffOfNextGrid;
    public float yDiffOfNextGrid;

    public GameObject shapePrefab;
    public GameObject rewardPrefab;

    [HideInInspector]
    public int numberOfUndestroyedShapes;
    [HideInInspector]
    public bool isEnd;
    [HideInInspector]
    public GameObject chestButton;

    private int[] objPosIndexes;
    private GameObject[] shapes;
    private GameObject rewardObj;
    private short chestKey;
    private int chapterIndex;
    private int chestIndex;

    private static readonly string textkey_tapOnAllShapes = "TapOnAllShapes";
    private static readonly string textkey_awesome = "Awesome";
    private static readonly string textkey_claim = "Claim";

    // Start is called before the first frame update
    void Start()
    {

    }

    public void GenerateObjects(int chapterIndex, int chestIndex)
    {
        chestKey = (short)(chapterIndex * 10 + chestIndex);
        this.chapterIndex = chapterIndex;
        this.chestIndex = chestIndex;
        //StartCoroutine(GenerateObjectsIE());
        //AnalyticsManager.instance.LogChestGameStarted(chapterIndex,chestIndex);
        RevealCard();
    }

    private IEnumerator GenerateObjectsIE()
    {
        isEnd = false;
        gameDescription.text = LanguageManager.instance.GetTheTextByKey(textkey_tapOnAllShapes);
        objPosIndexes = new int[numOfColumns * numOfRows];
        numberOfUndestroyedShapes = numOfShapes;
        shapes = new GameObject[numOfShapes];
        backCard.GetComponent<Image>().sprite = cardCreamBack;
        Essentials.InitAndShuffleArray(objPosIndexes);

        Transform shapeT;
        SpriteRenderer shapeSR;
        MiniGameClick childObject;

        for (int i = 0; i < numOfShapes; i++)
        {
            shapes[i] = Instantiate(shapePrefab, SetObjectPositions(objPosIndexes[i]), Quaternion.identity, MiniGameFather.transform);
            shapeT = shapes[i].GetComponent<Transform>();
            shapeT.localScale = new Vector3(objectScale, objectScale, 1f);
            shapeSR = shapes[i].GetComponent<SpriteRenderer>();
            //int shapeIndex = Random.Range(0, 3);
            //shapeSR.sprite = Chapters.instance.GetChapterThingSprite(chapterIndex, shapeIndex);
            childObject = shapes[i].GetComponent<MiniGameClick>();
            childObject.fatherScript = this;
            StartCoroutine(Transitions.scaleUp(shapes[i], Vector3.zero, 0.3f));
        }
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(Transitions.typeTheText(gameDescription, 0.1f));
    }

    public Vector3 SetObjectPositions(int objectIndex)
    {
        Vector3 objectPosition = new Vector3(xOfFirstGrid + (objectIndex % numOfColumns) * xDiffOfNextGrid,
                                             yOfFirstGrid - (objectIndex / numOfColumns) * yDiffOfNextGrid, 1);
        return objectPosition;
    }

    public void RevealCard()
    {
        StartCoroutine(RevealCardIE());
    }

    private IEnumerator RevealCardIE()
    {
        Sounds.instance.PlayGiveACard();
        VibrationManager.instance.VibrateSuccess();
        isEnd = true;
        gameDescription.text = "";
        yield return StartCoroutine(Transitions.RotateY(backCard, 90, true, 0.2f));
        backCard.GetComponent<Image>().sprite = cardWhiteBack;
        yield return StartCoroutine(Transitions.RotateY(backCard, 90, false, 0.2f));
        rewardObj = Instantiate(rewardPrefab, new Vector3(0f, 0f, 1f), Quaternion.identity, MiniGameFather.transform);
        rewardObj.transform.localScale = new Vector3(rewardScale, rewardScale, 1f);
        rewardObj.GetComponent<SpriteRenderer>().sprite = Chapters.instance.GetChapterThingSprite(chapterIndex, chestIndex);
        yield return StartCoroutine(Transitions.stamp(rewardObj, new Vector3(2 * rewardScale, 2 * rewardScale, 1f), 0.3f));
        gameDescription.text = LanguageManager.instance.GetTheTextByKey(textkey_awesome);
        claimButton.GetComponentInChildren<Text>().text = LanguageManager.instance.GetTheTextByKey(textkey_claim);
        yield return StartCoroutine(Transitions.typeTheText(gameDescription, 0.1f));
        claimButton.SetActive(true);
        StartCoroutine(Transitions.fadeIn(claimButton, 1f, 0.2f));
        //AnalyticsManager.instance.LogChestGameCompleted(chapterIndex,chestIndex);
    }

    public void ClaimCard() {
        StartCoroutine(ClaimCardIE());
    }

    public IEnumerator ClaimCardIE()
    {
        Quests.CheckQuestType(QuestType.EarnCards, true, 1, false,false);
        Manager.instance.chestCards[chestKey] = true;
        Manager.instance.Save(true);
        Destroy(rewardObj);
        claimButton.SetActive(false);
        MenuFather.MakeChestGold(chestButton, true, true, chestIndex == 2);
        MenuFather.UpdateProgressOfThisChapter(chapterIndex);
        yield return StartCoroutine(IntroduceCollection());
        AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.level_chest_card_claimed);
        gameObject.SetActive(false);
    }

    private IEnumerator IntroduceCollection() {
        if (Manager.instance.maxSeenLevelNum == 7 && !Manager.instance.isFTUECollectionShown)
        {
            MenuFather.CollectionIconFTUE.SetActive(true);
            MenuFather.CollectionIconFTUE.transform.position = MenuFather.CollectionIcon.transform.position;
            yield return StartCoroutine(MenuFather.FTUEScript.IntroduceAnElement(MenuFather.CollectionIconFTUE, 5, true,true));
        }
        else {
            yield return null;
        }
    }

}
