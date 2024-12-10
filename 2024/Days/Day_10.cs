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
        var scoreSum = _grid.Where(it => it.value == '0')
            .AsParallel().Sum(it => CalculateScore(it.pos));
        return scoreSum.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var ratingSum = _grid.Where(it => it.value == '0')
            .AsParallel().Sum(h => CalculateRating(h.pos));
        return ratingSum.ToString();
    }

    private int CalculateScore(Point p)
    {
        var set = new HashSet<Point>();
        CalculateScore(p, set);
        return set.Count;
    }

    private void CalculateScore(Point p, HashSet<Point> summits)
    {
        if (_grid[p] == '9')
            summits.Add(p);
        else
            foreach (var n in _grid.Get4NeighborsOf(p).Where(n => _grid[n] == _grid[p] + 1))
                CalculateScore(n, summits);
    }

    private int CalculateRating(Point p)
        => _grid[p] == '9'
            ? 1
            : _grid.Get4NeighborsOf(p)
                .Where(n => _grid[n] == _grid[p] + 1)
                .Sum(CalculateRating);
}