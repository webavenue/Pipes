using UnityEngine;

[CreateAssetMenu(fileName = "MapGeneratorParameters", menuName = "Badly Interrogated/MapGeneratorParameters")]
public class MapGeneratorParameters : ScriptableObject
{
    public bool hex;
    [Space]
    public int mapSizeXMin;
    public int mapSizeXMax;
    public int mapSizeYMax;
    [Space]
    public int minSourceCount;
    public int maxSourceCount;
    [Space]
    public int maxWellCount;
    public float wellProbability;
    [Space]
    public int maxRouteLength;
    [Space]
    public float ramificationProbability;
    public int maxRamificationCount;
    public int minBranchCount;
    public int maxBranchCount;
    [Space]
    public float newTilePreference;
    [Space]
    public int maxIntersectionCount;
}
