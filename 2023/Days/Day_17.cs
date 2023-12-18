using Core;
using System.Drawing;

namespace AoC_2023.Days;

public sealed class Day_17 : BaseDay
{
    private readonly FiniteGrid2D<int> _grid;

    public Day_17()
    {
        _grid = new FiniteGrid2D<int>(File.ReadAllLines(InputFilePath).SelectArray(line => line.Select(c => c - '0')));
    }

    public override async ValueTask<string> Solve_1()
    {
        var s = new AStarSearch<(Point pos, Direction d)>(null, Expander1);
        var goal = _grid.BottomRight;
        var initial = (_grid.TopLeft, (Direction)11);
        var res = s.FindFirst(initial, p => p.pos == goal, node => node.pos.ManhattanDistTo(goal));
        return res!.Cost.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var s = new AStarSearch<(Point pos, Direction d)>(null, Expander2);
        var goal = _grid.BottomRight;
        var initial = (_grid.TopLeft, (Direction)11);
        var res = s.FindFirst(initial, p => p.pos == goal, node => node.pos.ManhattanDistTo(goal));
        return res!.Cost.ToString();
    }

    private IEnumerable<((Point pos, Direction d), float)> Expander1((Point pos, Direction d) node)
    {
        var possibleDirections = node.pos == _grid.TopLeft
            ? [Direction.Down, Direction.Right]
            : node.d.Perpendicular();

        return possibleDirections.SelectMany(d => SelectNextStates(node.pos, d, 1, 3));
    }

    private IEnumerable<((Point pos, Direction d), float)> Expander2((Point pos, Direction d) node)
    {
        var possibleDirections = node.pos == _grid.TopLeft
            ? [Direction.Down, Direction.Right]
            : node.d.Perpendicular();

        return possibleDirections.SelectMany(d => SelectNextStates(node.pos, d, 4, 10));
    }

    private IEnumerable<((Point pos, Direction d), float)> SelectNextStates(
        Point pos,
        Direction walkingDir,
        int minMove,
        int maxMove)
    {
        var line = _grid.Line(pos, walkingDir).Skip(1).Take(maxMove).CumulativeSum();
        return line.Select((cost, idx) => (cost, distance: idx + 1))
            .Skip(minMove - 1)
            .Select(t => ((pos.MoveTo(walkingDir, t.distance), d: walkingDir), (float)t.cost));
    }
}