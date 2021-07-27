using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

[RequireComponent(typeof(MapGeneratorWrapper))]
public class LevelBaker : MonoBehaviour
{
	public const string BAKED_LEVELS_DIR = "Assets/Resources/BakedLevels/";

	MapGeneratorWrapper generator;
	BakedLevelsContainer bakedLevelsContainer;

	int iterator;
	string assetName;
	float bakeTime;
	Tuple<int, int> levelRange;

	public void Awake()
	{
		generator = GetComponent<MapGeneratorWrapper>();
	}

	public void Bake(string name, Tuple<int, int> levelRange)
	{
		bakeTime = Time.realtimeSinceStartup;

		iterator = levelRange.Item1;
		this.levelRange = levelRange;
		generator.MapGenerated += GenerateSeed;
		bakedLevelsContainer = ScriptableObject.CreateInstance<BakedLevelsContainer>();

		bakedLevelsContainer.generatorParametersID = generator.mapGeneratorParameterBundles[0].id;
		bakedLevelsContainer.rangeFrom = levelRange.Item1;
		bakedLevelsContainer.rangeTo = levelRange.Item2;
		bakedLevelsContainer.data = new byte[4 * (levelRange.Item2 - levelRange.Item1 + 1)];
		this.assetName = name;

		GenerateSeed(null);
	}

	void GenerateSeed(MapModel mapModel, int seed = -1)
	{
		if (seed != -1)
			SaveSeed(seed);

		if (iterator >= levelRange.Item2)
		{
			OnGenerationFinished();
			return;
		}

		if (seed != -1)
			iterator++;

        if (iterator <= levelRange.Item2)
            generator.GenerateMap(iterator);
	}

	void SaveSeed(int seed)
	{
		byte[] data = BitConverter.GetBytes(seed);
		if (data.Length != 4)
			throw new System.Exception("WRONG SIZE OF BYTE ARRAY!");

		for (int i = 0; i < data.Length; i++)
		{
			bakedLevelsContainer.data[(iterator - levelRange.Item1) * 4 + i] = data[i];
		}
	}

	void OnGenerationFinished()
	{
		generator.MapGenerated -= GenerateSeed;

#if UNITY_EDITOR
        AssetDatabase.CreateAsset(bakedLevelsContainer, BAKED_LEVELS_DIR + assetName + ".asset");
        AssetDatabase.SaveAssets();
#endif

		bakeTime = Time.realtimeSinceStartup - bakeTime;
		Debug.Log("Bake completed! Took: " + bakeTime + " seconds. " + (bakeTime / (levelRange.Item2 - levelRange.Item1 + 1)) + " seconds per level.");
	}
}
