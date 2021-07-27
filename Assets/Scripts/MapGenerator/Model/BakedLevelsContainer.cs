using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "BakedLevelsContainer", menuName = "Badly Interrogated/BakedLevelsContainer", order = 1)]
#endif
public class BakedLevelsContainer : ScriptableObject
{
	public int generatorParametersID;

	public int rangeFrom;
	public int rangeTo;

	public byte[] data;

	public int GetLevelData(int levelID)
	{
		if (levelID < rangeFrom || levelID > rangeTo)
			return -1;

		return BitConverter.ToInt32(data, (levelID - rangeFrom) * 4);
	}
}
