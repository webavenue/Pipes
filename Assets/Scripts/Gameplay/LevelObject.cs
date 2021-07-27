using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelObject", menuName = "Custom Assets/LevelObject")]
public class LevelObject : ScriptableObject
{
    public bool hex;
	public Point2D size;
	public List<SerializedTile> tiles;
}

[System.Serializable]
public class SerializedTile
{
	public SerializedTile(string posStr, int rotation, int correctRotation, TileType tileType,object standardTileType)
	{
        elementName = posStr;
        this.rotation = rotation;
		this.correctRotation = correctRotation;
		this.tileType = tileType;
        if (standardTileType is BranchType) {
            this.standardTileType = (BranchType)standardTileType;
        }
        else if (standardTileType is BranchTypeHex) {
            this.standardTileTypeHex = (BranchTypeHex)standardTileType;
        }
	}
    [Tooltip("Must fill by position x & y,for example: 2x3")]
    public string elementName;
	public int rotation;
	public int correctRotation;
	public TileType tileType;
    public BranchType standardTileType;
    public BranchTypeHex standardTileTypeHex;
}
