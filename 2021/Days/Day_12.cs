using Core;

namespace AoC_2021.Days
{
    public class Day_12 : BaseDay
    {
        readonly Dictionary<int, List<int>> Edges = new();
        readonly List<string> NodeNames;
        readonly int Start;
        readonly int End;

        public Day_12()
        {
            var input = File.ReadAllLines(InputFilePath).Select(ParseLine).ToList();
            var stringEdges = new Dictionary<string, List<string>>();

            foreach (var (left, right) in input)
            {
                AddEdge(left, right);
                AddEdge(right, left);
            }
            NodeNames = stringEdges.Keys.ToList();
            Start = NodeNames.IndexOf("start");
            End = NodeNames.IndexOf("end");

            foreach (var smallCave in NodeNames.Where(IsSmallCave))
            {
                var srcIndex = NodeNames.IndexOf(smallCave);
                Edges[srcIndex] = stringEdges[smallCave].SelectMany(ResolveNode).ToList();
            }

            (string src, string dest) ParseLine(string line) => line.Split("-").ToTuple2();

            void AddEdge(string from, string to)
            {
                if (to != "start")
                    stringEdges.AddToList(from, to);
            }

            IEnumerable<int> ResolveNode(string caveName)
            {
                return IsSmallCave(caveName)
                    ? NodeNames.IndexOf(caveName).ToEnumerable()
                    : stringEdges[caveName].Select(it => NodeNames.IndexOf(it));
            }
        }
        static bool IsSmallCave(string s) => char.IsLower(s[0]);

        public override async ValueTask<string> Solve_1()
        {
            return Explore(false).ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            return Explore(true).ToString();
        }

        private int Explore(bool allowOneDuplicate)
        {
            var pathCount = 0;
            var visitedCaves = new HashSet<int>();
            ExploreRecursively(Start, allowOneDuplicate);
            return pathCount;

            void ExploreRecursively(int node, bool allowOneDuplicate)
            {
                if (node == End)
                {
                    pathCount++;
                    return;
                }

                foreach (var neighbor in Edges[node])
                {
                    var isExcitingNewCave = visitedCaves.Add(neighbor);

                    if (allowOneDuplicate || isExcitingNewCave)
                        ExploreRecursively(neighbor, allowOneDuplicate && isExcitingNewCave);

                    if (isExcitingNewCave)
                        visitedCaves.Remove(neighbor);
                }
            }
        }
    }
}
