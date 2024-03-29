﻿using Core;
using System.Drawing;
using static MoreLinq.Extensions.SplitExtension;

namespace AoC_2021.Days
{
    public class Day_13 : BaseDay
    {
        private List<Point> _numbers;
        private List<(string coord, int amount)> _instr;
        private int maxx;
        private int maxy;

        public Day_13()
        {
            var input = File.ReadAllLines(InputFilePath).Split("").ToList();

            _numbers = input[0].Select(line => line.ParseInts(2)).Select(t => new Point(t[0], t[1])).ToList();
            _instr = input[1].Select(l => l[11..].Split("=").ToTuple2<string, int>()).ToList();
            maxx = _numbers.Max(p => p.X);
            maxy = _numbers.Max(p => p.Y);
        }

        public override async ValueTask<string> Solve_1()
        {
            var clone = _numbers.Select(p =>
            {
                if (p.X > 655)
                {
                    var d = p.X - 655;
                    return new Point(655 - d, p.Y);
                }
                return p;
            });
            return clone.ToHashSet().Count.ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var clone = _numbers.ToList();

            foreach (var (coord, amount) in _instr)
            {
                if (coord == "x")
                {
                    for (int i = 0; i < clone.Count; i++)
                    {
                        if (clone[i].X > amount)
                        {
                            var p = clone[i];
                            clone[i] = new Point(2 * amount - p.X, p.Y);
                        }
                    }
                }
                if (coord == "y")
                {
                    for (int i = 0; i < clone.Count; i++)
                    {
                        if (clone[i].Y > amount)
                        {
                            var p = clone[i];
                            clone[i] = new Point(p.X, 2 * amount - p.Y);
                        }
                    }
                }
            }

            foreach (var line in clone.PointsInBoundingRect())
            {
                foreach (var p in line)
                {
                    if (clone.Any(it => it == p))
                        Console.Write("◼");
                    else
                        Console.Write(" ");
                }
                Console.WriteLine();
            }

            return "";
        }
    }
}
