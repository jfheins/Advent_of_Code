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

        var allCheats = CountAllCheats(noCheatEfforts, maxCheatDistance);
        return allCheats.Where(it => it.Key >= cheatViability).Sum(it => it.Value);
    }

    private IEnumerable<(Point node, float cost)> ExpanderNoCheat(Point arg)
    {
        return from neighbor in _grid.Get4NeighborsOf(arg) 
            where _grid[neighbor] != '#' 
            select (neighbor, 1f);
    }

    private static IReadOnlyDictionary<int, int> CountAllCheats(IReadOnlyDictionary<Point, int> noCheatEfforts, int maxCheatDistance)
    {
        var allCheats = new Dictionary<int, int>();
        foreach (var point in noCheatEfforts.Keys)
        {
            var allReachable = noCheatEfforts.Keys
                .Where(it => it.ManhattanDistTo(point) <= maxCheatDistance)
                .Select(it => (point, it, advantage: CheatAdvantage(point, it)))
                .Where(it => it.advantage > 0)
                .CountBy(it => it.advantage);

            foreach (var possibleCheat in allReachable)
            {
                allCheats.AddOrModify(possibleCheat.Key, 0, old => old + possibleCheat.Value);
            }
        }

        return allCheats;

        int CheatAdvantage(Point s, Point d)
            => noCheatEfforts[d] - noCheatEfforts[s] - s.ManhattanDistTo(d);
    }
}