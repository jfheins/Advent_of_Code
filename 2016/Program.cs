using AoC_2016.Days;

using AoCHelper;

namespace AoC_2016
{
    class Program
    {
        static async Task Main()
        {
            await Solver.SolveLast(c => 
            {
                c.ShowConstructorElapsedTime = true;
                c.ShowTotalElapsedTimePerDay = true;
                c.ElapsedTimeFormatSpecifier = "0.0";
            });
            _ = Console.ReadLine();
        }
    }
}
