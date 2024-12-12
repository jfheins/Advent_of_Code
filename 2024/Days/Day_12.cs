using Core;
using Spectre.Console;
using System.Drawing;
using Core.Combinatorics;
using Size = System.Drawing.Size;

namespace AoC_2024.Days;

public sealed partial class Day_12 : BaseDay
{
    private readonly string[] _input;
    private FiniteGrid2D<char> _grid;

    public Day_12()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var grid = new FiniteGrid2D<char>(_input);

        var visited = new HashSet<Point>();
        var price = 0L;
        foreach (var (pos, letter) in grid)
        {
            if (visited.Contains(pos))
                continue;
            var bfs = new BreadthFirstSearch<Point>(null, it => grid.Get4NeighborsOf(it).Where(x => grid[x] == letter));
            var region = bfs.FindReachable(pos);
            visited.UnionWith(region);
            price += GetPrice(region);
        }

        return price.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        _grid = new FiniteGrid2D<char>(_input);

        var visited = new HashSet<Point>();
        var price = 0L;
        foreach (var (pos, letter) in _grid)
        {
            if (visited.Contains(pos))
                continue;
            var bfs = new BreadthFirstSearch<Point>(null,
                it => _grid.Get4NeighborsOf(it).Where(x => _grid[x] == letter));
            var region = bfs.FindReachable(pos);
            visited.UnionWith(region);
            price += GetPrice2(letter, region);
        }

        return price.ToString();
    }

    private long GetPrice2(char letter, IReadOnlyList<Point> region)
    {
        var area = region.Count;

        if (region.Count == 1)
        {
            return area * (region.Count * 4L);
        }

        var allEdges = new List<Edge>();
        foreach (var p in region)
        {
            var newEdges = MakeEdges(p);
            foreach (var e in newEdges)
            {
                if (!allEdges.Remove(e.Opposite()))
                {
                    allEdges.Add(e);
                }
            }
        }
        
        var combined = false;
        do
        {
            combined = false;
            for (var i = 0; i < allEdges.Count; i++)
            {
                for (var j = 0; j < allEdges.Count; j++)
                {
                    if (i != j && allEdges[i].CanBeCombined(allEdges[j]))
                    {
                        var n = allEdges[i].Combine(allEdges[j]);
                        allEdges[i] = n;
                        allEdges.RemoveAt(j);
                        combined = true;
                        break;
                    }
                }

                if (combined)
                    break;
            }
        } while (combined);

        ;

        return area * allEdges.Count;

        Edge[] MakeEdges(Point p)
        {
            var upper = new Edge(p, p.MoveBy(1, 0));
            var right = new Edge(p.MoveBy(1, 0), p.MoveBy(1, 1));
            var lower = new Edge(p.MoveBy(1, 1), p.MoveBy(0, 1));
            var left = new Edge(p.MoveBy(0, 1), p);
            return [upper, right, lower, left];
        }
    }

    private record Edge(Point Src, Point Dest)
    {
        public Edge Opposite() => new(Dest, Src);

        public bool CanBeCombined(Edge other)
        {
            var d = new Size(Dest.Minus(Src));
            var b = new Size(other.Dest.Minus(other.Src));
            var cl = CoLinear(d, b);
            // Console.Write((other.Src == Dest && cl) + "  ");
            // Console.Write(this + "   ");
            // Console.WriteLine(other);
            
            return Dest == other.Src && cl;
            
            bool CoLinear(Size a, Size b)
                => Math.Sign(a.Height) == Math.Sign(b.Height) && Math.Sign(a.Width) == Math.Sign(b.Width);
        }

        public Edge Combine(Edge other)
        {
            return new Edge(Src, other.Dest);
        }
    }

    private static long GetPrice(IReadOnlyList<Point> region)
    {
        var perimeter = region.Count * 4L;
        var area = region.Count;

        if (region.Count > 1)
        {
            foreach (var (a, b) in new TupleCombinations2<Point>(region))
            {
                if (a.ManhattanDistTo(b) == 1)
                    perimeter -= 2;
            }
        }

        return area * perimeter;
    }
}