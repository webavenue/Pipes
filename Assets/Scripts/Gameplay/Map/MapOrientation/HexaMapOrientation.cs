using System;
using System.Collections.Generic;

public class HexMapOrientation : MapOrientation
{
    public override TileModel GetTileByPositionAndConnection(Point2D position, Point2D connection, MapModel mapModel)
    {
        Point2D target = GetPositionByPositionAndConnection(position, connection, mapModel);

        if (!mapModel.ValidatePosition(target)) return null;

        return mapModel.GetTile(target);
    }

    public override Point2D GetPositionByPositionAndConnection(Point2D position, Point2D connection, MapModel mapModel)
    {
        Point2D target = position + connection;

        if (mapModel.ValidatePosition(target))
            return target;

        return Point2D.minusOne;
    }

    public override List<Point2D> GetAdjacentPositions(Point2D position, MapModel mapModel)
    {
        List<Point2D> adjacentPositions = new List<Point2D>();

        for (int i = 0; i < Enum.GetNames(typeof(DirectionHex)).Length; i++)
        {
            Point2D direction = Point2D.FromDirection<DirectionHex>((DirectionHex)i);
            Point2D target = GetPositionByPositionAndConnection(position, direction, mapModel);

            if (mapModel.ValidatePosition(target))
                adjacentPositions.Add(target);
        }

        return adjacentPositions;
    }
}
