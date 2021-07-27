using UnityEngine;

[CreateAssetMenu(fileName = "MapGeneratorParametersBundle", menuName = "Badly Interrogated/MapGeneratorParametersBundle")]
public class MapGeneratorParametersBundle : ScriptableObject
{
	public byte id;
	[Space]
	public MapGeneratorParameters hexGeneratorParameters;
	public MapGeneratorParameters tetraGeneratorParameters;
	[Space]
	public float hexToTetraPreferrence;
	[Space]
	public int seedsPerLevel;
}
