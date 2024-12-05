using Core;

namespace AoC_2024.Days;

public sealed class Day_05 : BaseDay
{
    private readonly Rule[] _rules;
    private readonly int[][] _updates;

    public Day_05()
    {
        var input = File.ReadAllLines(InputFilePath).SplitBy("", 2);
        _rules = input[0].SelectArray(Rule.Parse);
        _updates = input[1].SelectArray(it => it.ParseInts());
    }

    public override async ValueTask<string> Solve_1()
    {
        return _updates.Where(SatisfiesAllRules).Sum(u => u.CenterItem()).ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        return _updates.ExceptWhere(SatisfiesAllRules).Sum(u => Fix(u).CenterItem()).ToString();
    }

    private bool SatisfiesAllRules(int[] update)
        => _rules.All(r => r.IsCompliant(update));

    private record Rule(int First, int Second)
    {
        public static Rule Parse(string s)
        {
            var parts = s.Split('|');
            return new Rule(int.Parse(parts[0]), int.Parse(parts[1]));
        }

        public bool IsCompliant(int[] pageNumbers)
        {
            var firstIdx = Array.IndexOf(pageNumbers, First);
            var secondIdx = Array.IndexOf(pageNumbers, Second);
            return firstIdx == -1 || secondIdx == -1 || firstIdx < secondIdx;
        }

        public void FixOrder(int[] pages)
        {
            var leftIdx = Array.IndexOf(pages, First);
            var rightIdx = Array.IndexOf(pages, Second);
            (pages[leftIdx], pages[rightIdx]) = (pages[rightIdx], pages[leftIdx]);
        }
    }

    private int[] Fix(int[] update)
    {
        while (_rules.FirstOrDefault(r => !r.IsCompliant(update)) is { } brokenRule) 
            brokenRule.FixOrder(update);

        return update;
    }
}