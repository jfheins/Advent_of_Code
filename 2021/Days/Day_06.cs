namespace AoC_2021.Days
{
    public class Day_06 : BaseDay
    {
        private long[] _fishByAge;

        public Day_06()
        {
            var fish = File.ReadAllText(InputFilePath).Split(",").Select(int.Parse).ToArray();
            _fishByAge = Enumerable.Range(0, 9).Select(age => fish.LongCount(x => x == age)).ToArray();
        }
            
        public override async ValueTask<string> Solve_1()
        {
            var allFish = _fishByAge.ToArray();

            for (int i = 0; i < 80; i++)
            {
                var newParents = ShiftArray(ref allFish);
                allFish[6] += newParents;
                allFish[8] = newParents;
            }
            return allFish.Sum().ToString();
        }

        private long ShiftArray(ref long[] source)
        {
            var head = source[0];
            Array.Copy(source, 1, source, 0, source.Length - 1);
            return head;
        }

        public override async ValueTask<string> Solve_2()
        {
            var allFish = _fishByAge.ToArray();

            for (int i = 0; i < 256; i++)
            {
                var newParents = ShiftArray(ref allFish);
                allFish[6] += newParents;
                allFish[8] = newParents;
            }
            return allFish.Sum().ToString();
        }
    }
}
