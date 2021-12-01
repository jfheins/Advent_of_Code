using System.IO;
using System.Linq;

using Core;
using Core.Combinatorics;

using static MoreLinq.Extensions.WindowExtension;

namespace AoC_2015.Days
{
    public class Day_02 : BaseDay
    {
        private readonly List<int[]> _input;

        public Day_02()
        {
            _input = File.ReadAllLines(InputFilePath).Select(line => line.ParseNNInts(3)).ToList();
        }

        private IEnumerable<int> FaceAreas(int[] s)
        {
            yield return s[0] * s[1];
            yield return s[1] * s[2];
            yield return s[2] * s[0];
        }

        private IEnumerable<int> FacePerimeters(int[] s)
        {
            yield return 2 * (s[0] + s[1]);
            yield return 2 * (s[1] + s[2]);
            yield return 2 * (s[2] + s[0]);
        }

        private int CalculateArea(int[] s) => 2 * FaceAreas(s).Sum();
        private int SmallestFace(int[] s) => FaceAreas(s).Min();

        private long Volume(int[] s) => s.Product();

        public override async ValueTask<string> Solve_1()
        {
            return _input.Select(gift => CalculateArea(gift) + SmallestFace(gift)).Sum().ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            return _input.Select(gift => FacePerimeters(gift).Min() + Volume(gift)).Sum().ToString();
        }
    }
}
