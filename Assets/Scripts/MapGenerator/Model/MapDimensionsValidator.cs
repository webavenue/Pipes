using System;

public class MapDimensionsValidator : MapValidator
{
	public override bool Validate(MapModel mapModel, int parameter = -1)
	{
		Tuple<Point2D, Point2D> realSizeAndShift = mapModel.FindRealContentSizeAndShift();

		if (realSizeAndShift.Item1.x > realSizeAndShift.Item1.y)
			return false;

		return true;
	}
}
