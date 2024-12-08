using System.Drawing;
using Core;
using Core.Combinatorics;

namespace AoC_2024.Days;

public sealed class Day_08 : BaseDay
{
    private readonly FiniteGrid2D<char> _grid;

    public Day_08()
    {
        var input = File.ReadAllLines(InputFilePath);
        _grid = new FiniteGrid2D<char>(input);
    }

    public override async ValueTask<string> Solve_1()
        => Solve(2, 1);

    public override async ValueTask<string> Solve_2()
        => Solve(1, int.MaxValue);

    private string Solve(int skip, int limit)
    {
        var antennas = _grid.Where(it => char.IsAsciiLetterOrDigit(it.value))
            .ToLookup(it => it.value, it => it.pos);
        return antennas.SelectMany(freqBin => new TupleCombinations2<Point>([..freqBin]))
            .SelectMany(tuple => GetAntiNodes(tuple, skip, limit)).ToHashSet().Count.ToString();
    }

    private IEnumerable<Point> GetAntiNodes((Point, Point) pair, int skip, int limit)
    {
        var delta = new Size(pair.Item2.Minus(pair.Item1));
        return _grid.LineP(pair.Item1, delta).Skip(skip).Take(limit)
            .Concat(_grid.LineP(pair.Item2, -1 * delta).Skip(skip).Take(limit));
    }
}