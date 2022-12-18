using Core;

using MoreLinq.Extensions;

using Spectre.Console;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

using static MoreLinq.Extensions.IndexExtension;

namespace AoC_2022.Days
{
    public sealed class Day_17 : BaseDay
    {
        private string _input;

        private List<string> rocks = new List<string>
        {
            """
            ####
          """,
            """
             # 
            ###
             # 
          """,
            """
              #
              #
            ###
          """,
            """
            #
            #
            #
            #
          """,
            """
            ##
            ##
          """
        };

        private State initialState;

        public Day_17()
        {
            _input = File.ReadAllText(InputFilePath);
            var floor = Enumerable.Range(0, 7).Select(x => new Point(x, 0));
            initialState = new State(0, 0, floor.ToArray(), 0);
        }
        private record State(int RockIdx, int ShiftIdx, Point[] Tower, long Height);

        private class SamePatternComparer : IEqualityComparer<State>
        {
            public bool Equals(State? x, State? y)
            {
                return x.RockIdx == y.RockIdx && x.ShiftIdx == y.ShiftIdx && Enumerable.SequenceEqual(x.Tower, y.Tower);
            }

            public int GetHashCode([DisallowNull] State obj)
            {
                return HashCode.Combine(obj.RockIdx, obj.ShiftIdx);
            }
        }

        public override async ValueTask<string> Solve_1()
        {
            return CalculateMany(2022, initialState).ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var comparer = new SamePatternComparer();
            var seenStates = new Dictionary<State, int>(comparer);
            var blocks = 0;
            var state = initialState;

            long heightPerPeriod;
            int period;
            while (true)
            {
                state = CalcOneBlock(state);
                if (!seenStates.TryAdd(state, blocks))
                {
                    var prevStep = seenStates[state];
                    period = blocks - prevStep;
                    var prevState = seenStates.Keys.First(it => comparer.Equals(it, state));
                    heightPerPeriod = state.Height - prevState.Height;
                    break;
                }
                blocks++;
            }
            // State = prelude + one period
            var remBlocks = 1000000000000L - blocks;

            var remPeriods = remBlocks / period;
            var epilog = remBlocks - remPeriods * period;

            for (var i = 0; i < epilog; i++)
                state = CalcOneBlock(state);

            var totalHeight = heightPerPeriod * remPeriods + state.Height - 1;

            return totalHeight.ToString();
        }

        private long CalculateMany(int blocks, State state)
        {
            for (var i = 0; i < blocks; i++)
                state = CalcOneBlock(state);

            return state.Height;
        }

        private State CalcOneBlock(State s)
        {
            var rock = rocks[s.RockIdx].Replace('#', '@').Split("\r\n");
            var shiftIdx = s.ShiftIdx;
            var grid = new FiniteGrid2D<char>(7, 0, ' ');
            Debug.Assert(s.Tower.Max(p => p.Y) == 0);
            foreach (var p in s.Tower)
            {
                grid[p] = '#';
            }
            grid.SizeToFit();
            var oldHeight = -grid.Min(x => x.pos.Y);
            grid.EnlargeTop(rock.Length + 3);

            var y = grid.Bounds.Top;
            foreach (var line in rock)
            {
                foreach (var item in line.Index())
                {
                    if (item.Value != ' ')
                        grid[item.Key, y] = item.Value;
                }
                y++;
            }

            var shift = GetShift();
            while (Step(grid, shift))
            {
                shift = GetShift();
            }

            var newRockIdx = (s.RockIdx + 1) % 5;
            var newShiftIdx = shiftIdx % _input.Length;
            var newHeight = -grid.Min(x => x.pos.Y);
            var addedHeight = newHeight - oldHeight;

            // cull bottom
            var lowestFullRow = grid.GetRowIndices()
                .PairwiseWithOverlap()
                .First(y => AllColsFilled(y.Item1, y.Item2)).Item2;
            grid.RemoveWhere(p => p.Y > lowestFullRow);
            var newTower = grid.Keys.Select(p => p.MoveBy(0, -lowestFullRow))
                .OrderBy(p => p.X).ThenBy(p => p.Y).ToArray();

            return new State(newRockIdx, newShiftIdx, newTower, s.Height + addedHeight);

            char GetShift()
            {
                var result = _input[shiftIdx];
                shiftIdx = (shiftIdx + 1) % _input.Length;
                return result;
            }
            bool AllColsFilled(int y1, int y2)
                => grid.GetRow(y1, ' ').Zip(grid.GetRow(y2, ' ')).All(col => col.First == '#' || col.Second == '#');
        }

        Dictionary<char, Direction> dirmap = new() { { '>', Direction.Right }, { '<', Direction.Left } };

        private bool Step(FiniteGrid2D<char> grid, char shift)
        {
            var points = grid.Where(kvp => kvp.value == '@').Select(kvp => kvp.pos).ToList();
            var shifted = points.SelectList(it => it.MoveTo(dirmap[shift]));

            var newPoints = points;
            if (shifted.All(FreeSpace))
            {
                newPoints = shifted;
            }
            var sunk = newPoints.SelectList(it => it.MoveTo(Direction.Down));
            char filler = '@';
            if (sunk.All(FreeSpace))
            {
                newPoints = sunk;
            }
            else
            {
                // arrest
                filler = '#';
            }
            foreach (var oldpoint in points)
            {
                grid.RemoveAt(oldpoint);
            }
            foreach (var p in newPoints)
            {
                grid[p] = filler;
            }
            return filler == '@';

            bool FreeSpace(Point p) => grid.Bounds.Contains(p) && grid.GetValueOrDefault(p, ' ') != '#';
        }
    }
}