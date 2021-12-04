using Core;
using Core.Combinatorics;
using MoreLinq.Extensions;
using System.IO;
using System.Linq;

namespace AoC_2021.Days
{
    public class Day_04 : BaseDay
    {
        private string[] _input;
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
                foreach (var b in _boards)
                {
                    if (b.HasBingo())
                    {
                        var res = (drawn * b.Score()).ToString();
                        return res;
                    }
                }
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
                foreach (var b in _boards)
                {
                    if (b.HasBingo())
                    {
                        openBoards.Remove(b);
                        if (openBoards.Count ==0)
                        {
                            var res = (drawn * b.Score()).ToString();
                            return res;
                        }
                    }
                }
            }
            return "";
        }
    }

    public class Board
    {
        public (int num, bool wasDrawn)[][] Numbers { get; set; }

        public Board(IEnumerable<string> lines)
        {
            var numbers = lines.Select(line => line.Split(" ", StringSplitOptions.RemoveEmptyEntries));
            Numbers = numbers.Select(s => s.Select(item => (int.Parse(item), false)).ToArray()).ToArray();
        }

        internal void Draw(int drawn)
        {
            for (int x = 0; x < Numbers.Length; x++)
            {
                for (int y = 0; y < Numbers[x].Length; y++)
                {
                    if (Numbers[x][y].num == drawn)
                    {
                        Numbers[x][y] = (drawn, true);
                    }
                }
            }
        }

        internal bool HasBingo()
        {
            var rows = Numbers.Any(row => row.All(x => x.wasDrawn));
            var cols = Enumerable.Range(0, 5).Any(i => GetColumn(i).All(x => x));
            return rows || cols;

            IEnumerable<bool> GetColumn(int idx)
            {
                for (int i = 0; i < Numbers.Length; i++)
                {
                    yield return Numbers[i][idx].wasDrawn;
                }
            }
        }

        internal int Score()
        {
            return Numbers.Sum(row => row.Sum(x => x.wasDrawn ? 0 : x.num));
        }
    }
}
