using Core;
using static MoreLinq.Extensions.SplitExtension;
using Spectre.Console;
using System.Drawing;

namespace AoC_2023.Days;

public sealed partial class Day_05 : BaseDay
{
    private readonly string[] _input;

    public Day_05()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var cat = _input.Split("");
        var cats = cat.Skip(1).SelectList(ParseCat);

        var seeds = _input[0].ParseLongs();
        var loc = seeds.SelectList(s => FindLocation(s, cats)).Min();

        return loc.ToString();
    }

    private long FindLocation(long seed, List<Cat> cats)
    {
        foreach (var c in cats)
        {
            foreach (var rn in c.Ranges)
            {
                if (rn.Contains(seed))
                {
                    seed = rn.Map(seed);
                    break;
                }
            }
        }
        return seed;
    }

    private Cat ParseCat(IEnumerable<string> b)
    {
        var r = b.Skip(1).Select(line => line.ParseLongs(3)).SelectList(it => new CRange(it[0], it[1], it[2]));
        return new Cat(b.First(), r);
    }

    record Cat(string Title, List<CRange> Ranges);
    record CRange(long To, long From, long Length)
    {
        public bool Contains(long x) => x >= From && x < (From + Length);
        public long Map(long x) => To + (x - From);

        public long LastInkl => From + Length - 1;

        internal bool Overlaps((long st, long len) seed)
        {
            return Contains(seed.st) || Contains(seed.st + seed.len - 1) || Cont(From) || Cont(From + Length - 1);

            bool Cont(long l) => l >= seed.st && l < (seed.st + seed.len);
        }
    }

    public override async ValueTask<string> Solve_2()
    {
        var cat = _input.Split("");
        var cats = cat.Skip(1).SelectList(ParseCat);

        var seeds = _input[0].ParseLongs().Chunk(2).SelectList(range => range.ToTuple2());
        var loc = new List<long>();
        foreach (var r in seeds)
        {
            loc.AddRange(FindLocation2(r, cats));
            Console.WriteLine("Done with " + r.Item1);
        }
        return loc.Min().ToString();
    }

    private IEnumerable<long> FindLocation2((long start, long len) seedRange, List<Cat> cats)
    {
        var cand = new HashSet<(long start, long len)>() { seedRange };
        var next = new HashSet<(long start, long len)>();
        foreach (var c in cats)
        {
            Console.WriteLine(c.Title);
            foreach (var fromm in cand)
            {
                var fragments = new Stack<(long start, long len)>();
                fragments.Push(fromm);

                while (fragments.Any())
                {
                    var frag = fragments.Pop();
                    var mapping = c.Ranges.FirstOrDefault(it => it.Overlaps(frag));
                    if (mapping != null)
                    {
                        var intersect = Intersect(mapping, frag);
                        //Console.WriteLine($"Src range {frag.start} - {frag.start + frag.len - 1} overlaps with");
                        //Console.WriteLine($"Map range {mapping.From} - {mapping.LastInkl} in {intersect.start}-{intersect.start + intersect.len - 1}");
                        next.Add((mapping.Map(intersect.start), intersect.len));
                        var prefix = intersect.start - frag.start;
                        if (prefix > 0)
                            fragments.Push((frag.start, prefix));
                        var suffix = LastItem(frag) - LastItem(intersect);
                        if (suffix > 0)
                            fragments.Push((LastItem(intersect)+1, suffix));
                    }
                    else
                    {
                        next.Add(frag);
                    }
                    Console.WriteLine(fragments.Count);
                }
            }
            cand = next;
            next = [];
        }
        return cand.Select(it => it.start);

        (long start, long len) Intersect(CRange rn, (long start, long len) from)
        {
            var start = Math.Max(rn.From, from.start);
            var end = Math.Min(rn.LastInkl, from.start + from.len - 1);
            return (start, end - start + 1);
        }

        long LastItem((long start, long len) x) => x.start + x.len - 1;
    }

}