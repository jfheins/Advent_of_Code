using Core;
using Microsoft.Z3;
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
        var goal = Regex.Match(line, @"\[([.#]+)\]").Groups[1].Value.Replace('.', '0').Replace('#', '1').Reverse()
            .ToArray();
        var goalBits = Convert.ToInt32(new string(goal), 2);
        var buttonMatches = Regex.Matches(line, @"\(([\d,]+)\)");
        var buttons = buttonMatches.SelectArray(match
            => match.Value.ParseInts().Aggregate(0, (current, idx) => current | 1 << idx));
        var joltages = Regex.Match(line, @"\{.+\}");
        return new Machine(goalBits, buttons, joltages.Value.ParseInts());
    }

    public override async ValueTask<string> Solve_1()
    {
        var totalPresses = 0;
        foreach (var machine in _input)
        {
            var bfs = new BreadthFirstSearch<int>(null, Expand) { PerformParallelSearch = false };

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
        var totalPresses = _input.AsParallel().Sum(CalculatePresses);
        return totalPresses.ToString();
    }

    private static int CalculatePresses(Machine machine)
    {
        using var ctx = new Context();
        var optimize = ctx.MkOptimize();
        
        // Create integer variables for each button type (how many times each button is pressed)
        var buttonVars = new IntExpr[machine.Buttons.Length];
        for (var i = 0; i < machine.Buttons.Length; i++)
        {
            buttonVars[i] = ctx.MkIntConst($"button_{i}");
            // Ensure non-negative integer solutions
            optimize.Assert(ctx.MkGe(buttonVars[i], ctx.MkInt(0)));
        }
        
        // For each counter/joltage requirement, add constraint that sum of button presses equals target
        for (var counterIdx = 0; counterIdx < machine.JoltageReq.Length; counterIdx++)
        {
            var terms = new List<ArithExpr>();
            
            for (var buttonIdx = 0; buttonIdx < machine.Buttons.Length; buttonIdx++)
            {
                var button = machine.Buttons[buttonIdx];
                // Check if this button affects this counter
                if ((button & (1 << counterIdx)) != 0)
                {
                    terms.Add(buttonVars[buttonIdx]);
                }
            }
            
            // Sum of all button presses that affect this counter must equal the target
            if (terms.Count > 0)
            {
                var sum = terms.Count == 1 ? terms[0] : ctx.MkAdd(terms.ToArray());
                optimize.Assert(ctx.MkEq(sum, ctx.MkInt(machine.JoltageReq[counterIdx])));
            }
            else
            {
                // No button affects this counter, so target must be 0
                if (machine.JoltageReq[counterIdx] != 0)
                {
                    Console.WriteLine($"Error: Counter {counterIdx} requires {machine.JoltageReq[counterIdx]} but no button affects it");
                    return 0;
                }
            }
        }
        
        // Minimize the total number of button presses
        var totalPresses = ctx.MkAdd(buttonVars.Cast<ArithExpr>().ToArray());
        optimize.MkMinimize(totalPresses);
        
        // Solve
        var status = optimize.Check();
        if (status == Status.SATISFIABLE)
        {
            var model = optimize.Model;
            var result = 0;
            for (var i = 0; i < buttonVars.Length; i++)
            {
                var value = ((IntNum)model.Evaluate(buttonVars[i])).Int;
                result += value;
            }
            Console.WriteLine($"Done in {result} presses");
            return result;
        }
        else
        {
            Console.WriteLine($"No solution found: {status}");
            return 0;
        }
    }
}