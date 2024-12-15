using Core;
using Spectre.Console;
using System.Drawing;
using Size = System.Drawing.Size;

namespace AoC_2024.Days;

public sealed partial class Day_15 : BaseDay
{
    private readonly IReadOnlyList<ArraySegment<string>> _input;

    public Day_15()
    {
        _input = File.ReadAllLines(InputFilePath).SplitBy("");
    }

    public override async ValueTask<string> Solve_1()
    {
        var grid = new FiniteGrid2D<char>(_input[0]);
        var moves = string.Concat(_input[1].ToArray());
        var robot = grid.FindFirst('@');

        foreach (var move in moves)
        {
            var canMove = TryMove(robot, move, grid, out var boxes);
            if (canMove)
            {
                var d = move.Parse();
                grid[robot] = '.';
                foreach (var box in boxes)
                {
                    grid[box] = '.';
                }

                robot += d.ToSize();
                foreach (var box in boxes)
                {
                    grid[box.MoveTo(d)] = 'O';
                }

                grid[robot] = '@';
            }

            // Console.WriteLine("Move " + move);
            // Console.WriteLine(grid);
        }

        var boxLocations = grid.Where(it => it.value == 'O')
            .Select(it => it.pos).Select(it => it.Y * 100 + it.X);

        return boxLocations.Sum().ToString();
    }

    private bool TryMove(Point robot, char move, FiniteGrid2D<char> grid, out HashSet<Point> boxes)
    {
        var line = grid.LineP(robot, move.Parse().ToSize()).Skip(1);
        boxes =
        [
        ];
        foreach (var p in line)
        {
            if (grid[p] == 'O')
            {
                boxes.Add(p);
            }

            if (grid[p] == '.')
            {
                return true;
            }

            if (grid[p] == '#')
            {
                return false;
            }
        }

        return false;
    }

    public override async ValueTask<string> Solve_2()
    {
        var grid = new FiniteGrid2D<char>(_input[0].Select(line => line
            .Replace("#", "##")
            .Replace("O", "[]")
            .Replace(".", "..")
            .Replace("@", "@.")
        ));
        var moves = string.Concat(_input[1].ToArray());
        var robot = grid.FindFirst('@');

        foreach (var move in moves)
        {
            var boxes = new HashSet<Point>();
            var d = move.Parse();
            var canMove = TryMove2(robot, d, grid, boxes);
            if (canMove)
            {
                var newBoxes = new List<(Point p, char c)>();
                grid[robot] = '.';
                foreach (var box in boxes)
                {
                    newBoxes.Add((box.MoveTo(d), grid[box]));
                    grid[box] = '.';
                }

                robot += d.ToSize();
                foreach (var (p, c) in newBoxes) grid[p] = c;

                grid[robot] = '@';
            }
        }

        var boxLocations = grid.Where(it => it.value == '[')
            .Select(it => it.pos).Select(it => it.Y * 100 + it.X);

        return boxLocations.Sum().ToString();
    }

    private bool TryMove2(Point robot, Direction move, FiniteGrid2D<char> grid, HashSet<Point> boxes)
    {
        var nextLocation = robot.MoveTo(move);
        var neighbor = grid[nextLocation];
        if (neighbor == '.')
        {
            return true;
        }

        if (neighbor == '#')
        {
            return false;
        }

        if (neighbor is '[' && Directions.Vertical.Contains(move))
        {
            boxes.Add(nextLocation);
            boxes.Add(nextLocation + new Size(1, 0));
            var canMove1 = TryMove2(nextLocation, move, grid, boxes);
            var canMove2 = TryMove2(nextLocation + new Size(1, 0), move, grid, boxes);
            return canMove1 && canMove2;
        }

        if (neighbor is ']'&& Directions.Vertical.Contains(move))
        {
            boxes.Add(nextLocation + new Size(-1, 0));
            boxes.Add(nextLocation);
            var canMove1 = TryMove2(nextLocation + new Size(-1, 0), move, grid, boxes);
            var canMove2 = TryMove2(nextLocation , move, grid, boxes);
            return canMove1 && canMove2;
        }

        if (neighbor is '[' or ']')
        {
            // Move horizontal
            var secondHalf = nextLocation.MoveTo(move);
            boxes.Add(nextLocation);
            boxes.Add(secondHalf);
            return TryMove2(secondHalf, move, grid, boxes);
        }

        return false;
    }
}