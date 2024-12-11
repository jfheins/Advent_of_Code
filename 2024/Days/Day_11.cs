using Core;

namespace AoC_2024.Days;

public sealed class Day_11 : BaseDay
{
    private readonly Dictionary<(long stone, int times), long> _cache = new();
    private readonly int[] _stones;

    public Day_11()
    {
        var input = File.ReadAllText(InputFilePath);
        _stones = input.ParseInts();
    }

    public override void Clear() => _cache.Clear();

    public override async ValueTask<string> Solve_1()
    {
        return Solve(25).ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        return Solve(75).ToString();
    }

    private long Solve(int blinks)
        => _stones.Sum(it => EvolveRecursive(it, blinks));


    private long EvolveRecursive(long stone, int times)
    {
        if (times == 0)
            return 1;
        if (_cache.TryGetValue((stone, times), out var value))
            return value;

        var evolved = Evolve(stone);
        var result = evolved.Item2 == null
            ? EvolveRecursive(evolved.Item1, times - 1) 
            : EvolveRecursive(evolved.Item1, times - 1) + EvolveRecursive(evolved.Item2.Value, times - 1);
        
        _cache[(stone, times)] = result;
        return result;
    }

    private static (long, long?) Evolve(long stone)
    {
        if (stone == 0)
            return (1, null);
        
        var s = stone.ToString();
        if (s.Length % 2 == 0)
        {
            var left = s.AsSpan()[..(s.Length / 2)];
            var right = s.AsSpan()[(s.Length / 2)..];
                
            return (long.Parse(left), long.Parse(right));
        }
        return (stone * 2024, null);
    }
}