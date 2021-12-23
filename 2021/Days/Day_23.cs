using Core;
using Core.Combinatorics;
using MoreLinq.Extensions;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace AoC_2021.Days
{
    public class Day_23 : BaseDay
    {
        public record State
        {
            // 01.2.3.4.56
            public ImmutableList<char> WaitingSpaces { get; set; }

            public ImmutableStack<char>[] Rooms { get; set; } = new ImmutableStack<char>[4];
        }

        private FiniteGrid2D<char> _grid;

        public Day_23()
        {
            _grid = Grid2D.FromFile(InputFilePath);
        }

        public override async ValueTask<string> Solve_1()
        {
            var s = new AStarSearch<char[]>(EqualityComparer<char[]>.Default, Expander);


            var room1 = MakeRoom1(3);
            var room2 = MakeRoom1(5);
            var room3 = MakeRoom1(7);
            var room4 = MakeRoom1(9);

            var initial = new State
            {
                Rooms = new ImmutableStack<char>[] { room1, room2, room3, room4 },
                WaitingSpaces = ImmutableList.Create(".......".ToCharArray())
            };
            var waitingSpaces = ".......";
            var slots = string.Concat(_grid[3, 2], _grid[3, 3], _grid[5, 2], _grid[5, 3],
                _grid[7, 2], _grid[7, 3], _grid[9, 2], _grid[9, 3]);

            var initialState = (waitingSpaces + slots).ToCharArray();
            var path = s.FindFirst(initialState, Check, Heuristic)!;
            return path.Cost.ToString();
        }

        private ImmutableStack<char> MakeRoom1(int x)
        {
            var items = new char[] { _grid[x, 3], _grid[x, 2] };
            return ImmutableStack.Create(items);
        }

        private float Heuristic(char[] arg)
        {
            var remainder = 0;
            var idx = arg.Select((pod, idx) => (pod, idx)).Where(x => char.IsLetter(x.pod)).ToLookup(x => x.pod, x => x.idx);
            foreach (var pods in idx)
            {
                var todo = pods.Select(idx => IdxToPoint[idx].ManhattanDistTo(HomeSlot[pods.Key]));
                remainder += (todo.Sum() - 1) * GetStepCost(pods.Key);
            }
            return remainder;
        }

        public override async ValueTask<string> Solve_2()
        {
            return "";
        }

        private bool Check(char[] state)
        {
            return state[11] == 'C' && state[13] == 'D' && new string(state) == ".......AABBCCDD";
        }

        private Dictionary<int, Point> IdxToPoint = new()
        {
            { 0, new(1, 1) },
            { 1, new(2, 1) },
            { 2, new(4, 1) },
            { 3, new(6, 1) },
            { 4, new(8, 1) },
            { 5, new(10, 1) },
            { 6, new(11, 1) },
            { 7, new(3, 2) },
            { 8, new(3, 3) },
            { 9, new(5, 2) },
            { 10, new(5, 3) },
            { 11, new(7, 2) },
            { 12, new(7, 3) },
            { 13, new(9, 2) },
            { 14, new(9, 3) },
        };

        private Dictionary<char, Point> HomeSlot = new()
        {
            { 'A', new(3, 2) },
            { 'B', new(5, 2) },
            { 'C', new(7, 2) },
            { 'D', new(9, 2) },
        };

        private IEnumerable<(char[], float)> Expander(char[] node)
        {
            // Possible moves:
            // any pod can move out into a available waiting spot
            // any pod in a waiting spot can move into the right room

            /*
             ################
             #01.2..3..4..56#
             ###7# 9#11#13###
               #8#10#12#14#
               ############
             */
            var stateStr1 = string.Concat(node[..7]);
            var stateStr2 = string.Concat(node[7..]);
            var param = new[] {
                (slot: 0, idx: 7, name: 'A'),
                (slot: 1, idx: 9, name: 'B'),
                (slot: 2, idx: 11, name: 'C'),
                (slot: 3, idx: 13, name: 'D') };

            // Move out
            foreach (var (slot, idx, name) in param)
            {
                if (node[idx] != name || node[idx + 1] != name)
                {
                    if (char.IsLetter(node[idx]))
                    {
                        foreach (var space in FreeWaitingSpaces(slot, node))
                            yield return MovePod(idx, space);
                    }
                    else if (char.IsLetter(node[idx + 1]) && node[idx + 1] != name)
                    {
                        // Partially empty
                        foreach (var space in FreeWaitingSpaces(slot, node))
                            yield return MovePod(idx + 1, space);
                    }
                }
            }


            // Move in
            foreach (var (slot, idx, name) in param)
            {
                var waiting = OccupiedWaitingSpaces(slot, node).Where(i => node[i] == name).ToList();
                foreach (var pod in waiting)
                {
                    if (node[idx] == '.' && node[idx + 1] == '.')
                        yield return MovePod(pod, idx + 1);
                    if (node[idx] == '.' && node[idx + 1] == name)
                        yield return MovePod(pod, idx);
                }
            }

            (char[], float) MovePod(int from, int to)
            {
                Debug.Assert(char.IsLetter(node[from]));
                Debug.Assert(node[to] == '.');
                var newState = (char[])node.Clone();
                newState[to] = node[from];
                newState[from] = '.';
                var stepCost = GetStepCost(node[from]);
                var cost = IdxToPoint[from].ManhattanDistTo(IdxToPoint[to]) * stepCost;
                return (newState, cost);
            }
        }

        private static int GetStepCost(char podName)
            => podName switch { 'A' => 1, 'B' => 10, 'C' => 100, 'D' => 1000, _ => throw new NotImplementedException() };

        private IEnumerable<int> FreeWaitingSpaces(int slotIdx, char[] state)
        {
            var first = slotIdx + 1;
            // left
            for (int i = first; i >= 0; i--)
                if (state[i] == '.')
                    yield return i;
                else
                    break;
            for (int i = first + 1; i < 7; i++)
                if (state[i] == '.')
                    yield return i;
                else
                    break;
        }

        private IEnumerable<int> OccupiedWaitingSpaces(int slotIdx, char[] state)
        {
            var first = slotIdx + 1;
            // left
            for (int i = first; i >= 0; i--)
                if (state[i] != '.')
                {
                    yield return i;
                    break;
                }
            for (int i = first + 1; i < 7; i++)
                if (state[i] != '.')
                {
                    yield return i;
                    break;
                }
        }
    }
}
