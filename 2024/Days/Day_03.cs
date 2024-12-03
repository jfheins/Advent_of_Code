using Core;
using Spectre.Console;
using System.Drawing;
using System.Text.RegularExpressions;

namespace AoC_2024.Days;

public sealed partial class Day_03 : BaseDay
{
    private readonly string[] _input;

    public Day_03()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        long res = 0;
        var m = Regex.Matches(string.Concat(_input), @"mul\(\d{1,3},\d{1,3}\)");
        foreach (Match item in m)
        {
            var num = item.Value.ParseInts(2);
            res += num[0] * num[1];
        }
        return res.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        long res = 0;
        bool active = true;
        var m = Regex.Matches(string.Concat(_input), @"mul\(\d{1,3},\d{1,3}\)|do\(\)|don't\(\)");
        foreach (Match item in m)
        {
            if (item.Value == "do()")
            {
                active = true;
            }
            else if(item.Value == "don't()")
            {
                active = false;
            }
            else if(active)
            {
                var num = item.Value.ParseInts(2);
                res += num[0] * num[1];
            }
        }
        return res.ToString();
    }
}