using Core;

using static MoreLinq.Extensions.SplitExtension;
using static MoreLinq.Extensions.TransposeExtension;

using System.Collections;
using System.Diagnostics;
using System.Collections.Specialized;

namespace AoC_2022.Days
{
    public sealed class Day_05 : BaseDay
    {
        private List<List<char>> _state;
        private List<int[]> _moves;

        public Day_05()
        {
            var input = File.ReadAllLines(InputFilePath).Split("").ToList();
            _state = input[0].Reverse().Transpose()
                .Select(it => it.Where(c => c != ' ').ToList())
                .Where(it => it.Any())
                .Where(col => char.IsDigit(col.First()))
                .ToList();
            _moves = input[1].SelectList(line => line.ParseInts(3));
        }

        public override async ValueTask<string> Solve_1()
        {
            var state = _state.SelectList(col => col.ToList());
            foreach (var move in _moves)
            {
                for (int i = 0; i < move[0]; i++)
                {
                    MoveOne(state, move[1], move[2]);
                }
            }
            return string.Concat(state.Select(col => col.Last()));
        }

        public override async ValueTask<string> Solve_2()
        {
            var state = _state.SelectList(col => col.ToList());
            foreach (var move in _moves)
            {
                MoveMany(state, move[0], move[1], move[2]);
            }
            return string.Concat(state.Select(col => col.Last()));
        }

        private void MoveOne(List<List<char>> state, int fromIdx, int toIdx)
        {
            var item = state[fromIdx - 1].Last();
            Debug.Assert(char.IsLetter(item));
            state[fromIdx - 1].RemoveAt(state[fromIdx - 1].Count - 1);
            state[toIdx - 1].Add(item);
        }

        private void MoveMany(List<List<char>> state, int count, int fromIdx, int toIdx)
        {
            var items = state[fromIdx - 1].TakeLast(count).ToList();
            var removeIdx = state[fromIdx - 1].Count - count;
            state[fromIdx - 1].RemoveRange(removeIdx, count);
            state[toIdx - 1].AddRange(items);
        }
    }
}