using Core;
using System.Drawing;

namespace AoC_2023.Days;

public sealed partial class Day_17 : BaseDay
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

        foreach (var d in possibleDirections)
        {
            var line = _grid.Line(node.pos, d).Skip(1).Take(3).CumulativeSum().ToList();

            for (int i = 1; i <= 3 && i <= line.Count; i++)
            {
                var nextpos = node.pos.MoveTo(d, i);
                yield return ((nextpos, d), line[i - 1]);
            }
        }
    }

    private IEnumerable<((Point pos, Direction d), float)> Expander2((Point pos, Direction d) node)
    {
        var possibleDirections = node.pos == _grid.TopLeft
            ? [Direction.Down, Direction.Right]
            : node.d.Perpendicular();

        foreach (var d in possibleDirections)
        {
            var line = _grid.Line(node.pos, d).Skip(1).Take(10).CumulativeSum().ToList();

            for (int i = 4; i <= 10 && i <= line.Count; i++)
            {
                var nextpos = node.pos.MoveTo(d, i);
                yield return ((nextpos, d), line[i-1]);
            }
        }
    }
}