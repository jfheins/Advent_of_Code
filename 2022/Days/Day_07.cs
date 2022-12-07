using LanguageExt;

namespace AoC_2022.Days
{
    public sealed class Day_07 : BaseDay
    {
        private readonly string[] _input;
        private Directory root = new Directory("", null);
        private IReadOnlyList<long> _folderSizes = Array.Empty<long>();

        public Day_07()
        {
            _input = File.ReadAllLines(InputFilePath);
        }

        private sealed record Directory(string Name, Directory? Parent)
        {
            public long Size { get; private set; }
            public ICollection<Directory> Children { get; } = new List<Directory>();

            public Directory MakeChild(string name)
            {
                var item = new Directory(name, this);
                Children.Add(item);
                return item;
            }

            public void AddToSize(long size)
            {
                Size += size;
                Parent?.AddToSize(size);
            }

            public void AddContent(string line)
            {
                var sizeStr = line[..(line.IndexOf(' '))];
                AddToSize(long.Parse(sizeStr));
            }
        }

        public override async ValueTask<string> Solve_1()
        {
            Directory? current = null;

            foreach (var line in _input)
            {
                if (line.StartsWith("$ cd", StringComparison.Ordinal))
                    current = ChangeFolder(line[5..]);
                else if (char.IsAsciiDigit(line[0]))
                    current!.AddContent(line);
            }

            _folderSizes = AllFolderSizes().ToList();
            return _folderSizes.Where(it => it <= 100_000).Sum().ToString();

            Directory ChangeFolder(string folderName)
            {
                if (current == null)
                    return root = new Directory(folderName, null);

                return folderName == ".." && current.Parent != null
                    ? current.Parent
                    : current.MakeChild(folderName);
            }
        }

        public override async ValueTask<string> Solve_2()
        {
            var unusedSpace = 70000000 - root.Size;
            var neededSpace = 30000000 - unusedSpace;

            return _folderSizes.Where(it => it >= neededSpace).Min().ToString();
        }

        private IEnumerable<long> AllFolderSizes()
        {
            return Descendants(root).Select(it => it.Size);

            static IEnumerable<Directory> Descendants(Directory item)
                => item.Children.SelectMany(Descendants).Prepend(item);
        }
    }
}