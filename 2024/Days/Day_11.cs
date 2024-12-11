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
        => _stones.Sum(it => Evolve(it, blinks));


    private long Evolve(long stone, int times)
    {
        if (times == 0)
            return 1;
        if (_cache.TryGetValue((stone, times), out var value))
            return value;

        var evolved = new TinyList<long>(stackalloc long[2]);
        Evolve(stone, ref evolved);
        var result = 0L;
        foreach (var x in evolved.AsSpan())
        {
            result += Evolve(x, times - 1);
        }
        
        _cache[(stone, times)] = result;
        return result;
    }

    private static void Evolve(long x, ref TinyList<long> evolved)
    {
        if (x == 0)
        {
            evolved.Add(1);
            return;
        }

        var digits = GetDigits(x);
        if (digits % 2 == 0)
        {
            var factor = (long)Math.Pow(10, (int)(digits / 2));
            evolved.Add((int)(x / factor));
            evolved.Add((int)(x % factor));
        }
        else
        {
            evolved.Add(x * 2024);
        }
        
        static int GetDigits(long x) =>  (int)Math.Floor(Math.Log10(x) + 1);
    }
}