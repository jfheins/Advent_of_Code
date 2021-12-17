using Core;
using Core.Combinatorics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace AoC_2021.Days
{
    public class Day_17 : BaseDay
    {
        private (int left, int right, int bottom, int top) _target;
        private Rectangle _area;

        public Day_17()
        {
            var input = File.ReadAllText(InputFilePath).ParseInts(4);
            var width = input[1] + 1 - input[0];
            var height = input[3] + 1 - input[2];
            _target = (left: input[0], right: input[1], bottom: input[2], top: input[3]);
            _area = new Rectangle(input[0], input[2], width, height);
        }

        public override async ValueTask<string> Solve_1()
        {
            // Vy is limited by the target area as the points down have
            // the same y value as the points up. So the maximum permissible 
            // y speed is the one that barely hits the bottom line of the target.
            var maxVy = _target.bottom;
            return (maxVy * (maxVy + 1) / 2).ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var hits = 0;

            var slope = _target.left / (double)_target.bottom;
            var globalMinVx = (int)Math.Floor((Math.Sqrt(8 * _target.left + 1) - 1) / 2);
            for (int vy = _target.bottom; vy < -_target.bottom; vy++)
            {
                var slopeBound = (int)Math.Floor(vy * slope);
                var localMinVx = Math.Max(slopeBound, globalMinVx);
                var previousHit = false;
                for (int vx = localMinVx; vx <= _target.right; vx++)
                {
                    var hitTarget = TestHit(vx, vy);
                    if (previousHit && !hitTarget)
                        break;
                    previousHit = hitTarget;
                }
            }

            return hits.ToString();

            bool TestHit(int vx, int vy)
            {
                if (HitsTargetArea(new Velocity(vx, vy)))
                {
                    hits++;
                    return true;
                }
                return false;
            }
        }

        private bool HitsTargetArea(Velocity v)
        {
            var minT = (int)Math.Floor(CalcTime(_target.top, v.Y));
            var maxT = (int)Math.Ceiling(CalcTime(_target.bottom, v.Y));

            for (int t = minT; t <= maxT; t++)
            {
                var posY = v.Y * t - (t * (t - 1)) / 2;
                var posX = CalcX(t);
                if (_area.Contains(posX, posY))
                    return true;
            }
            return false;

            static double CalcTime(int displacement, int speed)
            {
                var s = displacement;
                var v = speed;
                var a = -1.0;

                var tmp = Math.Sqrt(a * a - 4 * a * v + 8 * a * s + 4 * v * v);
                return (double)((-tmp + a - 2 * v) / (2 * a));
            }

            int CalcX(int time)
            {
                if (time >= v.X)
                    return v.X * (v.X + 1) / 2;
                else
                    return time * (2 * v.X - time + 1) / 2;
            }
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
