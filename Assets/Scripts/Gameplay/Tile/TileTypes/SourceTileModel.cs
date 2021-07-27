using System;
using System.Collections.Generic;

public class SourceTileModel : TileModel
{
    public bool inUse { get; private set; }

    public SourceTileModel(Type orientationType, int id, Point2D position, int rotation, int correctRotation, List<Point2D> connections,TileRotator rotator ,MapModel mapModel,object branchType) : base(orientationType, id, position, rotation, correctRotation, connections,rotator,mapModel, TileType.Source,branchType)
    {
        hasWater = true;
    }

    public void SetUsed()
    {
        inUse = true;
    }

    public override void Deenergize()
    {
        isAddedToQueue = false;
        stepsFromSource = -1;
    }
}
