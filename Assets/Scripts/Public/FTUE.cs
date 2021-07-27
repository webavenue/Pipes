using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FTUE : MonoBehaviour
{
    public List<NewElement> newObjects;
    public float panelTime;
    public float dottedTileFadeInTime;
    public float dottedTileFadeOutTime;
    public Vector3 circleFinalScale;
    public GameObject dottedTetraTile;
    public GameObject dottedHexaTile;
    public GameObject introduceTextPrefab;
    public GameObject maskPanel;
    public GameObject panelCircle;
    public float unClickableTime;

    [HideInInspector]
    public bool isFaddingDottedTiles;
    [HideInInspector]
    public Text textPanelText;

    private List<IEnumerator> dottedTilesIE;
    private List<GameObject> dottedTilesList;
    private GameObject textPanel;
    private GameObject introducedElementLocal;
    private int elementIndexLocal;
    private bool hasMaskPanelLocal;

    void Awake() {
        dottedTilesIE = new List<IEnumerator>();
        dottedTilesList = new List<GameObject>();
        isFaddingDottedTiles = true;
    }

    public void DottedBackTile(IDictionary<Point2D, GameObject> tilesGO,System.Type mapType) {
        GameObject dottedTile;
        if (GameScenes.instance.levelIndex < 4) {
            foreach (GameObject tile in tilesGO.Values) {
                TileController tileController = tile.GetComponent<TileController>();
                if (tileController != null &&
                    tileController.thisTile.rotation != tileController.thisTile.correctRotation) // it should not be source type!
                {
                    if (mapType == typeof(Direction))
                    {
                        if (tileController.thisTile.tileType == TileType.Destination)
                        {
                            dottedTile = Instantiate(dottedTetraTile, tile.transform.position, Quaternion.identity, tile.transform.GetChild(0));
                        }
                        else
                        {
                            dottedTile = Instantiate(dottedTetraTile, tile.transform.position, Quaternion.identity, tile.transform);
                        }
                    }
                    else {
                        if (tileController.thisTile.tileType == TileType.Destination)
                        {
                            dottedTile = Instantiate(dottedHexaTile, tile.transform.position, Quaternion.identity, tile.transform.GetChild(0));
                        }
                        else
                        {
                            dottedTile = Instantiate(dottedHexaTile, tile.transform.position, Quaternion.identity, tile.transform);
                        }
                    }
                    dottedTilesIE.Add(FadeInAndOutDottedTiles(dottedTile));
                    dottedTilesList.Add(dottedTile);
                    StartCoroutine(dottedTilesIE[dottedTilesIE.Count - 1]);
                }
            }
        }
    }

    private IEnumerator FadeInAndOutDottedTiles(GameObject dottedTile) {
        while (isFaddingDottedTiles) {
            yield return StartCoroutine(Transitions.fadeOutSprite(dottedTile,1,dottedTileFadeOutTime));
            yield return StartCoroutine(Transitions.fadeInSprite(dottedTile, 1, dottedTileFadeInTime));
        }
    }

    public void StopDottedTiles() {
        foreach (IEnumerator IE in dottedTilesIE) {
            StopCoroutine(IE);
        }
        foreach (GameObject dottedTile in dottedTilesList) {
            dottedTile.SetActive(false);
        }
    }

    public IEnumerator IntroduceAnElement(GameObject introducedElement,int elementIndex,bool isNeedPointer,bool hasMaskPanel) {
        // element index is for element list.
        introducedElementLocal = introducedElement;
        elementIndexLocal = elementIndex;
        hasMaskPanelLocal = hasMaskPanel;
        if (hasMaskPanel) {
            StartCoroutine(Transitions.PanelAct(panelCircle, circleFinalScale, panelTime));
            StartCoroutine(Transitions.turnOnImageWithDelay(maskPanel, panelTime - 0.05f));
            yield return new WaitForSeconds(3 * panelTime / 4);
        }
        else{
            yield return null;
        }
        textPanel = Instantiate(introduceTextPrefab,newObjects[elementIndex].textPos,Quaternion.identity,transform);
        textPanelText = textPanel.transform.GetChild(0).GetComponent<Text>();
        textPanelText.text =
            LanguageManager.instance.GetTheTextByKey(newObjects[elementIndex].textKey);
        if (!newObjects[elementIndex].isPointingToUp)
        {
            Transform arrowTransform = textPanel.transform.GetChild(1);
            arrowTransform.localScale = new Vector3(arrowTransform.localScale.x, (-1)*arrowTransform.localScale.y,1);
            arrowTransform.localPosition = new Vector3(arrowTransform.localPosition.x, (-1) * arrowTransform.localPosition.y, 1); ;
        }
        if (!newObjects[elementIndex].isPointingToLeft)
        {
            Transform arrowTransform = textPanel.transform.GetChild(1);
            arrowTransform.localScale = new Vector3((-1)*arrowTransform.localScale.x, arrowTransform.localScale.y, 1);
            arrowTransform.localPosition = new Vector3((-1)*arrowTransform.localPosition.x, arrowTransform.localPosition.y, 1); ;
        }
        StartCoroutine(Transitions.scaleUp(textPanel, new Vector3(0.02f, 0.02f, 1), 0.2f));
        if (!isNeedPointer)
        {
            textPanel.transform.GetChild(1).gameObject.SetActive(false);
            if (hasMaskPanel) {
                yield return new WaitForSeconds(unClickableTime);
                maskPanel.GetComponent<Button>().enabled = true;
            }
        }
    }

    public void CallContinueAfterIntroduction() {
        StartCoroutine(ContinueAfterIntroduction());
    }

    private IEnumerator ContinueAfterIntroduction() {
        if (introducedElementLocal) {
            introducedElementLocal.SetActive(false);
            introducedElementLocal = null;
        }
        StartCoroutine(Transitions.scaleDownAndDestroy(textPanel,new Vector3(0.02f, 0.02f,1),0.2f));
        if (hasMaskPanelLocal)
        {
            yield return StartCoroutine(Transitions.fadeOut(maskPanel, 1, 0.2f));
            Destroy(maskPanel);
        }
        else {
            yield return new WaitForSeconds(0.2f);
        }
        if (elementIndexLocal == 0) {
            AnalyticsManager.instance.LogDesignEventByKey(AnalyticsManager.DesignKeys.ftue_limited_moves);
        }
    }

}

[System.Serializable]
public struct NewElement
{
    public string textKey;
    public bool isPointingToUp;
    public bool isPointingToLeft;
    public Vector3 textPos; 
}
 