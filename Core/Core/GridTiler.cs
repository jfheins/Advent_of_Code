using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Core;

public class GridTiler
{
    private readonly Rectangle _bounds;
    private readonly List<Point>?[][] _points;
    private const int TileWidth = 8;
    private const int TileHeight = 8;

    public GridTiler(ICollection<Point> points, Rectangle bounds)
    {
        _bounds = bounds;

        var tileCountX = 1 + _bounds.Width / TileWidth;
        var tileCountY = 1 + _bounds.Height / TileHeight;
        
        _points = new List<Point>[tileCountX][];
        for (var x = 0; x < tileCountX; x++) 
            _points[x] = new List<Point>?[tileCountY];

        foreach (var p in points)
        {
            var tile = PointToTile(p);
            (_points[tile.x][tile.y] ??= []).Add(p);
        }
    }

    public IEnumerable<Point> GetNeighborhood(Point pos, int manhattanRadius)
    {
        var topLeft = pos.MoveBy(-manhattanRadius, -manhattanRadius);
        var topLeftTile = PointToTile(topLeft);
        
        var bottomRight = pos.MoveBy(manhattanRadius, manhattanRadius);
        var bottomRightTile = PointToTile(bottomRight);
        var result = Enumerable.Empty<Point>();
        
        for (var tileX = topLeftTile.x; tileX <= bottomRightTile.x; tileX++)
        {
            for (var tileY = topLeftTile.y; tileY <= bottomRightTile.y; tileY++) 
                result = result.Concat(GetPointsOnTile(tileX, tileY));
        }
        
        return result.Where(p => p.ManhattanDistTo(pos) <= manhattanRadius);
    }

    private (int x, int y) PointToTile(Point p)
        => ((p.X - _bounds.Left) / TileWidth, (p.Y - _bounds.Top) / TileHeight);

    public IReadOnlyCollection<Point> GetPointsOnTile(int x, int y)
    {
        IReadOnlyCollection<Point>? list = _points.ElementAtOrDefault(x)?.ElementAtOrDefault(y); 
        return list ?? Array.Empty<Point>();
    }
}