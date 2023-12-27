using Core;

using Spectre.Console;

using System.Drawing;

namespace AoC_2023.Days;

public sealed partial class Day_23 : BaseDay
{
    private readonly FiniteGrid2D<char> _grid;
    private readonly Point _start;
    private readonly Point _target;
    private Dictionary<Point, List<(Point other, int distance)>> _nodes = new();

    public Day_23()
    {
        var input = File.ReadAllLines(InputFilePath);
        _grid = new FiniteGrid2D<char>(input);
        _start = new Point(1, 0);
        _target = _grid.BottomRight.MoveBy(-1, 0);
        _nodes.Add(_start, []);
        _nodes.Add(_target, []);
    }

    public override async ValueTask<string> Solve_1()
    {
        var res = new List<List<Point>>();
        Dfs([], _start, p => p == _target, res);

        var longest = res.Max(it => it.Count);
        return (longest - 1).ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        foreach (var node in _nodes.Keys)
        {
            var bfs = new DepthFirstSearch<Point>(null, ExpandUntilNode);
            var others = _nodes.Keys.ExceptFor(node).ToHashSet();
            var paths = bfs.FindAll(node, others.Contains);
            var longestPerTarget = paths.ToLookup(it => it.Target).Select(g => (g.Key, g.Max(p => p.Length)));

            _nodes[node].AddRange(longestPerTarget);

            IEnumerable<Point> ExpandUntilNode(Point point) 
                => _nodes.ContainsKey(point) && point != node 
                ? Array.Empty<Point>() 
                : Expand2(point);
        }
        var nodes = _nodes.Keys.ToList();
        var neighbors = nodes.SelectList(it => _nodes[it].SelectList(x => (byte)nodes.IndexOf(x.other)));
        var targetIdx = nodes.IndexOf(_target);

        var dfs = new IndexedDepthFirstSearch(nodes.Count, i => neighbors[i]);
        var res = dfs.FindAll((byte)nodes.IndexOf(_start), p => p == targetIdx);

        return res.Max(CountSteps).ToString();

        int CountSteps(IPath<byte> p)
        {
            var steps = 0;
            foreach (var (l, r) in p.Steps.PairwiseWithOverlap())
            {
                var left = nodes[l];
                steps += _nodes[left].First(it => it.other == nodes[r]).distance;
            }
            return steps;
        }
    }

    private void Dfs(List<Point> path, Point p, Func<Point, bool> predicte, List<List<Point>> results)
    {
        path.Add(p);
        if (predicte(p))
        {
            results.Add([.. path]);
        }

        var neighbors = Expand2(p).ToList();
        neighbors.RemoveAll(n => _grid[n] == '#');
        if (neighbors.Count > 2)
        {
            _nodes[p] = new List<(Point, int)>();
        }
        if ("<>^v".Contains(_grid[p]))
        {
            var d = _grid[p].Parse();
            neighbors = [p.MoveTo(d)];
        }
        foreach (var neighbor in neighbors)
        {
            if (!path.Contains(neighbor))
                Dfs(path, neighbor, predicte, results);
        }
        path.Remove(p);
    }

    private void Dfs2(Point start, Func<Point, bool> predicate, List<List<Point>> results)
    {
        var work = new Stack<(Point p, HashSet<Point> path)>();
        work.Push((start, []));

        while (work.TryPop(out var current))
        {
            current.path.Add(current.p);
            if (predicate(current.p))
            {
                results.Add(current.path.ToList());
                continue;
            }

            var n = Expand2(current.p).ToList();
            n.RemoveAll(p => _grid[p] == '#' || current.path.Contains(p));

            foreach (var later in n.Skip(1))
            {
                work.Push((later, [.. current.path]));
            }
            if (n.Count > 0)
                work.Push((n[0], current.path));
        }
    }

    private IEnumerable<Point> Expand2(Point point)
    {
        var tile = _grid[point];
        if (tile != '#')
            return _grid.Get4NeighborsOf(point);
        else
            return Array.Empty<Point>();
    }
}