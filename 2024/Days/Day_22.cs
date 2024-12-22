using Core;
using System.Linq;
using System.Drawing;

namespace AoC_2024.Days;

public sealed partial class Day_22 : BaseDay
{
    private readonly int[] _input;

    public Day_22()
    {
        _input = File.ReadAllLines(InputFilePath).SelectArray(line => line.ParseInts().Single());
    }

    public override async ValueTask<string> Solve_1()
    {
        var results = new List<long>();
        foreach (var line in _input)
        {
            long secret = line;
            for (int i = 0; i < 2000; i++)
            {
                secret = Prune(Mix(secret * 64, secret));
                secret = Prune(Mix(secret / 32, secret));
                secret = Prune(Mix(secret * 2048, secret));
            }

            results.Add(secret);
        }
        return results.Sum().ToString();
    }

    private long Prune(long mix)
        => mix.Modulo(16777216);

    private long Mix(long a, long b)
        => a ^ b;

    public override async ValueTask<string> Solve_2()
    {
        var results = new Dictionary<(int, int, int, int), long>();
        foreach (var line in _input)
        {
            long secret = line;
            var secretList = new List<long> { secret };
            for (int i = 0; i < 2000; i++)
            {
                secret = Prune(Mix(secret * 64, secret));
                secret = Prune(Mix(secret / 32, secret));
                secret = Prune(Mix(secret * 2048, secret));
                secretList.Add(secret);
            }

            var prices = secretList.SelectArray(it => (int)it.Modulo(10));
            var diffs = prices.Diff().ToArray();
            var secretMap = new Dictionary<(int, int, int, int), int>();
            for (int i = 0; i < diffs.Length-4; i++)
            {
                var deltaTuple = (diffs[i], diffs[i+1], diffs[i+2], diffs[i+3]);
                if (!secretMap.ContainsKey(deltaTuple)) 
                    secretMap.Add(deltaTuple, prices[i+4]);
            }
            // Add the bananas of this buyer into result
            foreach (var kvp in secretMap)
            {
                results.AddOrModify(kvp.Key, 0L, x => x+ kvp.Value);
            }
        }
        var maxBanana = results.Max(kvp => kvp.Value);
        return maxBanana.ToString();
    }
}