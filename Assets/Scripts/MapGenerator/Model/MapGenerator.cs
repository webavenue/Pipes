using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator
{
	MapGeneratorParametersBundle parametersBundle;
	MapGeneratorParameters parameters;
	System.Type orientationType;

	List<MapValidator> mapValidators;
	int[][] intersectionMatrix;

	MapModel generatedMap;
	int mapSizeX;
	int mapSizeY;
	int sourceCount;
	int currentID;
	int currentRamificationCount;

	public void Initialize(MapGeneratorParametersBundle parametersBundle)
	{
		this.parametersBundle = parametersBundle;
		InitializeValidators();
	}

	void InitializeValidators()
	{
		mapValidators = new List<MapValidator>();
		mapValidators.Add(new MapDimensionsValidator());
		mapValidators.Add(new MapWinnableValidator());
		mapValidators.Add(new EmptyConnectionsValidator());
		mapValidators.Add(new NotBannedSeedValidator());
	}

	public MapModel Generate(int seed)
	{
		Random.InitState(seed);
		RandomizeBaseParameters();
		InitializeStructures();
		GenerateMap();

		if (Validate(seed))
		{
			FindBranchType();
			Rotate();
			return generatedMap;
		}
		else
			return null;
	}

	void RandomizeBaseParameters()
	{
		if (Random.Range(0f, 1f) <= parametersBundle.hexToTetraPreferrence)
		{
			parameters = parametersBundle.hexGeneratorParameters;
			orientationType = typeof(DirectionHex);
		}
		else
		{
			parameters = parametersBundle.tetraGeneratorParameters;
			orientationType = typeof(Direction);
		}

		if (parameters.mapSizeXMax > parameters.mapSizeYMax)
			throw new System.Exception("Maximum map size X cannot be higher than maximum map size Y!");

		mapSizeX = Random.Range(parameters.mapSizeXMin, parameters.mapSizeXMax + 1);
		mapSizeY = Random.Range(mapSizeX, parameters.mapSizeYMax + 1);
		sourceCount = Random.Range(parameters.minSourceCount, parameters.maxSourceCount + 1);

		if (parameters.maxWellCount == 1)
			throw new System.Exception("Well count cannot be equal to 1!");

	}

	void InitializeStructures()
	{
		generatedMap = MapFactory.CreateMap(orientationType, new Point2D(mapSizeX, mapSizeY));

		intersectionMatrix = new int[mapSizeX][];
		for (int i = 0; i < mapSizeX; i++)
		{
			intersectionMatrix[i] = new int[mapSizeY];
			for (int j = 0; j < mapSizeY; j++)
			{
				intersectionMatrix[i][j] = 0;
			}
		}

		currentID = 0;
		currentRamificationCount = 0;
	}

	void GenerateMap()
	{
		for (int i = 0; i < sourceCount; i++)
		{
			Point2D sourcePosition = FindBestSourcePosition(Point2D.minusOne);
			if (sourcePosition != Point2D.minusOne)
				RecursiveGeneratePath(sourcePosition, 0);
		}
	}

	void RecursiveGeneratePath(Point2D position, int count)
	{
        bool sourceGeneration = false;

        if (count >= parameters.maxRouteLength)
		{
			FinishPath(position);
            return;
		}

		intersectionMatrix[position.x][position.y]++;

		TileModel currentTile = generatedMap.GetTile(position);
		Point2D targetPosition;
		TileModel targetTile;
		List<Tuple<Point2D, Point2D>> adjacentPositionsWithConnections;
		List<Tuple<Point2D, Point2D>> adjacentEmptyPositionsWithConnections;
		Tuple<Point2D, Point2D> chosenPositioinWithConnection;

		int branchCount = 1;

		if (currentTile == null)
		{
			currentTile = TileFactory.CreateTile(orientationType, currentID++, position, 0, 0, null, generatedMap, TileType.Source,null);
			generatedMap.AddTile(currentTile);
            sourceGeneration = true;
		}

		if (currentRamificationCount < parameters.maxRamificationCount && Random.Range(0f, 1f) < parameters.ramificationProbability)
		{
			currentRamificationCount++;
			branchCount += Random.Range(parameters.minBranchCount, parameters.maxBranchCount + 1);
		}

		//This is not too optimal, but easy to code and maintain. If performance is bad - change this as first thing
		for (int z = 0; z < branchCount; z++)
		{
			adjacentPositionsWithConnections = GetPossibleAdjacentPositionsWithConnections(currentTile);
			adjacentEmptyPositionsWithConnections = new List<Tuple<Point2D, Point2D>>();

			if (adjacentPositionsWithConnections.Count == 0)
			{
				FinishPath(position);
				return;
			}

            if (sourceGeneration)
            {
                (currentTile as SourceTileModel).SetUsed();
            }

            for (int i = 0; i < adjacentPositionsWithConnections.Count; i++)
			{
				if (generatedMap.GetTile(adjacentPositionsWithConnections[i].Item1) == null)
				{
					adjacentEmptyPositionsWithConnections.Add(adjacentPositionsWithConnections[i]);
					adjacentPositionsWithConnections.RemoveAt(i--);
				}
			}

			if (adjacentEmptyPositionsWithConnections.Count == 0 && parameters.newTilePreference == 1)
			{
				FinishPath(position);
                return;
			}

			List<Tuple<Point2D, Point2D>> positionsWithDirectionsToRandomFrom;

			if (adjacentEmptyPositionsWithConnections.Count != 0 && adjacentPositionsWithConnections.Count != 0)
				positionsWithDirectionsToRandomFrom = Random.Range(0f, 1f) <= parameters.newTilePreference ? adjacentEmptyPositionsWithConnections : adjacentPositionsWithConnections;
			else if (adjacentPositionsWithConnections.Count != 0)
				positionsWithDirectionsToRandomFrom = adjacentPositionsWithConnections;
			else if (adjacentEmptyPositionsWithConnections.Count != 0)
				positionsWithDirectionsToRandomFrom = adjacentEmptyPositionsWithConnections;
			else
				throw new System.Exception("This is very very wrong!");

			chosenPositioinWithConnection = positionsWithDirectionsToRandomFrom[Random.Range(0, positionsWithDirectionsToRandomFrom.Count)];

			currentTile.AddConnection(chosenPositioinWithConnection.Item2);
			targetPosition = generatedMap.GetPositionByPositionAndConnection(currentTile.position, chosenPositioinWithConnection.Item2);
			targetTile = generatedMap.GetTile(targetPosition);


			if (targetTile == null)
			{
				if (Random.Range(0f, 1f) <= parameters.wellProbability && generatedMap.wellTiles.Count + 2 <= parameters.maxWellCount)
				{
					Point2D wellTargetPosition = FindBestSourcePosition(targetPosition);
					if (wellTargetPosition != Point2D.minusOne)
					{
						TileModel wellTargetTile = TileFactory.CreateTile(orientationType, currentID++, wellTargetPosition, 0, 0, null, generatedMap, TileType.Well,null);
						if (GetPossibleAdjacentPositionsWithConnections(wellTargetTile).Count != 0)
						{
							generatedMap.AddTile(wellTargetTile);
							targetTile = TileFactory.CreateTile(orientationType, currentID++, targetPosition, 0, 0, null, generatedMap, TileType.Well,null);
							targetTile.AddConnection(chosenPositioinWithConnection.Item2.Opposite());
							generatedMap.AddTile(targetTile);
							RecursiveGeneratePath(wellTargetPosition, count);
							continue;
						}

					}
				}

				targetTile = TileFactory.CreateTile(orientationType, currentID++, targetPosition, 0, 0, null, generatedMap, TileType.Standard,null);
				generatedMap.AddTile(targetTile);

			}

            if (!targetTile.connections.Contains(chosenPositioinWithConnection.Item2.Opposite())) {
                targetTile.AddConnection(chosenPositioinWithConnection.Item2.Opposite());
                RecursiveGeneratePath(targetPosition, count + 1);
            }
		}
	}


	void FinishPath(Point2D position)
	{
		TileModel tile = generatedMap.GetTile(position);

		if (tile.tileType == TileType.Source && !(tile as SourceTileModel).inUse)
		{
			generatedMap.PopTile(position);
            return;
		}
		else if (tile.tileType == TileType.Well)
		{
			generatedMap.PopTile(tile.position);
			tile = TileFactory.CreateTile(orientationType, tile.id, tile.position, tile.rotation, tile.correctRotation, tile.connections, generatedMap, TileType.Destination,null);
			generatedMap.AddTile(tile);

			if (generatedMap.wellTiles.Count <= 1)
			{
				for (int i = 0; i < generatedMap.wellTiles.Count;)
				{
					TileModel t = generatedMap.wellTiles[i];
					generatedMap.PopTile(t.position);
					tile = TileFactory.CreateTile(orientationType, t.id, t.position, t.rotation, tile.correctRotation, t.connections, generatedMap, TileType.Destination,null);
					generatedMap.AddTile(tile);
				}
			}
		}
		else if (tile.tileType == TileType.Standard)
		{
			tile.tileType = TileType.Destination;
			generatedMap.destinationTiles.Add(tile);
		}
	}

	List<Tuple<Point2D, Point2D>> GetPossibleAdjacentPositionsWithConnections(TileModel tile)
	{
		List<Point2D> possibleConnections = new List<Point2D>();
		List<Tuple<Point2D, Point2D>> positionsWithConnections = new List<Tuple<Point2D, Point2D>>();

		for (int i = 0; i < System.Enum.GetNames(orientationType).Length; i++)
		{
			if (orientationType == typeof(DirectionHex))
				possibleConnections.Add(Point2D.FromDirection<DirectionHex>((DirectionHex)i));
			else if (orientationType == typeof(Direction))
				possibleConnections.Add(Point2D.FromDirection<Direction>((Direction)i));
			else
				throw new System.Exception("No such orientation type defined!");

		}

		foreach (Point2D connection in tile.connections)
		{
			possibleConnections.Remove(connection);
		}

		foreach (Point2D connection in possibleConnections)
		{
			Point2D position = generatedMap.GetPositionByPositionAndConnection(tile.position, connection);
			if (position != Point2D.minusOne)
			{
				TileModel targetTile = generatedMap.GetTile(position);

				if (targetTile == null || intersectionMatrix[position.x][position.y] < parameters.maxIntersectionCount)
					positionsWithConnections.Add(new Tuple<Point2D, Point2D>(position, connection));

			}
		}

		return positionsWithConnections;
	}

	Point2D FindBestSourcePosition(Point2D excludedPosition)
	{
		List<Point2D> emptyPositions = generatedMap.GetEmptyPositions();
		if (emptyPositions.Count == 0)
			return Point2D.minusOne;

		for (int i = 0; i < emptyPositions.Count; i++)
		{
			if (emptyPositions[i] == excludedPosition)
			{
				emptyPositions.RemoveAt(i);
				i--;
			}
		}

        if (generatedMap.type == typeof(DirectionHex)) {
            for (int i = 0; i < emptyPositions.Count; i++)
            {
                if ((emptyPositions[i].x % 2) != (emptyPositions[i].y % 2))
                {
                    emptyPositions.RemoveAt(i);
                    i--;
                }
            }
        }

		if (emptyPositions.Count == 0)
			return Point2D.minusOne;

		List<Point2D> adjacentPositions;
		List<Point2D> bestPositions = new List<Point2D>();

		foreach (Point2D emptyPosition in emptyPositions)
		{
			adjacentPositions = generatedMap.GetAdjacentPositions(emptyPosition);
			bool status = false;
			if (adjacentPositions.Count == System.Enum.GetNames(orientationType).Length)
			{
				status = true;
				foreach (Point2D adjacentPosition in adjacentPositions)
				{
					if (generatedMap.GetTile(adjacentPosition) != null)
					{
						status = false;
						break;
					}
				}
			}

			if (status)
				bestPositions.Add(emptyPosition);
		}

		if (bestPositions.Count != 0)
			return bestPositions[Random.Range(0, bestPositions.Count)];
		else
			return emptyPositions[Random.Range(0, emptyPositions.Count)];
	}

	void Rotate()
	{
		TileModel tile;

		for (int i = 0; i < generatedMap.size.x; i++)
		{
			for (int j = 0; j < generatedMap.size.y; j++)
			{
				tile = generatedMap.GetTile(i, j);
				if (tile != null)
				{
                    if (tile.tileType != TileType.Source)
                    {
                        int rotationCount = Random.Range(0, System.Enum.GetNames(orientationType).Length);
                        tile.rotation = rotationCount % LevelManger.GetNumOfTileState(generatedMap, tile.branchType);
                    }
                    else {
                        tile.rotation = tile.correctRotation;
                    }
				}
			}
		}
	}

	bool Validate(int seed)
	{
		foreach (MapValidator validator in mapValidators)
		{
			if (!validator.Validate(generatedMap, seed))
				return false;
		}

		return true;
	}

	private void FindBranchType() {
		List<int> connectionID = new List<int>();
		for (int i=0;i<generatedMap.size.x;i++) {
			for (int j = 0; j < generatedMap.size.y;j++) {
				TileModel thisTile = generatedMap.tiles[i][j];
				connectionID.Clear();
				if (thisTile != null) {
					if (generatedMap.type == typeof(Direction))
                    {
                        foreach (Point2D con in thisTile.connections)
                        {
                            connectionID.Add(TetraTileRotator.GetIDFromDirection(con.ToDirection()));
                        }
                        switch (thisTile.connections.Count) {
							case 1:
								thisTile.branchType = BranchType.singleType;
								break;
							case 2:
								switch (Math.Abs((connectionID[1] % 2) - (connectionID[0]) % 2)) {
									case 1:
										thisTile.branchType = BranchType.lType;
										break;
									case 0:
										thisTile.branchType = BranchType.straight;
										break;
									default:
										throw new System.Exception("invalid branch type!");
								}
								break;
							case 3:
								thisTile.branchType = BranchType.tType;
								break;
							case 4:
								thisTile.branchType = BranchType.plusType;
								break;
						}
					}
					else{
                        foreach (Point2D con in thisTile.connections)
                        {
                            connectionID.Add(HexTileRotator.GetIDFromDirection(con.ToDirection<DirectionHex>()));
                        }
                        switch (thisTile.connections.Count) {
							case 1:
								thisTile.branchType = BranchTypeHex.hex10;
								break;
							case 2:
								int diff = connectionID[0] - connectionID[1];
                                diff = GetAPositiveDiff(diff);
								switch (diff)
								{
									case 1:
										thisTile.branchType = BranchTypeHex.hex20;
										break;
									case 2:
										thisTile.branchType = BranchTypeHex.hex22;
										break;
									case 3:
										thisTile.branchType = BranchTypeHex.hex21;
										break;
									default:
										throw new System.Exception("invalid branch type!");
								}
								break;
							case 3:
								int sum = connectionID[0] + connectionID[1] + connectionID[2];
								if (sum % 3 == 1)
								{
									thisTile.branchType = BranchTypeHex.hex31;
								}
								else if (sum % 3 == 2)
								{
									thisTile.branchType = BranchTypeHex.hex31F;
								}
								else {
									if ((connectionID[0] % 2 ==  0 && connectionID[1] % 2 == 0 && connectionID[2] % 2 == 0) ||
										(connectionID[0] % 2 == 1 && connectionID[1] % 2 == 1 && connectionID[2] % 2 == 1)
										) {
										thisTile.branchType = BranchTypeHex.hex33;
									}
									else {
										thisTile.branchType = BranchTypeHex.hex30;
									}
								}
								break;
							case 4:
								sum = connectionID[0] + connectionID[1] + connectionID[2] + connectionID[3];
								if (sum % 2 == 1)
								{
									thisTile.branchType = BranchTypeHex.hex41;
								}
								else {
                                    int countDiff3 = 0;
                                    for (int k = 0; k < connectionID.Count; k++)
                                    {
                                        for (int t = k + 1; t < connectionID.Count; t++)
                                        {
                                            diff = connectionID[k] - connectionID[t];
                                            diff = GetAPositiveDiff(diff);
                                            if (diff == 3)
                                            {
                                                countDiff3++;
                                            }
                                        }
                                    }
                                    if (countDiff3 == 1)
                                    {
                                        thisTile.branchType = BranchTypeHex.hex40;
                                    }
                                    else {
                                        thisTile.branchType = BranchTypeHex.hex42;
                                    }
                                }
								break;
							case 5:
								thisTile.branchType = BranchTypeHex.hex50;
								break;
							case 6:
								thisTile.branchType = BranchTypeHex.hex60;
								break;
						}
					}
					thisTile.correctRotation = FindCorrectRotation(thisTile);
				}
			}
		}
	}
	private int FindCorrectRotation(TileModel tile) {
		List<Point2D> zeroRotationConList; // connections in rotation number 0!
        List<int> zeroRotationConListID = new List<int>();
        List<int> tileConListID = new List<int>();
		bool isCorrectRotation = true;
		if (generatedMap.type == typeof(Direction)) {
			zeroRotationConList = MapSerializer.GetConnections((BranchType)tile.branchType);
            foreach (Point2D con in zeroRotationConList) {
                zeroRotationConListID.Add(TetraTileRotator.GetIDFromDirection(con.ToDirection()));
            }
            foreach (Point2D con in tile.connections) {
                tileConListID.Add(TetraTileRotator.GetIDFromDirection(con.ToDirection()));
            }
		}
		else {
			zeroRotationConList = MapSerializer.GetConnectionsHex((BranchTypeHex)tile.branchType);
            foreach (Point2D con in zeroRotationConList)
            {
                zeroRotationConListID.Add(HexTileRotator.GetIDFromDirection(con.ToDirection<DirectionHex>()));
            }
            foreach (Point2D con in tile.connections)
            {
                tileConListID.Add(HexTileRotator.GetIDFromDirection(con.ToDirection<DirectionHex>()));
            }
        }
		for (int i=0;i<(generatedMap.type == typeof(Direction)?4:6);i++) {
			isCorrectRotation = true;
            foreach (int id in zeroRotationConListID)
			{
                if (!tileConListID.Contains((id+i)%(generatedMap.type == typeof(Direction) ? 4 : 6)))
                {
					isCorrectRotation = false;
                }
			}
			if (isCorrectRotation) {
				return i;
			}
		}
		throw new Exception("there is no correct rotation!");
	}

    private int GetAPositiveDiff(int diff) {
        if (Math.Abs(diff) > 3)
        {
            if (diff > 0)
            {
                diff = Math.Abs(diff - 6);
            }
            else
            {
                diff = Math.Abs(diff + 6);
            }
        }
        else
        {
            diff = Math.Abs(diff);
        }
        return diff;
    }

}