using Core;
using Core.Combinatorics;
using System.IO;
using System.Linq;

namespace AoC_2021.Days
{
    public class Day_12 : BaseDay
    {
        private readonly List<(string src, string dest)> _input;

        public Day_12()
        {
            _input = File.ReadAllLines(InputFilePath).Select(ParseLine).ToList();
        }

        private (string src, string dest) ParseLine(string line)
        {
            return line.Split("-").ToTuple2();
        }

        Dictionary<string, List<string>> edges = new();

        public override async ValueTask<string> Solve_1()
        {
            foreach (var (src, dest) in _input)
            {
                edges.GetOrAdd(src, new List<string>()).Add(dest);
                edges.GetOrAdd(dest, new List<string>()).Add(src);
            }

            var paths = new List<List<string>>();
            Explore(new List<string> { "start" }, paths, Expand1);
            return paths.Count.ToString();
        }

        private void Explore(List<string> path, List<List<string>> paths,
            Func<List<string>, IEnumerable<string>> expander)
        {
            if (path.Last() == "end")
            {
                paths.Add(path.ToList());
            }
            else
            {
                foreach (var n in expander(path).ToList())
                {
                    path.Add(n);
                    Explore(path, paths, expander);
                    path.RemoveAt(path.Count - 1);
                }
            }
        }

        private IEnumerable<string> Expand1(List<string> path)
        {
            var nei = edges[path.Last()];
            var seenSmallCaves = path.Where(x => char.IsLower(x[0])).ToHashSet();
            return nei.Where(it => !seenSmallCaves.Contains(it));
        }

        private IEnumerable<string> Expand2(List<string> path)
        {
            var nei = edges[path.Last()];
            var seenSmallCaves = path.Where(x => char.IsLower(x[0])).ToList();

            if (ContainsDuplicates(seenSmallCaves))
            {
                return nei.Where(it => !seenSmallCaves.Contains(it));
            }
            return nei.Where(it => it != "start");
        }
        public static bool ContainsDuplicates<T>(IEnumerable<T> enumerable)
        {
            var knownKeys = new HashSet<T>();
            return enumerable.Any(item => !knownKeys.Add(item));
        }

        public override async ValueTask<string> Solve_2()
        {

            var paths = new List<List<string>>();
            Explore(new List<string> { "start" }, paths, Expand2);
            return paths.Count.ToString();
        }
    }
}
