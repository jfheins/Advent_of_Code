using AoCHelper;

namespace AoC_2015
{
    class Program
    {
        static async Task Main()
        {
            await Solver.SolveLast(new SolverConfiguration
            {
                ShowConstructorElapsedTime = true,
                ShowTotalElapsedTimePerDay = true,
                ElapsedTimeFormatSpecifier = "0.0",
            });
            _ = Console.ReadLine();
        }
    }
}
