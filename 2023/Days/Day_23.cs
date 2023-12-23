using Core;

using Microsoft.Z3;

using Spectre.Console;

using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Xml.Linq;

namespace AoC_2023.Days;

public sealed partial class Day_23 : BaseDay
{
    private readonly string[] _input;
    private readonly FiniteGrid2D<char> _grid;

    public Day_23()
    {
        _input = File.ReadAllLines(InputFilePath);
        _grid = new FiniteGrid2D<char>(_input);
    }

    public override async ValueTask<string> Solve_1()
    {
        var start = new Point(1, 0);
        var target = _grid.BottomRight.MoveBy(-1, 0);

        var res = new List<List<Point>>();
        Dfs([], start, p => p == target, res);

        var longest = res.Max(it => it.Count);
        return (longest - 1).ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var start = new Point(1, 0);
        var target = _grid.BottomRight.MoveBy(-1, 0);
        _nodes.Add(start, []);
        _nodes.Add(target, []);
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

        var dfs = new DepthFirstSearch<Point>(null, ExpandCoarse);
        var res = dfs.FindAll(start, p => p == target);

        return res.Max(CountSteps).ToString();

        IEnumerable<Point> ExpandCoarse(Point point) => _nodes[point].Select(it => it.other);

        int CountSteps(IPath<Point> p)
        {
            var steps = 0;
            foreach (var (l, r) in p.Steps.PairwiseWithOverlap())
            {
                steps += _nodes[l].First(it => it.other == r).distance;
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

    int max = 0;
    private Dictionary<Point, List<(Point other, int distance)>> _nodes = new();

    private void Log(int c)
    {
        if (c > max)
        {
            Console.WriteLine($"Found path of l={c}");
            max = c;
        }
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
                Log(current.path.Count);
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