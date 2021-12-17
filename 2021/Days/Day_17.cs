using Core;
using Core.Combinatorics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace AoC_2021.Days
{
    public class Day_17 : BaseDay
    {
        private Rectangle _area;
        private int _rightmostTarget;
        private int _bottommostTarget;
        private List<Velocity> _hits;

        public Day_17()
        {
            var input = File.ReadAllText(InputFilePath).ParseInts(4);
            var width = input[1] + 1 - input[0];
            var height = input[3] + 1 - input[2];
            _area = new Rectangle(input[0], input[2], width, height);
            _rightmostTarget = input[1];
            _bottommostTarget = input[2];
        }

        public override async ValueTask<string> Solve_1()
        {
            _hits = new List<Velocity>();
            for (int vx = 0; vx <= _rightmostTarget; vx++)
            {
                for (int vy = _bottommostTarget; vy < 1000; vy++)
                {
                    var v = new Velocity(vx, vy);
                    if (HitsTargetArea(v))
                        _hits.Add(v);
                }
            }
            return _hits.Max(CalculateMaxHeight).ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            return _hits.Count().ToString();
        }

        private int CalculateMaxHeight(Velocity v)
        {
            // s = v*t + 0.5*a*t*(t-1)
            //var a = -1;
            if (v.Y <= 0)
                return 0;
            else
                return (v.Y * (v.Y + 1)) / 2;
        }

        private bool HitsTargetArea(Velocity v)
        {
            var minT = (int)Math.Floor(CalcTime(_area.Bottom, v.Y));
            var maxT = (int)Math.Ceiling(CalcTime(_area.Top, v.Y));

            var xState = (pos: 0, speed: v.X);
            for (int i = 0; i < minT; i++)
                xState = NextX(xState);

            for (int t = minT; t <= maxT; t++)
            {
                var posY = v.Y * t - (t * (t-1)) /2;
                if (_area.Contains(xState.pos, posY))
                    return true;
                xState = NextX(xState);
            }
            return false;

            static double CalcTime(int displacement, int speed)
            {
                var s = displacement;
                var v = speed;
                var a = -1.0;

                var tmp = Math.Sqrt(a * a - 4 * a * v + 8 * a * s + 4 * v * v);
                var t = (-tmp + a - 2 * v) / (2 * a);
                return t;
            }

            (int pos, int speed) NextX((int pos, int speed) x)
                => (x.pos + x.speed, x.speed > 0 ? x.speed - 1 : 0);
        }
    }

    public readonly record struct Velocity(int X, int Y)
    {
        public static Point operator +(Point p, Velocity v)
            => new(p.X + v.X, p.Y + v.Y);

        public static Point operator -(Point p, Velocity v)
            => new(p.X - v.X, p.Y - v.Y);

        public static Velocity operator -(Velocity v)
            => new(-v.X, -v.Y);

        public static Velocity operator *(Velocity v, int factor)
            => new(v.X * factor, v.Y * factor);
    }
}
