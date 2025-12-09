using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Core;

public class Rectangle2D(Point topLeft, int width, int height)
{
    public Point TopLeft { get; } = topLeft;

    /// <summary>
    /// Point that is NOT in the rectangle any more (exclusive upper bound)
    /// </summary>
    public Point BottomRight => new(TopLeft.X + Width, TopLeft.Y + Height);
        
    public int Width { get; } = width;
    public int Height { get; } = height;
    
    public long Area => Width * (long)Height;

    public Rectangle2D(int x, int y, int width, int height)
        : this(new Point(x, y), width, height)
    {
    }

    public Point Center => new(TopLeft.X + Width / 2, TopLeft.Y + Height / 2);

    /// <summary>
    ///  Yields all corners, inclusive
    /// </summary>
    public IEnumerable<Point> Corners
    {
        get
        {
            var inclEnd = new Point(BottomRight.X - 1, BottomRight.Y - 1);
            yield return TopLeft;
            yield return new Point(inclEnd.X, TopLeft.Y);
            yield return inclEnd;
            yield return new Point(TopLeft.X, inclEnd.Y);
        }
    }

    public IEnumerable<ILineSegment2D> GetEdges()
    {
        // Inclusive bound that is still part of the rectangle
        var inclEnd = new Point(BottomRight.X - 1, BottomRight.Y - 1);
        // Top edge
        yield return new LineSegment2Dx(TopLeft.X, inclEnd.X, TopLeft.Y);
        // Bottom edge
        yield return new LineSegment2Dx(TopLeft.X, inclEnd.X, inclEnd.Y);
        // Left edge
        yield return new LineSegment2Dy(TopLeft.X, TopLeft.Y, inclEnd.Y);
        // Right edge
        yield return new LineSegment2Dy(inclEnd.X, TopLeft.Y, inclEnd.Y);
    }

    /// <summary>
    /// Checks if a point is inside the rectangle
    /// </summary>
    public bool Contains(Point p)
    {
        return (p.X >= TopLeft.X && p.X < BottomRight.X)
               && (p.Y >= TopLeft.Y && p.Y < BottomRight.Y);
    }
}

public interface ILineSegment2D : IEnumerable<Point>
{
    public Point ClosestPointTo(Point p);
}

public readonly record struct LineSegment2Dx(int MinX, int MaxX, int Y) : ILineSegment2D
{
    public Point ClosestPointTo(Point p)
    {
        if (p.X <= MinX)
            return new Point(MinX, Y);
        if (p.X >= MaxX)
            return new Point(MaxX, Y);
        return p with { Y = Y };
    }

    public IEnumerator<Point> GetEnumerator()
    {
        var y = Y;
        return Enumerable.Range(MinX, MaxX - MinX + 1)
            .Select(x => new Point(x, y))
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public readonly record struct LineSegment2Dy(int X, int MinY, int MaxY) : ILineSegment2D
{

    public Point ClosestPointTo(Point p)
    {
        if (p.Y <= MinY)
            return new Point(X, MinY);
        if (p.Y >= MaxY)
            return new Point(X, MaxY);
        return p with { X = X };
    }

    public IEnumerator<Point> GetEnumerator()
    {
        var x = X;
        return Enumerable.Range(MinY, MaxY - MinY + 1)
            .Select(y => new Point(x, y))
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}