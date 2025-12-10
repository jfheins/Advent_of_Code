using Core;
using System.Text.RegularExpressions;

namespace AoC_2025.Days;

public sealed class Day_10 : BaseDay
{
    private readonly Machine[] _input;

    public Day_10()
    {
        _input = File.ReadAllLines(InputFilePath).SelectArray(ParseMachine);
    }

    record Machine(int Goal, int[] Buttons, int[] JoltageReq)
    {
        public override string ToString()
            => $"{Convert.ToString(Goal, 2)} " +
               $"Buttons: [{string.Join(", ", Buttons.Select(b => Convert.ToString(b, 2)))}] " +
               $"JoltageReq: {{{string.Join(", ", JoltageReq)}}}";
    }

    private Machine ParseMachine(string line)
    {
        /*
         * [.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
           [...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}
           [.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}
         */
        var goal = Regex.Match(line, @"\[([.#]+)\]").Groups[1].Value.Replace('.', '0').Replace('#', '1').Reverse().ToArray();
        var goalBits = Convert.ToInt32(new string(goal), 2);
        var buttonMatches = Regex.Matches(line, @"\(([\d,]+)\)");
        var buttons = buttonMatches.SelectArray(match => match.Value.ParseInts().Aggregate(0, (current, idx) => current | 1 << idx));
        var joltages = Regex.Match(line, @"\{.+\}");
        return new Machine(goalBits, buttons, joltages.Value.ParseInts());
    }

    public override async ValueTask<string> Solve_1()
    {
        var totalPresses = 0;
        foreach (var machine in _input)
        {
            var bfs = new BreadthFirstSearch<int>(null, Expand) {PerformParallelSearch = false };

            var presses = bfs.FindFirst(0, it => it == machine.Goal);
            if (presses != null)
                totalPresses += presses.Length;
            else
                Console.WriteLine("Error");
            
            IEnumerable<int> Expand(int arg)
                => machine.Buttons.SelectArray(b => arg ^ b);
        }
        
        return totalPresses.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var totalPresses = 0;
        foreach (var machine in _input)
        {
            var bfs = new BreadthFirstSearch<int[]>(new ArrayComparer(), Expand) {PerformParallelSearch = false };

            var zero = new int[machine.JoltageReq.Length];
            
            var presses = bfs.FindFirst(zero, it => it.SequenceEqual(machine.JoltageReq));
            if (presses != null)
            {
                Console.WriteLine($"Done in {presses.Length} presses");
                totalPresses += presses.Length;
            }
            else
                Console.WriteLine("Error");
            
            IEnumerable<int[]> Expand(int[] ints)
                => machine.Buttons.Select(b =>
                {
                    var x = ints.ToArray();
                    for (var i = 0; i < x.Length; i++)
                    {
                        x[i] += (b & (1 << i)) > 0 ? 1 : 0;
                    }
                    return x;
                });
        }
        
        return totalPresses.ToString();
    }

    class ArrayComparer : IEqualityComparer<int[]>
    {
        public bool Equals(int[]? x, int[]? y)
            => x?.SequenceEqual(y) ?? false;

        public int GetHashCode(int[] obj)
            => obj.Aggregate(new HashCode(), (hc, v) => { hc.Add(v); return hc; }).ToHashCode();
    }
}