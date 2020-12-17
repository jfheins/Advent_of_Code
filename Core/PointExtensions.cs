﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Core
{

    public static class PointExtensions
    {
        private static readonly Dictionary<Direction, Size> _mapDirectionToSize = new()
        {
            { Direction.Left, new Size(-1, 0) },
            { Direction.Up, new Size(0, -1) },
            { Direction.Right, new Size(1, 0) },
            { Direction.Down, new Size(0, 1) }
        };

        public static Point MoveTo(this Point p, Direction dir, int steps = 1) => p + (steps * _mapDirectionToSize[dir]);
        public static IEnumerable<Point> MoveLURD(this Point p)
            => ((IEnumerable<Size>)_mapDirectionToSize.Values).Select(s => p + s);
        public static IEnumerable<Point> MoveLURDDiag(this Point p)
        {
            var sizes = new[] {  new Size(-1, -1), new Size(0, -1),  new Size(1, -1),
                new Size(-1, 0), new Size(1, 0),
                new Size(-1, 1), new Size(0, 1),  new Size(1, 1),
            };
            return sizes.Select(s => p + s);
        }
        public static int Manhattan(this Point p) => Math.Abs(p.X) + Math.Abs(p.Y);
        public static Point TurnClockwise(this Point p, int degrees)
        {
            var rad = degrees * Math.PI / 180;
            var x = p.X * Math.Cos(rad) - p.Y * Math.Sin(rad);
            var y = p.X * Math.Sin(rad) + p.Y * Math.Cos(rad);
            return new Point(Convert.ToInt32(x), Convert.ToInt32(y));
        }
        public static Point TurnCounterClockwise(this Point p, int degrees) => p.TurnClockwise(-degrees);
    }
}
