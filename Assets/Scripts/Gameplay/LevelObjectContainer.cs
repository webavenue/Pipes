using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelObjectContainer", menuName = "Custom Assets/LevelObjectContainer")]
public class LevelObjectContainer : ScriptableObject
{
    public List<LevelObject> levelObjects;
}