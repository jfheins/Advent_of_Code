using Core;

using NoAlloq;

using System.Collections.Immutable;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;

namespace AoC_2023.Days;

public sealed partial class Day_12 : BaseDay
{
    private readonly Spring[] _input;

    public Day_12()
    {
        _input = File.ReadAllLines(InputFilePath).SelectArray(ParseLine);
    }

    private Spring ParseLine(string s)
    {
        var parts = s.Split(' ');
        var map = parts[0].Prepend('.').Append('.').ToImmutableArray();
        return new Spring(map, parts[1].ParseInts());
    }

    record Spring(ImmutableArray<char> Map, int[] Runs)
    {
        public override string ToString()
        {
            return new string(Map.AsSpan()) + " | " + string.Join(", ", Runs);
        }
    }

    public override async ValueTask<string> Solve_1()
    {
        var xxx = CountArrangements(new Spring(".????#?.?..????.??#.".ToImmutableArray(), [2, 1, 1, 2, 1, 1]));
        long res = 0L;
        foreach (var s in _input)
        {
            var xx = CountArrangements(s);
            Console.WriteLine(s.ToString() + "\t\t => " + xx);
            res += xx;
        }
        return res.ToString(); // 8247 too low
    }

    long CountArrangements(Spring s)
    {
        var qidx = s.Map.IndexOf('?');
        if (qidx > -1)
        {
            var left = MakeChild(s.Map.SetItem(qidx, '.'), s.Runs);
            var right = MakeChild(s.Map.SetItem(qidx, '#'), s.Runs);
            var res = 0L;
            if (left != null)
                res += CountArrangements(left);
            if (right != null)
                res += CountArrangements(right);
            return res;
        }
        else
        {
            var a = IsValid(s) ? 1 : 0;
            return a;
        }

        static Spring? MakeChild(ImmutableArray<char> arr, int[] runs)
        {
            if (runs.Length == 0)
            {
                return arr.Contains('#') ? null : new Spring(['.'], runs);
            }
            if (arr.AsSpan().Count('#') + arr.AsSpan().Count('?') < runs.Length)
                return null;
            // Can it be simplified?
            var (RunLen, PrefixLen) = StartRun(arr.AsSpan());
            if (RunLen > 0)
            {
                if (RunLen == runs[0])
                {
                    // Shorten
                    var newRuns = new int[runs.Length - 1];
                    Array.Copy(runs, 1, newRuns, 0, newRuns.Length);
                    var newMap = arr.RemoveRange(0, PrefixLen);
                    return new Spring(newMap, newRuns);
                }
                else
                    return null;
            }
            (RunLen, var SuffixLen) = EndRun(arr.AsSpan());
            if (RunLen > 0)
            {
                if (RunLen == runs.Last())
                {
                    // Shorten
                    var newRuns = new int[runs.Length - 1];
                    Array.Copy(runs, 0, newRuns, 0, newRuns.Length);
                    var newMap = arr.RemoveRange(arr.Length - SuffixLen, SuffixLen);
                    return new Spring(newMap, newRuns);
                }
                else
                    return null;
            }
            var sp = new Spring(arr, runs);
            return sp;
        }
    }

    private static bool IsValid(Spring s)
    {
        if (s.Runs.Length == 0)
            return s.Map.AllEqual('.');

        var map = s.Map.AsSpan();
        foreach (var runLen in s.Runs)
        {
            var (len, prefix) = StartRun(map);
            if (len != runLen)
                return false;
            map = map.Slice(prefix);
        }
        return map.All(it => it == '.');
    }

    public override async ValueTask<string> Solve_2()
    {
        return "-";
    }

    private static (int RunLen, int PrefixLen) StartRun(ReadOnlySpan<char> s)
    {
        var start = s.IndexOf("#");
        var q = s.IndexOf("?");
        if (start == -1 || (q > -1 && q < start))
            return (0, -1);
        var length = s[start..].IndexOfAny('?', '.');
        return s[start + length] == '.' ? (length, start + length) : (0, -1);
    }

    private static (int RunLen, int SuffixLen) EndRun(ReadOnlySpan<char> s)
    {
        var end = s.LastIndexOf("#");
        if (end == -1 || s.LastIndexOf("?") > end)
            return (0, -1);
        var start = s[..end].LastIndexOfAny('?', '.');
        return s[start] == '.' ? (end - start, s.Length - start - 1) : (0, -1);
    }
}