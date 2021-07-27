using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TileController : MonoBehaviour
{
    public float backSmallerScale;
    [Tooltip("just for one SetScale!")]
    public float backBounceTime;
    [Tooltip("delay between two SetScales.")]
    public float backBounceDelay;

    [HideInInspector]
    public LevelManger levelManger;
    [HideInInspector]
    public TileModel thisTile;
    [HideInInspector]
    public IDictionary<Point2D,GameObject> foamDict = new Dictionary<Point2D,GameObject>();
    [HideInInspector]
    public GameObject ring;
    [HideInInspector]
    public GameObject back;
    [HideInInspector]
    public GameObject wellWater; // the blue water using in wells! 
    [HideInInspector]
    public GameObject destinationPoof; // destination poof particle.
    [HideInInspector]
    public GameObject wellPoof; // well poof particle.
    [HideInInspector]
    public Vector3 backScale;
    [HideInInspector]
    public Vector3 wellWaterScale;
    [HideInInspector]
    public SpriteRenderer tileSR;
    [HideInInspector]
    public SpriteRenderer ringSR;
    [HideInInspector]
    public IEnumerator backBounceIE;

    public static int stepDegree;
    public static int numOfDirections;

    private IEnumerator fillAndEmptyIE;
    private IEnumerator greenDestinationIE;
    private IEnumerator checkWellsIE;
    private IEnumerator fillFoamIE;
    private IDictionary<string,TileEntryController> entries = new Dictionary<string,TileEntryController>();
    private bool rotating = false;

    public void Start()
    {
        tileSR = GetComponent<SpriteRenderer>();
    }

    public void Init()
    {
        if (levelManger.thisMap.type == typeof(Direction)) {
            stepDegree = 90;
            numOfDirections = 4;
        }
        else if(levelManger.thisMap.type == typeof(DirectionHex))
        {
            stepDegree = 60;
            numOfDirections = 6;
        }
    }

    public void InitStandard()
    {
        int entryCount = transform.childCount;
        Transform thisChild;
        for (int i = 0; i < entryCount; i++)
        {
            thisChild = transform.GetChild(i);
            entries.Add(thisChild.name,thisChild.GetComponent<TileEntryController>());
            entries[thisChild.name].InitEntryParticlePos();
        }
        InitAnimation();
    }

    // only called for destination tiles.
    public void InitDestination() {
        ring = transform.GetChild(0).gameObject;
        ringSR = ring.GetComponent<SpriteRenderer>();
        back = transform.GetChild(1).gameObject;
        backScale = back.transform.localScale;
        destinationPoof = transform.GetChild(2).gameObject;
        destinationPoof.SetActive(false);
        StartBounce();
    }

    public void InitWell() {
        ring = transform.GetChild(0).gameObject;
        ringSR = ring.GetComponent<SpriteRenderer>();
        back = transform.GetChild(1).gameObject;
        backScale = back.transform.localScale;
        wellWater = transform.GetChild(2).gameObject;
        wellWaterScale = wellWater.transform.localScale;
        wellWater.SetActive(false);
        wellPoof = transform.GetChild(3).gameObject;
        wellPoof.SetActive(false);
        StartBounce();
    }

    public void InitAnimation() {
        foreach (TileEntryController entry in entries.Values)
        {
            entry.InitAnimation(thisTile.branchType);
        }
    }

    public void OnMouseDown()
    {
        if (levelManger.isWin == false && LevelManger.clickLock)
        {
            if (levelManger.IsThereMovesLeft())
                StartCoroutine(RotateTile(false));
            else
                StartCoroutine(TileErrorRotation());
        }
    }

    public IEnumerator RotateTile(bool isReversed) {
        levelManger.ConsumeAMove(isReversed);
        LevelManger.clickLock = false;
        StopMyCoroutines();
        DeleteLastFoams();
        if (!isReversed)
        {
            // the order of these two function is important!
            levelManger.OnlineProgress(thisTile, isReversed,false);
            HandleRotationParam(isReversed);
        }
        else {
            // the order of these two function is important!
            HandleRotationParam(isReversed);
            levelManger.OnlineProgress(thisTile, isReversed,false);
        }
        levelManger.Energize();
        ResetHalfAnimsAndFoams();
        levelManger.Energize();
        ClearTrail();
        HandleNeighborTiles();
        SetEnabledTrail(false);
        VibrationManager.instance.VibrateTouch();
        if (thisTile.tileType == TileType.Standard)
        {
            if (isReversed)
            {
                Sounds.instance.PlayUndo();
            }
            else {
                Sounds.instance.PlayRotatePipes();
            }
            yield return StartCoroutine(Transitions.rotateZ(gameObject, stepDegree, !isReversed, 0.1f));
        }
        else if (thisTile.tileType == TileType.Destination)
        {
            if (isReversed)
            {
                Sounds.instance.PlayUndo();
            }
            else
            {
                Sounds.instance.PlayRotateDestination();
            }
            yield return StartCoroutine(Transitions.rotateZ(ring, stepDegree, !isReversed, 0.1f));
        }
        else if(thisTile.tileType == TileType.Well) {
            if (isReversed)
            {
                Sounds.instance.PlayUndo();
            }
            else
            {
                Sounds.instance.PlayRotateWells();
            }
            yield return StartCoroutine(Transitions.rotateZ(ring, stepDegree, !isReversed, 0.1f));
        }
        if (!isReversed) {
            levelManger.lastClickTile = this;
            levelManger.uiManager.undoBtn.interactable = true;
            if (levelManger.numOfMoves == 5) {
                levelManger.uiManager.NumOfMovesWarning();
            }
        }
        SetEnabledTrail(true);
        ClearTrail();
        if (thisTile.tileType == TileType.Well) {
            fillFoamIE = FillFoam();
            StartCoroutine(fillFoamIE);
        }
        fillAndEmptyIE = levelManger.FillOrEmptyTiles(thisTile.position);
        StartCoroutine(fillAndEmptyIE);
        greenDestinationIE = levelManger.GreenDestinations(thisTile);
        StartCoroutine(greenDestinationIE);
        checkWellsIE = levelManger.CheckWells();
        StartCoroutine(checkWellsIE);
        LevelManger.clickLock = true;
        yield return new WaitForSeconds(levelManger.timeOfFillAndEmpty);
    }

    private IEnumerator TileErrorRotation()
    {
        if (!rotating)
        {
            levelManger.ConsumeAMove(false);
            rotating = true;
            Sounds.instance.PlayError();
            VibrationManager.instance.VibrateWarning();
            if (thisTile.tileType == TileType.Standard)
            {
                yield return StartCoroutine(Transitions.ErrorRotateZ(gameObject, 15, false, 0.1f, 4));
            }
            else
            {
                yield return StartCoroutine(Transitions.ErrorRotateZ(ring, 15, false, 0.1f, 4));
            }
            rotating = false;
        }
    }

    private void StopMyCoroutines() {
        if (fillAndEmptyIE != null)
        {
            StopCoroutine(fillAndEmptyIE);
        }
        if (greenDestinationIE != null) {
            StopCoroutine(greenDestinationIE);
        }
        if (checkWellsIE != null) {
            StopCoroutine(checkWellsIE);
        }
    }

    private void ResetHalfAnimsAndFoams() {
        foreach (TileController thisTileController in levelManger.tileControllerScripts.Values) {
            thisTileController.thisTile.foamDir.Clear();
            if (thisTileController.thisTile.tileType == TileType.Standard) {
                if (thisTileController.tileSR.color != ZColor.WaterBlue && thisTileController.thisTile.preHasWater
                    && !thisTileController.thisTile.hasWater)
                {
                    if(levelManger.fillIE[thisTileController.thisTile.position] != null)
                        levelManger.StopCoroutine(levelManger.fillIE[thisTileController.thisTile.position]);
                    foreach (TileEntryController entry in thisTileController.entries.Values)
                    {
                        entry.anim.Stop();
                    }
                    thisTileController.EndOfAnimation();
                    thisTileController.ClearTrail();
                    thisTileController.tileSR.color = ZColor.NoWaterBlue;
                    thisTileController.thisTile.preHasWater = false;
                    if (thisTileController.fillFoamIE != null)
                    {
                        thisTileController.StopCoroutine(thisTileController.fillFoamIE);
                    }
                    thisTileController.EmptyFoam();
                }
            }
            else if (thisTileController.thisTile.tileType == TileType.Destination) {
                SpriteRenderer tempChildSR = thisTileController.ringSR;
                if (tempChildSR.color != ZColor.WaterBlue && thisTileController.thisTile.preHasWater
                    && !thisTileController.thisTile.hasWater) {
                    thisTileController.tileSR.sprite = levelManger.dryLeaf;
                    thisTileController.destinationPoof.SetActive(false);
                    tempChildSR.color = ZColor.OrangeLeaf;
                    LevelManger.greenDestinationCounter--;
                    thisTileController.thisTile.preHasWater = false;
                    if(levelManger.destinationBackIE[thisTileController.thisTile.position] != null)
                        levelManger.StopCoroutine(levelManger.destinationBackIE[thisTileController.thisTile.position]);
                    if(levelManger.destinationCallChangeColorIE[thisTileController.thisTile.position] != null)
                        levelManger.StopCoroutine(levelManger.destinationCallChangeColorIE[thisTileController.thisTile.position]);
                    if(levelManger.destinationChangeColorIE[thisTileController.thisTile.position] != null)
                        levelManger.StopCoroutine(levelManger.destinationChangeColorIE[thisTileController.thisTile.position]);
                    if (levelManger.destinationFillFoamIE[thisTileController.thisTile.position] != null)
                        levelManger.StopCoroutine(levelManger.destinationFillFoamIE[thisTileController.thisTile.position]);
                    thisTileController.EmptyFoam();
                    thisTileController.back.transform.localScale = thisTileController.backScale;
                }
            }
            else if (thisTileController.thisTile.tileType == TileType.Well) {
                SpriteRenderer tempChildSR = thisTileController.ringSR;
                if (tempChildSR.color != ZColor.WaterBlue && tempChildSR.color != ZColor.OrangeLeaf)
                {
                    if (thisTileController.thisTile.hasWater != thisTileController.thisTile.preHasWater)
                    {
                        tempChildSR.color = ZColor.OrangeLeaf;
                        thisTileController.thisTile.preHasWater = false;
                        thisTileController.EmptyFoam();

                        // stop coroutines
                        if(levelManger.wellBackIE[thisTileController.thisTile.position] != null)
                            levelManger.StopCoroutine(levelManger.wellBackIE[thisTileController.thisTile.position]);
                        if (levelManger.callChangeWellsIE[thisTileController.thisTile.position] != null)
                            levelManger.StopCoroutine(levelManger.callChangeWellsIE[thisTileController.thisTile.position]);
                        if (levelManger.wellChangeColorIE[thisTileController.thisTile.position] != null)
                            levelManger.StopCoroutine(levelManger.wellChangeColorIE[thisTileController.thisTile.position]);
                        if (levelManger.wellFillFoamIE[thisTileController.thisTile.position] != null)
                            levelManger.StopCoroutine(levelManger.wellFillFoamIE[thisTileController.thisTile.position]);
                        if (levelManger.wellWaterIE[thisTileController.thisTile.position] != null)
                            levelManger.StopCoroutine(levelManger.wellWaterIE[thisTileController.thisTile.position]);

                        thisTileController.back.transform.localScale = thisTileController.backScale;
                        thisTileController.wellWater.transform.localScale = thisTileController.wellWaterScale;
                        thisTileController.wellWater.SetActive(false);
                        thisTileController.wellPoof.SetActive(false);
                        thisTileController.tileSR.sprite = levelManger.emptyWell;
                    }
                }
            }
        }
        foreach (SourceTileController sourceTileController in levelManger.sourceControllerScripts.Values) {
            sourceTileController.thisTile.foamDir.Clear();
        }
    }

    private void DeleteLastFoams() {
        foreach (GameObject foam in foamDict.Values) {
            Destroy(foam);
        }
        foamDict.Clear();
    }

    private void HandleRotationParam(bool isReversed)
    {
        if (isReversed)
        {
            thisTile.rotation--;
            if (thisTile.rotation < 0) {
                thisTile.rotation += numOfDirections;
            }
            thisTile.rotationCounter--;
        }
        else {
            thisTile.rotation++;
            thisTile.rotation %= numOfDirections;
            thisTile.rotationCounter++;
        }
        thisTile.rotation %= LevelManger.GetNumOfTileState(levelManger.thisMap,thisTile.branchType);
    }

    private void HandleNeighborTiles() {
        Point2D neighborConnection = Point2D.minusOne;
        TileModel neighborTile;
        TileController neighborTileController;
        SourceTileController neighborSourceTileController;
        for (int i = 0; i < numOfDirections; i++)
        {
            if (levelManger.thisMap.type == typeof(Direction))
                neighborConnection = Point2D.FromDirection(TetraTileRotator.GetDirectionFromID(i));
            else if (levelManger.thisMap.type == typeof(DirectionHex))
                neighborConnection = Point2D.FromDirection(HexTileRotator.GetDirectionFromID(i));
            neighborTile = levelManger.thisMap.GetTileByPositionAndConnection(thisTile.position, neighborConnection);
            if (neighborTile != null) {
                if (neighborTile.tileType == TileType.Standard
                    || neighborTile.tileType == TileType.Destination
                    || neighborTile.tileType == TileType.Well)
                {
                    neighborTileController = levelManger.tileControllerScripts[neighborTile.position];
                    if (neighborTile.hasWater && neighborTile.preHasWater)
                    {
                        if (neighborTile.foamDir.Contains(neighborConnection.Opposite()) &&
                        !neighborTileController.foamDict.ContainsKey(neighborConnection.Opposite()))
                        {
                            neighborTileController.AddSingleFoam(neighborConnection.Opposite());
                        }
                        else if (!neighborTile.foamDir.Contains(neighborConnection.Opposite()) &&
                        neighborTileController.foamDict.ContainsKey(neighborConnection.Opposite()))
                        {
                            neighborTileController.RemoveSingleFoam(neighborConnection.Opposite());
                        }
                    }
                }
                else if (neighborTile.tileType == TileType.Source)
                {
                    neighborSourceTileController = levelManger.sourceControllerScripts[neighborTile.position];
                    if (neighborTile.foamDir.Contains(neighborConnection.Opposite()) &&
                        !neighborSourceTileController.foamDict.ContainsKey(neighborConnection.Opposite()))
                    {
                        neighborSourceTileController.AddSingleFoam(neighborConnection.Opposite());
                    }
                    else if (!neighborTile.foamDir.Contains(neighborConnection.Opposite()) &&
                    neighborSourceTileController.foamDict.ContainsKey(neighborConnection.Opposite()))
                    {
                        neighborSourceTileController.RemoveSingleFoam(neighborConnection.Opposite());
                    }
                }
            }
        }
    }

    public IEnumerator Fill()
    {
        thisTile.preHasWater = true;
        EndOfAnimation();
        ClearTrail();
        if (thisTile.stepsFromSource > 0)
        {
            yield return new WaitForSeconds(thisTile.stepsFromSource * levelManger.delayOfEachStep);
        }
        StartFillFoam();
        SetEnabledTrail(true);
        foreach (Point2D conn in thisTile.RotatedConnections) {
            TileModel neighborTile = levelManger.thisMap.GetTileByPositionAndConnection(thisTile.position, conn);
            TileController neighborTileController;
            SourceTileController neighborSourceTileController;
            bool tileIsConnected = false;
            if (neighborTile != null) {
                if (neighborTile.tileType == TileType.Standard
                    || neighborTile.tileType == TileType.Destination
                    || neighborTile.tileType == TileType.Well)
                {
                    neighborTileController = levelManger.tileControllerScripts[neighborTile.position];
                    if (neighborTileController.thisTile.RotatedConnections.Contains(conn.Opposite())
                        && neighborTileController.thisTile.hasWater
                        && (neighborTileController.thisTile.stepsFromSource < thisTile.stepsFromSource
                        || (thisTile.stepsFromSource == -1 && neighborTileController.thisTile.stepsFromSource == -1))) {
                        tileIsConnected = true;
                    }
                }
                else if (neighborTile.tileType == TileType.Source) {
                    neighborSourceTileController = levelManger.sourceControllerScripts[neighborTile.position];
                    if (neighborSourceTileController.thisTile.RotatedConnections.Contains(conn.Opposite())
                        && neighborSourceTileController.thisTile.hasWater)
                    {
                        tileIsConnected = true;
                    }
                }
            }
            int dirNum = 0;
            if (tileIsConnected)
            {
                if (levelManger.thisMap.type == typeof(Direction))
                {
                    dirNum = (TetraTileRotator.GetIDFromDirection(conn.ToDirection())
                        - thisTile.rotation + numOfDirections) % numOfDirections;
                }
                else if (levelManger.thisMap.type == typeof(DirectionHex))
                {
                    dirNum = (HexTileRotator.GetIDFromDirection(conn.ToDirection<DirectionHex>())
                        - thisTile.rotation + numOfDirections) % numOfDirections;
                }
                dirNum = CorrectDirectionNum(dirNum,thisTile,levelManger.thisMap,thisTile.branchType);
                entries["Entry" + dirNum].Fill();
            }
        }
        yield return new WaitForSeconds(levelManger.makePipesBlueDelay);
        if (thisTile.hasWater)
        {
            tileSR.color = ZColor.WaterBlue;
        }
        ClearTrail();
    }

    public void Empty()
    {
        thisTile.preHasWater = false;
        tileSR.color = ZColor.NoWaterBlue;
        EmptyFoam();
        SetEnabledTrail(true);
        foreach (TileEntryController entry in entries.Values)
        {
            StartCoroutine(entry.Empty());
        }
    }

    public IEnumerator FillFoam() {
        foreach (Point2D foamConn in thisTile.foamDir)
        {
            AddSingleFoam(foamConn);
        }
        yield return null;
    }

    // use in update neighbor's foams and fillFoam!
    public void AddSingleFoam(Point2D foamConn) {
        if (!foamDict.ContainsKey(foamConn))
        {
            GameObject foam = null;
            Vector3 foamPose;
            if (levelManger.thisMap.type == typeof(Direction))
            {
                foamPose = new Vector3(foamConn.x * levelManger.foamDist + transform.position.x,
                                        foamConn.y * levelManger.foamDist + transform.position.y, 1);
                foam = Instantiate(levelManger.foamPrefab, foamPose, Quaternion.identity, transform);
                foam.transform.eulerAngles = new Vector3(0, 0,
                        TetraTileRotator.GetIDFromDirection(foamConn.ToDirection()) * (-1) * stepDegree);
            }
            else if (levelManger.thisMap.type == typeof(DirectionHex))
            {
                foamPose = new Vector3(foamConn.x * levelManger.foamDistHex * Mathf.Sqrt(3) + transform.position.x,
                foamConn.y * levelManger.foamDistHex + transform.position.y, 1);
                foam = Instantiate(levelManger.foamPrefab, foamPose, Quaternion.identity, transform);
                foam.transform.eulerAngles = new Vector3(0, 0,
                        HexTileRotator.GetIDFromDirection(foamConn.ToDirection<DirectionHex>()) * (-1) * stepDegree);
                float foamScale = foam.transform.localScale.x;
                foam.transform.localScale = new Vector3(foamScale * levelManger.foamScaleMagnifierHex,
                    foamScale * levelManger.foamScaleMagnifierHex, 1);
            }
            foamDict.Add(foamConn, foam);
        }
    }

    // use for delete all foams of a tile
    public void EmptyFoam() {
        foreach (GameObject foam in foamDict.Values) {
            StartCoroutine(Transitions.scaleDownAndDestroy(foam, new Vector3(0.02f, 0.02f, 1), 0.1f));
        }
        foamDict.Clear();
    }

    public void RemoveSingleFoam(Point2D foamConn)
    {
        GameObject foam;
        foam = foamDict[foamConn];
        foamDict.Remove(foamConn);
        StartCoroutine(Transitions.scaleDownAndDestroy(foam, new Vector3(0.02f, 0.02f, 1), 0.1f));
    }

    private void ClearTrail() {
        if (thisTile.tileType == TileType.Standard) {
            foreach (TileEntryController entry in entries.Values)
            {
                entry.ClearTrail();
            }
        }
    }
    
    public void EndOfAnimation()
    {
        foreach (TileEntryController entry in entries.Values)
        {
            entry.EndOfAnimation();
        }
    }

    private void SetEnabledTrail(bool show)
    {
        if (thisTile.tileType == TileType.Standard)
        {
            foreach (TileEntryController entry in entries.Values)
            {
                entry.SetEnabledTrail(show);
            }
        }
    }

    public IEnumerator backBounce() {
        Vector3 backSecondScale = new Vector3(backSmallerScale,backSmallerScale,1);
        back.transform.localScale = backScale;
        yield return new WaitForSeconds(backBounceDelay);
        while (true) {
            yield return StartCoroutine(Transitions.setScale(back,backSecondScale,backBounceTime));
            yield return StartCoroutine(Transitions.setScale(back,backScale,backBounceTime));
            yield return new WaitForSeconds(backBounceDelay);
        }
    }

    public void StartBounce() {
        backBounceIE = backBounce();
        StartCoroutine(backBounceIE);
    }

    public void StopBounce() {
        StopCoroutine(backBounceIE);
    }

    public void StartFillFoam() {
        fillFoamIE = FillFoam();
        StartCoroutine(fillFoamIE);
    }

    public IEnumerator HintRotates(int hintTilesIndex) {
        List<int> numOfTileRotate = new List<int>();
        LevelManger.clickLock = false;
        StopMyCoroutines();
        for (int i=hintTilesIndex;i<levelManger.hintRandList.Count;i+=3) {
            levelManger.tileControllerScripts[levelManger.hintRandList[i]].DeleteLastFoams();
            numOfTileRotate.Add(levelManger.tileControllerScripts[levelManger.hintRandList[i]].FindHintTilesNumOfRotate());
            levelManger.tileControllerScripts[levelManger.hintRandList[i]].thisTile.rotation =
                levelManger.tileControllerScripts[levelManger.hintRandList[i]].thisTile.correctRotation;
            levelManger.tileControllerScripts[levelManger.hintRandList[i]].thisTile.rotationCounter
                += numOfTileRotate[numOfTileRotate.Count - 1];
        }
        levelManger.Energize();
        ResetHalfAnimsAndFoams();
        levelManger.Energize();
        int numOfTileRotateIndex = 0;
        for (int i = hintTilesIndex; i < levelManger.hintRandList.Count; i += 3)
        {
            levelManger.tileControllerScripts[levelManger.hintRandList[i]].ClearTrail();
            levelManger.tileControllerScripts[levelManger.hintRandList[i]].HandleNeighborTiles();
            levelManger.tileControllerScripts[levelManger.hintRandList[i]].SetEnabledTrail(false);
            if (levelManger.tileControllerScripts[levelManger.hintRandList[i]].thisTile.tileType == TileType.Standard)
            {
                StartCoroutine(Transitions.rotateZ(levelManger.tileControllerScripts[levelManger.hintRandList[i]].gameObject, numOfTileRotate[numOfTileRotateIndex] * stepDegree, true, 0.1f));
            }
            else
            {
                StartCoroutine(Transitions.rotateZ(levelManger.tileControllerScripts[levelManger.hintRandList[i]].ring, numOfTileRotate[numOfTileRotateIndex] * stepDegree, true, 0.1f));
            }
            numOfTileRotateIndex++;
            levelManger.tileControllerScripts[levelManger.hintRandList[i]].SetEnabledTrail(true);
            levelManger.tileControllerScripts[levelManger.hintRandList[i]].ClearTrail();
            if (levelManger.tileControllerScripts[levelManger.hintRandList[i]].thisTile.tileType == TileType.Well)
            {
                levelManger.tileControllerScripts[levelManger.hintRandList[i]].fillFoamIE = levelManger.tileControllerScripts[levelManger.hintRandList[i]].FillFoam();
                levelManger.tileControllerScripts[levelManger.hintRandList[i]].StartCoroutine(levelManger.tileControllerScripts[levelManger.hintRandList[i]].fillFoamIE);
            }
        }
        int sumOfMoves = 0;
        foreach (int moves in numOfTileRotate) {
            sumOfMoves += moves;
        }
        levelManger.OnlineProgress(null,false,true,sumOfMoves);
        yield return new WaitForSeconds(0.1f);
        fillAndEmptyIE = levelManger.FillOrEmptyTiles(thisTile.position);
        StartCoroutine(fillAndEmptyIE);
        greenDestinationIE = levelManger.GreenDestinations(thisTile);
        StartCoroutine(greenDestinationIE);
        checkWellsIE = levelManger.CheckWells();
        StartCoroutine(checkWellsIE);
        LevelManger.clickLock = true;
        yield return new WaitForSeconds(levelManger.timeOfFillAndEmpty);
    }

    public int FindHintTilesNumOfRotate() {
        int diff = thisTile.correctRotation - thisTile.rotation;
        if (diff < 0) {
            diff += numOfDirections;
        }
        return diff %= LevelManger.GetNumOfTileState(levelManger.thisMap,thisTile.branchType);
    }

    private int CorrectDirectionNum(int dirNum,TileModel tile,MapModel thisMap, object branchType) {
        if (thisMap.type == typeof(Direction))
        {
            if ((BranchType)branchType == BranchType.straight)
            {
                if (tile.rotationCounter/2 % 2 == 1) {
                    dirNum = (dirNum + 2) % 4;
                }
            }
            else if ((BranchType)branchType == BranchType.plusType)
            {
                dirNum = (dirNum + (tile.rotationCounter*3) % 4) % 4;
            }
        }
        else if (thisMap.type == typeof(DirectionHex))
        {
            if ((BranchTypeHex)branchType == BranchTypeHex.hex21
                || (BranchTypeHex)branchType == BranchTypeHex.hex42)
            {
                if (tile.rotationCounter / 3 % 2 == 1)
                {
                    dirNum = (dirNum + 3) % 6;
                }
            }
            else if ((BranchTypeHex)branchType == BranchTypeHex.hex33)
            {
                if (tile.rotationCounter / 2 % 3 == 1)
                {
                    dirNum = (dirNum + 4) % 6;
                }

                if(tile.rotationCounter / 2 % 3 == 2)
                {
                    dirNum = (dirNum + 2) % 6;
                }
            }
            else if ((BranchTypeHex)branchType == BranchTypeHex.hex60)
            {
                dirNum = (dirNum + (tile.rotationCounter * 5) % 6) % 6;
            }
        }
        return dirNum;
    }

}
