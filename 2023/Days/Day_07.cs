using Core;

using System.Diagnostics;
using System.Drawing;

namespace AoC_2023.Days;

public sealed partial class Day_07 : BaseDay
{
    private readonly Line[] _input;

    public Day_07()
    {
        _input = File.ReadAllLines(InputFilePath).SelectArray(ParseLine);

        Line ParseLine(string s)
        {
            var arr = s.Split(' ');
            return new Line(arr[0], int.Parse(arr[1]));
        }
    }

    record Line(string Hand, int Bid)
    {
        public string Repl() => Hand.Replace('A', 'Z')
                .Replace('K', 'Y')
                .Replace('Q', 'X')
                .Replace('J', 'W')
                .Replace('T', 'V');
        public string Repl2() => Hand.Replace('A', 'Z')
                .Replace('K', 'Y')
                .Replace('Q', 'X')
                .Replace('J', '1')
                .Replace('T', 'V');

        public int Type()
        {
            var hist = Hand.Histogram().SelectList(t => t.count);
            if (hist.Count == 1)
                return 25; // five of a kind
            if (hist.Max() == 4) // four of a kind
                return 24;
            if (hist.Max() == 3 && hist.Min() == 2)
                return 20; // full house
            if (hist.Max() == 3)
                return 13; // three of a kind
            if (hist.Histogram().Any(t => t.item == 2 && t.count == 2))
                return 12; // two pair
            if (hist.Max() == 2)
                return 10; // pair
            return 5; // high card
        }
        public int Type2()
        {
            var hist = Hand.Histogram().ToDictionary(t => t.item, t => t.count);
            var joker = hist.GetValueOrDefault('J');
            if (joker > 0 && joker < 5)
            {
                var otherMax = hist.Where(kvp => kvp.Key != 'J').MaxBy(t => t.Value);
                hist[otherMax.Key] += joker;
                hist.Remove('J');
            }
            var newHand = string.Concat(hist.Select(t => new string(t.Key, t.Value)));
            Console.WriteLine(Hand + " => " + newHand);

            if (hist.Count == 1)
                return 25; // five of a kind
            if (hist.Values.Max() == 4) // four of a kind
                return 24;
            if (hist.Values.Max() == 3 && hist.Values.Min() == 2)
                return 20; // full house
            if (hist.Values.Max() == 3)
                return 13; // three of a kind
            if (hist.Values.Histogram().Any(t => t.item == 2 && t.count == 2))
                return 12; // two pair
            if (hist.Values.Max() == 2)
                return 10; // pair
            return 5; // high card
        }
    }

    public override async ValueTask<string> Solve_1()
    {
        var orderedHands = _input.OrderBy(it => it.Type()).ThenBy(it => it.Repl()).ToList();
        var xx = orderedHands.Select((h, rank) => h.Bid * (1 + rank)).ToList();
        return xx.Sum().ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var orderedHands = _input.OrderBy(it => it.Type2()).ThenBy(it => it.Repl2()).ToList();
        var xx = orderedHands.Select((h, rank) => h.Bid * (1 + rank)).ToList();
        return xx.Sum().ToString();
        // 249814394 too low
        // 249959407 too high
    }
}