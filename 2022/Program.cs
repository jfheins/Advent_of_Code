using AoCHelper;

namespace AoC_2022
{
    static class Program
    {
        static async Task Main()
        {
            await Solver.SolveLast(c => 
            {
                c.ShowConstructorElapsedTime = true;
                c.ShowTotalElapsedTimePerDay = true;
                c.ElapsedTimeFormatSpecifier = "0.0";
                c.ClearConsole = false;
            });
        }
    }
}