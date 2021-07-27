using System.Collections.Generic;
using System;

public class TetraTileRotator : TileRotator
{
    public override List<Point2D> GetRotatedConnections(List<Point2D> currentConnections, int rotation, int correctRotation)
    {
        List<Point2D> calculatedConnections = new List<Point2D>(currentConnections.Count);

        for (int i = 0; i < currentConnections.Count; i++)
        {
            int connectionID = GetIDFromDirection(currentConnections[i].ToDirection<Direction>());
            // + 4 is for handle - values!
            connectionID = (connectionID + rotation - correctRotation + 4) % Enum.GetNames(typeof(Direction)).Length;
            calculatedConnections.Add(Point2D.FromDirection(GetDirectionFromID(connectionID)));
        }
        return calculatedConnections;
    }

    public static int GetIDFromDirection(Direction direction)
    {
        switch(direction)
        {
            case Direction.Up:
                return 0;
            case Direction.Right:
                return 1;
            case Direction.Down:
                return 2;
            case Direction.Left:
                return 3;
        }
        return (int)default(Direction);
    }

    public static Direction GetDirectionFromID(int id)
    {
        switch(id % Enum.GetNames(typeof(Direction)).Length)
        {
            case 0:
                return Direction.Up;
            case 1:
                return Direction.Right;
            case 2:
                return Direction.Down;
            case 3:
                return Direction.Left;
        }
        return default(int);
    }

}
