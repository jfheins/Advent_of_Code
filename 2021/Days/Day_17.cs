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
        private List<(Size v, (int maxHeight, bool isHit) res)> _results;

        public Day_17()
        {
            var input = File.ReadAllText(InputFilePath).ParseInts(4);
            var width = input[1] + 1 - input[0];
            var height = input[3] + 1 - input[2];
            _area = new Rectangle(input[0], input[2], width, height);
        }

        public override async ValueTask<string> Solve_1()
        {
            var test = new List<Size>();
            for (int vx = 0; vx < 1000; vx++)
            {
                for (int vy = -100; vy < 1000; vy++)
                {
                    test.Add(new Size(vx, vy));
                }
            }
            _results = test.Select(x => (v: x, res: CalculateProbe(x))).ToList();
            var r = _results.Where(x => x.res.isHit)
                .MaxBy(x => x.res.maxHeight);
            return r.ToString();
        }

        private (int maxHeight, bool isHit) CalculateProbe(Size velocity)
        {
            var hasHit = false;
            var maxH = 0;
            var pos = new Point(0, 0);
            for (int i = 0; i < 1000; i++)
            {
                pos += velocity;
                var newVx = Math.Max(velocity.Width - 1, 0);
                velocity = new Size(newVx, velocity.Height - 1);
                if (_area.Contains(pos))
                    hasHit = true;
                maxH = Math.Max(maxH, pos.Y);
            }
            return (maxH, hasHit);
        }

        public override async ValueTask<string> Solve_2()
        {
            return _results.Where(x => x.res.isHit).Distinct().Count().ToString();
        }
    }
}
