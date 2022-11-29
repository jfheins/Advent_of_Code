using Core;

namespace AoC_2015.Days
{
    public class Day_02 : BaseDay
    {
        private readonly List<int[]> _input;

        public Day_02()
        {
            _input = File.ReadAllLines(InputFilePath).Select(line => line.ParseNNInts(3)).ToList();
        }

        private static IEnumerable<int> ShiftLeft(IEnumerable<int> source)
        {
            return source.Skip(1).Append(source.First());
        }

        private IEnumerable<(int, int)> ZipWithNextWraparound(IEnumerable<int> source)
        {
            var all = source.ToList();
            return all.Select((x, i) => (x, all[(i + 1) % all.Count]));
        }

        private IEnumerable<int> FaceAreas(int[] s)
            => s.Zip(ShiftLeft(s)).Select(t => t.First * t.Second);

        private IEnumerable<int> FacePerimeters(int[] s)
            => s.Zip(ShiftLeft(s)).Select(t => 2* (t.First + t.Second));

        private int CalculateArea(int[] s) => 2 * FaceAreas(s).Sum();
        private int SmallestFace(int[] s) => FaceAreas(s).Min();

        private long Volume(int[] s) => s.Product();

        public override async ValueTask<string> Solve_1()
        {
            return _input
                .Select(gift => CalculateArea(gift) + SmallestFace(gift)).Sum().ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var xx = ShiftLeft(FaceAreas(_input[0])).ToList();
            return _input
                .Select(gift => FacePerimeters(gift).Min() + Volume(gift)).Sum().ToString();
        }
    }
}
