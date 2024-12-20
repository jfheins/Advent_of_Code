using Core;
using System.Linq;
using System.Drawing;

namespace AoC_2024.Days;

public sealed partial class Day_20 : BaseDay
{
    private readonly string[] _input;
    private FiniteGrid2D<char> _grid;

    public Day_20()
    {
        _input = File.ReadAllLines(InputFilePath);
        _grid = new FiniteGrid2D<char>(_input);
    }

    public override async ValueTask<string> Solve_1()
    {
        return "-";
        var start = _grid.FindFirst('S');
        var dest = _grid.FindFirst('E');

        var noCheat = new DijkstraSearch<Point>(null, ExpanderNoCheat)
            .FindFirst(start, it => it == dest)!;
        Console.WriteLine("No cheat length: " + noCheat.Length);

        var allCheats = new DijkstraSearch<(Point pos, Point? cheatLocation)>(null, ExpanderCheatOnce)
            .FindAll((start, null), it => noCheat.Steps.Contains(it.Item1));

        var bestCheatLocations = allCheats.Where(it => it.Target.cheatLocation.HasValue)
            .Where(it => CheatAdvantage(it) >= 100)
            .DistinctBy(it => it.Target.cheatLocation)
            .Count();
        
        return bestCheatLocations.ToString();
        
        
        int CheatAdvantage(IPath<(Point pos, Point? cheatLocation)> p)
        {
            var normalEffort = Array.IndexOf(noCheat.Steps, p.Target.pos);
            return normalEffort - p.Length;
        }
    }

    private IEnumerable<(Point node, float cost)> ExpanderNoCheat(Point arg)
    {
        return from neighbor in _grid.Get4NeighborsOf(arg) 
            where _grid[neighbor] != '#' 
            select (neighbor, 1f);
    }
    private IEnumerable<((Point, Point?) node, float cost)> ExpanderCheatOnce(
        (Point p, Point? cheat) arg)
    {
        if (arg.cheat.HasValue && arg.cheat.Value != arg.p)
        {
            // Second expansion after cheating => no more need
            yield break;
        }
        foreach (var neighbor in _grid.Get4NeighborsOf(arg.Item1))
        {
            var isWall = _grid[neighbor] == '#';
            if (!isWall)
            {
                yield return ((neighbor, arg.cheat), 1f);
            }
            else if (arg.cheat is null) // cheat only once
            {
                yield return ((neighbor, neighbor), 1f);
            }
        }
    }

    public override async ValueTask<string> Solve_2()
    {
        var start = _grid.FindFirst('S');
        var dest = _grid.FindFirst('E');

        var noCheat = new DijkstraSearch<Point>(null, ExpanderNoCheat)
            .FindFirst(start, it => it == dest)!;
        Console.WriteLine("No cheat length: " + noCheat.Length);
        var noCheatEfforts = noCheat.Steps.Index()
            .ToDictionary(it => it.Item, it => it.Index);

        var allCheats = new List<(Point start, Point end, int advantage)>(100_000);
        foreach (var point in noCheat.Steps)
        {
            var allReachable = noCheatEfforts.Keys
                .Where(it => it.ManhattanDistTo(point) <= 20)
                .Where(it => noCheatEfforts[it] > noCheatEfforts[point]).ToList();
            allCheats.AddRange(allReachable
                .Select(it => (point, it, CheatAdvantage(point, it)))
                .Where(it => it.Item3 > 0));
        }
        
        var bestCheatLocations = allCheats
            .Count(it => it.advantage >= 100);
        
        return bestCheatLocations.ToString();
        
        int CheatAdvantage(Point s, Point d)
        {
            var cheatLength = s.ManhattanDistTo(d);
            return noCheatEfforts[d] - noCheatEfforts[s] - cheatLength;
        }
    }
}