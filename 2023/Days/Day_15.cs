using Core;

using Microsoft.Z3;

using MoreLinq;

using System.Diagnostics;
using System.Drawing;
using System.Linq.Expressions;

namespace AoC_2023.Days;

public sealed partial class Day_15 : BaseDay
{
    private readonly string _input;

    public Day_15()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        return _input.Split(['\n', ',']).Select(Hash).Sum().ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var instructions = _input.Split(['\n', ',']).ToList();
        var boxes = new List<(string label, int lens)>[256];

        foreach (var inst in instructions)
        {
            var parts = inst.Split(['-', '=']);
            var boxId = Hash(parts[0]);
            var label = parts[0];
            var content = boxes[boxId] ??= [];
            if (inst.Contains("-"))
            {
                content.RemoveAll(it => it.label == label);
            }
            else
            {
                var idx = content.FindIndex(it => it.label == label);
                if (idx > -1)
                    content[idx] = (label, int.Parse(parts[1]));
                else
                    content.Add((label, int.Parse(parts[1])));
            }
        }

        var focusPower = 0L;
        foreach (var box in boxes.Index())
        {
            if (box.Value != null)
                foreach (var lens in box.Value.Index())
                {
                    focusPower += (box.Key + 1) * (lens.Key + 1) * lens.Value.lens;
                }
        }

        return focusPower.ToString();
    }

    int Hash(string s)
    {
        var val = 0;
        foreach (var c in s)
        {
            val += (byte)c;
            val = (val * 17) % 256;
        }
        return val;
    }
}