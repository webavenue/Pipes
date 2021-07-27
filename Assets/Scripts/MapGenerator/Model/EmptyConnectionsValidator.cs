using UnityEngine;

public class EmptyConnectionsValidator : MapValidator
{
	public override bool Validate(MapModel mapModel, int parameter = -1)
	{
		for (int i = 0; i < mapModel.size.x; i++)
		{
			for (int j = 0; j < mapModel.size.y; j++)
			{
				TileModel tile = mapModel.GetTile(i, j);

				if (tile != null && tile.connections.Count == 0)
				{
					Debug.LogWarning("Empty connection array in tile!");
					return false;
				}
			}
		}

		return true;
	}

}
