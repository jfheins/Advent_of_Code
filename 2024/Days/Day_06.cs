using Core;
using Spectre.Console;
using System.Drawing;

namespace AoC_2024.Days;

public sealed partial class Day_06 : BaseDay
{
    private readonly string[] _input;

    public Day_06()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var grid = new FiniteGrid2D<char>(_input);
        var guard = grid.FindFirst('^');
        var heading = Direction.Up;
        var visited = new HashSet<Point>();
        grid[guard] = '.';
        for (int i = 0; i < 20000; i++)
        {
            visited.Add(guard);
            var next = grid.GetValueOrDefault(guard.MoveTo(heading), '+');
            if (next == '.')
            {
                guard = guard.MoveTo(heading);

            }
            else if (next == '+')
                break;
            else
                heading = heading.TurnClockwise();
        }
        foreach (Point p in visited)
            grid[p] = 'x';

        return visited.Count.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var grid = new FiniteGrid2D<char>(_input);
        var guard = grid.FindFirst('^');
        var start = guard;
        var heading = Direction.Up;
        var visited = new HashSet<(Point, Direction)>();
        var po = new HashSet<Point>();
        grid[guard] = '.';
        while (true)
        {
            visited.Add((guard, heading));
            var nextPos = guard.MoveTo(heading);
            var next = grid.GetValueOrDefault(nextPos, '~');

            if (next == '.')
            {
                // Can I place an obstacle instead?
                if (MakesLoop(guard, heading, visited, grid))
                {
                    po.Add(nextPos);
                }

                guard = nextPos;
            }
            else if (next == '~')
                break;
            else
            {
                heading = heading.TurnClockwise();
            }
        }
        po.Remove(start);

        foreach (var p in visited)
            grid[p.Item1] = 'x';

        foreach (var p in po)
            grid[p] = 'O';

        grid[start] = '^';
        Console.WriteLine(grid.ToString());

        return po.Count.ToString(); // not 703
    }

    private bool MakesLoop(Point guard, Direction d, HashSet<(Point, Direction)> visited, FiniteGrid2D<char> grid)
    {
        if (guard.Y == 123)
            ;

        var newD = d.TurnClockwise();
        foreach (var po in grid.LineP(guard, newD.ToSize()).Skip(1))
        {
            if (visited.Contains((po, newD)))
            {
                return true;
            }
            if (grid.GetValueOrDefault(po, '~') != '.')
                break;
        }
        return false;
    }
}