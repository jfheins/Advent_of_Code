using Core;

using System.Diagnostics;
using System.Drawing;

namespace AoC_2023.Days;

public sealed partial class Day_03 : BaseDay
{
    private readonly FiniteGrid2D<char> grid;

    public Day_03()
    {
        grid = Grid2D.FromFile(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var numbers = new List<Point>();
        foreach (var (pos, _) in grid.Where(it => IsSymbol(it.value)))
        {
            numbers.AddRange(
                grid.Get8NeighborsOf(pos).Where(p => char.IsDigit(grid[p])));
        }

        var distinctNumbers = numbers.Select(GetNumberStart).ToHashSet();
        return distinctNumbers.Sum(GetNumberAt).ToString();

        static bool IsSymbol(char x) => x != '.' && !char.IsDigit(x);
    }

    public override async ValueTask<string> Solve_2()
    {
        var gears = new List<(Point, Point)>();
        foreach (var (pos, _) in grid.Where(it => it.value == '*'))
        {
            var neighborNumbers = grid.Get8NeighborsOf(pos)
                    .Where(p => char.IsDigit(grid[p]))
                    .Select(GetNumberStart).ToHashSet();
            if (neighborNumbers.Count == 2)
                gears.Add(neighborNumbers.ToTuple2());
        }

        return gears.Select(CalcGearRatio).Sum().ToString();

        long CalcGearRatio((Point, Point) g)
            => GetNumberAt(g.Item1) * GetNumberAt(g.Item2);
    }

    private Point GetNumberStart(Point pos)
        => grid.MoveWhile(Direction.Left, pos, char.IsDigit);

    private long GetNumberAt(Point pos)
        => grid.Line(pos, Direction.Right)
               .TakeWhile(char.IsDigit)
               .Aggregate(0L, (sum, d) => (sum * 10) + (d - '0'));
}