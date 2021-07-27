using UnityEngine;

public class MapWinnableValidator : MapValidator
{
	LevelManger gameplayModel;

	public override bool Validate(MapModel mapModel, int parameter = -1)
	{
        Energize(mapModel);
        if (!checkWinnability(mapModel))
        {
            Debug.LogWarning("MAP NOT WINNABLE!");
            return false;
        }
        return true;
	}

    private bool checkWinnability(MapModel thisMap) {
        TileModel tileModel;
        Vector2 mapSize = new Vector2(thisMap.size.x, thisMap.size.y);

        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                tileModel = thisMap.GetTile(i, j);
                if (tileModel != null)
                {
                    if (!tileModel.hasWater) {
                        return false;
                    }
                }
            }
        }

        return true;

    }

    private void Energize(MapModel thisMap)
    {
        TileModel tileModel;
        Vector2 mapSize = new Vector2(thisMap.size.x,thisMap.size.y);

        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                tileModel = thisMap.GetTile(i, j);
                if (tileModel != null)
                {
                    tileModel.Deenergize();
                }
            }
        }
        thisMap.numOfHasWaterTiles = 0;
        thisMap.fillingQueue.Clear();
        foreach (SourceTileModel source in thisMap.sourceTiles)
        {
            thisMap.fillingQueue.Enqueue(source);
            source.isAddedToQueue = true;
        }
        TileModel tile = thisMap.fillingQueue.Dequeue();
        tile.RecursiveEnergize();
    }

}
