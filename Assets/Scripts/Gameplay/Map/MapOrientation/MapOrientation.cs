using System.Collections.Generic;

public abstract class MapOrientation
{
    public abstract TileModel GetTileByPositionAndConnection(Point2D position, Point2D connection, MapModel mapModel);
    public abstract Point2D GetPositionByPositionAndConnection(Point2D position, Point2D connection, MapModel mapModel);
    public abstract List<Point2D> GetAdjacentPositions(Point2D position, MapModel mapModel);
}
