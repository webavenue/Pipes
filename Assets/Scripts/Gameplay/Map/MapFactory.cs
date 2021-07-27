public static class MapFactory
{
    public static MapModel CreateMap(System.Type type, Point2D size)
    {
        if (type == typeof(Direction))
            return CreateTetraMap(size);
        if (type == typeof(DirectionHex))
            return CreateHexMap(size);

        return null;
    }

    static MapModel CreateTetraMap(Point2D size)
    {
        return new MapModel(typeof(Direction), size, new TetraMapOrientation());
    }

    static MapModel CreateHexMap(Point2D size)
    {
        return new MapModel(typeof(DirectionHex), size, new TetraMapOrientation());
    }
}

