using AoCHelper;

namespace AoC_2022
{
    static class Program
    {
        static void Main()
        {
            Solver.SolveLast(new SolverConfiguration
            {
                ShowConstructorElapsedTime = true,
                ShowTotalElapsedTimePerDay = true,
                ElapsedTimeFormatSpecifier = "0.0",
            });
            _ = Console.ReadLine();
        }
    }
}