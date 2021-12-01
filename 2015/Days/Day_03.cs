using System.Collections.Immutable;
using System.Drawing;
using System.IO;
using System.Linq;

using Core;
using Core.Combinatorics;

using MoreLinq.Extensions;

namespace AoC_2015.Days
{
    public class Day_03 : BaseDay
    {
        private readonly Direction[] _input;

        public Day_03()
        {
            _input = File.ReadAllText(InputFilePath).Select(c => c switch
            {
                '>' => Direction.Right,
                '^' => Direction.Up,
                '<' => Direction.Left,
                'v' => Direction.Down,
            }).ToArray();
        }

        public override async ValueTask<string> Solve_1()
        {
            var currentPos = new Point(0, 0);
            var positions = new List<Point> { currentPos };

            foreach (var dir in _input)
            {
                currentPos += dir.ToSize();
                positions.Add(currentPos);
            }
            return new HashSet<Point>(positions).Count.ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var currentPos = new Point(0, 0);
            var positions = new List<Point> { currentPos };

            var emptyList = ImmutableList.Create(new Point(0, 0));

            // Funktional, aber schlechte performance
            var santasHouses = _input.StepBy(2)
                .Aggregate(emptyList, (list, dir) => list.Add(list.Last() + dir.ToSize()));

            var roboSantasHouses = _input.StepBy(2, 1)
                .Aggregate(emptyList, (list, dir) => list.Add(list.Last() + dir.ToSize()));

            var allHouses = new HashSet<Point>(santasHouses);
            allHouses.UnionWith(roboSantasHouses);

            return allHouses.Count.ToString();

            //foreach (var dir in _input.StepBy(2))
            //{
            //    currentPos += dir.ToSize();
            //    positions.Add(currentPos);
            //}
            //currentPos = new Point(0, 0);
            //foreach (var dir in _input.StepBy(2, 1))
            //{
            //    currentPos += dir.ToSize();
            //    positions.Add(currentPos);
            //}
            //return new HashSet<Point>(positions).Count.ToString();
        }
    }
}
