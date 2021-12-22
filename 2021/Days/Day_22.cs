using Core;

namespace AoC_2021.Days
{
    public class Day_22 : BaseDay
    {
        private readonly IReadOnlyList<(Cuboid volume, string cmd)> _steps;

        public Day_22()
        {
            var input = File.ReadAllLines(InputFilePath);
            _steps = input.Select(ParseCuboid).ToList();
        }

        public override async ValueTask<string> Solve_1()
        {
            var regionOfInterest = new Cube(new Point3(-50, -50, -50), 101);
            return CalculateOnCubes(_steps.SelectMany(IntersectWithRoI)).ToString();

            IEnumerable<(Cuboid volume, string cmd)> IntersectWithRoI((Cuboid volume, string cmd) t)
            {
                var intersection = t.volume.Intersect(regionOfInterest);
                if (intersection != null)
                    yield return (intersection, t.cmd);
            }
        }

        public override async ValueTask<string> Solve_2()
        {
            return CalculateOnCubes(_steps).ToString();
        }

        private (Cuboid volume, string cmd) ParseCuboid(string line)
        {
            var cmd = line[0..2];
            var ints = line.ParseInts(6);
            var volume = new Cuboid
            {
                Location = new Point3(ints[0], ints[2], ints[4]),
                Width = ints[1] - ints[0] + 1,
                Height = ints[3] - ints[2] + 1,
                Depth = ints[5] - ints[4] + 1
            };
            return (volume, cmd);
        }

        private static long CalculateOnCubes(IEnumerable<(Cuboid volume, string cmd)> input)
        {
            var volumes = new Dictionary<Cuboid, int>();
            foreach (var (cuboid, cmd) in input)
            {
                // Level the playing field and "remove" any existing on areas
                // either by deleting (if the same size exists) or by adding with opposite sign.
                // in case some areas have also been switched off later, we need to cancel that as well
                foreach (var (previous, sign) in volumes.ToList())
                {
                    var intersection = previous.Intersect(cuboid);
                    if (intersection != null)
                    {
                        var cancelSign = -sign;
                        volumes.AddOrModify(intersection, 0, x => x + cancelSign);
                    }
                }
                if (cmd == "on")
                    volumes.AddOrModify(cuboid, 0, val => val + 1);
            }
            return volumes.Sum(kvp => kvp.Key.Size * kvp.Value);
        }
    }
}
