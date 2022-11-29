namespace AoC_2021.Days
{
    public class Day_03 : BaseDay
    {
        private string[] _input;
        private long[] _numbers;

        public Day_03()
        {
            _input = File.ReadAllLines(InputFilePath).ToArray();
            _numbers = _input.Select(long.Parse).ToArray();
        }

        private int PopCount(int idx, IEnumerable<string> arr)
        {
            return arr.Count(x => x[idx] == '1');
        }

        public override async ValueTask<string> Solve_1()
        {
            var digits = _input[0].Length;
            var overallCount = _input.Length;
            var gammaRateStr = new string('0', digits).ToCharArray();
            for (int idx = 0; idx < digits; idx++)
            {
                var popcount = PopCount(idx, _input);
                if (popcount > overallCount / 2)
                {
                    gammaRateStr[idx] = '1';
                }
            }
            var gamma = Convert.ToInt32(new string(gammaRateStr), 2);
            var epsilon = (~gamma) & 0b111111111111;
            return (gamma * epsilon).ToString();
        }

        private List<string> FilterForOxygen(List<string> list, int idx)
        {
            if (list.Count <= 1)
                return list;

            var popcount = PopCount(idx, list);
            var filterchar = '0';
            if (2 * popcount >= list.Count)
                filterchar = '1';
            return list.Where(s => s[idx] == filterchar).ToList();
        }

        private List<string> FilterForCO2(List<string> list, int idx)
        {
            if (list.Count <= 1)
                return list;

            var popcount = PopCount(idx, list);
            var filterchar = '1';
            if (2 * popcount >= list.Count)
                filterchar = '0';
            return list.Where(s => s[idx] == filterchar).ToList();
        }

        public override async ValueTask<string> Solve_2()
        {
            var digits = _input[0].Length;

            var oxygenRating = Enumerable.Range(0, digits).Aggregate(
                _input.ToList(),
                (list, idx) => FilterForOxygen(list, idx),
                list => list.First());

            var co2Rating = Enumerable.Range(0, digits).Aggregate(
                _input.ToList(),
                (list, idx) => FilterForCO2(list, idx),
                list => list.First());

            var a = Convert.ToInt32(oxygenRating, 2);
            var b = Convert.ToInt32(co2Rating, 2);
            return (a * b).ToString();
        }
    }
}
