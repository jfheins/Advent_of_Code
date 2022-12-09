using System;
using System.Drawing;
using System.Numerics;

using Core;


namespace AoC_2022.Days
{
    public sealed class Day_09 : BaseDay
    {
        private readonly string[] _input;
        public Day_09()
        {
            _input = File.ReadAllLines(InputFilePath);
        }

        public override async ValueTask<string> Solve_1()
        {
            var head = new Point(0, 0);
            var tail = head;
            var charMap = new Dictionary<char, Direction>
            {
                { 'R', Direction.Right },
                { 'L', Direction.Left },
                { 'U', Direction.Up },
                { 'D', Direction.Down } };

            var visited = new HashSet<Point>() { tail };

            foreach (var input in _input)
            {
                var step = input.Split(' ');
                var dir = charMap[input[0]];

                for (int i = 0; i < int.Parse(step[1]); i++)
                {
                    head = head + dir.ToSize();
                    var diff = Point.Subtract(head, (Size)tail);
                    if (diff.X != 0 && diff.Y != 0)
                    { // move diag
                        if (diff.Manhattan() > 2)
                        tail = tail.MoveBy(Clip(diff.X), Clip(diff.Y));
                    }
                    else if(diff.Manhattan() > 1)
                    { // move straight
                        tail += new Size(Clip(diff.X), Clip(diff.Y));
                    }

                    visited.Add(tail);
                }
            }

            return visited.Count.ToString();
        }

        private int Clip(int x) => x > 1 ? 1 : (x < -1 ? -1 : x);

        public override async ValueTask<string> Solve_2()
        {
            var rope = new Point[10];
            var charMap = new Dictionary<char, Direction>
            {
                { 'R', Direction.Right },
                { 'L', Direction.Left },
                { 'U', Direction.Up },
                { 'D', Direction.Down } };

            var visited = new HashSet<Point>();

            foreach (var input in _input)
            {
                var step = input.Split(' ');
                var dir = charMap[input[0]];

                for (int i = 0; i < int.Parse(step[1]); i++)
                {
                    rope[0] += dir.ToSize();
                    for (int moveIdx = 1; moveIdx < rope.Length; moveIdx++)
                    {
                        var prev = rope[moveIdx - 1];
                        var diff = Point.Subtract(prev, (Size)rope[moveIdx]);
                        if (diff.X != 0 && diff.Y != 0)
                        { // move diag
                            if (diff.Manhattan() > 2)
                                rope[moveIdx] = rope[moveIdx].MoveBy(Clip(diff.X), Clip(diff.Y));
                        }
                        else if (diff.Manhattan() > 1)
                        { // move straight
                            rope[moveIdx] += new Size(Clip(diff.X), Clip(diff.Y));
                        }
                    }
                    visited.Add(rope.Last());
                }
            }

            return visited.Count.ToString();
        }
    }
}