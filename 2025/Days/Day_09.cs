using System.Collections.Concurrent;
using Core;
using System.Drawing;
using Core.Combinatorics;

namespace AoC_2025.Days;

public sealed partial class Day_09 : BaseDay
{
    private readonly Point[] _input;

    public Day_09()
    {
        _input = File.ReadAllLines(InputFilePath).SelectArray(l => l.ParseInts().ToPoint());
    }

    public override async ValueTask<string> Solve_1()
    {
        var rects = new TupleCombinations2<Point>(_input).Select(RectArea);
        return rects.Max().ToString();
        
        static long RectArea(Point p1, Point p2)
        {
            var width = Math.Abs(p1.X - p2.X + 1);
            var height = Math.Abs(p1.Y - p2.Y + 1);
            return width * (long)height;
        }
    }

    public override async ValueTask<string> Solve_2()
    {
        var closedPolygon = _input.Append(_input[0]).ToList();
        var pointCache = new ConcurrentDictionary<Point, bool>();
        var notInPolygon = new ConcurrentBag<Point>();

        var possibleRectangles = new TupleCombinations2<Point>(closedPolygon)
            .SelectList(it => MakeRect(it.Item1, it.Item2));
        possibleRectangles.Sort((a, b) => b.Area.CompareTo(a.Area)); // descending order

        var maxAllowed = possibleRectangles.AsParallel().First(rect =>
            rect.Corners.All(InPolygon)
            && !notInPolygon.Any(rect.Contains)
            && rect.GetEdges()
                .SelectMany(edge => edge)
                .All(InPolygon));

        return maxAllowed.Area.ToString();

        bool InPolygon(Point p)
        {
            var res = pointCache.GetOrAdd(p, it => IsInPolygon(it, closedPolygon));
            if (!res) notInPolygon.Add(p);
            return res;
        }
    }

    private static Rectangle2D MakeRect(Point cornerA, Point cornerB)
        => new(
            Math.Min(cornerA.X, cornerB.X),
            Math.Min(cornerA.Y, cornerB.Y),
            Math.Abs(cornerA.X - cornerB.X) + 1,
            Math.Abs(cornerA.Y - cornerB.Y) + 1
        );


    public static bool IsInPolygon(Point point, IReadOnlyCollection<Point> polygon)
    {
        bool result = false;
        var a = polygon.Last();
        foreach (var b in polygon)
        {
            if ((b.X == point.X) && (b.Y == point.Y))
                return true;

            if ((b.Y == a.Y) && (point.Y == a.Y))
            {
                if ((a.X <= point.X) && (point.X <= b.X))
                    return true;

                if ((b.X <= point.X) && (point.X <= a.X))
                    return true;
            }

            if ((b.Y < point.Y) && (a.Y >= point.Y) || (a.Y < point.Y) && (b.Y >= point.Y))
            {
                if (b.X + (point.Y - b.Y) / (a.Y - b.Y) * (a.X - b.X) <= point.X)
                    result = !result;
            }

            a = b;
        }

        return result;
    }
}