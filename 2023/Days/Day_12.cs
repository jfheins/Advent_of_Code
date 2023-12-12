using Core;

using NoAlloq;

using System.Collections.Immutable;

namespace AoC_2023.Days;

public sealed partial class Day_12 : BaseDay
{
    private readonly string[] _input;

    public Day_12()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    private Spring ParseLine1(string s)
    {
        var parts = s.Split(' ');
        var map = parts[0].Prepend('.').Append('.').ToImmutableArray();
        return new Spring(map, parts[1].ParseInts());
    }

    private Spring ParseLine2(string s)
    {
        var parts = s.Split(' ');

        var springs = string.Join('?', Enumerable.Repeat(parts[0], 5));
        var groups = string.Join(' ', Enumerable.Repeat(parts[1], 5));

        var map = springs.Prepend('.').Append('.').ToImmutableArray();
        return new Spring(map, groups.ParseInts());
    }

    record Spring(ImmutableArray<char> Map, int[] Runs) : IEquatable<Spring>
    {
        public override string ToString()
        {
            return new string(Map.AsSpan()) + " | " + string.Join(", ", Runs);
        }

        public override int GetHashCode()
        {
            var h = new HashCode();
            foreach (var item in Map.AsSpan())
                h.Add(item);
            return h.ToHashCode();
        }

        public virtual bool Equals(Spring? o)
        {
            return o is Spring s
                && Map.AsSpan().SequenceEqual(s.Map.AsSpan())
                && Runs.SequenceEqual(s.Runs);
        }
    }

    public override async ValueTask<string> Solve_1()
    {
        var springs = _input.SelectArray(ParseLine1);
        return springs.Sum(CountCached).ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var springs = _input.SelectArray(ParseLine2);
        //   var x = springs.Max(it => it.Runs.Max());
        return springs.Sum(CountCached).ToString();
    }

    Dictionary<string, long> _cache = new(100_000);

    long CountCached(Spring s)
    {
        var key = new string(s.Map.AsSpan()) + string.Join(',', s.Runs);
        if (_cache.TryGetValue(key, out var count))
            return count;
        count = CountArrangements(s);
        _cache[key] = count;
        return count;
    }

    long CountArrangements(Spring s)
    {
        var qidx = s.Map.IndexOf('?');
        if (qidx > -1)
        {
            Span<char> map = stackalloc char[s.Map.Length];
            s.Map.AsSpan().CopyTo(map);

            map[qidx] = '.';
            var left = MakeChild(map, s.Runs);
            map[qidx] = '#';
            var right = MakeChild(map, s.Runs);

            var res = 0L;
            if (left != null)
                res += CountCached(left);
            if (right != null)
                res += CountCached(right);
            return res;
        }
        else
        {
            return IsValid(s) ? 1 : 0;
        }

        static Spring? MakeChild(ReadOnlySpan<char> arr, int[] runs)
        {
            if (runs.Length == 0)
            {
                return arr.Contains('#') ? null : new Spring(['.'], runs);
            }
            if (arr.Count('#') + arr.Count('?') < runs.Sum())
                return null;

            // Can it be simplified?
            var (RunLen, PrefixLen) = StartRun(arr);
            if (RunLen > 0)
            {
                if (RunLen == runs[0])
                {
                    // Shorten
                    var newRuns = new int[runs.Length - 1];
                    Array.Copy(runs, 1, newRuns, 0, newRuns.Length);
                    var newMap = arr[PrefixLen..];
                    return new Spring([.. newMap], newRuns);
                }
                else
                {
                    return null;
                }
            }
            (RunLen, var SuffixLen) = EndRun(arr);
            if (RunLen > 0)
            {
                if (RunLen == runs.Last())
                {
                    // Shorten
                    var newRuns = new int[runs.Length - 1];
                    Array.Copy(runs, 0, newRuns, 0, newRuns.Length);
                    var newMap = arr[..^SuffixLen];
                    return new Spring([.. newMap], newRuns);
                }
                else
                {
                    return null;
                }
            }
            var dotPrefix = arr.IndexOfAnyExcept('.');
            if (dotPrefix > 1)
                arr = arr.Slice(dotPrefix - 1, arr.Length - dotPrefix + 1);

            return new Spring([.. arr], runs);
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