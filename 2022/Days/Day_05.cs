using Core;
using static MoreLinq.Extensions.SplitExtension;
using static MoreLinq.Extensions.TransposeExtension;

namespace AoC_2022.Days
{
    public sealed class Day_05 : BaseDay
    {
        private readonly IReadOnlyList<Stack<char>> _state;
        private readonly IReadOnlyList<(int c, int from, int to)> _moves;

        public Day_05()
        {
            var input = File.ReadAllLines(InputFilePath).Split("").ToList();
            _state = input[0].Reverse().Transpose()
                .Where(col => char.IsDigit(col.First()))
                .SelectList(col => new Stack<char>(col.Where(char.IsLetterOrDigit).Reverse()));
            _moves = input[1].SelectList(line => line.ParseInts(3).ToTuple3());
        }

        public override async ValueTask<string> Solve_1()
        {
            var state = CloneState();
            foreach (var (c, from, to) in _moves)
            {
                Move(state[from - 1], state[to - 1], c, true);
            }

            return TopItems(state);
        }

        public override async ValueTask<string> Solve_2()
        {
            var state = CloneState();
            foreach (var (c, from, to) in _moves)
            {
                Move(state[from - 1], state[to - 1], c, false);
            }

            return TopItems(state);
        }

        private string TopItems(IEnumerable<Stack<char>> state)
            => string.Concat(state.Select(col => col.First()));

        private IReadOnlyList<Stack<char>> CloneState()
            => _state.SelectList(col => new Stack<char>(col));

        private void Move(Stack<char> source, Stack<char> destination, int count, bool firstOutFirstIn)
        {
            var items = source.PopMany(count).ToList();
            if (!firstOutFirstIn)
                items.Reverse();
            foreach (var item in items)
                destination.Push(item);
        }
    }
}