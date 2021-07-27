using System;
using System.Collections.Generic;

public class WellTileModel : TileModel
{
    public WellTileModel(Type orientationType, int id, Point2D position, int rotation, int correctRotation, List<Point2D> connections, TileRotator rotator, MapModel mapModel,object branchType) : base(orientationType, id, position, rotation, correctRotation, connections, rotator, mapModel, TileType.Well,branchType)
    {
    }

    public override void RecursiveEnergize()
    {
        foreach (WellTileModel tile in mapModel.wellTiles)
        {
            if (!tile.isAddedToQueue)
            {
                mapModel.fillingQueue.Enqueue(tile);
                tile.stepsFromSource = stepsFromSource;
                tile.isAddedToQueue = true;
            }
        }
        base.RecursiveEnergize();
    }

}
