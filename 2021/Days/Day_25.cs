using Core;
using System.Drawing;

namespace AoC_2021.Days
{
    public class Day_25 : BaseDay
    {
        private FiniteGrid2D<char> _grid;

        public Day_25()
        {
            _grid = Grid2D.FromFile(InputFilePath);
        }

        public override async ValueTask<string> Solve_1()
        {
            var steps = 0;
            // Key = Source
            var moves = new Dictionary<Point, (Point to, char c)>();
            do
            {
                moves.Clear();
                steps++;
                var eastMoves = new HashSet<Point>();
                foreach (var cc in _grid.Where(it => it.value == '>'))
                {
                    var neighbor = _grid.GetTupleWraparound(cc.pos.MoveTo(Direction.Right));
                    if (neighbor.value == '.')
                    {
                        moves.Add(cc.pos, (neighbor.pos, '>'));
                        eastMoves.Add(neighbor.pos);
                    }
                }
                foreach (var cc in _grid.Where(it => it.value == 'v'))
                {
                    var neighbor = _grid.GetTupleWraparound(cc.pos.MoveTo(Direction.Down));
                    if ((neighbor.value == '.' && !eastMoves.Contains(neighbor.pos))
                        || (neighbor.value == '>' && moves.ContainsKey(neighbor.pos)))
                        moves.Add(cc.pos, (neighbor.pos, 'v'));
                }
                if (moves.Count > 0)
                {
                    foreach (var sourceField in moves.Keys)
                        _grid[sourceField] = '.';

                    foreach (var (to, c) in moves.Values)
                        _grid[to] = c;

                }
            } while (moves.Count > 0);
            return steps.ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            return "DIY";
        }
    }
}
