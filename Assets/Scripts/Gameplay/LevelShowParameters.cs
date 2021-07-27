using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public struct TileShowParams
{
    [Tooltip("for example: 2x3")]
    public string mapSize; // for example: 2x3
    public Vector2 leftMostPos;
}

[CreateAssetMenu(fileName = "LevelShowParameters", menuName = "Custom Assets/LevelShowParameters")]
public class LevelShowParameters : ScriptableObject
{
    public float diffToNextTile;
    public float tileScale;
    [Tooltip("use for synchronize hex destination rings and inner sprites in Destinations and source!")]
    public float destinationRingScaleMagnifier;
    [Tooltip("use for synchronize hex well rings and inner sprites!")]
    public float wellRingScaleMagnifier;
    [Tooltip("use for synchronize hex foams and tiles sprites!")]
    public float foamScaleMagnifier;
    public TileShowParams[] tileShowParams;
    public IDictionary<string,int> tileShowParamIndex = new Dictionary<string,int>();

    public void OnEnable()
    {
        for (int i=0;i<tileShowParams.Length;i++) {
            if (!tileShowParamIndex.ContainsKey(tileShowParams[i].mapSize)) {
                tileShowParamIndex.Add(tileShowParams[i].mapSize, i);
            }
        }
    }
}
