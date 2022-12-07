using Core;

using static MoreLinq.Extensions.WindowExtension;

namespace AoC_2022.Days
{
    public sealed class Day_07 : BaseDay
    {
        private readonly string[] _input;
        private Dictionary<string, Item> tree;

        public Day_07()
        {
            _input = File.ReadAllLines(InputFilePath);
        }

        private record Item(string Path, bool IsFile, Item? Parent)
        {
            public long Size { get; set; }
        }

        public override async ValueTask<string> Solve_1()
        {
            tree = new Dictionary<string, Item>();
            var currentPath = Path.Combine("D:\\");
            var currentDir = new Item(currentPath, false, null);
            tree.Add(currentPath, currentDir);
            foreach (var line in _input)
            {
                if (line.StartsWith("$ cd"))
                {
                    currentPath = Path.GetFullPath(Path.Combine(currentPath, line[5..]));
                    currentDir = 
                        tree.GetOrAdd(currentPath, p => new Item(p, false, currentDir));
                }
                else if (line[0] != '$')
                {
                    if (!line.StartsWith("dir"))
                    {
                        var details = line.Split(" ");
                        var path = Path.GetFullPath(Path.Combine(currentPath, details[1]));
                        tree.Add(path, new Item(path, true, currentDir)
                        { Size = int.Parse(details[0]) });
                        var p = currentDir;
                        do
                        {
                            p.Size += long.Parse(details[0]);
                            p = p.Parent;
                        } while (p != null);
                    }

                }
            }

            var dirs = tree.Values.Where(kvp => !kvp.IsFile && kvp.Size <= 100000).ToList();

            return dirs
                .Sum(kvp => kvp.Size).ToString();
        }

        public override async ValueTask<string> Solve_2() {

            var exSpace = 70000000 - tree.Values.Max(x => x.Size);
            var neededSpace = 30000000 - exSpace;

            var toDelete = tree.Values.Where(kvp => !kvp.IsFile && kvp.Size >= neededSpace)
                .MinBy(it => it.Size);

            return toDelete.Size.ToString();
        }
    }
}