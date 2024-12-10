using System.Drawing;
using Core;

namespace AoC_2024.Days;

public sealed class Day_10 : BaseDay
{
    private readonly FiniteGrid2D<char> _grid;

    public Day_10()
    {
        var input = File.ReadAllLines(InputFilePath);
        _grid = new FiniteGrid2D<char>(input);
    }

    public override async ValueTask<string> Solve_1()
    {
        var bfs = new BreadthFirstSearch<Point>(null, p => _grid.Get4NeighborsOf(p)
            .Where(n => _grid[n] == _grid[p] + 1)) { PerformParallelSearch = false };

        var scoreSum = _grid.Where(it => it.value == '0')
            .Select(h => bfs.FindAll(h.pos, p => _grid[p] == '9').Count)
            .Sum();
        return scoreSum.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var ratingSum = _grid.Where(it => it.value == '0').Sum(h => CalculateRating(h.pos, _grid));
        return ratingSum.ToString();
    }

    private static int CalculateRating(Point p, FiniteGrid2D<char> grid)
        => grid[p] == '9'
            ? 1
            : grid.Get4NeighborsOf(p)
                .Where(n => grid[n] == grid[p] + 1)
                .Sum(n => CalculateRating(n, grid));
}