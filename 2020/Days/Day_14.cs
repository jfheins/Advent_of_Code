﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Core;

using static MoreLinq.Extensions.SubsetsExtension;

namespace AoC_2020.Days
{
    public class Day_14 : BaseDay
    {
        private readonly string[] input;

        public Day_14()
        {
            input = File.ReadAllLines(InputFilePath);
        }

        public override string Solve_1()
        {
            var mask = 0L;
            var maskBits = 0L;
            var mem = new Dictionary<int, long>();

            foreach (var line in input)
            {
                if (line.StartsWith("mask ="))
                {
                    mask = Convert.ToInt64(line[^36..].Replace("X", "0"), 2);
                    maskBits = Convert.ToInt64(line[^36..].Replace("0", "1").Replace("X", "0"), 2);
                }
                else if (line.StartsWith("mem"))
                {
                    var numbers = line.ParseLongs(2);
                    var addr = (int)numbers[0];
                    var value = numbers[1];
                    value = (value & ~maskBits) | mask;
                    mem[addr] = value;
                }
            }


            return mem.Values.Sum().ToString();
        }

        public override string Solve_2()
        {
            var mask = 0L;
            var maskBits = 0L;
            var maskX = new HashSet<long>();
            var allX = 0L;
            var mem = new Dictionary<long, long>();

            foreach (var line in input)
            {
                if (line.StartsWith("mask ="))
                {
                    mask = Convert.ToInt64(line[^36..].Replace("X", "0"), 2);
                    maskBits = Convert.ToInt64(line[^36..].Replace("X", "0"), 2);
                    maskX = line.IndexWhere(c => c == 'X').Select(i => 42-i).Select(i => 1L << i).ToHashSet();
                    allX = Convert.ToInt64(line[^36..].Replace("1", "0").Replace("X", "1"), 2);
                }
                else if (line.StartsWith("mem"))
                {
                    var numbers = line.ParseLongs(2);
                    var addr = numbers[0];
                    var value = numbers[1];

                    foreach (var setOfFloatingBits  in maskX.Subsets())
                    {
                        var faddr = ((addr & ~maskBits) | mask) & ~allX;
                        foreach (var oneBitNumber in setOfFloatingBits )
                            faddr |= oneBitNumber;
                        mem[faddr] = value;
                    }
                }
            }

            return mem.Values.Sum().ToString();
        }
    }
}

