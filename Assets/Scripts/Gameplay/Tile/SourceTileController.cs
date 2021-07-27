using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceTileController : MonoBehaviour
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
    public GameObject sourceRing;
    [HideInInspector]
    public GameObject sourceBack;
    [HideInInspector]
    public IDictionary<Point2D, GameObject> foamDict = new Dictionary<Point2D, GameObject>();
    [HideInInspector]
    public Vector3 backScale;
    [HideInInspector]
    public IEnumerator backBounceIE;

    private bool rotating = false;

    public void InitFoams() {
        Vector3 foamPose;
        GameObject foam;
        if (levelManger.thisMap.type == typeof(Direction))
        {
            foreach (Point2D foamConn in thisTile.foamDir)
            {
                foamPose = new Vector3(foamConn.x * levelManger.foamDist + transform.position.x, foamConn.y * levelManger.foamDist + transform.position.y, 1);
                foam = Instantiate(levelManger.foamPrefab, foamPose, Quaternion.identity, sourceRing.transform);
                foam.transform.eulerAngles = new Vector3(0, 0, TetraTileRotator.GetIDFromDirection(foamConn.ToDirection()) * (-90));
                if (!foamDict.ContainsKey(foamConn))
                {
                    foamDict.Add(foamConn, foam);
                }
            }
        }
        else if(levelManger.thisMap.type == typeof(DirectionHex))
        {
            foreach (Point2D foamConn in thisTile.foamDir)
            {
                foamPose = new Vector3(foamConn.x * levelManger.foamDistHex * Mathf.Sqrt(3) + transform.position.x,
                    foamConn.y * levelManger.foamDistHex + transform.position.y, 1);
                foam = Instantiate(levelManger.foamPrefab, foamPose, Quaternion.identity, transform);
                foam.transform.eulerAngles = new Vector3(0, 0,
                    HexTileRotator.GetIDFromDirection(foamConn.ToDirection<DirectionHex>()) * (-60));
                float foamScale = foam.transform.localScale.x;
                foam.transform.localScale = new Vector3(foamScale * levelManger.foamScaleMagnifierHex,
                    foamScale * levelManger.foamScaleMagnifierHex, 1);
                if (!foamDict.ContainsKey(foamConn))
                {
                    foamDict.Add(foamConn, foam);
                }
            }
        }
    }

    public void Init()
    {
        sourceRing = transform.GetChild(0).gameObject;
        sourceBack = transform.GetChild(1).gameObject;
        backScale = sourceBack.transform.localScale;
        StartBounce();
    }

    public void OnMouseDown()
    {
        if (levelManger.isWin == false)
        {
            StartCoroutine(SourceErrorRotation());
        }
    }

    private IEnumerator SourceErrorRotation()
    {
        if (!rotating)
        {
            rotating = true;
            Sounds.instance.PlayError();
            VibrationManager.instance.VibrateWarning();
            yield return StartCoroutine(Transitions.ErrorRotateZ(sourceRing, 15, false, 0.1f, 4));
            rotating = false;
        }
    }

    public IEnumerator SourceBackBounce()
    {
        Vector3 backSecondScale = new Vector3(backSmallerScale, backSmallerScale, 1);
        sourceBack.transform.localScale = backScale;
        yield return new WaitForSeconds(backBounceDelay);
        while (true)
        {
            yield return StartCoroutine(Transitions.setScale(sourceBack, backSecondScale, backBounceTime));
            yield return StartCoroutine(Transitions.setScale(sourceBack, backScale, backBounceTime));
            yield return new WaitForSeconds(backBounceDelay);
        }
    }

    public void StartBounce()
    {
        backBounceIE = SourceBackBounce();
        StartCoroutine(backBounceIE);
    }

    public void StopBounce()
    {
        StopCoroutine(backBounceIE);
    }

    // use in update neighbor's foams and fillFoam!
    public void AddSingleFoam(Point2D foamConn)
    {
        GameObject foam = null;
        Vector3 foamPose;
        if (levelManger.thisMap.type == typeof(Direction))
        {
            foamPose = new Vector3(foamConn.x * levelManger.foamDist + transform.position.x,
                                    foamConn.y * levelManger.foamDist + transform.position.y, 1);
            foam = Instantiate(levelManger.foamPrefab, foamPose, Quaternion.identity, transform);
            foam.transform.eulerAngles = new Vector3(0, 0,
                    TetraTileRotator.GetIDFromDirection(foamConn.ToDirection()) * (-1) * TileController.stepDegree);
        }
        else if (levelManger.thisMap.type == typeof(DirectionHex))
        {
            foamPose = new Vector3(foamConn.x * levelManger.foamDistHex * Mathf.Sqrt(3) + transform.position.x,
            foamConn.y * levelManger.foamDistHex + transform.position.y, 1);
            foam = Instantiate(levelManger.foamPrefab, foamPose, Quaternion.identity, transform);
            foam.transform.eulerAngles = new Vector3(0, 0,
                    HexTileRotator.GetIDFromDirection(foamConn.ToDirection<DirectionHex>()) * (-1) * TileController.stepDegree);
            float foamScale = foam.transform.localScale.x;
            foam.transform.localScale = new Vector3(foamScale * levelManger.foamScaleMagnifierHex,
                foamScale * levelManger.foamScaleMagnifierHex, 1);
        }
        if (!foamDict.ContainsKey(foamConn))
        {
            foamDict.Add(foamConn, foam);
        }
    }

    public void RemoveSingleFoam(Point2D foamConn)
    {
        GameObject foam;
        foam = foamDict[foamConn];
        foamDict.Remove(foamConn);
        StartCoroutine(Transitions.scaleDownAndDestroy(foam, new Vector3(0.02f, 0.02f, 1), 0.1f));
    }

}
