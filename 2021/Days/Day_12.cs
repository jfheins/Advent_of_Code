using Core;

namespace AoC_2021.Days
{
    public class Day_12 : BaseDay
    {
        readonly Dictionary<string, List<string>> Edges = new();

        public Day_12()
        {
            var input = File.ReadAllLines(InputFilePath).Select(ParseLine).ToList();

            foreach (var (left, right) in input)
            {
                AddEdge(left, right);
                AddEdge(right, left);
            }

            (string src, string dest) ParseLine(string line) => line.Split("-").ToTuple2();

            void AddEdge(string from, string to)
            {
                if (to != "start")
                    Edges.AddToList(from, to);
            }
        }

        public override async ValueTask<string> Solve_1()
        {
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

            void ExploreRecursively(string node, bool allowOneDuplicate)
            {
                if (node == "end")
                {
                    pathCount++;
                    return;
                }

                foreach (var neighbor in Edges[node])
                {
                    if (IsSmallCave(neighbor))
                    {
                        var isExcitingNewCave = seenSmallCaves.Add(neighbor);

                        if (allowOneDuplicate || isExcitingNewCave)
                            ExploreRecursively(neighbor, allowOneDuplicate && isExcitingNewCave);

                        if (isExcitingNewCave)
                            seenSmallCaves.Remove(neighbor);
                    }
                    else
                        ExploreRecursively(neighbor, allowOneDuplicate);
                }
            }

            static bool IsSmallCave(string s) => char.IsLower(s[0]);
        }
    }
}
