using Core;
using System.Diagnostics;

using static MoreLinq.Extensions.SplitExtension;

namespace AoC_2021.Days
{
    public class Day_04 : BaseDay
    {
        private int[] _numbers;
        private List<Board> _boards;

        public Day_04()
        {
            var input = File.ReadAllLines(InputFilePath).ToArray();
            _numbers = input[0].ParseInts();
            _boards = input.Skip(2).Split("").Select(block => new Board(block)).ToList();
        }

        public override async ValueTask<string> Solve_1()
        {
            foreach (var drawn in _numbers)
            {
                foreach (var b in _boards)
                {
                    b.Draw(drawn);
                }
                var winningScore = _boards
                    .Where(b => b.HasBingo())
                    .Select(b => b.Score())
                    .FirstOrDefault();

                if (winningScore > 0)
                    return winningScore.ToString();
            }
            return "";
        }

        public override async ValueTask<string> Solve_2()
        {
            var openBoards = new HashSet<Board>(_boards);

            foreach (var drawn in _numbers)
            {
                foreach (var b in _boards)
                {
                    b.Draw(drawn);
                }
                var doneBoards = openBoards.Where(b => b.HasBingo()).ToList();
                openBoards.ExceptWith(doneBoards);

                if (openBoards.Count == 0)
                {
                    // Last one was just removed
                    return doneBoards[0].Score().ToString();
                }
            }
            return "";
        }
    }

    public class Board
    {
        public (int num, bool wasDrawn)[][] Numbers { get; set; }
        private int lastDrawn = -1;

        public Board(IEnumerable<string> lines)
        {
            var numbers = lines.Select(line => line.Split(" ", StringSplitOptions.RemoveEmptyEntries));
            Numbers = numbers.Select(s => s.Select(item => (int.Parse(item), false)).ToArray()).ToArray();
        }

        internal void Draw(int drawn)
        {
            lastDrawn = drawn;

            for (int x = 0; x < Numbers.Length; x++)
                for (int y = 0; y < Numbers[x].Length; y++)
                    if (Numbers[x][y].num == drawn)
                        Numbers[x][y].wasDrawn = true;
        }

        internal bool HasBingo()
        {
            var rows = Numbers.Any(row => row.All(x => x.wasDrawn));
            var cols = GetColumns().Any(col => col.All(x => x.wasDrawn));
            return rows || cols;
        }

        private IEnumerable<IEnumerable<(int num, bool wasDrawn)>> GetColumns()
        {
            for (int i = 0; i < Numbers[0].Length; i++)
                yield return GetColumn(i);
            
            IEnumerable<(int num, bool wasDrawn)> GetColumn(int idx)
                => Numbers.Select(row => row[idx]);
        }


        internal int Score()
        {
            Debug.Assert(HasBingo());
            return lastDrawn * Numbers.Sum(row => row.Sum(x => x.wasDrawn ? 0 : x.num));
        }
    }
}
