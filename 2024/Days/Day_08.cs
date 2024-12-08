using Core;
using Spectre.Console;
using System.Drawing;
using Core.Combinatorics;
using Size = System.Drawing.Size;

namespace AoC_2024.Days;

public sealed partial class Day_08 : BaseDay
{
    private readonly string[] _input;

    public Day_08()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var grid = new FiniteGrid2D<char>(_input);

        var antennas = grid.Where(it => char.IsAsciiLetterOrDigit(it.value))
            .ToLookup(it => it.value, it => it.pos);

        HashSet<Point> antinodes = new();
        foreach (var freq in antennas)
        {
            foreach (var other in grid.Keys)
            {
                if (freq.Contains(other))
                    continue;
                var dist = freq.SelectList(a => new Size(a.Minus(other)));
                if (dist.Any(d => dist.Contains(2*d)))
                {
                    antinodes.Add(other);
                    grid[other] = '#';
                }
            }
        }
        
        return antinodes.Count.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var grid = new FiniteGrid2D<char>(_input);

        var antennas = grid.Where(it => char.IsAsciiLetterOrDigit(it.value))
            .ToLookup(it => it.value, it => it.pos);

        HashSet<Point> antinodes = new();
        foreach (var freq in antennas)
        {
            var c = new TupleCombinations2<Point>(freq.ToList());

            foreach (var tuple in c)
            {
                var delta = new Size(tuple.Item1.Minus(tuple.Item2));
                foreach (var reso in grid.LineP(tuple.Item1, delta).Skip(1))
                {
                    antinodes.Add(reso);
                    grid[reso] = '#';
                }
                foreach (var reso in grid.LineP(tuple.Item1, -1*delta).Skip(1))
                {
                    antinodes.Add(reso);
                    grid[reso] = '#';
                }
                foreach (var reso in grid.LineP(tuple.Item2, delta).Skip(1))
                {
                    antinodes.Add(reso);
                    grid[reso] = '#';
                }
                foreach (var reso in grid.LineP(tuple.Item2, -1*delta).Skip(1))
                {
                    antinodes.Add(reso);
                    grid[reso] = '#';
                }
            }
        }
        
        return antinodes.Count.ToString();
    }

    private bool AreMultiplies(Size a, Size b)
    {
        if (a.IsEmpty || b.IsEmpty)
        {
            return false;
        }
        
        if (a.Width == 0 || b.Width == 0)
        {
            if (a.Width != 0 || b.Width != 0)
            {
                return false;
            }

            if (a.Height >= b.Height)
            {
                var scaling = Math.DivRem(a.Height, b.Height, out var remainder);
                return a.Height == scaling * b.Height;
            }
            else
            {
                var scaling = Math.DivRem(b.Height, a.Height, out var remainder);
                return b.Height == scaling * a.Height;
            }
        }
        
        if (a.Width >= b.Width)
        {
            var scaling = Math.DivRem(a.Width, b.Width, out var remainder);
            if (remainder > 0)
            {
                return false;
            }
            return a.Height == scaling * b.Height;
        }
        else
        {
            var scaling = Math.DivRem(b.Width, a.Width, out var remainder);
            if (remainder > 0)
            {
                return false;
            }
            return b.Height == scaling * a.Height;
        }
    }
}