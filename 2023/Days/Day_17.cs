using Core;
using Spectre.Console;
using System.Diagnostics.CodeAnalysis;
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
        var s = new AStarSearch<(Point pos, Direction a, Direction b, Direction c)>(null, Expander);
        var goal = _grid.BottomRight;
        var initial = (_grid.TopLeft, (Direction)33, (Direction)11, (Direction)22);
        var res = s.FindFirst(initial, p => p.pos == goal, node => node.pos.ManhattanDistTo(goal));

        var total = res.Cost;
        foreach (var item in res.Steps)
        {
            _grid[item.pos] = 0;
        }
        Console.WriteLine(_grid.ToString());
        return total.ToString(); // 855 too high, 836 too low
    }

    //class MyComparer : IEqualityComparer<(Point pos, Direction a, Direction b, Direction c)>
    //{
    //    public bool Equals((Point pos, Direction a, Direction b, Direction c) x, (Point pos, Direction a, Direction b, Direction c) y)
    //    {
    //        return x.pos.Equals(y.pos) && x.a.Equals(y.a) && x.b.Equals(y.b);
    //    }

    //    public int GetHashCode([DisallowNull] (Point pos, Direction a, Direction b, Direction c) obj)
    //    {
    //        return obj.pos.GetHashCode();
    //    }
    //}

    private IEnumerable<((Point pos, Direction a, Direction b, Direction c), float)> Expander((Point pos, Direction a, Direction b, Direction c) node)
    {
        Direction? forbidden = null;
        if (node.a == node.b && node.b == node.c)
        {
            forbidden = node.a;
        }

        foreach (var d in Directions.All4)
        {
            if (d == forbidden || d == node.c.Opposite())
                continue;

            var nextpos = node.pos.MoveTo(d);
            if (_grid.Contains(nextpos))
            {
                var nextnode = (nextpos, node.b, node.c, d);
                yield return (nextnode, _grid[nextpos]);
            }
        }
    }

    public override async ValueTask<string> Solve_2()
    {
        return "-";
    }
}