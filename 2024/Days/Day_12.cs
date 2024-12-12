using System.Drawing;
using Core;
using Size = System.Drawing.Size;

namespace AoC_2024.Days;

public sealed class Day_12 : BaseDay
{
    private readonly FiniteGrid2D<char> _grid;
    private IReadOnlyCollection<IReadOnlyList<Point>> _regions = [];

    public Day_12()
    {
        var input = File.ReadAllLines(InputFilePath);
        _grid = new FiniteGrid2D<char>(input);
    }

    public override async ValueTask<string> Solve_1()
    {
        _regions = FindRegions().ToList();
        return _regions.Sum(GetRegularPrice).ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        return _regions.Sum(GetDiscountedPrice).ToString();
    }

    private IEnumerable<IReadOnlyList<Point>> FindRegions()
    {
        var visited = new HashSet<Point>();
        var currentLetter = '.';
        // ReSharper disable once AccessToModifiedClosure
        var bfs = new BreadthFirstSearch<Point>(null,
            it => _grid.Get4NeighborsOf(it).Where(x => _grid[x] == currentLetter));
        foreach (var (pos, letter) in _grid)
        {
            if (visited.Contains(pos))
                continue;
            currentLetter = letter;
            var region = bfs.FindReachable(pos);
            visited.UnionWith(region);
            yield return region;
        }
    }

    private static long GetRegularPrice(IReadOnlyList<Point> region)
    {
        var perimeter = GetAllEdgesOfRegion(region).Count;
        var area = region.Count;
        return area * perimeter;
    }

    private static long GetDiscountedPrice(IReadOnlyList<Point> region)
    {
        if (region.Count == 1)
            return region.Count * 4L * region.Count;

        var edges = GetAllEdgesOfRegion(region).ToList();
        CombineCoLinearEdges(edges);

        return region.Count * edges.Count;
    }

    private static HashSet<Edge> GetAllEdgesOfRegion(IReadOnlyList<Point> region)
    {
        var allEdges = new HashSet<Edge>();
        foreach (var p in region)
        {
            var newEdges = MakeEdges(p);
            foreach (var e in newEdges)
            {
                // Opposite edges cancel each other => add only if no opposite is present
                if (!allEdges.Remove(e.Opposite()))
                    allEdges.Add(e);
            }
        }

        return allEdges;

        Edge[] MakeEdges(Point p)
        {
            var upper = new Edge(p, p.MoveBy(1, 0));
            var right = new Edge(p.MoveBy(1, 0), p.MoveBy(1, 1));
            var lower = new Edge(p.MoveBy(1, 1), p.MoveBy(0, 1));
            var left = new Edge(p.MoveBy(0, 1), p);
            return [upper, right, lower, left];
        }
    }
    
    private static void CombineCoLinearEdges(List<Edge> edges)
    {
        bool combined;
        do
        {
            combined = false;
            for (var i = 0; i < edges.Count; i++)
            {
                for (var j = i + 1; j < edges.Count;)
                {
                    if (edges[i].CanBeCombined(edges[j]))
                    {
                        edges[i] = edges[i].Combine(edges[j]);
                        edges.RemoveAt(j);
                        combined = true;
                    }
                    else
                        j++;
                }
            }
        } while (combined);
    }

    private record Edge(Point Src, Point Dest)
    {
        public Edge Opposite() => new(Dest, Src);

        public Size Direction => new(Dest.Minus(Src));

        public bool CanBeCombined(Edge other)
        {
            return IsCoLinear(Direction, other.Direction) && (Dest == other.Src || Src == other.Dest);

            bool IsCoLinear(Size a, Size b)
                => Math.Sign(a.Height) == Math.Sign(b.Height) && Math.Sign(a.Width) == Math.Sign(b.Width);
        }

        public Edge Combine(Edge other)
            => Dest == other.Src ? new Edge(Src, other.Dest) : new Edge(other.Src, Dest);
    }
}