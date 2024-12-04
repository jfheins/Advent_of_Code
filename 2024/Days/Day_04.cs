using Core;
using Spectre.Console;
using System.Drawing;
using System.Text.RegularExpressions;

namespace AoC_2024.Days;

public sealed partial class Day_04 : BaseDay
{
    private readonly string[] _input;

    public Day_04()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var grid = new FiniteGrid2D<char>(_input);
        int xmascount = 0;

        foreach (var x in grid.Where(it => it.value == 'X'))
        {
            foreach (var dir in Directions.All8)
            {
                var ray = grid.LineV(x.pos, dir.ToSize()).Take(4).ToArray();
                var str = new string(ray);
                xmascount += str == "XMAS" ? 1 : 0;
            }
        }
        return xmascount.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var grid = new FiniteGrid2D<char>(_input);
        int xmascount = 0;

        foreach (var a in grid.Where(it => it.value == 'A'))
        {
            HashSet<Direction8> found = new();
            foreach (var dir in Directions.Diagonal)
            {
                var one = a.pos.MoveTo(dir);
                var two = a.pos.MoveTo(dir.Opposite());
                if (grid.GetValueOrDefault(one, '.') == 'M' && grid.GetValueOrDefault(two, '.') == 'S')
                    found.Add(dir);
            }
            if(found.Any(it => found.Contains(it.TurnClockwise()) || found.Contains(it.TurnCounterClockwise())))
                xmascount++;
        }
        return xmascount.ToString(); // not 1930
    }
}