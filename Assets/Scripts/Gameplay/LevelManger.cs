using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManger : MonoBehaviour
{
    public float foamDist; // distance of foam from tile center
    public float foamDistHex; // half of distance of the hex side to its center!
    public float changeDestinationTime;
    public float wellAndDestExtraDelay; // delay for wells and destination to turn on after water reaches.
    public float backScaleDownTime; // time of the scale down of the circles of sources and destinations !
    public float delayAfterWin; // delay between win and make the screen green.
    public float animSpeed;
    public float trailWidth;
    public float timeOfFillAndEmpty;
    public float delayOfEachStep;
    public float makePipesBlueDelay; // use for make pipes blue after their filling.
    // Percent of extra moves need for win with 1 star. 
    public float totalExtraMovesPercent;
    public float extraMovesPercentFor3Star;
    public float extraMovesPercentFor2Star;
    [Tooltip("Should be as equal as the parameter in Init()")]
    public int firstChapterSize;
    // These two variables use for generate random max Progress degree.
    public float maxProgressFirstBound;
    public float maxProgressSecondBound;
    public float firstFaultProgressMultiplier;
    public float secondFaultProgressMultiplier;
    public float thirdFaultProgressMultiplier;
    [Header("Level complete particle")]
    public float winParticleTotalTime;
    public float particleDistFromCenterX;
    public float particleDistFromCenterY;
    public GameObject winParticle;
    [Header("Score")]
    public int withoutPowerUpBaseScore;
    public int withHintBaseScore;
    public int withInfiniteBaseScore;
    public int normalMoveScore;
    public int extraMoveScore;
    public int withPowerUpExtraMoveScore;
    [Header("")]
    public UIManager uiManager;
    public GameObject sourceTilePrefab;
    public GameObject destinationTilePrefab;
    [Tooltip("order of these objects should be like branchType enum except single type!")]
    public GameObject[] standardTilePrefabTetra;
    public GameObject wellTilePrefab;
    [Tooltip("order of these objects should be like branchType enum!")]
    public Sprite[] ringsTypeTetra;
    [Tooltip("order of these objects should be like branchType enum except single type!")]
    public GameObject[] standardTilePrefabHex;
    [Tooltip("order of these objects should be like branchType enum!")]
    public Sprite[] ringsTypeHex;
    public LevelObjectContainer levelObjectContainer;
    public LevelShowParameters levelShowParametersTetra;
    public LevelShowParameters levelShowParametersHex;
    [Tooltip("pass chapters in order")]
    public BakedLevelsContainer[] bakedLevelsContainerEasy;
    [Tooltip("pass chapters in order")]
    public BakedLevelsContainer[] bakedLevelsContainerHard;
    public MapGenerator mapGenerator;
    [Tooltip("pass chapters in order")]
    public MapGeneratorParametersBundle[] mapGeneratorParametersBundleEasy;
    [Tooltip("pass chapters in order")]
    public MapGeneratorParametersBundle[] mapGeneratorParametersBundleHard;
    public GameObject foamPrefab;
    public Sprite greenLeaf;
    public Sprite dryLeaf;
    public Sprite emptyWell;
    public Sprite fullWell;
    public FTUE FTUEScript;

    [HideInInspector]
    public IDictionary<Point2D, GameObject> tilesGO = new Dictionary<Point2D, GameObject>();
    [HideInInspector]
    public IDictionary<Point2D, TileController> tileControllerScripts = new Dictionary<Point2D, TileController>();
    [HideInInspector]
    public IDictionary<Point2D, SourceTileController> sourceControllerScripts = new Dictionary<Point2D, SourceTileController>();

    //IEs
    [HideInInspector]
    public IDictionary<Point2D, IEnumerator> fillIE = new Dictionary<Point2D, IEnumerator>();
    [HideInInspector]
    public IDictionary<Point2D, IEnumerator> destinationFillFoamIE = new Dictionary<Point2D, IEnumerator>();
    [HideInInspector]
    public IDictionary<Point2D, IEnumerator> destinationCallChangeColorIE = new Dictionary<Point2D, IEnumerator>();
    [HideInInspector]
    public IDictionary<Point2D, IEnumerator> destinationBackIE = new Dictionary<Point2D, IEnumerator>();
    [HideInInspector]
    public IDictionary<Point2D, IEnumerator> destinationChangeColorIE = new Dictionary<Point2D, IEnumerator>();
    [HideInInspector]
    public IDictionary<Point2D, IEnumerator> callChangeWellsIE = new Dictionary<Point2D, IEnumerator>();
    [HideInInspector]
    public IDictionary<Point2D, IEnumerator> wellBackIE = new Dictionary<Point2D, IEnumerator>();
    [HideInInspector]
    public IDictionary<Point2D, IEnumerator> wellWaterIE = new Dictionary<Point2D, IEnumerator>();
    [HideInInspector]
    public IDictionary<Point2D, IEnumerator> wellChangeColorIE = new Dictionary<Point2D, IEnumerator>();
    [HideInInspector]
    public IDictionary<Point2D, IEnumerator> wellFillFoamIE = new Dictionary<Point2D, IEnumerator>();
    [HideInInspector]
    public IEnumerator winIE;
    [HideInInspector]
    public IDictionary<Point2D, IEnumerator> sourceBackIE = new Dictionary<Point2D, IEnumerator>();

    [HideInInspector]
    public short levelNum; // real level number
    [HideInInspector]
    public bool isWin;
    [HideInInspector]
    public bool firstTime;
    [HideInInspector]
    public Vector2 mapSize;
    [HideInInspector]
    public MapModel thisMap;
    [HideInInspector]
    public float foamScaleMagnifierHex;
    [HideInInspector]
    public int numOfMoves;
    [HideInInspector]
    public int totalExtraMoves, extraMovesFor3, extraMovesFor2;
    [HideInInspector]
    public bool infiniteMove;
    [HideInInspector]
    public TileController lastClickTile;
    [HideInInspector]
    public List<Point2D> hintRandList;
    [HideInInspector]
    public float progressDegree; // a float between 0 and 3
    [HideInInspector]
    public int score;
    [HideInInspector]
    public int levelChapterNumber;
    [HideInInspector]
    public int numOfGivenHint;

    public static int greenDestinationCounter;
    public static bool clickLock;

    private TileShowParams tileShowParams;
    private LevelShowParameters levelShowParameters;
    private GameObject[] standardTilePrefab;
    private Sprite[] ringsType;
    private bool isWellOn;
    private bool isHard;
    private const int numOfHandMadeChapters = 4;
    private int movesNeededForWin;
    private int consumedMoves;
    private float maxProgressDegree; // a float between 0 and 3
    private float lastAddedProgressDegree;
    private float faultProgressMultiplier;
    private bool isUsedUndo;
    private int levelTimerInSecond;
    private IEnumerator timerIE;
    private bool isUsedHint;
    private int normalMoveCounter; // use for count normal moves in infinite move mode.
    private bool isFirstMove;

    private static readonly string textkey_TapToSkip = "TapToSkip";

    // Start is called before the first frame update
    void Start()
    {
        hintRandList = new List<Point2D>();
        mapGenerator = new MapGenerator();
        levelNum = GameScenes.instance.levelIndex;
        StartNewLevel();
        FirstFill();
        timerIE = LevelTimer();
        StartCoroutine(timerIE);
        isFirstMove = true;
        AnalyticsManager.instance.LogLevelStarted(levelChapterNumber - 1, levelNum);
    }

    public void StartNewLevel()
    {
        Init();
        ShowMap();
    }

    private void Init()
    {
        clickLock = true;
        firstTime = true;
        greenDestinationCounter = 0;
        FindAMap();
        SetVisualDesignParams();
        firstChapterSize = 20; // should be the same as the editor value!
        maxProgressDegree = Random.Range(maxProgressFirstBound, maxProgressSecondBound);
        faultProgressMultiplier = 1;
    }

    private void FindAMap()
    {
        int levelIndex = levelNum - 1;
        FindChapterNum();
        if (levelIndex < levelObjectContainer.levelObjects.Count)
        {
            thisMap = MapSerializer.Deserialize(levelObjectContainer.levelObjects[levelIndex]);
        }
        else
        {
            if (isHard)
            {
                mapGenerator.Initialize(mapGeneratorParametersBundleHard[0]);
                thisMap = mapGenerator.Generate(bakedLevelsContainerHard[levelChapterNumber - numOfHandMadeChapters - 1].GetLevelData(levelNum));
            }
            else
            {
                mapGenerator.Initialize(mapGeneratorParametersBundleEasy[0]);
                thisMap = mapGenerator.Generate(bakedLevelsContainerEasy[levelChapterNumber - numOfHandMadeChapters - 1].GetLevelData(levelNum));
            }
        }
    }

    private void SetVisualDesignParams()
    {
        System.Tuple<Point2D, Point2D> realContentSizeAndShift = thisMap.FindRealContentSizeAndShift();
        mapSize = new Vector2(realContentSizeAndShift.Item1.x, realContentSizeAndShift.Item1.y);
        if (thisMap.type == typeof(DirectionHex))
        {
            if (realContentSizeAndShift.Item2.y % 2 == 1) {
                mapSize.y++;
            }
        }
        if (realContentSizeAndShift.Item2.x != 0 || realContentSizeAndShift.Item2.y != 0) {
            if (thisMap.type == typeof(Direction)) {
                ShiftMap(realContentSizeAndShift.Item2);
            }
            else {
                if (realContentSizeAndShift.Item2.y % 2 == 0)
                {
                    ShiftMap(realContentSizeAndShift.Item2);
                }
                else{
                    ShiftMap(new Point2D(realContentSizeAndShift.Item2.x, realContentSizeAndShift.Item2.y - 1));
                }
            }
        }
        tileShowParams.mapSize = "" + mapSize.x + "x" + mapSize.y;
        float diffToNextTile;
        if (thisMap.type == typeof(DirectionHex))
        {
            levelShowParameters = levelShowParametersHex;
            diffToNextTile = levelShowParameters.diffToNextTile;
            if (mapSize.x % 2 == 0)
                tileShowParams.leftMostPos.x = -(Mathf.Sqrt(3) / 4 * diffToNextTile + (((mapSize.x - 2) / 2) * Mathf.Sqrt(3)/2*diffToNextTile));
            else
                tileShowParams.leftMostPos.x = -(Mathf.Sqrt(3) / 2 * diffToNextTile * ((mapSize.x - 1) / 2));
            int maxTilesPerColumn = Mathf.CeilToInt((mapSize.y) / 2f);
            if (MapSerializer.hexMapUsesZeroY)
            {
                if (maxTilesPerColumn % 2 == 0)
                    tileShowParams.leftMostPos.y = -(diffToNextTile/2 + (((maxTilesPerColumn - 2) / 2) * diffToNextTile));
                else
                    tileShowParams.leftMostPos.y = -(diffToNextTile * ((maxTilesPerColumn - 1) / 2));
            }
            else
            {
                if (maxTilesPerColumn % 2 == 0)
                    tileShowParams.leftMostPos.y = -(diffToNextTile/2 + (((maxTilesPerColumn - 2) / 2) * diffToNextTile)) - diffToNextTile/2;
                else
                    tileShowParams.leftMostPos.y = -(diffToNextTile * ((maxTilesPerColumn - 1) / 2)) - diffToNextTile/2;
            }
            if (MapSerializer.hexMapUsesZeroY && mapSize.y % 2 == 0)
                tileShowParams.leftMostPos.y -= diffToNextTile/4;
            else if(!MapSerializer.hexMapUsesZeroY && mapSize.y % 2 ==1)
                tileShowParams.leftMostPos.y += diffToNextTile/4;
        }
        else if (thisMap.type == typeof(Direction))
        {
            levelShowParameters = levelShowParametersTetra;
            diffToNextTile = levelShowParameters.diffToNextTile;
            if (mapSize.x % 2 == 0)
                tileShowParams.leftMostPos.x = -(diffToNextTile/2 + (((mapSize.x - 2) / 2) * diffToNextTile));
            else
                tileShowParams.leftMostPos.x = -(diffToNextTile * ((mapSize.x - 1) / 2));
            if (mapSize.y % 2 == 0)
                tileShowParams.leftMostPos.y = -(diffToNextTile/2 + (((mapSize.y - 2) / 2) * diffToNextTile));
            else
                tileShowParams.leftMostPos.y = -(diffToNextTile * ((mapSize.y - 1) / 2));
        }
    }

    private void ShowMap()
    {
        TileModel thisTile;
        Vector3 thisTileRotation;
        Point2D tileKey;
        float tileScale = levelShowParameters.tileScale;
        int rotationStep = -90;
        float destinationRingScaleMagnifier = levelShowParameters.destinationRingScaleMagnifier;
        float wellRingScaleMagnifier = levelShowParameters.wellRingScaleMagnifier;
        if (thisMap.type == typeof(Direction))
        {
            standardTilePrefab = standardTilePrefabTetra;
            ringsType = ringsTypeTetra;
            rotationStep = -90;
        }
        else if (thisMap.type == typeof(DirectionHex))
        {
            standardTilePrefab = standardTilePrefabHex;
            ringsType = ringsTypeHex;
            rotationStep = -60;
        }
        foamScaleMagnifierHex = levelShowParameters.foamScaleMagnifier;
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                thisTile = thisMap.tiles[i][j];
                tileKey = new Point2D(i, j);
                if (thisTile != null)
                {
                    if (thisTile.tileType == TileType.Source)
                    {
                        tilesGO.Add(tileKey, Instantiate(sourceTilePrefab, SetTilePos(thisTile), Quaternion.identity, transform));
                        sourceControllerScripts.Add(tileKey, tilesGO[tileKey].GetComponent<SourceTileController>());
                        sourceControllerScripts[tileKey].levelManger = this;
                        sourceControllerScripts[tileKey].thisTile = thisTile;
                        sourceControllerScripts[tileKey].Init();
                        sourceControllerScripts[tileKey].sourceRing.GetComponent<SpriteRenderer>().sprite = ringsType[(int)thisTile.branchType];
                        thisTileRotation = sourceControllerScripts[tileKey].sourceRing.transform.eulerAngles;
                        sourceControllerScripts[tileKey].sourceRing.transform.eulerAngles =
                            new Vector3(thisTileRotation.x, thisTileRotation.y, thisTile.rotation * (rotationStep));
                    }
                    else if (thisTile.tileType == TileType.Destination)
                    {
                        tilesGO.Add(tileKey, Instantiate(destinationTilePrefab, SetTilePos(thisTile), Quaternion.identity, transform));
                        tileControllerScripts.Add(tileKey, tilesGO[tileKey].GetComponent<TileController>());
                        tileControllerScripts[tileKey].InitDestination();
                        tileControllerScripts[tileKey].ringSR.sprite = ringsType[(int)thisTile.branchType];
                        thisTileRotation = tileControllerScripts[tileKey].ring.transform.eulerAngles;
                        tileControllerScripts[tileKey].ring.transform.eulerAngles =
                            new Vector3(thisTileRotation.x, thisTileRotation.y, thisTile.rotation * (rotationStep));
                        destinationCallChangeColorIE.Add(tileKey, null);
                        destinationBackIE.Add(tileKey, null);
                        destinationChangeColorIE.Add(tileKey, null);
                        destinationFillFoamIE.Add(tileKey, null);
                    }
                    else if (thisTile.tileType == TileType.Well)
                    {
                        tilesGO.Add(tileKey, Instantiate(wellTilePrefab, SetTilePos(thisTile), Quaternion.identity, transform));
                        tileControllerScripts.Add(tileKey, tilesGO[tileKey].GetComponent<TileController>());
                        tileControllerScripts[tileKey].InitWell();
                        tileControllerScripts[tileKey].ringSR.sprite = ringsType[(int)thisTile.branchType];
                        thisTileRotation = tileControllerScripts[tileKey].ring.transform.eulerAngles;
                        tileControllerScripts[tileKey].ring.transform.eulerAngles =
                            new Vector3(thisTileRotation.x, thisTileRotation.y, thisTile.rotation * (rotationStep));
                        callChangeWellsIE.Add(tileKey, null);
                        wellBackIE.Add(tileKey, null);
                        wellWaterIE.Add(tileKey, null);
                        wellChangeColorIE.Add(tileKey, null);
                        wellFillFoamIE.Add(tileKey, null);
                    }
                    else
                    {
                        tilesGO.Add(tileKey, Instantiate(standardTilePrefab[(int)thisTile.branchType - 1], SetTilePos(thisTile), Quaternion.identity, transform));
                        tileControllerScripts.Add(tileKey, tilesGO[tileKey].GetComponent<TileController>());
                        thisTileRotation = tilesGO[tileKey].transform.eulerAngles;
                        tilesGO[tileKey].transform.eulerAngles =
                            new Vector3(thisTileRotation.x, thisTileRotation.y, thisTile.rotation * (rotationStep));
                        fillIE.Add(tileKey, null);
                    }
                    if (tileControllerScripts.ContainsKey(tileKey))
                    {
                        tileControllerScripts[tileKey].thisTile = thisTile;
                        tileControllerScripts[tileKey].levelManger = this;
                        tileControllerScripts[tileKey].Init();
                        hintRandList.Add(tileKey);
                    }
                    if (thisTile.tileType == TileType.Standard)
                    {
                        tilesGO[tileKey].transform.localScale = new Vector3(tileScale, tileScale, 1);
                        tileControllerScripts[tileKey].InitStandard();
                    }
                    // use for all types that has ring!
                    else
                    {
                        if (thisTile.tileType == TileType.Well)
                        {
                            tilesGO[tileKey].transform.localScale = new Vector3(tileScale * wellRingScaleMagnifier,
                                tileScale * wellRingScaleMagnifier, 1);
                        }
                        else
                        {
                            tilesGO[tileKey].transform.localScale = new Vector3(tileScale * destinationRingScaleMagnifier,
                                tileScale * destinationRingScaleMagnifier, 1);
                        }
                        float ringScale;
                        if (thisTile.tileType == TileType.Source)
                        {
                            ringScale = sourceControllerScripts[tileKey].sourceRing.transform.localScale.x;
                            sourceControllerScripts[tileKey].sourceRing.transform.localScale =
                                new Vector3(ringScale / destinationRingScaleMagnifier, ringScale / destinationRingScaleMagnifier, 1);
                        }
                        else if (thisTile.tileType == TileType.Destination)
                        {
                            ringScale = tileControllerScripts[tileKey].ring.transform.localScale.x;
                            tileControllerScripts[tileKey].ring.transform.localScale =
                                new Vector3(ringScale / destinationRingScaleMagnifier, ringScale / destinationRingScaleMagnifier, 1);
                        }
                        else if (thisTile.tileType == TileType.Well)
                        {
                            ringScale = tileControllerScripts[tileKey].ring.transform.localScale.x;
                            tileControllerScripts[tileKey].ring.transform.localScale =
                                new Vector3(ringScale / wellRingScaleMagnifier, ringScale / wellRingScaleMagnifier, 1);
                        }
                    }
                    numOfMoves += (thisTile.correctRotation - thisTile.rotation + TileController.numOfDirections) % GetNumOfTileState(thisMap, thisTile.branchType);
                }
            }
        }
        InitNumOfMoves();
        Essentials.initAndShuffleList(hintRandList);
        FTUEScript.DottedBackTile(tilesGO, thisMap.type);
        FTUEAfterStartLevel();
    }

    private Vector2 SetTilePos(TileModel thisTile)
    {
        Vector2 leftMostTilePos = tileShowParams.leftMostPos;
        float diffToNextTile = levelShowParameters.diffToNextTile;
        if (thisMap.type == typeof(Direction))
        {
            return new Vector2(leftMostTilePos.x + thisTile.position.x * diffToNextTile,
            leftMostTilePos.y + thisTile.position.y * diffToNextTile);
        }
        if (thisMap.type == typeof(DirectionHex))
        {
            return new Vector2(leftMostTilePos.x + thisTile.position.x * diffToNextTile * (Mathf.Sqrt(3) / 2),
            leftMostTilePos.y + thisTile.position.y * diffToNextTile / 2);
        }
        throw new System.Exception("there is no such type!");
    }

    public void Energize()
    {
        TileModel tileModel;

        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                tileModel = thisMap.GetTile(i, j);
                if (tileModel != null)
                {
                    tileModel.Deenergize();
                }
            }
        }
        thisMap.numOfHasWaterTiles = 0;
        thisMap.fillingQueue.Clear();
        foreach (SourceTileModel source in thisMap.sourceTiles)
        {
            thisMap.fillingQueue.Enqueue(source);
            source.isAddedToQueue = true;
        }
        TileModel tile = thisMap.fillingQueue.Dequeue();
        tile.RecursiveEnergize();
    }

    public IEnumerator FillOrEmptyTiles(Point2D clickedTilePos)
    {
        TileModel thisTile;
        foreach (TileController tileController in tileControllerScripts.Values)
        {
            if (tileController.thisTile.tileType == TileType.Standard)
            {
                thisTile = tileController.thisTile;
                if (clickedTilePos != Point2D.minusOne && thisTile.position == clickedTilePos)
                {
                    if (thisTile.hasWater)
                    {
                        if (thisTile.preHasWater)
                        {
                            tileController.tileSR.color = ZColor.WaterBlue;
                            tileController.StartFillFoam();
                        }
                        else
                        {
                            Quests.CheckQuestType(QuestType.FillPipes, true, 1, false, false);
                            fillIE[thisTile.position] = tileController.Fill();
                            StartCoroutine(fillIE[thisTile.position]);
                            Sounds.instance.PlayWaterFlow();
                            VibrationManager.instance.VibrateSuccess();
                        }
                    }
                    else
                    {
                        if (thisTile.preHasWater)
                        {
                            Quests.CheckQuestType(QuestType.FillPipes, false, 1, false, false);
                        }
                        tileController.tileSR.color = ZColor.NoWaterBlue;
                        tileController.thisTile.preHasWater = false;
                    }
                }
                else if (thisTile.hasWater != thisTile.preHasWater)
                {
                    if (thisTile.hasWater)
                    {
                        Quests.CheckQuestType(QuestType.FillPipes, true, 1, false, false);
                        fillIE[thisTile.position] = tileController.Fill();
                        StartCoroutine(fillIE[thisTile.position]);
                        Sounds.instance.PlayWaterFlow();
                        VibrationManager.instance.VibrateSuccess();
                    }
                    else
                    {
                        Quests.CheckQuestType(QuestType.FillPipes, false, 1, false, false);
                        tileController.Empty();
                    }
                }
            }
        }
        yield return null;
    }

    public IEnumerator GreenDestinations(TileModel clickedTile)
    {
        TileModel thisTile;
        for (int i = 0; i < thisMap.destinationTiles.Count; i++)
        {
            thisTile = thisMap.destinationTiles[i];
            if (clickedTile != null && thisTile == clickedTile && thisTile.hasWater && thisTile.preHasWater)
            {
                destinationFillFoamIE[thisTile.position] = tileControllerScripts[thisTile.position].FillFoam();
                StartCoroutine(destinationFillFoamIE[thisTile.position]);
            }
            if (thisTile.hasWater && !thisTile.preHasWater)
            {
                destinationCallChangeColorIE[thisTile.position] = CallChangeColor(true, thisTile);
                StartCoroutine(destinationCallChangeColorIE[thisTile.position]);
                greenDestinationCounter++;
            }
            else if (!thisTile.hasWater && thisTile.preHasWater)
            {
                destinationCallChangeColorIE[thisTile.position] = CallChangeColor(false, thisTile);
                StartCoroutine(destinationCallChangeColorIE[thisTile.position]);
                greenDestinationCounter--;
            }
        }
        yield return new WaitForSeconds(CalculateWinDelay());
        CheckWinState();
    }

    private IEnumerator CallChangeColor(bool turnOn, TileModel thisTile)
    {
        thisTile.preHasWater = thisTile.hasWater;
        TileController thisTileController = tileControllerScripts[thisTile.position];
        if (turnOn)
        {
            Quests.CheckQuestType(QuestType.GrowPlants,true,1,false,false);
            if (thisTile.stepsFromSource >= 0)
            {
                yield return new WaitForSeconds(thisTile.stepsFromSource * delayOfEachStep + wellAndDestExtraDelay);
            }
            Sounds.instance.PlayPlantGrow();
            VibrationManager.instance.VibrateSuccess();
            thisTileController.StopBounce();
            thisTileController.destinationPoof.SetActive(true);
            destinationBackIE[thisTile.position] = Transitions.setScale(thisTileController.back,
                new Vector3(0.02f, 0.02f, 1), backScaleDownTime);
            StartCoroutine(destinationBackIE[thisTile.position]);
            destinationChangeColorIE[thisTile.position] = Transitions.changeColor(thisTileController.ringSR,
                ZColor.WaterBlue, changeDestinationTime);
            StartCoroutine(destinationChangeColorIE[thisTile.position]);
            destinationFillFoamIE[thisTile.position] = thisTileController.FillFoam();
            StartCoroutine(destinationFillFoamIE[thisTile.position]);
        }
        else
        {
            Quests.CheckQuestType(QuestType.GrowPlants, false, 1, false, false);
            Sounds.instance.PlayPlantFade();
            VibrationManager.instance.VibrateFailure();
            thisTileController.tileSR.sprite = dryLeaf;
            thisTileController.destinationPoof.SetActive(false);
            thisTileController.back.SetActive(true);
            thisTileController.StartBounce();
            destinationBackIE[thisTile.position] = Transitions.setScale(thisTileController.back,
                thisTileController.backScale, backScaleDownTime);
            StartCoroutine(destinationBackIE[thisTile.position]);
            destinationChangeColorIE[thisTile.position] = Transitions.changeColor(thisTileController.ringSR,
                ZColor.OrangeLeaf, changeDestinationTime);
            StartCoroutine(destinationChangeColorIE[thisTile.position]);
            thisTileController.EmptyFoam();
        }
        yield return new WaitForSeconds(changeDestinationTime);
        if (turnOn)
        {
            thisTileController.back.SetActive(false);
            thisTileController.tileSR.sprite = greenLeaf;
        }
    }

    public IEnumerator CheckWells()
    {
        TileModel thisTile;
        for (int i = 0; i < thisMap.wellTiles.Count; i++)
        {
            thisTile = thisMap.wellTiles[i];
            if (thisTile.hasWater && !thisTile.preHasWater)
            {
                callChangeWellsIE[thisTile.position] = callChangeWells(true, thisTile);
                StartCoroutine(callChangeWellsIE[thisTile.position]);
            }
            else if (!thisTile.hasWater && thisTile.preHasWater)
            {
                callChangeWellsIE[thisTile.position] = callChangeWells(false, thisTile);
                StartCoroutine(callChangeWellsIE[thisTile.position]);
            }
        }
        yield return null;
    }

    private IEnumerator callChangeWells(bool turnOn, TileModel thisTile)
    {
        thisTile.preHasWater = thisTile.hasWater;
        TileController thisTileController = tileControllerScripts[thisTile.position];
        if (turnOn)
        {
            Quests.CheckQuestType(QuestType.FillWells,true,1,false,false);
            if (thisTile.stepsFromSource >= 0)
            {
                yield return new WaitForSeconds(thisTile.stepsFromSource * delayOfEachStep + wellAndDestExtraDelay);
            }
            if (!isWellOn)
            {
                Sounds.instance.PlayWellFill();
                VibrationManager.instance.VibrateSuccess();
                isWellOn = true;
            }
            thisTileController.tileSR.sprite = fullWell;
            thisTileController.wellPoof.SetActive(true);
            thisTileController.StopBounce();
            wellBackIE[thisTile.position] = Transitions.setScale(thisTileController.back,
                thisTileController.backScale, backScaleDownTime);
            StartCoroutine(wellBackIE[thisTile.position]);
            thisTileController.wellWater.SetActive(true);
            wellWaterIE[thisTile.position] = Transitions.setScale(thisTileController.wellWater,
                thisTileController.wellWaterScale, backScaleDownTime);
            StartCoroutine(wellWaterIE[thisTile.position]);
            wellChangeColorIE[thisTile.position] = Transitions.changeColor(thisTileController.ringSR,
                ZColor.WaterBlue, changeDestinationTime);
            StartCoroutine(wellChangeColorIE[thisTile.position]);
            wellFillFoamIE[thisTile.position] = thisTileController.FillFoam();
            StartCoroutine(wellFillFoamIE[thisTile.position]);
        }
        else
        {
            Sounds.instance.PlayWellEmpty();
            VibrationManager.instance.VibrateFailure();
            Quests.CheckQuestType(QuestType.FillWells,false,1,false,false);
            isWellOn = false;
            thisTileController.tileSR.sprite = emptyWell;
            thisTileController.wellPoof.SetActive(false);
            thisTileController.StartBounce();
            wellBackIE[thisTile.position] = Transitions.setScale(thisTileController.wellWater,
                new Vector3(0.02f, 0.02f, 1), backScaleDownTime);
            StartCoroutine(wellBackIE[thisTile.position]);
            wellChangeColorIE[thisTile.position] = Transitions.changeColor(thisTileController.ringSR,
                ZColor.OrangeLeaf, changeDestinationTime);
            StartCoroutine(wellChangeColorIE[thisTile.position]);
            thisTileController.EmptyFoam();
        }
        yield return new WaitForSeconds(changeDestinationTime);
        if (!turnOn)
        {
            thisTileController.wellWater.SetActive(false);
        }
        yield return null;
    }

    private void CheckWinState()
    {
        if (thisMap.numOfHasWaterTiles == thisMap.numOfTiles)
        {
            bool canWin = true;
            foreach (TileController tileController in tileControllerScripts.Values) { 
                if (tileController.foamDict.Count != 0)
                {
                    canWin = false;
                }
            }
            foreach (SourceTileController sourceTileController in sourceControllerScripts.Values)
            {
                if (sourceTileController.foamDict.Count != 0)
                {
                    canWin = false;
                }
            }
            if (!isWin && canWin)
            {
                isWin = true;
                winIE = Win();
                StartCoroutine(winIE);
            }
        }
        if (!isWin && numOfMoves == 0) {
            StopCoroutine(timerIE);
            Quests.CheckQuestType(QuestType.PlayLevelsForSomeMinutes, true, levelTimerInSecond / 60, false, false);
            Quests.CheckQuestType(QuestType.PlayLevelsForSomeMinutesContinuously, true, levelTimerInSecond / 60, false, false);
            Quests.CheckQuestType(QuestType.WinLevelsContinuously, true, 0, true,false);
            Quests.CheckQuestType(QuestType.RotateTiles, true, consumedMoves, false, false);
            Manager.instance.Save(false);
            StartCoroutine(Transitions.moveToPosition(uiManager.BottomBar,new Vector3(0,-6,0),0.2f));
            FTUEScript.isFaddingDottedTiles = false;
            FTUEScript.StopDottedTiles();
            uiManager.ShowLevelFailed();
            AnalyticsManager.instance.LogLevelFailed(levelChapterNumber - 1,levelNum);
        }
    }

    public void CleanLastLevel()
    {
        foreach (GameObject tileObject in tilesGO.Values)
        {
            Destroy(tileObject);
        }
        tilesGO.Clear();
        tileControllerScripts.Clear();
        sourceControllerScripts.Clear();
        fillIE.Clear();
        destinationFillFoamIE.Clear();
        destinationCallChangeColorIE.Clear();
        destinationBackIE.Clear();
        destinationChangeColorIE.Clear();
        callChangeWellsIE.Clear();
        wellBackIE.Clear();
        wellWaterIE.Clear();
        wellChangeColorIE.Clear();
        wellFillFoamIE.Clear();
        sourceBackIE.Clear();
    }

    public IEnumerator Win()
    {
        int oldStars = 0;
        uiManager.levelCompleteIsOnScreen = true;
        uiManager.backButton.SetActive(false);
        FTUEScript.isFaddingDottedTiles = false;
        FTUEScript.StopDottedTiles();
        CalculateScore();
        StopCoroutine(timerIE);
        Quests.CheckQuestType(QuestType.PlayLevelsForSomeMinutes, true, levelTimerInSecond / 60, false, false);
        Quests.CheckQuestType(QuestType.PlayLevelsForSomeMinutesContinuously, true, levelTimerInSecond / 60, false, false);
        Quests.CheckQuestType(QuestType.FinishLevels,true,1, false,false);
        Quests.CheckQuestType(QuestType.WinLevelsContinuously,true,1, false,false);
        Quests.CheckQuestType(QuestType.RotateTiles, true, consumedMoves, false, false);
        if (!isUsedUndo) {
            Quests.CheckQuestType(QuestType.WinWithoutUndo,true,1,false,false);
        }
        StartCoroutine(Transitions.moveToPosition(uiManager.BottomBar, new Vector3(0, -6, 0), 0.2f));
        int score = GetNumOfStarsAfterWin();
        if (Manager.instance.levelStars.ContainsKey(levelNum))
        {
            oldStars = 1 + (Manager.instance.levelStars[levelNum] ? 1 : 0);
            if (score == 2)
            {
                Manager.instance.levelStars[levelNum] = true;
            }
            else if (score == 3)
            {
                Manager.instance.levelStars.Remove(levelNum);
            }   
        }
        else
        {
            if (levelNum < Manager.instance.maxSeenLevelNum)
            {
                oldStars = 3;
            }
            else
            {
                if (score == 1)
                {
                    Manager.instance.levelStars.Add(levelNum, false);
                }
                else if (score == 2)
                {
                    Manager.instance.levelStars.Add(levelNum, true);
                }
            }
        }
        if (score == 3 && score > oldStars)
        {
            Quests.CheckQuestType(QuestType.WinWith3Stars, true, 1, false, false);
        }
        if (score > oldStars)
        {
            Manager.instance.chapterStars[(byte)Chapters.instance.GetChapterIndexOfThisLevel(levelNum)]
                += (byte)(score - oldStars);
        }
        if (levelNum == Manager.instance.maxSeenLevelNum) {
            AchievementManager.CheckAchievements_LevelCompleted(levelNum);
            Quests.CheckQuestType(QuestType.ReachSpecificLevel,true,1,false,false);
            Manager.instance.maxSeenLevelNum++;
        }
        Manager.instance.Save(true);
        foreach (SourceTileController sourceTileController in sourceControllerScripts.Values)
        {
            sourceTileController.StopBounce();
            sourceBackIE[sourceTileController.thisTile.position] =
                (Transitions.scaleDownAndDestroy(sourceTileController.sourceBack,
                    new Vector3(0.02f, 0.02f, 1), backScaleDownTime));
            StartCoroutine(sourceBackIE[sourceTileController.thisTile.position]);
        }
        yield return new WaitForSeconds(backScaleDownTime + delayAfterWin);
        yield return StartCoroutine(uiManager.FillProgressSectorsAfterWin(score, numOfMoves, extraMovesFor2, extraMovesFor3));
        uiManager.skipText.text = LanguageManager.instance.GetTheTextByKey(textkey_TapToSkip);
        if (levelNum >= 2) {
            StartCoroutine(Transitions.scaleUp(uiManager.skipButton, new Vector3(0.8f, 0.8f, 1), 0.2f));
        }
        yield return StartCoroutine(WinParticles());
        uiManager.ShowLevelCompleted(score,oldStars);
    }

    private IEnumerator WinParticles()
    {
        if (numOfMoves != 0)
        {
            float timeForEachParticle = winParticleTotalTime / numOfMoves;
            int particleCounter = numOfMoves;
            Sounds.instance.PlayPoof3Seconds();
            VibrationManager.instance.VibrateSuccess();
            Vector2 particlePos = new Vector2();
            while (particleCounter > 0 && !uiManager.isSkipEndGame)
            {
                particleCounter--;
                uiManager.NumOfMoves.text = particleCounter.ToString();
                particlePos.x = Random.Range(-particleDistFromCenterX, particleDistFromCenterX);
                particlePos.y = Random.Range(-particleDistFromCenterY, particleDistFromCenterY);
                Instantiate(winParticle, particlePos, Quaternion.identity);
                yield return new WaitForSeconds(timeForEachParticle);
            }
        }
        yield return null;
    }

    private float CalculateWinDelay()
    {
        int maxStepFromSource = 0;
        foreach (TileModel thisTile in thisMap.destinationTiles)
        {
            if (thisTile.stepsFromSource > 0 && thisTile.stepsFromSource > maxStepFromSource)
            {
                maxStepFromSource = thisTile.stepsFromSource;
            }
        }
        return changeDestinationTime + maxStepFromSource * delayOfEachStep + wellAndDestExtraDelay;
    }

    public void FirstFill()
    {
        Sounds.instance.PlayLevelStart();
        Energize();
        StartCoroutine(FillOrEmptyTiles(Point2D.minusOne));
        StartCoroutine(GreenDestinations(null));
        StartCoroutine(CheckWells());
        SourcesFirstFill();
        firstTime = false;
    }

    private void SourcesFirstFill()
    {
        foreach (SourceTileController sourceTileController in sourceControllerScripts.Values)
        {
            sourceTileController.InitFoams();
        }
    }

    public static int GetNumOfTileState(MapModel thisMap, object branchType)
    {
        if (thisMap.type == typeof(Direction))
        {
            if ((BranchType)branchType == BranchType.straight)
            {
                return 2;
            }
            else if ((BranchType)branchType == BranchType.plusType)
            {
                return 1;
            }
            else
            {
                return 4;
            }
        }
        else if (thisMap.type == typeof(DirectionHex))
        {
            if ((BranchTypeHex)branchType == BranchTypeHex.hex21
                || (BranchTypeHex)branchType == BranchTypeHex.hex42)
            {
                return 3;
            }
            else if ((BranchTypeHex)branchType == BranchTypeHex.hex33)
            {
                return 2;
            }
            else if ((BranchTypeHex)branchType == BranchTypeHex.hex60)
            {
                return 1;
            }
            else
            {
                return 6;
            }
        }
        throw new System.Exception("it is a strange map type!");
    }

    public void ConsumeAMove(bool isReversed)
    {
        if (!infiniteMove)
        {
            if (isReversed)
            {
                score -= normalMoveScore;
                numOfMoves++;
                uiManager.NumOfMoves.text = (LanguageManager.instance.isLanguageRTL ? LanguageManager.arabicFix(numOfMoves.ToString()) : numOfMoves.ToString());
            }
            else
            {
                if (isFirstMove) {
                    StartCoroutine(FTUEAfterSomeMoves());
                }
                if (numOfMoves > 0)
                {
                    score += normalMoveScore;
                    numOfMoves--;
                    uiManager.NumOfMoves.text = (LanguageManager.instance.isLanguageRTL ? LanguageManager.arabicFix(numOfMoves.ToString()) : numOfMoves.ToString());
                }
                
            }
        }
        else {
            if (isReversed)
            {
                normalMoveCounter--;
            }
            else
            {
                normalMoveCounter++;
            }
        }
    }

    public void OnlineProgress(TileModel clickedTile, bool isReversed,bool isHint,int hintMoves = 0)
    {
        if (!isReversed)
        {
            if (!isHint)
            {
                if (clickedTile.rotation == clickedTile.correctRotation)
                {
                    movesNeededForWin += GetNumOfTileState(thisMap, clickedTile.branchType);
                    if (faultProgressMultiplier == 1)
                    {
                        faultProgressMultiplier = firstFaultProgressMultiplier;
                    }
                    else if (faultProgressMultiplier == firstFaultProgressMultiplier)
                    {
                        faultProgressMultiplier = secondFaultProgressMultiplier;
                    }
                    else if(faultProgressMultiplier == secondFaultProgressMultiplier) {
                        faultProgressMultiplier = thirdFaultProgressMultiplier;
                    }
                }
                lastAddedProgressDegree = faultProgressMultiplier *
                    (maxProgressDegree - progressDegree) / (movesNeededForWin - consumedMoves);
                StartCoroutine(uiManager.FillProgressSectors(faultProgressMultiplier *
                    (maxProgressDegree - progressDegree) / (movesNeededForWin - consumedMoves)));
                consumedMoves++;
            }
            else {
                StartCoroutine(uiManager.FillProgressSectors(hintMoves *
                    faultProgressMultiplier * (maxProgressDegree - progressDegree) / (movesNeededForWin - consumedMoves)));
                consumedMoves += hintMoves;
            }
        }
        else
        {
            consumedMoves--;
            if (clickedTile.rotation == clickedTile.correctRotation)
            {
                movesNeededForWin -= GetNumOfTileState(thisMap, clickedTile.branchType);
                if (faultProgressMultiplier == firstFaultProgressMultiplier)
                {
                    faultProgressMultiplier = 1;
                }
                else if (faultProgressMultiplier == secondFaultProgressMultiplier)
                {
                    faultProgressMultiplier = firstFaultProgressMultiplier;
                }
                else if (faultProgressMultiplier == thirdFaultProgressMultiplier)
                {
                    faultProgressMultiplier = secondFaultProgressMultiplier;
                }
            }
            progressDegree -= lastAddedProgressDegree;
            if (progressDegree < 1) {
                uiManager.offStar.SetActive(true);
                uiManager.oneStar.SetActive(false);
                if (progressDegree + lastAddedProgressDegree < 1) {
                    uiManager.progressSectors[0].fillAmount -= lastAddedProgressDegree;
                }
                else
                {
                    uiManager.progressSectors[0].fillAmount -= lastAddedProgressDegree - uiManager.progressSectors[1].fillAmount;
                    uiManager.progressSectors[1].fillAmount = 0;
                }
            }
            else {
                uiManager.offStar.SetActive(false);
                uiManager.oneStar.SetActive(true);
                uiManager.progressSectors[1].fillAmount -= lastAddedProgressDegree;
            }
        }
    }

    public void InitNumOfMoves()
    {
        movesNeededForWin = numOfMoves;
        Debug.Log("Moves:" + numOfMoves);
        totalExtraMoves = (int)(numOfMoves * totalExtraMovesPercent);
        extraMovesFor3 = (int)(numOfMoves * extraMovesPercentFor3Star);
        extraMovesFor2 = (int)(numOfMoves * extraMovesPercentFor2Star);
        if (levelNum < 4) {
            totalExtraMoves += 8;
        }
        Debug.Log("ExtraMoves:" + totalExtraMoves + "," + extraMovesFor3 + "," + extraMovesFor2);
        numOfMoves += totalExtraMoves;
        Debug.Log("Final Moves:" + numOfMoves);
        uiManager.NumOfMoves.text = (LanguageManager.instance.isLanguageRTL ? LanguageManager.arabicFix(numOfMoves.ToString()) : numOfMoves.ToString());
        infiniteMove = false;
    }

    public bool IsThereMovesLeft()
    {
        return numOfMoves > 0;
    }

    public int GetNumOfStarsAfterWin()
    {
        if(numOfMoves >= extraMovesFor3)
        {
            return 3;
        }
        else if (numOfMoves >= extraMovesFor2)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }

    private void ShiftMap(Point2D lowest)
    {
        if (lowest.x != 0 || lowest.y != 0) {
            for (int i = 0; i < thisMap.size.x; i++)
            {
                for (int j = 0; j < thisMap.size.y; j++)
                {
                    if (thisMap.tiles[i][j] != null)
                    {
                        thisMap.tiles[i][j].position = new Point2D(i - lowest.x, j - lowest.y);
                        thisMap.tiles[i - lowest.x][j - lowest.y] = thisMap.tiles[i][j];
                        thisMap.tiles[i][j] = null;
                    }
                }
            }
        }
    }

    public void Undo()
    {
        isUsedUndo = true;
        StartCoroutine(lastClickTile.RotateTile(true));
        uiManager.undoBtn.interactable = false;
        AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.ingame_undo);
    }

    public void Hint()
    {
        if (numOfGivenHint < 3) {
            Sounds.instance.PlayHintUse();
            VibrationManager.instance.VibrateSuccess();
            isUsedHint = true;
            Quests.CheckQuestType(QuestType.UsePowerUps, true, 1, false, false);
            AchievementManager.UnlockAchievement_FirstHint();
            Manager.instance.numOfHint--;
            uiManager.hintNumber.text = Manager.instance.numOfHint.ToString();
            Manager.instance.Save(false);
            if (Manager.instance.numOfHint == 0)
            {
                uiManager.ToggleHintPlusAndBadge(true);
            }
            StartCoroutine(tileControllerScripts[hintRandList[0]].HintRotates(numOfGivenHint));
            numOfGivenHint++;
            AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.ingame_hint, ":" + numOfGivenHint);
        }
    }

    private void FindChapterNum()
    {
        int levelIndexInChapter = levelNum; // real level number
        int numOfCompletedChapter;

        if (levelIndexInChapter < 81)
        {
            if (levelIndexInChapter % firstChapterSize == 0)
            {
                isHard = true;
                levelChapterNumber = levelIndexInChapter / firstChapterSize;
            }
            else
            {
                if (levelIndexInChapter % firstChapterSize <= firstChapterSize / 2)
                {
                    isHard = false;
                }
                else
                {
                    isHard = true;
                }
                levelChapterNumber = levelIndexInChapter / firstChapterSize + 1;
            }
            print("Chapter: " + levelChapterNumber);
            return;
        }

        else if (levelIndexInChapter > 80)
        {
            numOfCompletedChapter = 4;
            levelIndexInChapter -= 80;
            firstChapterSize = 25;
            while (true)
            {
                if (levelIndexInChapter < 2 * firstChapterSize + 1)
                {
                    if (levelIndexInChapter % firstChapterSize == 0)
                    {
                        isHard = true;
                        levelChapterNumber = numOfCompletedChapter + (levelIndexInChapter / firstChapterSize);
                    }
                    else
                    {
                        if (levelIndexInChapter % firstChapterSize <= firstChapterSize / 2)
                        {
                            isHard = false;
                        }
                        else
                        {
                            isHard = true;
                        }
                        levelChapterNumber = numOfCompletedChapter + (levelIndexInChapter / firstChapterSize) + 1;
                    }
                    print("Chapter: " + levelChapterNumber);
                    return;
                }
                else
                {
                    numOfCompletedChapter += 2;
                    levelIndexInChapter -= 2 * firstChapterSize;
                    if (firstChapterSize < 50) {
                        firstChapterSize += 5;
                    }
                }
            }
        }
    }

    private IEnumerator LevelTimer() {
        yield return new WaitForSeconds(1);
        levelTimerInSecond++;
    }

    private void OnApplicationQuit()
    {
        Quests.CheckQuestType(QuestType.PlayLevelsForSomeMinutesContinuously,false,0,true,true);
    }

    private void CalculateScore() {
        if (infiniteMove)
        {
            score += withInfiniteBaseScore;
            if (numOfMoves > normalMoveCounter) {
                score += (numOfMoves - normalMoveCounter) * withPowerUpExtraMoveScore;
            }
            if (score > 16000) {
                score = (score - 16000) / 100 + 16000;
            }
        }
        else if (isUsedHint)
        {
            score += withHintBaseScore;
            score += withPowerUpExtraMoveScore * numOfMoves;
        }
        else {
            score += withoutPowerUpBaseScore;
            score += extraMoveScore * numOfMoves;
        }
    }

    private IEnumerator FTUEAfterSomeMoves() {
        isFirstMove = false;
        if (levelNum == 2 && Manager.instance.maxSeenLevelNum == levelNum)
        {
            StartCoroutine(FTUEScript.IntroduceAnElement(uiManager.movesNumberFTUE, 0,false,true));
            uiManager.movesNumberFTUE.SetActive(true);
            uiManager.movesNumberFTUE.transform.position = uiManager.NumOfMoves.transform.parent.position;
            uiManager.NumOfMovesFTUE.text = (numOfMoves-1).ToString(); // -1 is because of this code execute before submit move in database!
            if (LanguageManager.instance.isLanguageRTL)
                uiManager.NumOfMovesFTUE.text = LanguageManager.arabicFix(uiManager.NumOfMovesFTUE.text);
        }
        else if (levelNum == 3 && Manager.instance.maxSeenLevelNum == levelNum)
        {
            StartCoroutine(FTUEScript.IntroduceAnElement(uiManager.undoBtnFTUE.gameObject, 1,true,true));
            uiManager.undoImage.sprite = uiManager.undoSprite;
            uiManager.undoImage.transform.localPosition = new Vector3(0, 0, 0);
            yield return StartCoroutine(Transitions.stamp(uiManager.undoImage.gameObject,new Vector3(1.2f, 1.2f,1),0.2f));
            uiManager.undoBtnFTUE.gameObject.SetActive(true);
            uiManager.undoBtnFTUE.transform.position = uiManager.undoBtn.transform.position;
            uiManager.undoBtn.enabled = true;
        }
        yield return null;
    }

    private void FTUEAfterStartLevel() {
        if (levelNum == 1 && Manager.instance.maxSeenLevelNum == levelNum) {
            uiManager.backButton.SetActive(false);
            infiniteMove = true;
            uiManager.NumOfMoves.transform.parent.gameObject.SetActive(false);
            StartCoroutine(FTUEScript.IntroduceAnElement(uiManager.firstTileFTUE,6,true,true));
            uiManager.firstTileFTUE.SetActive(true);
            tilesGO[new Point2D(0, 0)].transform.GetChild(2).GetComponent<SpriteRenderer>().sortingOrder = 11;
            tilesGO[new Point2D(1, 0)].transform.GetChild(2).GetComponent<SpriteRenderer>().enabled = false;
            AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.ftue_start);
        }
        else if (levelNum == 13 && !Manager.instance.isFTUEHintShown)
        {
            Manager.instance.numOfHint += Manager.instance.numOfHint_init;
            Manager.instance.Save(false);
            GameObject hintPlus = uiManager.hintBtn.transform.GetChild(2).gameObject;
            if (hintPlus.activeSelf) {
                uiManager.ToggleHintPlusAndBadge(false);
            }
            StartCoroutine(FTUEScript.IntroduceAnElement(uiManager.hintBtnFTUE.gameObject, 2, true,true));
            uiManager.hintBtnFTUE.gameObject.SetActive(true);
            uiManager.hintBtnFTUE.transform.position = uiManager.hintBtn.transform.position;
            uiManager.hintNumberFTUE.text = Manager.instance.numOfHint.ToString();
            if (LanguageManager.instance.isLanguageRTL)
                uiManager.hintNumberFTUE.text = LanguageManager.arabicFix(uiManager.hintNumberFTUE.text);
        }
        else if (levelNum == 20 && !Manager.instance.isFTUEInfiniteMoveShown)
        {
            Manager.instance.numOfIMItem += Manager.instance.numOfIMItem_init;
            Manager.instance.Save(false);
            GameObject infiniteMovePlus = uiManager.InfiniteMoveButton.transform.GetChild(2).gameObject;
            if (infiniteMovePlus.activeSelf) {
                uiManager.ToggleInfiniteMovesPlusAndBadge(false);
            }
            StartCoroutine(FTUEScript.IntroduceAnElement(uiManager.InfiniteMoveButtonFTUE.gameObject, 3, true,true));
            uiManager.InfiniteMoveButtonFTUE.gameObject.SetActive(true);
            uiManager.InfiniteMoveButtonFTUE.transform.position = uiManager.InfiniteMoveButton.transform.position;
            uiManager.InfiniteMoveItemNumberFTUE.text = Manager.instance.numOfIMItem.ToString();
            if (LanguageManager.instance.isLanguageRTL)
                uiManager.InfiniteMoveItemNumberFTUE.text =
                    LanguageManager.arabicFix(uiManager.InfiniteMoveItemNumberFTUE.text);
        }
    }

    public void UndoClickFTUE() {
        AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.ftue_undo);
        Undo();
        FTUEScript.CallContinueAfterIntroduction();
    }

    public void HintClickFTUE() {
        Manager.instance.isFTUEHintShown = true;
        Hint();
        FTUEScript.CallContinueAfterIntroduction();
        AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.ftue_hint);
    }

    public void InfiniteMoveClickFTUE() {
        Manager.instance.isFTUEInfiniteMoveShown = true;
        uiManager.MakeMovesInfinite();
        FTUEScript.CallContinueAfterIntroduction();
        AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.ftue_infinite_moves);
    }

    public void FirstTileClickFTUE() {
        tilesGO[new Point2D(0, 0)].transform.GetChild(2).GetComponent<SpriteRenderer>().sortingOrder = 5;
        tilesGO[new Point2D(1, 0)].transform.GetChild(2).GetComponent<SpriteRenderer>().enabled = true;
        tilesGO[new Point2D(0, 0)].GetComponent<TileController>().OnMouseDown();
        FTUEScript.CallContinueAfterIntroduction();
        AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.ftue_tap_on_pipes);
    }

}
        