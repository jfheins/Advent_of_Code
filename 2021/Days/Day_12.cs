using Core;
using Core.Combinatorics;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AoC_2021.Days
{
    public class Day_12 : BaseDay
    {
        private readonly List<(string src, string dest)> _input;
        readonly Dictionary<string, List<string>> Edges = new();

        public Day_12()
        {
            _input = File.ReadAllLines(InputFilePath).Select(ParseLine).ToList();
        }

        private (string src, string dest) ParseLine(string line) => line.Split("-").ToTuple2();

        public override async ValueTask<string> Solve_1()
        {
            foreach (var (src, dest) in _input)
            {
                Edges.AddToList(src, dest);
                Edges.AddToList(dest, src);
            }

            return Explore("start", false).ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            return Explore("start", true).ToString();
        }

        private int Explore(string start, bool allowOneDuplicate)
        {
            var pathCount = 0; 
            var seenSmallCaves = new HashSet<string>();

            ExploreRecursively(start, allowOneDuplicate);
            return pathCount;

            void ExploreRecursively(string node, bool allowDuplicate)
            {
                if (node == "end")
                {
                    pathCount++;
                    return;
                }

                foreach (var (next, isSmall) in Expand(node))
                {
                    if (isSmall)
                    {
                        var wasNew = seenSmallCaves.Add(next);

                        if (allowDuplicate)
                            ExploreRecursively(next, wasNew);
                        else if (wasNew)
                            ExploreRecursively(next, allowDuplicate);

                        if (wasNew)
                            seenSmallCaves.Remove(next);
                    }
                    else
                        ExploreRecursively(next, allowDuplicate);
                }
            }
        }

        private IEnumerable<(string neighbor, bool isSmall)> Expand(string currentCave)
        {
            var neighbors = Edges[currentCave];
            return neighbors.ExceptFor("start").Select(n => (n, IsSmallCave(n)));

            static bool IsSmallCave(string s) => char.IsLower(s[0]);
        }
    }
}
