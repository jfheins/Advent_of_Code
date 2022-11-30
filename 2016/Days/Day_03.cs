using Core;

using static MoreLinq.Extensions.TransposeExtension;

namespace AoC_2016.Days
{
    public class Day_03 : BaseDay
    {
        private readonly List<int[]> _input;

        public Day_03()
        {
            _input = File.ReadAllLines(InputFilePath).Select(line => line.ParseInts(3)).ToList();
        }

        public override async ValueTask<string> Solve_1()
        {
            var allShapes = _input.Select(GetSidesOrdered);
            var triangles = allShapes.Count(x => x.a + x.b > x.c);
            return triangles.ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var blocks = _input.Chunk(3);
            var allShapes = blocks.SelectMany(GetSidesFromChunks);
            var triangles = allShapes.Where(x => x.a + x.b > x.c);
            return triangles.Count().ToString();
        }


        private static IEnumerable<(int a, int b, int c)> GetSidesFromChunks(int[][] lines)
        {
            return lines.Transpose().Select(it => it.Order().ToTuple3());
        }

        private static (int a, int b, int c) GetSidesOrdered(int[] line) => line.Order().ToTuple3();
    }
}
