using UnityEngine;
using System.Collections;

public abstract class MapValidator
{
	protected int[] parameters;

	protected MapValidator(params int[] parameters)
	{
		this.parameters = parameters;
	}

	public abstract bool Validate(MapModel mapModel, int parameter = -1);
}
