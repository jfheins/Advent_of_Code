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
            var currentCycle = 1;
            var register = 1;
            var lineIdx = 0;
            int? carryOver = null;
            var signalStrength = 0;
            var crtLine = new char[40];
            Console.WriteLine();

            while (true)
            {
                var crtPos = (currentCycle - 1) % 40;
                var sprite = Math.Abs(register - crtPos) > 1 ? ' ' : '█';
                crtLine[crtPos] = sprite;
                if (crtPos == 39)
                    Console.WriteLine(crtLine);
                
                if((currentCycle + 20) % 40 == 0)
                    signalStrength += currentCycle * register;
                
                if (carryOver == null)
                {
                    if (lineIdx == _input.Count) // EOF
                        break;
                    var line = _input[lineIdx++];
                    if (line.StartsWith("addx"))
                        carryOver = line.ParseInts(1).First();
                }
                else
                {
                    register += carryOver.Value;
                    carryOver = null;
                }
                currentCycle++;
            }

            Console.WriteLine();
            return signalStrength.ToString();
        }


        public override async ValueTask<string> Solve_2()
        {
            return "^= See top =^";
        }
    }
}