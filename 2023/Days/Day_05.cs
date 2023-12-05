using System.Diagnostics;
using Core;
using static MoreLinq.Extensions.SplitExtension;

namespace AoC_2023.Days;

public sealed class Day_05 : BaseDay
{
    private readonly long[] _seeds;
    private readonly List<MappingBlock> _mappings;

    public Day_05()
    {
        var input = File.ReadAllLines(InputFilePath);
        _seeds = input[0].ParseLongs();
        _mappings = input.Split("").Skip(1).SelectList(ParseMapping);
    }

    public override async ValueTask<string> Solve_1()
        => _seeds.Min(ExecuteAllMappings).ToString();

    public override async ValueTask<string> Solve_2()
        => _seeds
            .Chunk(2)
            .SelectList(range => LongInterval.FromStartAndLength(range[0], range[1]))
            .Min(ExecuteAllMappings).ToString();

    private static MappingBlock ParseMapping(IEnumerable<string> lines)
    {
        var chunk = lines.ToList();
        var r = chunk[1..].Select(line => line.ParseLongs(3)).SelectList(it => new MappingRange(it[0], it[1], it[2]));
        return new MappingBlock(r);
    }

    private record MappingBlock(List<MappingRange> Ranges)
    {
        public long Map(long number)
        {
            var map = Ranges.Find(range => range.Source.Contains(number));
            return map?.Map(number) ?? number;
        }
    }

    private record MappingRange
    {
        public LongInterval Source { get; }
        private long Destination { get; }

        public MappingRange(long to, long from, long length)
        {
            Destination = to;
            Source = LongInterval.FromStartAndLength(from, length);
        }

        public long Map(long x) => Destination + (x - Source.Start);

        public LongInterval Map(LongInterval it) => new(Map(it.Start), Map(it.End));
    }

    private long ExecuteAllMappings(long seed)
    {
        foreach (var mapping in _mappings)
            seed = mapping.Map(seed);
        return seed;
    }

    private long ExecuteAllMappings(LongInterval seedRange)
    {
        var sourceRanges = new List<LongInterval> { seedRange };
        var destRanges = new List<LongInterval>();

        foreach (var mapping in _mappings)
        {
            while (sourceRanges.TryPop(out var fragment))
            {
                var mappingRange = mapping.Ranges.Find(it => it.Source.OverlapsWith(fragment));
                if (mappingRange != null)
                    fragment = MapIntersection(mappingRange, fragment);
                destRanges.Add(fragment);
            }
            (sourceRanges, destRanges) = (destRanges, sourceRanges);

            LongInterval MapIntersection(MappingRange m, LongInterval fragment)
            {
                var (prefix, intersection, suffix) = fragment.Intersect(m.Source);
                Debug.Assert(intersection is not null);
                if (prefix != null) sourceRanges.Add(prefix.Value);
                if (suffix != null) sourceRanges.Add(suffix.Value);
                return m.Map(intersection.Value);
            }
        }
        return sourceRanges.Min(r => r.Start);
    }
}