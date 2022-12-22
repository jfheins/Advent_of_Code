using Core;

using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;

namespace AoC_2022.Days
{
    public sealed class Day_22 : BaseDay
    {
        private readonly FiniteGrid2D<char> _map;
        private readonly string[] _moves;

        private enum Turn { Clockwise = 1, Twice = 2, CounterCl = -1 };

        public Day_22()
        {
            var input = File.ReadAllLines(InputFilePath);
            _map = new FiniteGrid2D<char>(input[0..^1]);
            _map.FillGaps(' ');
            _moves = Regex.Split(input.Last(), @"(R|L)");
        }

        public override async ValueTask<string> Solve_1()
        {
            var start = _map.GetRow(0, ' ').IndexWhere(it => it != ' ').First();
            var face = Direction.Right;
            var pos = new Point(start, 0);

            foreach (var move in _moves)
            {
                if (move == "R")
                    face = face.TurnClockwise();
                else if (move == "L")
                    face = face.TurnCounterClockwise();
                else
                {
                    var steps = int.Parse(move);
                    for (int i = 0; i < steps; i++)
                    {
                        var nextTile = _map.GetTupleWraparound(pos.MoveTo(face, 1));
                        if (nextTile.value == ' ')
                        {
                            if (face == Direction.Right)
                                nextTile = _map.GetRowTuple(pos.Y, ' ').First(t => t.value != ' ');
                            if (face == Direction.Left)
                                nextTile = _map.GetRowTuple(pos.Y, ' ').Last(t => t.value != ' ');
                            if (face == Direction.Down)
                                nextTile = _map.GetColTuple(pos.X, ' ').First(t => t.value != ' ');
                            if (face == Direction.Up)
                                nextTile = _map.GetColTuple(pos.X, ' ').Last(t => t.value != ' ');
                        }
                        if (nextTile.value != '#')
                            pos = nextTile.pos;
                    }
                }
                Console.WriteLine(pos);
            }

            var solution = 1000 * (pos.Y + 1) + 4 * (pos.X + 1);
            solution += face switch
            {
                Direction.Right => 0,
                Direction.Down => 1,
                Direction.Left => 2,
                Direction.Up => 3,
                _ => throw new NotImplementedException(),
            };
            return solution.ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var start = _map.GetRow(0, ' ').IndexWhere(it => it != ' ').First();
            var face = Direction.Right;
            var pos = new Point(start, 0);

            foreach (var move in _moves)
            {
                if (move == "R")
                    face = face.TurnClockwise();
                else if (move == "L")
                    face = face.TurnCounterClockwise();
                else
                {
                    var steps = int.Parse(move);
                    for (int i = 0; i < steps; i++)
                    {
                        (pos, face) = MoveAndWrapOnCube(pos, face);
                        Debug.Assert(_map[pos] == '.');
                        //_map[pos] = face switch
                        //{
                        //    Direction.Left => '<',
                        //    Direction.Right => '>',
                        //    Direction.Up => '^',
                        //    Direction.Down => 'v'
                        //};
                    }
                }
            }

            var solution = 1000 * (pos.Y + 1) + 4 * (pos.X + 1);
            solution += face switch
            {
                Direction.Right => 0,
                Direction.Down => 1,
                Direction.Left => 2,
                Direction.Up => 3,
                _ => throw new NotImplementedException(),
            };
            return solution.ToString();
            // 95380 too high
        }

        private (Point pos, Direction facing) MoveAndWrapOnCube(Point pos, Direction face)
        {
            var nextPos = pos.MoveTo(face, 1);
            var nextTile = _map.GetValueOrDefault(nextPos, ' ');
            if (nextTile == '#')
                return (pos, face);
            if (nextTile != ' ')
                return (nextPos, face);
            else
            {
                var exitTile = (x: pos.X / 50, y: pos.Y / 50);
                var nextFace = face;
                if (face == Direction.Right)
                {
                    if (exitTile == (2, 0))
                    {
                        nextFace = Rotate(Turn.Twice);
                        nextPos = FlipY(Translate(-1, 2));
                    }
                    if (exitTile == (1, 1))
                    {
                        nextFace = Rotate(Turn.CounterCl);
                        nextPos = FlipXYCC(Translate(1, -1));
                    }
                    if (exitTile == (1, 2))
                    {
                        nextFace = Rotate(Turn.Twice);
                        nextPos = FlipY(Translate(1, -2));
                    }
                    if (exitTile == (0, 3))
                    {
                        nextFace = Rotate(Turn.CounterCl);
                        nextPos = FlipXYCC(Translate(1, -1));
                    }
                }
                if (face == Direction.Left)
                {
                    if (exitTile == (1, 0))
                    {
                        nextFace = Rotate(Turn.Twice);
                        nextPos = FlipY(Translate(-1, 2));
                    }
                    if (exitTile == (1, 1))
                    {
                        nextFace = Rotate(Turn.CounterCl);
                        nextPos = FlipXYCC(Translate(-1, 1));
                    }
                    if (exitTile == (0, 2))
                    {
                        nextFace = Rotate(Turn.Twice);
                        nextPos = FlipY(Translate(1, -2));
                    }
                    if (exitTile == (0, 3))
                    {
                        nextFace = Rotate(Turn.CounterCl);
                        nextPos = FlipXYCC(Translate(1, -3));
                    }
                }

                if (face == Direction.Down)
                {
                    if (exitTile == (0, 3))
                    {
                        nextFace = Rotate(Turn.Twice);
                        nextPos = TurnXY(Translate(2, -3));
                    }
                    if (exitTile == (1, 2))
                    {
                        nextFace = Rotate(Turn.Clockwise);
                        nextPos = FlipXYClockwise(Translate(-1, 1));
                    }
                    if (exitTile == (2, 0))
                    {
                        nextFace = Rotate(Turn.Clockwise);
                        nextPos = FlipXYClockwise(Translate(-1, 1));
                    }
                }

                if (face == Direction.Up)
                {
                    if (exitTile == (0, 2))
                    {
                        nextFace = Rotate(Turn.Clockwise);
                        nextPos = FlipXYClockwise(Translate(1, -1));
                    }
                    if (exitTile == (1, 0))
                    {
                        nextFace = Rotate(Turn.Clockwise);
                        nextPos = FlipXYClockwise(Translate(-1, 3));
                    }
                    if (exitTile == (2, 0))
                    {
                        nextFace = Rotate(Turn.Twice);
                        nextPos = TurnXY(Translate(-2, 3));
                    }
                }
                if (_map[nextPos] == '#')
                    return (pos, face);
                else
                    return (nextPos, nextFace);

                throw new Exception("Unknown tile" + exitTile);
            }
            throw new Exception("Unknown tile");

            Direction Rotate(Turn t) => (Direction)((int)face + (int)t).Modulo(4);
            Point Translate(int dx, int dy) => pos.MoveBy(dx * 50, dy * 50);
            Point FlipY(Point p)
            {
                var offs = p.Y % 50;
                var xx = 49 - offs;
                return new Point(p.X, p.Y - offs + xx);
            }
            Point FlipXYCC(Point p)
            {
                var xoff = p.X % 50;
                var yoff = p.Y % 50;
                return new Point(p.X - xoff + yoff, p.Y - yoff + xoff);
            }
            Point FlipXYClockwise(Point p) => FlipXYCC(FlipXYCC(FlipXYCC(p)));
            Point TurnXY(Point p) => FlipXYCC(FlipXYCC(p));
        }

    }
}