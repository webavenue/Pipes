using System;
using UnityEngine;

public enum Direction
{
    Right = 0,
    Down = 1,
    Left = 2,
    Up = 3
}

public enum DirectionHex
{
    N = 0,
    NE = 1,
    SE = 2,
    S = 3,
    SW = 4,
    NW = 5
}

[System.Serializable]
public struct Point2D
{
    public int x;
    public int y;

    public static Point2D up = new Point2D(0, 1); 

    public static Point2D right = new Point2D(1, 0);

    public static Point2D down = new Point2D(0, -1);

    public static Point2D left = new Point2D(-1, 0);

    public static Point2D N = new Point2D(0, 2);

    public static Point2D NE = new Point2D(1, 1);

    public static Point2D SE = new Point2D(1, -1);

    public static Point2D S = new Point2D(0, -2);

    public static Point2D SW = new Point2D(-1, -1);

    public static Point2D NW = new Point2D(-1, 1);

    public static Point2D minusOne = new Point2D(-1, -1);

    public static Point2D zero = new Point2D(0, 0);

    public Point2D(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Point2D Opposite()
    {
        return new Point2D(-x, -y);
    }

    public static bool operator ==(Point2D left, Point2D right)
    {
        if (left.x == right.x && left.y == right.y)
        {
            return true;
        }
        return false;
    }

    public static bool operator !=(Point2D left, Point2D right)
    {
        if (left.x != right.x || left.y != right.y)
        {
            return true;
        }
        return false;
    }

    public static Point2D operator +(Point2D left, Point2D right)
    {
        return new Point2D(left.x + right.x, left.y + right.y);
    }

    public static Point2D operator -(Point2D left, Point2D right)
    {
        return new Point2D(left.x - right.x, left.y - right.y);
    }

    public static Point2D operator *(int left, Point2D right)
    {
        return new Point2D(left * right.x, left * right.y);
    }

    public static Point2D operator *(Point2D left, int right)
    {
        return right * left;
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }

    public override bool Equals(object obj)
    {
        return this == (Point2D)obj;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static Point2D FromDirection(Direction type)
    {
        switch (type)
        {
            case Direction.Up:
                return Point2D.up;
            case Direction.Down:
                return Point2D.down;
            case Direction.Left:
                return Point2D.left;
            case Direction.Right:
                return Point2D.right;
        }
        return Point2D.zero;
    }

    public static Point2D FromDirection<T>(T type)
    {
        if (typeof(T) == typeof(Direction))
        {
            return FromDirection((Direction)(object)type);
        }
        if (typeof(T) == typeof(DirectionHex))
        {
            switch ((DirectionHex)(object)type)
            {
                case DirectionHex.N:
                    return N;
                case DirectionHex.NE:
                    return NE;
                case DirectionHex.SE:
                    return SE;
                case DirectionHex.S:
                    return S;
                case DirectionHex.SW:
                    return SW;
                case DirectionHex.NW:
                    return NW;
            }
        }
        throw new System.ArgumentException("type must be direction enum type!");
    }

    public Direction ToDirection()
    {
        if (this == Point2D.up)
            return Direction.Up;
        else if (this == Point2D.down)
            return Direction.Down;
        else if (this == Point2D.left)
            return Direction.Left;
        else if (this == Point2D.right)
            return Direction.Right;
        else
            throw new System.Exception("Cannot convert this vector to direction!");
    }

    public T ToDirection<T>()
    {
        if (typeof(T) == typeof(Direction))
        {
            return (T)(object)(ToDirection());
        }
        if (typeof(T) == typeof(DirectionHex))
        {
            if (this == N)
                return (T)(object)DirectionHex.N;
            else if (this == NE)
                return (T)(object)DirectionHex.NE;
            else if (this == SE)
                return (T)(object)DirectionHex.SE;
            else if (this == S)
                return (T)(object)DirectionHex.S;
            else if (this == SW)
                return (T)(object)DirectionHex.SW;
            else if (this == NW)
                return (T)(object)DirectionHex.NW;
            else
                throw new System.Exception("Cannot convert this vector " + this.ToString() + " to direction!");
        }
        throw new System.Exception("Cannot convert this vector " + this.ToString() + " to direction!");
    }

    public Point2D RotateClockWise(bool hexa, int rotatetimes)
    {
        if (hexa)
        {
            DirectionHex dir = ToDirection<DirectionHex>();
            int dirNum = (int)dir;
            dirNum = (dirNum + rotatetimes) % Enum.GetNames(typeof(DirectionHex)).Length;
            return FromDirection<DirectionHex>((DirectionHex)dirNum);
        }
        else
        {
            Direction dir = ToDirection();
            int dirNum = (int)dir;
            dirNum = (dirNum + rotatetimes) % Enum.GetNames(typeof(Direction)).Length;
            return FromDirection((Direction)dirNum);
        }
    }
}