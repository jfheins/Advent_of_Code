using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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

        private static readonly Dictionary<Direction8, Size> _mapDirection8ToSize = new()
        {
            { Direction8.UpLeft , new Size(-1, -1) },
            { Direction8.Up , new Size( 0, -1) },
            { Direction8.UpRight , new Size( 1, -1) },
            { Direction8.Left , new Size(-1,  0) },
            { Direction8.Right , new Size( 1,  0) },
            { Direction8.DownLeft , new Size(-1,  1) },
            { Direction8.Down , new Size( 0,  1) },
            { Direction8.DownRight , new Size( 1,  1) },
        };

        public static Point MoveTo(this Point p, Direction dir, int steps = 1) => p + (steps * _mapDirectionToSize[dir]);
        public static Point MoveBy(this Point p, int dx, int dy) => p + new Size(dx, dy);
        public static Point Minus(this Point a, Point b) => new(a.X - b.X, a.Y - b.Y);

        public static IEnumerable<Point> MoveLURD(this Point p)
            => _mapDirectionToSize.Values.Select(s => p + s);
        public static IEnumerable<Point> MoveLURDDiag(this Point p)
             => _mapDirection8ToSize.Values.Select(s => p + s);
        public static int Manhattan(this Point p) => Math.Abs(p.X) + Math.Abs(p.Y);
        public static int MaxAbs(this Point p) => Math.Max(Math.Abs(p.X), Math.Abs(p.Y));
        public static int ManhattanDistTo(this Point p, Point other)
            => Math.Abs(p.X - other.X) + Math.Abs(p.Y - other.Y);
        public static int ManhattanDistTo(this Point3 p, Point3 other)
            => Math.Abs(p.X - other.X) + Math.Abs(p.Y - other.Y) + Math.Abs(p.Z - other.Z);

        public static float EuklidDistTo(this Point3 p, Point3 other)
        {
            return MathF.Sqrt(Sqr(p.X, other.X) + Sqr(p.Y, other.Y) + Sqr(p.Z, other.Z));
            static int Sqr(int a, int b) => (a - b) * (a - b);
        }

        public static Point TurnClockwise(this Point p, int degrees)
        {
            var rad = degrees * Math.PI / 180;
            var x = p.X * Math.Cos(rad) - p.Y * Math.Sin(rad);
            var y = p.X * Math.Sin(rad) + p.Y * Math.Cos(rad);
            return new Point(Convert.ToInt32(x), Convert.ToInt32(y));
        }
        public static Point TurnCounterClockwise(this Point p, int degrees) => p.TurnClockwise(-degrees);

        public static (Point whole, Size rem) DivMod(this Point p, int x, int y)
        {
            var rem = new Size(p.X.Modulo(x), p.Y.Modulo(y));
            return (p - rem, rem);
        }
    }
}
