using System.Numerics;
using Core;

namespace AoC_2024.Days;

public sealed class Day_11 : BaseDay
{
    private readonly string _input;

    public Day_11()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var stones = _input.ParseLongs();
        var total = stones.Sum(it => Evolve2(it, 25));
        return total.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var stones = _input.ParseLongs();
        var total = stones.Sum(it => Evolve2(it, 75));
        return total.ToString();
    }

    private Dictionary<(long, int), long> _cache = new();
    
    private long Evolve2(long stone, int times)
    {
        if (times == 0)
            return 1;
        if (_cache.TryGetValue((stone, times), out var value))
        {
            return value;
        }
        var evolved = Evolve(stone);
        var result = evolved.Sum(it => Evolve2(it, times - 1));
        _cache[(stone, times)] = result;
        return result;
    }

    private IEnumerable<long> Evolve(long x)
    {
        int digits = (int)Math.Floor(Math.Log10(x) + 1);

        if (x == 0)
            return [1L];
        if (digits % 2 ==0)
        {
            var a = x / Math.Pow(10, digits / 2);
            var b = x % Math.Pow(10, digits / 2);
            return [(long)a, (long)b];
        }
        return [x * 2024];
    }

    private IEnumerable<BigInteger> Evolve(BigInteger x)
    {
        int digits = (int)Math.Floor(BigInteger.Log10(x) + 1);

        if (x == BigInteger.Zero)
            return [BigInteger.One];
        if (digits % 2 ==0)
        {
            var a = x / BigInteger.Pow(10, digits / 2);
            var b = x % BigInteger.Pow(10, digits / 2);
            return [a, b];
        }
        return [x * 2024];
    }
}