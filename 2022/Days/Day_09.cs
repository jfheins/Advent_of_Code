using System;
using System.Drawing;
using System.Numerics;

using Core;


namespace AoC_2022.Days
{
    public sealed class Day_09 : BaseDay
    {
        private readonly List<(char dir, int dist)> _input;
        private readonly Dictionary<char, Size> _charToSize = new()
        {
                { 'L', Direction.Left.ToSize() },
                { 'U', Direction.Up.ToSize() },
                { 'R', Direction.Right.ToSize() },
                { 'D', Direction.Down.ToSize() }
        };

        public Day_09()
        {
            _input = File.ReadAllLines(InputFilePath)
                .Select(line => (line[0], int.Parse(line[2..])))
                .ToList();
        }

        public override async ValueTask<string> Solve_1()
        {
            return Solve(2).ToString();
        }

        private int Clip(int x) => Math.Sign(x);

        public override async ValueTask<string> Solve_2()
        {
            return Solve(10).ToString();
        }

        private int Solve(int ropeLength)
        {
            var rope = new Point[ropeLength];
            var visited = new HashSet<Point>();

            foreach (var (dir, dist) in _input)
            {
                for (var i = 0; i < dist; i++)
                {
                    rope[0] += _charToSize[dir];
                    for (var moveIdx = 1; moveIdx < rope.Length; moveIdx++)
                    {
                        var diff = rope[moveIdx - 1].Minus(rope[moveIdx]);
                        if (diff.MaxAbs() > 1)
                            rope[moveIdx] = rope[moveIdx].MoveBy(Clip(diff.X), Clip(diff.Y));
                    }
                    visited.Add(rope.Last());
                }
            }

            return visited.Count;
        }
    }
}