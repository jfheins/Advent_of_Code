using AoC_2016.Days;

using AoCHelper;

namespace AoC_2016
{
    class Program
    {
        static async Task Main()
        {
            await Solver.Solve<Day_02>(new SolverConfiguration
            {
                ShowConstructorElapsedTime = true,
                ShowTotalElapsedTimePerDay = true,
                ElapsedTimeFormatSpecifier = "0.0",
            });
            _ = Console.ReadLine();
        }
    }
}
