using Core;

namespace AoC_2024.Days;

public sealed class Day_19 : BaseDay
{
    private readonly string[] _towels;
    private readonly string[] _designs;
    private Dictionary<string, bool> _matchCache = new();

    public Day_19()
    {
        var input = File.ReadAllLines(InputFilePath).SplitBy("");
        _towels = input[0].Single().Split(",", StringSplitOptions.TrimEntries);
        _designs = input[1].ToArray();
    }

    public override void Clear()
    {
        _matchCache.Clear();
    }

    public override async ValueTask<string> Solve_1()
    {
        _matchCache = new Dictionary<string, bool>(20_000) { { "", true } };
        var cache =_matchCache.GetAlternateLookup<ReadOnlySpan<char>>();
        return _designs.Count(design => IsMatch(design, cache)).ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var countCache = new Dictionary<string, long>(20_000) { { "", 1 } };
        var cache = countCache.GetAlternateLookup<ReadOnlySpan<char>>();
        var cache2 = _matchCache.GetAlternateLookup<ReadOnlySpan<char>>();
        return _designs.Sum(design => CountMatches(design, cache, cache2)).ToString();
    }
    
    private bool IsMatch(ReadOnlySpan<char> design, Dictionary<string, bool>.AlternateLookup<ReadOnlySpan<char>> cache)
    {
        if (cache.ContainsKey(design))
            return cache[design];

        foreach (var t in _towels)
        {
            if (design.StartsWith(t) && IsMatch(design[t.Length..], cache))
                return cache[design] = true;
        }

        return cache[design] = false;
    }
    
    private long CountMatches(ReadOnlySpan<char> design,
        Dictionary<string, long>.AlternateLookup<ReadOnlySpan<char>> cache,
        Dictionary<string, bool>.AlternateLookup<ReadOnlySpan<char>> canMatchCache)
    {
        if (cache.ContainsKey(design))
            return cache[design];

        if (IsDeadEnd(design))
            return 0L;

        var matches = 0L;
        foreach (var t in _towels)
        {
            if (design.StartsWith(t)) 
                matches += CountMatches(design[t.Length..], cache, canMatchCache);
        }
        
        return cache[design] = matches;
        
        bool IsDeadEnd(ReadOnlySpan<char> x)
            => canMatchCache.TryGetValue(x, out var m) && !m;
    }
}