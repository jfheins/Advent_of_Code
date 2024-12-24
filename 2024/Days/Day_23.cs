using Core;

namespace AoC_2024.Days;

public sealed class Day_23 : BaseDay
{
    private readonly string[] _input;

    public Day_23()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var co = new Dictionary<string, List<string>>();

        foreach (var line in _input)
        {
            var p = line.Split("-");
            co.AddOrModifyAction(p[0], () => new(), l => l.Add(p[1]));
            co.AddOrModifyAction(p[1], () => new(), l => l.Add(p[0]));
        }

        HashSet<string> tri = new();
        foreach (var computer in co.Keys.Where(it => it.StartsWith("t")))
        {
            CountTriangles(computer);
        }
        return tri.Count.ToString();


        void CountTriangles(string c)
        {
            var others = co[c].ToHashSet();
            foreach (var two in co[c])
            {
                foreach (var pt in co[two])
                {
                    if (others.Contains(pt))
                    {
                        var name = new[] { c, two, pt };
                        tri.Add(string.Join("-", name.Order()));
                    }
                }
            }
        }
    }

    public override async ValueTask<string> Solve_2()
    {
        var co = new Dictionary<string, List<string>>();

        foreach (var line in _input)
        {
            var p = line.Split("-");
            co.AddOrModifyAction(p[0], () => new(), l => l.Add(p[1]));
            co.AddOrModifyAction(p[1], () => new(), l => l.Add(p[0]));
        }

        HashSet<string> tri = new();
        foreach (var computer in co.Keys)
        {
            CountTriangles(computer);
        }

        var ccc = new Dictionary<string, int>();
        foreach (var t in tri)
        {
            foreach (var node in t.Split("-"))
            {
                ccc.AddOrModify(node, 0, x => x+1);
            }
        }

        var max = ccc.Values.Max();

        var allNodes = ccc.Where(k => k.Value == max).Select(k => k.Key);
        
        return string.Join(",", allNodes.Order());


        void CountTriangles(string c)
        {
            var others = co[c].ToHashSet();
            foreach (var two in co[c])
            {
                foreach (var pt in co[two])
                {
                    if (others.Contains(pt))
                    {
                        var name = new[] { c, two, pt };
                        tri.Add(string.Join("-", name.Order()));
                    }
                }
            }
        }
    }
}