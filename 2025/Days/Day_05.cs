using Core;

namespace AoC_2025.Days;

public sealed class Day_05 : BaseDay
{
    private readonly IReadOnlyCollection<LongInterval> _freshIdRanges;
    private readonly IReadOnlyCollection<long> _availableIngredientIds;

    public Day_05()
    {
        var input = File.ReadAllLines(InputFilePath).SplitBy("");
        _freshIdRanges = input[0].SelectArray(it => LongInterval.ParseInclusive(it));
        _availableIngredientIds = input[1].SelectArray(long.Parse);
    }

    public override async ValueTask<string> Solve_1()
    {
        return _availableIngredientIds.Count(IsFresh).ToString();

        bool IsFresh(long id) => _freshIdRanges.Any(it => it.Contains(id));
    }

    public override async ValueTask<string> Solve_2()
    {
        var nonOverlapping = _freshIdRanges.OrderBy(it => it.Start).Aggregate(
            new List<LongInterval>(),
            (acc, elem) =>
            {
                if (acc.LastOrDefault().OverlapsWith(elem)) 
                    acc[^1] = acc[^1].Union(elem);
                else
                    acc.Add(elem);
                return acc;
            }
        );
        
        return nonOverlapping.Sum(it => it.Length).ToString();
    }
}