using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

public class MapGeneratorWrapper : MonoBehaviour
{
	MapGenerator generator;

	public List<MapGeneratorParametersBundle> mapGeneratorParameterBundles;

	public UnityAction<MapModel, int> MapGenerated;

	private void Awake()
	{
		generator = new MapGenerator();
	}

	public void GenerateMap(int levelNumber, int seed = -1, int parametersID = 0)
	{
		generator.Initialize(FindParametersBundle(parametersID));
		StartCoroutine(GenerateCoroutine(levelNumber, seed, parametersID));
	}

	MapGeneratorParametersBundle FindParametersBundle(int id)
	{
		foreach (MapGeneratorParametersBundle parameters in mapGeneratorParameterBundles)
		{
			if (parameters.id == id)
				return parameters;
		}
		return mapGeneratorParameterBundles[0];
	}

	IEnumerator GenerateCoroutine(int levelNumber, int seed, int parametersID)
	{
		if (seed == -1)
		{
			MapModel mapModel = null;
			for (int i = 0; i < mapGeneratorParameterBundles[parametersID].seedsPerLevel; i++)
			{
				mapModel = generator.Generate(levelNumber * mapGeneratorParameterBundles[parametersID].seedsPerLevel + i);
				if (mapModel != null)
				{
					Debug.Log("Level number: " + levelNumber + " Iteration count: " + (i + 1) + " Seed: " + (levelNumber * mapGeneratorParameterBundles[parametersID].seedsPerLevel + i));
					if (MapGenerated != null)
						MapGenerated(mapModel, levelNumber * mapGeneratorParameterBundles[parametersID].seedsPerLevel + i);
					yield break;
				}
				yield return 0;
			}
		}
		else
		{
			if (MapGenerated != null)
				MapGenerated(generator.Generate(seed), seed);
			yield break;
		}

		throw new System.Exception("Seed per level count exceeded!");
	}
}
