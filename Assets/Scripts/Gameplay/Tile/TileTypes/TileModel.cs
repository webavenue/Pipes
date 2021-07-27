
using System.Collections.Generic;
using UnityEngine;

public class TileModel
{
    public int id;
    public Point2D position;
    public int rotation; // state of tile not the real rotation of that tile.
    public int correctRotation; // correct state of tile.
    public int rotationCounter; // use to count rotation to handle the water animations! (real rotations)
    public List<Point2D> connections { get; private set; }
    TileRotator rotator;
    public MapModel mapModel;
    public System.Type orientationType { get; private set; }
    public bool hasWater;
    public bool preHasWater;
    public TileType tileType;
    public object branchType;
    public bool isAddedToQueue;
    public List<Point2D> foamDir;
    public int stepsFromSource;

    public List<Point2D> RotatedConnections
    {
        get
        {
            return rotator.GetRotatedConnections(connections, rotation, correctRotation);
        }
    }

    public TileModel(System.Type orientationType, int id, Point2D position, int rotation, int correctRotation, List<Point2D> connections,TileRotator rotator ,MapModel mapModel, TileType tileType, object branchType)
    {
        this.orientationType = orientationType;
        this.id = id;
        this.position = position;
        this.rotation = rotation;
        this.correctRotation = correctRotation;
        rotationCounter = rotation;
        this.connections = connections == null ? new List<Point2D>() : connections;
        this.rotator = rotator;
        this.mapModel = mapModel;
        this.tileType = tileType;
        this.branchType = branchType;
        foamDir = new List<Point2D>();
    }

    public void AddConnection(Point2D connection)
    {
        foreach (Point2D conn in connections)
        {
            if (conn == connection)
                throw new System.Exception("Unable to add already existing connection!");
        }

        connections.Add(connection);
    }

    public virtual void Deenergize()
    {
        hasWater = false;
        isAddedToQueue = false;
        stepsFromSource = -1;
    }

    public virtual void RecursiveEnergize()
    {
        isAddedToQueue = true;
        hasWater = true;
        mapModel.numOfHasWaterTiles++;
        
        TileModel childTile;
        List<Point2D> rotatedConns = RotatedConnections;

        foreach (Point2D conn in rotatedConns)
        {
            childTile = mapModel.GetTileByPositionAndConnection(position, conn);
            if (childTile != null)
            {
                if (childTile.RotatedConnections.Contains(conn.Opposite()))
                {
                    if (!childTile.isAddedToQueue)
                    {
                        if (!childTile.preHasWater)
                        {
                            childTile.stepsFromSource = stepsFromSource + 1;
                        }
                        else
                        {
                            childTile.stepsFromSource = stepsFromSource;
                        }
                        mapModel.fillingQueue.Enqueue(childTile);
                        childTile.isAddedToQueue = true;
                    }
                }
                else
                {
                    foamDir.Add(conn);
                }
            }
            else {
                foamDir.Add(conn);
            }
        }
        if (mapModel.fillingQueue.Count != 0){
            TileModel tile = mapModel.fillingQueue.Dequeue();
            tile.RecursiveEnergize();
        }
    }

}
