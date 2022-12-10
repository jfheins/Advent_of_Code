using System;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;

using Core;

using Microsoft.VisualBasic.FileIO;

namespace AoC_2022.Days
{
    public sealed class Day_10 : BaseDay
    {
        private readonly List<string> _input;

        public Day_10()
        {
            _input = File.ReadAllLines(InputFilePath).ToList();
        }

        public override async ValueTask<string> Solve_1()
        {
            var cycle = 1;
            var register = 1;
            var xx = new List<int>();
            foreach (var line in _input)
            {
                if (line == "noop")
                {
                    if ((cycle + 20) % 40 == 0)
                    {
                        xx.Add(cycle * register);
                    }

                    cycle++;
                }
                else if (line.StartsWith("addx"))
                {
                    if ((cycle + 20) % 40 == 39)
                    {
                        xx.Add((cycle + 1) * register);
                    }
                    if ((cycle + 20) % 40 == 0)
                    {
                        xx.Add(cycle * register);
                    }

                    cycle += 2;
                    register += line.ParseInts(1).First();
                }
                else
                    Debug.Assert(false);

               
            }


            return xx.Sum().ToString();
        }


        public override async ValueTask<string> Solve_2()
        {
            var currentCycle = 1;
            var register = 1;
            var lineIdx = 0;
            int? carryOver = null;

            while (lineIdx < _input.Count)
            {

                if (carryOver == null)
                {
                    var line = _input[lineIdx++];
                    if (line == "noop")
                    {
                        // nothing
                    }
                    else if (line.StartsWith("addx"))
                    {
                       carryOver = line.ParseInts(1).First();
                    }
                }
                else
                {
                    register += carryOver.Value;
                    carryOver = null;
                }

                var xPos = currentCycle.OneBasedModulo(40);
                var sprite = Math.Abs(register - xPos) > 1 ? '.' : '#';
                Console.Write(sprite);
                if (xPos == 40)
                    Console.WriteLine();

                currentCycle++;
            }

            Console.WriteLine();
            return "^= See top =^";
        }
    }
}