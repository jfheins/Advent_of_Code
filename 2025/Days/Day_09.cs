using Core;
using System.Drawing;
using Core.Combinatorics;

namespace AoC_2025.Days;

public sealed class Day_09 : BaseDay
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
        var polygonEdges = closedPolygon.PairwiseWithOverlap().ToList();

        var possibleRectangles = new TupleCombinations2<Point>(closedPolygon)
            .SelectList(it => MakeRect(it.Item1, it.Item2));
        possibleRectangles.Sort((a, b) => b.Area.CompareTo(a.Area)); // descending order

        var maxAllowed = possibleRectangles.AsParallel().First(rect =>
            rect.Corners.All(c => AlgebraHelpers.IsPointInPolygon(c, _input))
            && NoEdgeIntersectsPolygon(rect));

        return maxAllowed.Area.ToString();

        bool NoEdgeIntersectsPolygon(Rectangle2D r)
        {
            var intersections = from polyEdge in polygonEdges
                from rectEdge in r.GetEdges()
                select rectEdge.IntersectsLine(polyEdge.Item1, polyEdge.Item2);
            return intersections.AllEqual(false);
        }
    }

    private static Rectangle2D MakeRect(Point cornerA, Point cornerB)
        => new(
            Math.Min(cornerA.X, cornerB.X),
            Math.Min(cornerA.Y, cornerB.Y),
            Math.Abs(cornerA.X - cornerB.X) + 1,
            Math.Abs(cornerA.Y - cornerB.Y) + 1
        );
}