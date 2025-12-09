using Core;
using System.Drawing;
using Core.Combinatorics;

namespace AoC_2025.Days;

public sealed partial class Day_09 : BaseDay
{
    private readonly (long, long)[] _input;

    public Day_09()
    {
        _input = File.ReadAllLines(InputFilePath).SelectArray(l => l.ParseLongs().ToTuple2());
    }

    public override async ValueTask<string> Solve_1()
    {
        var rects = new TupleCombinations2<(long, long)>(_input).Select2(RectArea);
        return rects.Max().ToString();
    }

    private static long RectArea((long, long) p1, (long, long) p2)
    {
        var width = Math.Abs(p1.Item1 - p2.Item1 + 1);
        var height = Math.Abs(p1.Item2 - p2.Item2 + 1);
        return width * height;
    }

    private static long RectArea(Rectangle2D rect)
    {
        return rect.Width * (long)rect.Height;
    }

    public override async ValueTask<string> Solve_2()
    {
        var points = _input.Select2((x, y) => new Point((int)x, (int)y)).ToList();

        var closedPolygon = points.Append(points[0]).ToList();
        var pointCache = new Dictionary<Point, bool>();

        var rects = new List<Rectangle2D>();

        var allCornerCombis = new TupleCombinations2<Point>(closedPolygon).ToList();
        var i = 0;
        foreach (var (tl, br) in allCornerCombis)
        {
            i++;
            var rect = new Rectangle2D(
                Math.Min(tl.X, br.X),
                Math.Min(tl.Y, br.Y),
                Math.Abs(tl.X - br.X) + 1,
                Math.Abs(tl.Y - br.Y) + 1
            );

            if (rect.Corners.All(InPolygon))
            {
                var p = i / (float)allCornerCombis.Count;
                Console.WriteLine($"Checking {p:P2} {i}/{allCornerCombis.Count}");
                // check that all points on all edges are also in polygon
                var allEdgePointsInPolygon = rect.GetEdges()
                    .SelectMany(edge => edge)
                    .All(InPolygon);

                if (allEdgePointsInPolygon)
                {
                    rects.Add(rect);
                }
            }
        }

        return rects.Max(RectArea).ToString(); // not 4521627081
        // 4602673662 too high

        bool InPolygon(Point p)
            => pointCache.GetOrAdd(p, it => IsInPolygon(it, closedPolygon));
    }


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