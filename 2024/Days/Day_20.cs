using System.Drawing;
using Core;

namespace AoC_2024.Days;

public sealed class Day_20 : BaseDay
{
    private readonly FiniteGrid2D<char> _grid;

    public Day_20()
    {
        var input = File.ReadAllLines(InputFilePath);
        _grid = new FiniteGrid2D<char>(input);
    }

    public override async ValueTask<string> Solve_1()
    {
        return SumAllCheatsBetterThan(2, 100).ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        return SumAllCheatsBetterThan(20, 100).ToString();
    }

    private int SumAllCheatsBetterThan(int maxCheatDistance, int cheatViability)
    {
        var start = _grid.FindFirst('S');
        var dest = _grid.FindFirst('E');

        var noCheat = new DijkstraSearch<Point>(null, ExpanderNoCheat).FindFirst(start, it => it == dest)!;
        var noCheatEfforts = noCheat.Steps.Index().ToDictionary(it => it.Item, it => it.Index);

        var tiled = new GridTiler(noCheat.Steps, _grid.Bounds);
        var allCheats = CountAllCheats(noCheatEfforts, tiled, maxCheatDistance);
        return allCheats.Where(it => it.Key >= cheatViability).Sum(it => it.Value);
    }

    private IEnumerable<(Point node, float cost)> ExpanderNoCheat(Point arg)
    {
        return from neighbor in _grid.Get4NeighborsOf(arg) 
            where _grid[neighbor] != '#' 
            select (neighbor, 1f);
    }

    private static IEnumerable<KeyValuePair<int, int>> CountAllCheats(
        IReadOnlyDictionary<Point, int> noCheatEfforts,
        GridTiler tiled,
        int maxCheatDistance)
    {
        return noCheatEfforts.Keys
            .AsParallel()
            .SelectMany(point => tiled.GetNeighborhood(point, maxCheatDistance)
                .Select(it => (point, it, advantage: CheatAdvantage(point, it)))
                .Where(it => it.advantage > 0))
                .CountBy(it => it.advantage);

        int CheatAdvantage(Point s, Point d)
            => noCheatEfforts[d] - noCheatEfforts[s] - s.ManhattanDistTo(d);
    }
}
