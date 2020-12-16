using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Core;

using static MoreLinq.Extensions.SplitExtension;

namespace AoC_2020.Days
{
    internal record TicketField(string Name, List<(int Min, int Max)> Ranges)
    {
        public bool IsValid(int value) => Ranges.Any(r => r.Min <= value && value <= r.Max);
        public bool IsValidForAll(IEnumerable<int> values) => values.All(v => IsValid(v));
    }

    public class Day_16 : BaseDay
    {
        private readonly TicketField[] fields;
        private readonly int[] myTicket;
        private readonly List<int[]> nearbyTickets;

        public Day_16()
        {
            var input = File.ReadAllLines(InputFilePath).Split("").ToArray();
            Debug.Assert(input.Length == 3);
            Debug.Assert(input[1].First().Contains("your"));

            fields = input[0].Select(ParseField).ToArray();
            myTicket = input[1].ElementAt(1).ParseInts();
            nearbyTickets = input[2].Skip(1).Select(l => l.ParseInts()).ToList();
        }

        private TicketField ParseField(string line)
        {
            var name = line.Substring(0, line.IndexOf(':'));
            var ranges = line.ParseNNInts().Pairwise();
            return new TicketField(name, ranges.ToList());
        }

        public override string Solve_1()
        {
            bool IsInvalidValue(int value) => !fields.Any(f => f.IsValid(value));

            long errorRate = 0;
            var invalidIdx = new List<int>();
            foreach (var ticket in nearbyTickets)
            {
                errorRate += ticket.Where(IsInvalidValue).Sum();
            }
            // Remove invalid tickwets for part 2
            _ = nearbyTickets.RemoveAll(t => t.Any(IsInvalidValue));
            return errorRate.ToString();
        }

        public override string Solve_2()
        {
            var possibleIdx = new Dictionary<string, List<int>>();
            foreach (var field in fields)
            {
                possibleIdx[field.Name] = new List<int>();
            }

            for (int idx = 0; idx < fields.Length; idx++)
            {
                var values = nearbyTickets.Select(t => t[idx]);
                foreach (var field in fields.Where(f => f.IsValidForAll(values)))
                    possibleIdx[field.Name].Add(idx);
            }

            var knownIdx = new Dictionary<string, int>();
            for (int i = 0; i < fields.Length; i++)
            {
                // Find the field with only one possible index
                var (name, indicies) = possibleIdx.First(x => x.Value.Count == 1);
                var index = indicies[0];
                _ = possibleIdx.Remove(name);
                knownIdx[name] = index;
                foreach (var other in possibleIdx)
                {
                    _ = other.Value.Remove(index);
                }
            }

            return knownIdx.Where(kvp => kvp.Key.StartsWith("departure"))
                .Select(kvp => myTicket[kvp.Value]).Product().ToString();
        }
    }
}

