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

            var paths = new List<Path>();
            Explore(new Path("start", false), paths);
            return paths.Count.ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var paths = new List<Path>();
            Explore(new Path("start", true), paths);
            return paths.Count.ToString();
        }

        private void Explore(Path currentPath, List<Path> paths)
        {
            if (currentPath.LastNode == "end")
            {
                paths.Add(currentPath);
            }
            else
            {
                foreach (var n in Expand(currentPath))
                {
                    Explore(n, paths);
                }
            }
        }

        private IEnumerable<Path> Expand(Path p)
        {
            var neighbors = Edges[p.LastNode];
            var list = new List<Path>();
            foreach (var neighbor in neighbors.ExceptFor("start"))
            {
                if (IsSmallCave(neighbor))
                {
                    if (p.AllowOneDuplicate)
                        list.Add( p.ExtendBy(neighbor, !p.Nodes.Contains(neighbor)));
                    else if(!p.Nodes.Contains(neighbor))
                        list.Add(p.ExtendBy(neighbor));
                }
                else
                    list.Add(p.ExtendByLarge(neighbor));
            }
            return list;

            static bool IsSmallCave(string s) => char.IsLower(s[0]);
        }

        private record Path
        {
            public bool AllowOneDuplicate { get; init; }
            public ImmutableHashSet<string> Nodes { get; private init; }
            public string LastNode { get; private init; }

            public Path(string start, bool allowDuplicate)
                : this(ImmutableHashSet.Create<string>(), start, allowDuplicate)
            {
            }

            private Path(ImmutableHashSet<string> nodes, string lastNode, bool allowDuplicate)
            {
                AllowOneDuplicate = allowDuplicate;
                Nodes = nodes;
                LastNode = lastNode;
            }

            public Path ExtendBy(string node) => ExtendBy(node, AllowOneDuplicate);
            public Path ExtendBy(string node, bool allowDuplicate)
                => new(Nodes.Add(node), node, allowDuplicate);

            public Path ExtendByLarge(string node) => new(Nodes, node, AllowOneDuplicate);
        }
    }
}
