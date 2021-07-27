
using System;
using System.Collections.Generic;
using UnityEngine;

public class MapModel
{
	public Point2D size;
    public TileModel[][] tiles;
    MapOrientation mapOrientation;
    public System.Type type { get; private set; }

    public List<SourceTileModel> sourceTiles { get; private set; }
    public List<TileModel> destinationTiles { get; private set; }
    public List<TileModel> wellTiles { get; private set; }

    public Queue<TileModel> fillingQueue = new Queue<TileModel>();

    public int numOfTiles;
    public int numOfHasWaterTiles;

    public MapModel(System.Type type, Point2D size, MapOrientation mapOrientation)
    {
        numOfTiles = 0;
        this.type = type;
        this.size = size;
        this.mapOrientation = mapOrientation;
        Initialize();
    }

    public void Initialize()
    {
        tiles = new TileModel[size.x][];
        for (int i = 0; i < size.x; i++)
        {
            tiles[i] = new TileModel[size.y];
        }
        sourceTiles = new List<SourceTileModel>();
        destinationTiles = new List<TileModel>();
        wellTiles = new List<TileModel>();
    }

    public void AddTile(TileModel tile)
    {
        if (tiles[tile.position.x][tile.position.y] != null)
            throw new System.Exception("Already occupied!");

        tiles[tile.position.x][tile.position.y] = tile;
        numOfTiles++;

        switch (tile.tileType)
        {
            case TileType.Destination:
                destinationTiles.Add(tile);
                break;
            case TileType.Source:
                sourceTiles.Add(tile as SourceTileModel);
                break;
            case TileType.Well:
                wellTiles.Add(tile);
                break;
        }
    }

    public TileModel GetTile(Point2D position)
    {
        return tiles[position.x][position.y];
    }

    public TileModel GetTile(int i, int j)
    {
        return GetTile(new Point2D(i, j));
    }

    public TileModel PopTile(Point2D position)
    {
        numOfTiles--;
        return PopTile(position.x, position.y);
    }

    public TileModel PopTile(int i, int j)
    {
        TileModel tile = GetTile(i, j);

        if (tile != null)
            tiles[i][j] = null;

        switch (tile.tileType)
        {
            case TileType.Destination:
                if (destinationTiles.Contains(tile))
                    destinationTiles.Remove(tile);
                break;
            case TileType.Source:
                if (sourceTiles.Contains(tile as SourceTileModel))
                    sourceTiles.Remove(tile as SourceTileModel);
                break;
            case TileType.Well:
                if (wellTiles.Contains(tile as WellTileModel))
                    wellTiles.Remove(tile as WellTileModel);
                break;
        }

        return tile;
    }

    public bool ValidatePosition(Point2D position)
    {
        if (position.x < 0 || position.x >= size.x || position.y < 0 || position.y >= size.y)
            return false;
        return true;
    }

    public List<Point2D> GetEmptyPositions()
    {
        List<Point2D> emptyPositions = new List<Point2D>();

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Point2D position = new Point2D(i, j);
                if (GetTile(position) == null)
                    emptyPositions.Add(position);
            }
        }

        return emptyPositions;
    }

    public Tuple<Point2D, Point2D> FindRealContentSizeAndShift()
    {
        int lowestX = int.MaxValue;
        int highestX = 0;
        int lowestY = int.MaxValue;
        int highestY = 0;

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if (GetTile(i, j) != null)
                {
                    if (i < lowestX)
                        lowestX = i;
                    if (i > highestX)
                        highestX = i;
                    if (j < lowestY)
                        lowestY = j;
                    if (j > highestY)
                        highestY = j;
                }
            }
        }

        return new Tuple<Point2D, Point2D>(new Point2D(highestX - lowestX + 1, highestY - lowestY + 1), new Point2D(lowestX, lowestY));
    }

    public List<Point2D> GetAdjacentPositions(Point2D position)
    {
        return mapOrientation.GetAdjacentPositions(position, this);
    }

    public TileModel GetTileByPositionAndConnection(Point2D position, Point2D connection)
    {
        return mapOrientation.GetTileByPositionAndConnection(position, connection, this);
    }

    public Point2D GetPositionByPositionAndConnection(Point2D position, Point2D connection)
    {
        return mapOrientation.GetPositionByPositionAndConnection(position, connection, this);
    }

}
