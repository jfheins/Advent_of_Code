using AoC_2022.Days;

using AoCHelper;

namespace AoC_2022
{
    static partial class Program
    {
        static async Task Main()
        {
            for (int i = 0; i < 10; i++)
            {
                var d = new Day_11();
                await d.Solve_1();
                await d.Solve_2();
            }
            await Task.Delay(100);
            HelloFrom("sdfsdvf");

            await Solver.SolveLast(c => 
            {
                c.ShowConstructorElapsedTime = true;
                c.ShowTotalElapsedTimePerDay = true;
                c.ElapsedTimeFormatSpecifier = "0.0";
                c.ClearConsole = false;
            });
        }

        static partial void HelloFrom(string name);
    }
}