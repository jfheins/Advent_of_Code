using Core;
using System.Linq;
using System.Drawing;
using System.Xml;
using Flurl.Util;

namespace AoC_2025.Days;

public sealed partial class Day_02 : BaseDay
{
    private readonly string _input;

    public Day_02()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var ranges = _input.Split(",").SelectArray(it => it.Split("-"));

        var invalid = new List<long>();
        foreach (var range in ranges)
        {
            var lower = long.Parse(range[0]);
            var upper = long.Parse(range[1]);
            for (var id = lower; id <= upper; id++)
            {
                var strId = id.ToString();
                if (strId.Length % 2 == 1)
                    continue;
                var halfLen =  strId.Length / 2;
                if (strId[0..halfLen] == strId[halfLen..])
                {
                    invalid.Add(id);
                }
            }
        }
        
        return invalid.Sum().ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var ranges = _input.Split(",").SelectArray(it => it.Split("-"));

        var invalid = new List<long>();
        foreach (var range in ranges)
        {
            var lower = long.Parse(range[0]);
            var upper = long.Parse(range[1]);
            for (var id = lower; id <= upper; id++)
            {
                if (IsInvalid2(id.ToString()))
                {
                    invalid.Add(id);
                }
            }
        }
        
        return invalid.Sum().ToString();
    }

    private static bool IsInvalid2(ReadOnlySpan<char> id)
    {
        for (var prefixLen = 1; prefixLen <= id.Length/2; prefixLen++)
        {
            if (id.Length % prefixLen > 0)
                continue;
            var prefix = id[..prefixLen];
            var allMatch = true;
            for (var i = 1; i < id.Length/prefixLen; i++)
            {
                var part = id[(prefixLen * i)..(prefixLen * (i + 1))];
                if (!prefix.SequenceEqual(part))
                {
                    allMatch = false;
                    break;
                }
            }

            if (allMatch)
                return true;
        }

        return false;
    }
}