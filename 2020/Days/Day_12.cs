using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Intrinsics.X86;
using System.Text.RegularExpressions;
using System.Threading.Tasks.Dataflow;
using System.Transactions;

using Core;

namespace AoC_2020.Days
{
    public class Day_12 : BaseDay
    {
        private readonly string[] input;

        public Day_12()
        {
            input = File.ReadAllLines(InputFilePath);
        }

        public override string Solve_1()
        {
            var state = (pos: new Point(0, 0), dir: Direction.Right);

            foreach (var instr in input)
            {
                var cmd = instr[0];
                var val = int.Parse(instr[1..]);
                var angle4 = val / 90;
                state = cmd switch
                {
                    'W' => (state.pos.MoveTo(Direction.Left, val), state.dir),
                    'N' => (state.pos.MoveTo(Direction.Up, val), state.dir),
                    'E' => (state.pos.MoveTo(Direction.Right, val), state.dir),
                    'S' => (state.pos.MoveTo(Direction.Down, val), state.dir),
                    'R' => (state.pos, state.dir.TurnClockwise(angle4)),
                    'L' => (state.pos, state.dir.TurnCounterClockwise(angle4)),
                    'F' => (state.pos.MoveTo(state.dir, val), state.dir),
                    _ => throw new InvalidDataException()
                };
            }
            return state.pos.Manhattan().ToString();
        }

        public override string Solve_2()
        {
            var ship = (pos: new Point(0, 0), dir: Direction.Right);
            var wp = new Point(10, -1);

            foreach (var instr in input)
            {
                var cmd = instr[0];
                var val = int.Parse(instr[1..]);
                var offset = new Size(wp);
                switch (cmd)
                {
                    case 'W':
                        wp = wp.MoveTo(Direction.Left, val); break;
                    case 'N':
                        wp = wp.MoveTo(Direction.Up, val); break;
                    case 'S':
                        wp = wp.MoveTo(Direction.Down, val); break;
                    case 'E':
                        wp = wp.MoveTo(Direction.Right, val); break;
                    case 'L':
                        wp = wp.TurnCounterClockwise(val);
                        break;
                    case 'R':
                        wp = wp.TurnClockwise(val);
                        break;
                    case 'F':
                        ship = (ship.pos + (offset * val), ship.dir);
                        break;
                };
            }
            return (Math.Abs(ship.pos.X) + Math.Abs(ship.pos.Y)).ToString();
        }
    }
}

