using System.Collections.Generic;
using UnityEngine;

public static class MapSerializer
{
    public static bool hexMapUsesZeroY;

    public static MapModel Deserialize(LevelObject serializedMap)
    {
        return Deserialize(serializedMap.hex,serializedMap.tiles, serializedMap.size);
    }

    public static MapModel Deserialize(bool hex,List<SerializedTile> tiles, Point2D size)
    {
        int id = 0;
        TileModel tileModel;
        MapModel toReturn = MapFactory.CreateMap(hex ? typeof(DirectionHex) : typeof(Direction), size);
        Point2D position;
        if (hex) {
            hexMapUsesZeroY = false;
            foreach (SerializedTile tile in tiles)
            {
                char[] separator = { 'x' };
                if (!hexMapUsesZeroY && tile.elementName.Split(separator)[1] == "0")
                    hexMapUsesZeroY = true;
                position = SetPosition(tile.elementName);
                tileModel = TileFactory.CreateTile(typeof(DirectionHex), id++, position, tile.rotation, tile.correctRotation, GetRotatedConnections(true, tile.standardTileTypeHex, tile.correctRotation), toReturn, tile.tileType, tile.standardTileTypeHex);
                toReturn.AddTile(tileModel);
            }
        }
        else {
            foreach (SerializedTile tile in tiles)
            {
                position = SetPosition(tile.elementName);
                tileModel = TileFactory.CreateTile(typeof(Direction), id++, position, tile.rotation, tile.correctRotation, GetRotatedConnections(false, tile.standardTileType, tile.correctRotation), toReturn, tile.tileType, tile.standardTileType);
                toReturn.AddTile(tileModel);
            }
        }

        return toReturn;
    }

    private static Point2D SetPosition(string stringPos)
    {
        string[] substr = stringPos.Split('x');
        Point2D pos = new Point2D(int.Parse(substr[0]),int.Parse(substr[1]));
        return pos;
    } 

    public static LevelObject Serialize(List<SerializedTile> tiles, Point2D size)
    {
        LevelObject levelObject = ScriptableObject.CreateInstance<LevelObject>();
        levelObject.size = size;
        levelObject.tiles = tiles;

        return levelObject;
    }

    public static LevelObject Serialize(MapModel map)
    {
        List<SerializedTile> tiles = new List<SerializedTile>();
        string posStr;

        for (int i = 0; i < map.size.x; i++)
        {
            for (int j = 0; j < map.size.y; j++)
            {
                Point2D position = new Point2D(i, j);
                TileModel tileModel = map.GetTile(position);

                if (tileModel != null)
                {
                    posStr = position.x + "x" + position.y;
                    tiles.Add(new SerializedTile(posStr, tileModel.rotation, tileModel.correctRotation, tileModel.tileType, tileModel.branchType));
                }
            }
        }

        return Serialize(tiles, map.size);
    }


    public static List<Point2D> GetConnections(BranchType branch)
    {
        List<Point2D> theList = new List<Point2D>();
        switch (branch)
        {
            case BranchType.singleType:
                theList.Add(Point2D.up);
                break;
            case BranchType.straight:
                theList.Add(Point2D.up);
                theList.Add(Point2D.down);
                break;
            case BranchType.lType:
                theList.Add(Point2D.up);
                theList.Add(Point2D.right);
                break;
            case BranchType.tType:
                theList.Add(Point2D.down);
                theList.Add(Point2D.right);
                theList.Add(Point2D.left);
                break;
            case BranchType.plusType:
                theList.Add(Point2D.up);
                theList.Add(Point2D.right);
                theList.Add(Point2D.down);
                theList.Add(Point2D.left);
                break;
        }
        return theList;
    }

    public static List<Point2D> GetConnectionsHex(BranchTypeHex branch)
    {
        //TODO
        List<Point2D> theList = new List<Point2D>();
        switch (branch)
        {
            case BranchTypeHex.hex10:
                theList.Add(Point2D.N);
                break;
            case BranchTypeHex.hex20:
                theList.Add(Point2D.N);
                theList.Add(Point2D.NE);
                break;
            case BranchTypeHex.hex21:
                theList.Add(Point2D.N);
                theList.Add(Point2D.S);
                break;
            case BranchTypeHex.hex22:
                theList.Add(Point2D.N);
                theList.Add(Point2D.SE);
                break;
            case BranchTypeHex.hex30:
                theList.Add(Point2D.N);
                theList.Add(Point2D.NE);
                theList.Add(Point2D.SE);
                break;
            case BranchTypeHex.hex31:
                theList.Add(Point2D.N);
                theList.Add(Point2D.NE);
                theList.Add(Point2D.S);
                break;
            case BranchTypeHex.hex31F:
                theList.Add(Point2D.N);
                theList.Add(Point2D.NW);
                theList.Add(Point2D.S);
                break;
            case BranchTypeHex.hex33:
                theList.Add(Point2D.N);
                theList.Add(Point2D.SE);
                theList.Add(Point2D.SW);
                break;
            case BranchTypeHex.hex40:
                theList.Add(Point2D.N);
                theList.Add(Point2D.NE);
                theList.Add(Point2D.SE);
                theList.Add(Point2D.S);
                break;
            case BranchTypeHex.hex41:
                theList.Add(Point2D.N);
                theList.Add(Point2D.NE);
                theList.Add(Point2D.SE);
                theList.Add(Point2D.SW);
                break;
            case BranchTypeHex.hex42:
                theList.Add(Point2D.N);
                theList.Add(Point2D.NE);
                theList.Add(Point2D.S);
                theList.Add(Point2D.SW);
                break;
            case BranchTypeHex.hex50:
                theList.Add(Point2D.N);
                theList.Add(Point2D.NE);
                theList.Add(Point2D.SE);
                theList.Add(Point2D.S);
                theList.Add(Point2D.SW);
                break;
            case BranchTypeHex.hex60:
                theList.Add(Point2D.N);
                theList.Add(Point2D.NE);
                theList.Add(Point2D.SE);
                theList.Add(Point2D.S);
                theList.Add(Point2D.SW);
                theList.Add(Point2D.NW);
                break;
        }
        return theList;
    }

    private static List<Point2D> GetRotatedConnections(bool hexa, object branch, int rotateTimes)
    {
        if (branch is BranchType)
        {
            List<Point2D> theList = GetConnections((BranchType)branch);
            List<Point2D> theRotatedList = new List<Point2D>();
            foreach (Point2D point in theList)
            {
                theRotatedList.Add(point.RotateClockWise(hexa, rotateTimes));
            }
            return theRotatedList;
        }
        else if(branch is BranchTypeHex)
        {
            List<Point2D> theList = GetConnectionsHex((BranchTypeHex)branch);
            List<Point2D> theRotatedList = new List<Point2D>();
            foreach (Point2D point in theList)
            {
                theRotatedList.Add(point.RotateClockWise(hexa, rotateTimes));
            }
            return theRotatedList;
        }
        else
        {
            return null;
        }
    }
}
