using AoC_2022.Days;

using AoCHelper;

namespace AoC_2022
{
    static class Program
    {
        static async Task Main()
        {
            // Warmup
            await Solver.SolveAll();
            await Solver.SolveAll();

            Console.Clear();
            await Task.Delay(500);

            await Solver.SolveAll(c => 
            {
                c.ShowConstructorElapsedTime = true;
                c.ShowTotalElapsedTimePerDay = true;
                c.ElapsedTimeFormatSpecifier = "0.0";
                c.ClearConsole = false;
            });
        }
    }
}