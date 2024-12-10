using Core;

namespace AoC_2024.Days;

public sealed class Day_09 : BaseDay
{
    private readonly string[] _input;

    public Day_09()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        // var grid = new FiniteGrid2D<char>(_input);
        var line = _input[0];

        var mem = new List<int>();
        var fileId = 0;
        for (int i = 0; i < line.Length; i++)
        {
            if (i % 2 == 0)
            {
                mem.AddRange(Enumerable.Repeat(fileId++, line[i] - '0'));
            }
            else
            {
                mem.AddRange(Enumerable.Repeat(-1, line[i] - '0'));
            }
        }
        //  Console.WriteLine(string.Concat(mem.Select(it => it == -1 ? '.' : it.ToString()[^1])));

        var lastIdx = mem.Count - 1;
        for (int i = 0; i < mem.Count; i++)
        {
            if (mem[i] == -1)
            {
                // Move
                mem[i] = mem[lastIdx];
                mem[lastIdx] = -1;
                while (mem[lastIdx] == -1)
                    lastIdx--;
            }

            //   Console.WriteLine(string.Concat(mem.Select(it => it == -1 ? '.' : it.ToString()[^1])));
            if (i >= lastIdx)
            {
                break;
            }
        }
        //   Console.WriteLine(string.Concat(mem.Select(it => it == -1 ? '.' : it.ToString()[^1])));

        var cs = mem.Where(it => it >= 0).Select((x, i) => (long)x * i).Sum();

        return cs.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        // var grid = new FiniteGrid2D<char>(_input);
        var line = _input[0];

        var mem = new List<int>();
        var fileIdx = 0;
        for (int i = 0; i < line.Length; i++)
        {
            if (i % 2 == 0)
            {
                mem.AddRange(Enumerable.Repeat(fileIdx++, line[i] - '0'));
            }
            else
            {
                mem.AddRange(Enumerable.Repeat(-1, line[i] - '0'));
            }
        }

      //  Console.WriteLine(string.Concat(mem.Select(it => it == -1 ? '.' : it.ToString()[^1])));


        //     var freeSpace = new List<(int idx, int len)>(); // index to size
        var runs = new List<(int idx, int elem, int len)>();
        AssignRuns();

        var maxFileId = runs.Max(it => it.elem);
        for (int fileId = maxFileId; fileId >= 0; fileId--)
        {
            var file = runs.Find(it => it.elem == fileId);
            var spaceIdx = runs.FindIndex(it => it.elem == -1 && it.len >= file.len);
            if (spaceIdx == -1)
                continue;
            var space = runs[spaceIdx];
            if (space.idx > file.idx)
                continue;
            for (int k = file.idx; k < file.idx + file.len; k++)
            {
                mem[k] = -1; // remove old file
            }

            for (int j = space.idx; j < space.idx + file.len; j++)
            {
                mem[j] = file.elem; // add new file
            }

            AssignRuns();
        }

     //   Console.WriteLine(string.Concat(mem.Select(it => it == -1 ? '.' : it.ToString()[^1])));

        var cs = mem.Index().Where(it => it.Item >= 0).Select(x => (long)x.Item * x.Index).Sum();

        return cs.ToString();


        void AssignRuns()
        {
            runs.Clear();
            var runIdx = 0;
            foreach (var r in mem.Runs())
            {
                runs.Add((runIdx, r.Element, r.Count));
                runIdx += r.Count;
            }
        }
    }
}