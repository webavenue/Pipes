using System.Collections.Generic;

public abstract class TileRotator
{
    public abstract List<Point2D> GetRotatedConnections(List<Point2D> currentConnections, int rotation, int correctRotation);
}
