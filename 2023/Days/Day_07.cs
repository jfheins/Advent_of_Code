using Core;

using Newtonsoft.Json.Linq;

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

    record struct Line(string Hand, int Bid)
    {
        public int Tiebreaker;
        public int TypeId;
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
            // var newHand = string.Concat(hist.Select(t => new string(t.Key, t.Value)));
            // Console.WriteLine(Hand + " => " + newHand);

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
        Process1();
        foreach (var item in _input)
        {
            if (item.TypeId != item.Type())
            {
                ;
            }
        }
        Array.Sort(_input, CompareLines);
        return _input.Select((h, rank) => h.Bid * (1 + rank)).Sum().ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var orderedHands = _input.OrderBy(it => it.Type2()).ThenBy(it => it.Repl2()).ToList();
        var xx = orderedHands.Select((h, rank) => h.Bid * (1 + rank)).ToList();
        return xx.Sum().ToString();
    }

    void Process1()
    {
        Span<byte> hist = stackalloc byte[5];
        for (var i = 0; i < _input.Length; i++)
        {
            CalcHist(_input[i].Hand, hist);
            hist.Sort();
            if (hist[4] == 5)
                _input[i].TypeId = 25; // five of a kind
            else if (hist[4] == 4)
                _input[i].TypeId = 24; // four of a kind
            else if (hist[4] == 3 && hist[3] == 2)
                _input[i].TypeId = 20; // full house
            else if (hist[4] == 3)
                _input[i].TypeId = 13; // three of a kind
            else if (hist[4] == 2 && hist[3] == 2)
                _input[i].TypeId = 12; // two pair
            else if (hist[4] == 2)
                _input[i].TypeId = 10; // pair
            else
                _input[i].TypeId = 5; // high card

            var tb = 0;
            foreach (var c in _input[i].Repl())
            {
                tb = (tb << 6) | (c - '0');
            }
            _input[i].Tiebreaker = tb;
        }
    }

    private static void CalcHist(string hand, Span<byte> hist)
    {
        hist.Clear();
        hist[0] = hand[1] == hand[0] ? (byte)2 : (byte)1;
        hist[1] = hand[1] == hand[0] ? (byte)0 : (byte)1;

        if (hand[2] == hand[0])
            hist[0]++;
        else if (hand[2] == hand[1])
            hist[1]++;
        else
            hist[2] = 1;

        if (hand[3] == hand[0])
            hist[0]++;
        else if (hand[3] == hand[1])
            hist[1]++;
        else if (hand[3] == hand[2])
            hist[2]++;
        else
            hist[3] = 1;

        if (hand[4] == hand[0])
            hist[0]++;
        else if (hand[4] == hand[1])
            hist[1]++;
        else if (hand[4] == hand[2])
            hist[2]++;
        else if (hand[4] == hand[3])
            hist[3]++;
        else
            hist[4] = 1;
    }

    int CompareLines(Line a, Line b)
    {
        var res = a.TypeId.CompareTo(b.TypeId);
        return res == 0 ? a.Tiebreaker.CompareTo(b.Tiebreaker) : res;
    }
}