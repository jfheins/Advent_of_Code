using Core;

namespace AoC_2024.Days;

public sealed class Day_19 : BaseDay
{
    private readonly string[] _towels;
    private readonly string[] _designs;

    public Day_19()
    {
        var input = File.ReadAllLines(InputFilePath).SplitBy("");
        _towels = input[0].Single().Split(",", StringSplitOptions.TrimEntries);
        _designs = input[1].ToArray();
        
        
        AppDomain domain = AppDomain.CurrentDomain;
        // Set a timeout interval of 2 seconds.
        domain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromSeconds(2));
    }

    public override async ValueTask<string> Solve_1()
    {
        var matchCache = new Dictionary<string, bool> { { "", true } };
        var cache = matchCache.GetAlternateLookup<ReadOnlySpan<char>>();
        return _designs.Count(design => IsMatch(design, cache)).ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var countCache = new Dictionary<string, long> { { "", 1 } };
        var cache = countCache.GetAlternateLookup<ReadOnlySpan<char>>();
        return _designs.Sum(design => CountMatches(design, cache)).ToString();
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
    
    private long CountMatches(ReadOnlySpan<char> design, Dictionary<string, long>.AlternateLookup<ReadOnlySpan<char>> cache)
    {
        if (cache.ContainsKey(design))
            return cache[design];

        var matches = 0L;
        foreach (var t in _towels)
        {
            if (design.StartsWith(t)) 
                matches += CountMatches(design[t.Length..], cache);
        }
        cache[design] = matches;
        return matches;
    }
}