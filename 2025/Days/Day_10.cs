using Core;
using Google.OrTools.Sat;
using System.Text.RegularExpressions;

namespace AoC_2025.Days;

public sealed class Day_10 : BaseDay
{
    private readonly Machine[] _input;

    public Day_10()
    {
        _input = File.ReadAllLines(InputFilePath).SelectArray(ParseMachine);
    }

    private record Machine(int Goal, int[] Buttons, int[] JoltageReq)
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
        var model = new CpModel();
        var maxPresses = machine.JoltageReq.Max();
        
        // Create integer variables for each button type (how many times each button is pressed)
        var buttonVars = Enumerable.Range(0, machine.Buttons.Length)
                .Select(i => model.NewIntVar(0, maxPresses, $"button_{i}"))
                .ToArray();
        
        // For each counter/joltage requirement, add constraint that sum of button presses equals target
        foreach (var (counterIdx, targetValue) in machine.JoltageReq.Index())
        {
            // Find all buttons that affect this counter
            var affectingButtons = machine.Buttons
                .Index()
                .Where(b => (b.Item & (1 << counterIdx)) != 0)
                .Select(b => buttonVars[b.Index])
                .ToList();
            
            // Sum of all button presses that affect this counter must equal the target
            if (affectingButtons.Count > 0)
            {
                model.Add(LinearExpr.Sum(affectingButtons) == targetValue);
            }
            else if (targetValue != 0)
            {
                // No button affects this counter, so target must be 0
                Console.WriteLine($"Error: Counter {counterIdx} requires {targetValue} but no button affects it");
                return 0;
            }
        }
        
        // Minimize the total number of button presses
        model.Minimize(LinearExpr.Sum(buttonVars));
        
        // Solve
        var solver = new CpSolver();
        var status = solver.Solve(model);
        
        if (status is CpSolverStatus.Optimal or CpSolverStatus.Feasible)
        {
            return buttonVars.Sum(buttonVar => (int)solver.Value(buttonVar));
        }

        Console.WriteLine($"No solution found: {status}");
        return 0;
    }
}