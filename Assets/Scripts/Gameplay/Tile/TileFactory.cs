using System.Collections.Generic;
using UnityEngine;

public static class TileFactory
{
    public static TileModel CreateTile(System.Type orientationType, int id, Point2D position, int rotation, int correctRotation, List<Point2D> connections, MapModel mapModel, TileType tileType,object branchType)
    {
        TileRotator tileRotator = GetTileRotator(orientationType);
        TileModel tileModel = null;

        switch (tileType)
        {
            case TileType.Destination:
                tileModel = new TileModel(orientationType,id, position, rotation, correctRotation, connections, tileRotator, mapModel, tileType,branchType);
                break;
            case TileType.Standard:
                tileModel = new TileModel(orientationType,id, position, rotation, correctRotation, connections, tileRotator, mapModel, tileType,branchType);
                break;
            case TileType.Source:
                tileModel = new SourceTileModel(orientationType, id, position, rotation, correctRotation, connections, tileRotator, mapModel,branchType);
                break;
            case TileType.Well:
                tileModel = new WellTileModel(orientationType, id, position, rotation, correctRotation, connections, tileRotator, mapModel,branchType);
                break;
            default:
                throw new System.Exception("add the tile tipe case here!");
        }

        return tileModel;
    }

    static TileRotator GetTileRotator(System.Type orientationType)
    {
        if (orientationType == typeof(Direction))
        {
            return new TetraTileRotator();
        }
        if (orientationType == typeof(DirectionHex))
        {
            return new HexTileRotator();
        }
        return null;
    }

}