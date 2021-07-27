using System;
using System.Collections.Generic;

public class HexTileRotator : TileRotator
{
    public override List<Point2D> GetRotatedConnections(List<Point2D> currentConnections, int rotation,int correctRotation)
    {
        List<Point2D> calculatedConnections = new List<Point2D>(currentConnections.Count);

        for (int i = 0; i < currentConnections.Count; i++)
        {
            int connectionID = (int)currentConnections[i].ToDirection<DirectionHex>();
            // + 6 is for handle - values!
            connectionID = (connectionID + rotation - correctRotation + 6) % Enum.GetNames(typeof(DirectionHex)).Length;
            calculatedConnections.Add(Point2D.FromDirection((DirectionHex)connectionID));
        }

        return calculatedConnections;
    }

    public static int GetIDFromDirection(DirectionHex directionHex)
    {
        switch (directionHex)
        {
            case DirectionHex.N:
                return 0;
            case DirectionHex.NE:
                return 1;
            case DirectionHex.SE:
                return 2;
            case DirectionHex.S:
                return 3;
            case DirectionHex.SW:
                return 4;
            case DirectionHex.NW:
                return 5;
        }
        return (int)default(DirectionHex);
    }

    public static DirectionHex GetDirectionFromID(int id)
    {
        switch (id % Enum.GetNames(typeof(DirectionHex)).Length)
        {
            case 0:
                return DirectionHex.N;
            case 1:
                return DirectionHex.NE;
            case 2:
                return DirectionHex.SE;
            case 3:
                return DirectionHex.S;
            case 4:
                return DirectionHex.SW;
            case 5:
                return DirectionHex.NW;
        }
        return default(int);
    }

}
