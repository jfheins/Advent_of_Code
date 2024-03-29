﻿using Core;
using System.Collections.Immutable;

namespace AoC_2021.Days
{
    public class Day_23 : BaseDay
    {
        public record State
        {
            // 01.2.3.4.56
            public ImmutableList<char> WaitingSpaces { get; init; }

            public ImmutableStack<char>[] Rooms { get; set; } = new ImmutableStack<char>[4];

            public override string ToString()
            {
                return string.Join("\r\n", new string(WaitingSpaces.ToArray()),
                    new string(Rooms[0].ToArray()),
                    new string(Rooms[1].ToArray()),
                    new string(Rooms[2].ToArray()),
                    new string(Rooms[3].ToArray()));
            }
            public virtual bool Equals(State? other)
            {
                if (other == null)
                    return false;
                return Enumerable.SequenceEqual(WaitingSpaces, other.WaitingSpaces)
                    && Enumerable.SequenceEqual(Rooms[0], other.Rooms[0])
                    && Enumerable.SequenceEqual(Rooms[1], other.Rooms[1])
                    && Enumerable.SequenceEqual(Rooms[2], other.Rooms[2])
                    && Enumerable.SequenceEqual(Rooms[3], other.Rooms[3]);
            }

            public override int GetHashCode()
                => string.Concat(WaitingSpaces).GetHashCode();
        }

        private FiniteGrid2D<char> _grid;

        private int stackdepth = 2;

        public Day_23()
        {
            _grid = Grid2D.FromFile(InputFilePath);
        }

        public override async ValueTask<string> Solve_1()
        {
            var initial = new State
            {
                Rooms = RoomIdxToX.Values.Select(MakeRoom1).ToArray(),
                WaitingSpaces = ImmutableList.Create(".......".ToCharArray())
            };

            var path = new AStarSearch<State>(null, Expander)
                .FindFirst(initial, IsFinal, Heuristic)!;
            return path.Cost.ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            stackdepth = 4;

            var initial = new State
            {
                Rooms = RoomIdxToX.Values.Select(MakeRoom2).ToArray(),
                WaitingSpaces = ImmutableList.Create(".......".ToCharArray())
            };

            var path = new AStarSearch<State>(null, Expander)
                .FindFirst(initial, IsFinal, Heuristic)!;
            return path.Cost.ToString();
        }

        private ImmutableStack<char> MakeRoom1(int x)
        {
            var items = new char[] { _grid[x, 3], _grid[x, 2] };
            return ImmutableStack.Create(items);
        }

        private ImmutableStack<char> MakeRoom2(int x)
        {
            var newLines = new string[] { "#D#C#B#A#", "#D#B#A#C#" };
            var items = new char[] { _grid[x, 3], newLines[1][x - 2], newLines[0][x - 2], _grid[x, 2] };
            return ImmutableStack.Create(items);
        }

        private bool IsFinal(State state)
        {
            return state.WaitingSpaces.All(it => it == '.')
                && state.Rooms[0].All(it => it == 'A')
                && state.Rooms[1].All(it => it == 'B')
                && state.Rooms[2].All(it => it == 'C')
                && state.Rooms[3].All(it => it == 'D');
        }

        private float Heuristic(State s)
        {
            // Heuristic: For each pod that is not in the right room:
            // What are the costs to move up and across the hallway above its room?
            var res = 0;

            foreach (var (room, idx) in s.Rooms.Select((room, idx) => (room, idx)))
            {
                var roomName = "ABCD"[idx];
                var wayToHallway = stackdepth - room.Count() + 1;
                var pods = room.Select((name, pos) => (name, pos)).Where(t => t.name != roomName);
                foreach (var pod in pods)
                {
                    var homeRoomIdx = "ABCD".IndexOf(pod.name);
                    var wayToRoom = Math.Abs(RoomIdxToX[homeRoomIdx] - RoomIdxToX[idx]);
                    res += (wayToHallway + pod.pos + wayToRoom + 1) * GetStepCost(pod.name);
                }
            }
            return res;
        }

        private readonly Dictionary<int, int> WaitingIdxToX = new()
        {
            { 0, 1 },
            { 1, 2 },
            { 2, 4 },
            { 3, 6 },
            { 4, 8 },
            { 5, 10 },
            { 6, 11 },
        };

        private readonly Dictionary<int, int> RoomIdxToX = new()
        {
            { 0, 3 },
            { 1, 5 },
            { 2, 7 },
            { 3, 9 },
        };

        private IEnumerable<(State node, float edgeCost)> Expander(State state)
        {
            var param = new[] {
                (room: 0, name: 'A'),
                (room: 1, name: 'B'),
                (room: 2, name: 'C'),
                (room: 3, name: 'D') };

            // Move out
            foreach (var (roomIdx, name) in param)
            {
                var room = state.Rooms[roomIdx];
                var isDone = room.All(it => it == name);

                if (isDone || room.IsEmpty)
                    continue;

                foreach (var space in FreeWaitingSpaces(roomIdx, state.WaitingSpaces))
                {
                    yield return MovePodOut(roomIdx, space);
                }
            }
            // Move in
            foreach (var (roomIdx, name) in param)
            {
                var room = state.Rooms[roomIdx];
                var isDone = room.All(it => it == name);

                if (room.Count() < stackdepth && isDone)
                {
                    var waiting = OccupiedWaitingSpaces(roomIdx, state).Where(i => state.WaitingSpaces[i] == name).ToList();
                    foreach (var space in waiting)
                        yield return MovePodIn(roomIdx, space);
                }
            }

            (State, float) MovePodOut(int fromRoom, int toIdx)
            {
                var newRooms = (ImmutableStack<char>[])state.Rooms.Clone();
                newRooms[fromRoom] = state.Rooms[fromRoom].Pop(out var mover);
                var newHallway = state.WaitingSpaces.SetItem(toIdx, mover);

                var len1 = stackdepth - newRooms[fromRoom].Count(); // 1 or 2
                var len2 = Math.Abs(WaitingIdxToX[toIdx] - RoomIdxToX[fromRoom]);
                var stepCost = GetStepCost(mover);

                var cost = stepCost * (len1 + len2);
                return (new State { Rooms = newRooms, WaitingSpaces = newHallway }, cost);
            }

            (State, float) MovePodIn(int toRoom, int fromSpace)
            {
                var newRooms = (ImmutableStack<char>[])state.Rooms.Clone();
                var mover = state.WaitingSpaces[fromSpace];
                newRooms[toRoom] = state.Rooms[toRoom].Push(mover);
                var newHallway = state.WaitingSpaces.SetItem(fromSpace, '.');

                var len1 = stackdepth - state.Rooms[toRoom].Count(); // 1 or 2
                var len2 = Math.Abs(WaitingIdxToX[fromSpace] - RoomIdxToX[toRoom]);
                var stepCost = GetStepCost(mover);

                var cost = stepCost * (len1 + len2);
                return (new State { Rooms = newRooms, WaitingSpaces = newHallway }, cost);
            }
        }

        private static int GetStepCost(char podName)
            => podName switch { 'A' => 1, 'B' => 10, 'C' => 100, 'D' => 1000, _ => throw new NotImplementedException() };


        private IEnumerable<int> FreeWaitingSpaces(int roomIdx, ImmutableList<char> hallway)
        {
            var first = roomIdx + 1;
            // left
            for (int i = first; i >= 0; i--)
                if (hallway[i] == '.')
                    yield return i;
                else
                    break;
            for (int i = first + 1; i < 7; i++)
                if (hallway[i] == '.')
                    yield return i;
                else
                    break;
        }

        private IEnumerable<int> OccupiedWaitingSpaces(int roomIdx, State state)
        {
            var first = roomIdx + 1;
            // left
            for (int i = first; i >= 0; i--)
                if (state.WaitingSpaces[i] != '.')
                {
                    yield return i;
                    break;
                }
            for (int i = first + 1; i < 7; i++)
                if (state.WaitingSpaces[i] != '.')
                {
                    yield return i;
                    break;
                }
        }
    }
}
