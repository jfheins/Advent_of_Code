using Core;

namespace AoC_2022.Days
{
    public sealed class Day_03 : BaseDay
    {
        private string[] _input;
        private readonly string prio = " " + Constants.LowerCaseAbc + Constants.UpperCaseAbc;

        public Day_03()
        {
            _input = File.ReadAllLines(InputFilePath);
        }

        public override async ValueTask<string> Solve_1()
        {
            return _input
                .Select(line => NonEmptyList.Create(line.SplitInto(2)))
                .Sum(PriorityOfCommonItem).ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            return _input.Select(it => it.ToCharArray()).Chunk(3)
                .Select(NonEmptyList.Create)
                .Sum(PriorityOfCommonItem).ToString();
        }

        private int PriorityOfCommonItem(NonEmptyList<char[]> parts)
        {
            var commonItems = parts.Head.ToHashSet();
            foreach (var nextPart in parts.Tail)
            {
                commonItems.IntersectWith(nextPart);
            }
            return prio.IndexOfOrThrow(commonItems.Single());
        }
    }
}