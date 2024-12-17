using System.Drawing;
using System.Globalization;
using Core;

namespace AoC_2024.Days;

public sealed class Day_16 : BaseDay
{
    private int _maxCost;
    private FiniteGrid2D<char> _grid;
    private HashSet<Point> _pathPoints2 = [];

    public Day_16()
    {
        var input = File.ReadAllLines(InputFilePath);
        _grid = new FiniteGrid2D<char>(input);
    }

    public override async ValueTask<string> Solve_1()
    {
        var start = _grid.FindFirst('S');
        var dest = _grid.FindFirst('E');

        var d = new AStarSearch<(Point p, Direction d)>(null, Expander);
        var r = d.FindFirst((start, Direction.Right), it => it.p == dest, it => it.p.ManhattanDistTo(dest));
        _maxCost = (int)Math.Round(r!.Cost);
        _pathPoints2 = r.Steps.Select(it => it.p).ToHashSet();
        return r.Cost.ToString(CultureInfo.CurrentCulture);

        IEnumerable<((Point p, Direction d) node, float cost)> Expander((Point p, Direction d) arg)
        {
            var nextPos = arg.p.MoveTo(arg.d);
            if (_grid[nextPos] != '#')
                yield return ((nextPos, arg.d), 1);

            // turning costs 1000
            yield return ((arg.p, arg.d.TurnClockwise()), 1000);
            yield return ((arg.p, arg.d.TurnCounterClockwise()), 1000);
        }
    }

    public override async ValueTask<string> Solve_2()
    {
        var start = _grid.FindFirst('S');
        var dest = _grid.FindFirst('E');

        var allPoints = new HashSet<Point>();
        foreach (var po in _pathPoints2)
        {
            SimulateObstacle(start, dest, po, allPoints);
        }
        
        return allPoints.Count.ToString();
    }

    private void SimulateObstacle(Point start, Point dest, Point po, HashSet<Point> allPoints)
    {
        var oldV = _grid[po];
        _grid[po] = '#';
        var d = new AStarSearch<(Point p, Direction d)>(null, Expander2);
        var r = d.FindFirst((start, Direction.Right), it => it.p == dest, Heuristic, null, _maxCost + 1f);
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
        
        float Heuristic((Point p, Direction d) state)
            => state.p.ManhattanDistTo(dest);
    }

    private IEnumerable<((Point p, Direction d) node, float cost)> Expander2((Point p, Direction d) arg)
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
}