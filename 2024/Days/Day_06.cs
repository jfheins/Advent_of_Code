using Core;
using Spectre.Console;
using System.Drawing;

namespace AoC_2024.Days;

public sealed partial class Day_06 : BaseDay
{
    private readonly string[] _input;
    private HashSet<Point> _visited;

    public Day_06()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var grid = new FiniteGrid2D<char>(_input);
        var guard = grid.FindFirst('^');
        var heading = Direction.Up;
        _visited = new HashSet<Point>();
        grid[guard] = '.';
        while (true)
        {
            _visited.Add(guard);
            var next = grid.GetValueOrDefault(guard.MoveTo(heading), '~');
            if (next == '.')
            {
                guard = guard.MoveTo(heading);

            }
            else if (next == '~')
                break;
            else
                heading = heading.TurnClockwise();
        }

        return _visited.Count.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var grid = new FiniteGrid2D<char>(_input);
        var guard = grid.FindFirst('^');
        grid[guard] = '.';
        var po = new HashSet<Point>();

        foreach (var p in _visited.ExceptFor(guard))
        {
            if (MakesLoop(grid, guard, p))
                po.Add(p);

        }

        foreach (var p in _visited)
            grid[p] = 'x';

        foreach (var p in po)
            grid[p] = 'O';

        grid[guard] = '^';
        Console.WriteLine(grid.ToString());

        return po.Count.ToString(); // not 703
    }

    private bool MakesLoop(FiniteGrid2D<char> grid, Point guard, Point po)
    {
        if (guard.Y == 6)
            ;
        var visited = new HashSet<(Point, Direction)>();
        var heading = Direction.Up;

        while (true)
        {
            visited.Add((guard, heading));
            var nextPos = guard.MoveTo(heading);

            if (visited.Contains((nextPos, heading)))
                return true;

            var next = nextPos == po ? '#' : grid.GetValueOrDefault(nextPos, '~');
            if (next == '.')
                guard = nextPos;
            else if (next == '~')
            {
                //var clone = new FiniteGrid2D<char>(grid);
                //clone[po] = 'M';
                //foreach (var p in visited)
                //    clone[p.Item1] = 'x';
                //Console.WriteLine(clone.ToString());
                return false;
            }
            else
                heading = heading.TurnClockwise();
        }
    }
}