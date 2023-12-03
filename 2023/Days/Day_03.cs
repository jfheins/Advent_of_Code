using Core;

using Spectre.Console;

using System.Drawing;
using System.Text.RegularExpressions;

namespace AoC_2023.Days;

public sealed partial class Day_03 : BaseDay
{
    private FiniteGrid2D<char> grid;

    public Day_03()
    {
        //_input = File.ReadAllLines(InputFilePath);
        grid = Grid2D.FromFile(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var numbers = new HashSet<Point>();
        foreach (var (pos, value) in grid)
        {
            var trigger = !(value == '.' || char.IsDigit(value));
            if (trigger)
            {
                foreach (var item2 in grid.Get8NeighborsOf(pos).Where(p => char.IsDigit(grid[p])))
                {
                    var numberstart = item2;
                    while (char.IsDigit(grid.GetValueOrDefault(numberstart.MoveTo(Direction.Left), '.')))
                    {
                        numberstart = numberstart.MoveTo(Direction.Left);
                    }
                    numbers.Add(numberstart);
                }
            }
        }

        var numm = new List<long>();
        foreach (var pos in numbers)
        {
            Point digitpos = pos;
            long number = grid[pos] - '0';
            while (char.IsDigit(grid.GetValueOrDefault(digitpos.MoveTo(Direction.Right), '.')))
            {
                digitpos = digitpos.MoveTo(Direction.Right);
                number = (number * 10) + (grid[digitpos] - '0');
                Console.WriteLine(number);
            }
            numm.Add(number);
            Console.WriteLine("> " + number);
        }

        return numm.Sum().ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var gears = new HashSet<(Point, Point)>();
        foreach (var (pos, value) in grid)
        {
            var trigger = value == '*';
            if (trigger)
            {
                var numneigh = new HashSet<Point>();
                foreach (var item2 in grid.Get8NeighborsOf(pos).Where(p => char.IsDigit(grid[p])))
                {
                    var numberstart = item2;
                    while (char.IsDigit(grid.GetValueOrDefault(numberstart.MoveTo(Direction.Left), '.')))
                    {
                        numberstart = numberstart.MoveTo(Direction.Left);
                    }
                    numneigh.Add(numberstart);
                }
                if (numneigh.Count == 2)
                    gears.Add(numneigh.ToTuple2());
            }
        }

        var numm = new List<long>();
        foreach (var pos in gears)
        {
            var rat = GetNumber(pos.Item1) * GetNumber(pos.Item2);
            numm.Add(rat);
        }

        return numm.Sum().ToString();

        long GetNumber(Point p)
        {
            Point digitpos = p;
            long number = grid[p] - '0';
            while (char.IsDigit(grid.GetValueOrDefault(digitpos.MoveTo(Direction.Right), '.')))
            {
                digitpos = digitpos.MoveTo(Direction.Right);
                number = (number * 10) + (grid[digitpos] - '0');
            }
            return number;
        }
    }
}