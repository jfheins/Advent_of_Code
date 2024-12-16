using System.Collections.Immutable;
using Core;
using Spectre.Console;
using System.Drawing;

namespace AoC_2024.Days;

public sealed partial class Day_16 : BaseDay
{
    private readonly string[] _input;
    private int _maxCost;
    private FiniteGrid2D<char> _grid;
    private HashSet<(Point, Direction, int)> _pathPoints;
    private HashSet<Point> _pathPoints2;

    public Day_16()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var grid = new FiniteGrid2D<char>(_input);

        var start = grid.FindFirst('S');
        var dest = grid.FindFirst('E');

        var d = new DijkstraSearch<(Point p, Direction d)>(null, Expander);
        var r = d.FindFirst((start, Direction.Right), it => it.p == dest);
        _maxCost = (int)Math.Round(r!.Cost);

        var cost = 0;
        var prevDir = Direction.Right;
        _pathPoints = new();
        _pathPoints2 = r.Steps.Select(it => it.p).ToHashSet();
        foreach (var tile in r.Steps.Skip(1))
        {
            cost += tile.d == prevDir ? 1 : 1000;
            _pathPoints.Add((tile.p, tile.d, cost));
            prevDir = tile.d;
        }
        return r.Cost.ToString();

        IEnumerable<((Point p, Direction d) node, float cost)> Expander((Point p, Direction d) arg)
        {
            var nextPos = arg.p.MoveTo(arg.d);
            if (grid[nextPos] != '#')
            {
                yield return ((nextPos, arg.d), 1);
            }

            // turning 1000
            yield return ((arg.p, arg.d.TurnClockwise()), 1000);
            yield return ((arg.p, arg.d.TurnCounterClockwise()), 1000);
        }
    }

    public override async ValueTask<string> Solve_2()
    {
        _grid = new FiniteGrid2D<char>(_input);

        var start = _grid.FindFirst('S');
        var dest = _grid.FindFirst('E');

        var allPoints = new HashSet<Point>();
        foreach (var po in _pathPoints2)
        {
            SimulateObstacle(start, dest, po, allPoints);
        }

        foreach (var pp in allPoints)
        {
            _grid[pp] = 'O';
        }

        Console.WriteLine(_grid);
        
        return allPoints.Count.ToString();
    }

    private void SimulateObstacle(Point start, Point dest, Point po, HashSet<Point> allPoints)
    {
        var oldV = _grid[po];
        _grid[po] = '#';
        var d = new DijkstraSearch<(Point p, Direction d)>(null, Expander5);
        var r = d.FindFirst((start, Direction.Right), it => it.p == dest);
        var cost = (int)Math.Round(r?.Cost ?? 0);
        
        if (cost == _maxCost)
        {
            var newPath = r!.Steps.Select(it => it.p).ToHashSet();
            newPath.ExceptWith(allPoints); // Find the detour
            allPoints.UnionWith(newPath);
            if (newPath.Count > 1)
            {
                foreach (var po2 in newPath)
                {
                    SimulateObstacle(start, dest, po2, allPoints);
                }
            }
        }

        _grid[po] = oldV;
    }
    
    IEnumerable<((Point p, Direction d) node, float cost)> Expander5((Point p, Direction d) arg)
    {
        var nextPos = arg.p.MoveTo(arg.d);
        if (_grid[nextPos] != '#')
        {
            yield return ((nextPos, arg.d), 1);
        }

        // turning 1000
        yield return ((arg.p, arg.d.TurnClockwise()), 1000);
        yield return ((arg.p, arg.d.TurnCounterClockwise()), 1000);
    }
    

    private IEnumerable<(Point p, Direction d, int cost)> Expander3((Point p, Direction d, int cost) arg)
    {
        // turning 1000
        if (arg.cost + 1000 <= _maxCost)
        {
            yield return (arg.p, arg.d.TurnClockwise(), arg.cost + 1000);
            yield return (arg.p, arg.d.TurnCounterClockwise(), arg.cost + 1000);
        }

        if (arg.cost + 1 <= _maxCost)
        {
            var nextPos = arg.p.MoveTo(arg.d);
            if (_grid[nextPos] != '#')
            {
                yield return (nextPos, arg.d, arg.cost + 1);
            }
        }
    }

    private IEnumerable<((Point p, Direction d, int cost) node, float cost)>
        Expander2((Point p, Direction d, int cost) arg)
    {
        if (arg.p == new Point(11, 11))
        {
            ;
        }
        
        // turning 1000
        if (arg.cost + 1000 <= _maxCost)
        {
            yield return ((arg.p, arg.d.TurnClockwise(), arg.cost + 1000), 1000);
            yield return ((arg.p, arg.d.TurnCounterClockwise(), arg.cost + 1000), 1000);
        }

        if (arg.cost + 1 > _maxCost)
            yield break;
        
        var nextPos = arg.p.MoveTo(arg.d);
        if (_grid[nextPos] != '#')
        {
            yield return ((nextPos, arg.d, arg.cost+1), 1);
        }
    }


    private void FindPathsRecursive(
        (Point p, Direction d) state,
        float cost,
        Point dest, List<(float c, ICollection<Point> p)> paths,
        List<Point> currentPath)
    {
        if (cost + 1 > _maxCost)
            return;

        currentPath.Add(state.p);

        if (state.p == dest)
        {
            paths.Add((cost, currentPath.ToArray()));
            Console.WriteLine("Found a path with cost " + cost);
        }

        var nextPos = state.p.MoveTo(state.d);
        var past = currentPath.IndexOf(nextPos);
        if ((past == -1 || past == currentPath.Count - 1) && _grid[nextPos] != '#')
        {
            var nextState = (nextPos, state.d);
            FindPathsRecursive(nextState, cost + 1, dest, paths, currentPath);
        }

        // turning 1000
        FindPathsRecursive((state.p, state.d.TurnClockwise()), cost + 1000, dest, paths, currentPath);
        FindPathsRecursive((state.p, state.d.TurnCounterClockwise()), cost + 1000, dest, paths, currentPath);
        currentPath.RemoveAt(currentPath.Count - 1);
    }
}

public class MyComparer : IEqualityComparer<(Point p, Direction d, ImmutableArray<int> cost)>, 
    IEqualityComparer<(Point p, Direction d, int cost)>
{
    public bool Equals((Point p, Direction d, ImmutableArray<int> cost) x,
        (Point p, Direction d, ImmutableArray<int> cost) y)
    {
        return x.Item1.Equals(y.Item1)
               && x.Item2 == y.Item2
               && x.cost.SequenceEqual(y.cost);
    }

    public int GetHashCode((Point p, Direction d, ImmutableArray<int> cost) obj)
    {
        return HashCode.Combine(obj.Item1, (int)obj.Item2, obj.Item3.LastOrDefault());
    }

    public bool Equals((Point p, Direction d, int cost) x, (Point p, Direction d, int cost) y)
    {
        return x.p == y.p && x.d == y.d && x.cost == y.cost;
    }

    public int GetHashCode((Point p, Direction d, int cost) obj)
    {
        return HashCode.Combine(obj.Item1, (int)obj.Item2, obj.Item3);
    }
}