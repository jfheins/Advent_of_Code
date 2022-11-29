using Core;

namespace AoC_2021.Days
{
    public class Day_21 : BaseDay
    {
        private int _player1Start;
        private int _player2Start;

        public Day_21()
        {
            var input = File.ReadAllLines(InputFilePath).ToArray();
            _player1Start = input[0].ParseInts(2)[1];
            _player2Start = input[1].ParseInts(2)[1];
        }

        public override async ValueTask<string> Solve_1()
        {
            var dice = Enumerable.Range(1, 100).RepeatIndefinitely();
            var dicerolls = 0;
            var dice2 = RollDiceThreeTimes().GetEnumerator();

            var scores = new int[2];
            var positions = new int[2] { _player1Start, _player2Start };
            for (int i = 0; i < 2000; i++)
            {
                // Player 1 move
                dice2.MoveNext();
                var rolls = dice2.Current;
                positions[0] = (positions[0] + rolls).OneBasedModulo(10);
                scores[0] += positions[0];
                if (scores[0] >= 1000)
                {
                    break;
                }

                // Player 2 move
                dice2.MoveNext();
                rolls = dice2.Current;
                positions[1] = (positions[1] + rolls).OneBasedModulo(10);
                scores[1] += positions[1];
                if (scores[1] >= 1000)
                {
                    break;
                }
            }
            var score = scores.Min() * dicerolls;

            return score.ToString();

            IEnumerable<int> RollDiceThreeTimes()
            {
                foreach (var chunk in dice.Chunks(3))
                {
                    dicerolls += 3;
                    yield return chunk.Sum();
                }
            }
        }

        public override async ValueTask<string> Solve_2()
        {
            var diracDice = new (ushort sum, ushort count)[] { (3, 1), (4, 3), (5, 6), (6, 7), (7, 6), (8, 3), (9, 1) };

            var overallWins = PlayOneTurn(_player1Start, _player2Start, 0, 0);
            return overallWins.ToString();

            long PlayOneTurn (int p1pos, int p2pos, int p1score, int p2score)
            {
                long oneWon = 0;
                foreach (var (p1sum, p1count) in diracDice)
                {
                    var newp1pos = (p1pos + p1sum).OneBasedModulo(10);
                    var newp1score = p1score + newp1pos;
                    if (newp1score >= 21)
                    {
                        oneWon += p1count;
                    }
                    else
                    {
                        foreach (var (p2sum, p2count) in diracDice)
                        {
                            var newp2pos = (p2pos + p2sum).OneBasedModulo(10);
                            var newp2score = p2score + newp2pos;
                            if (newp2score < 21)
                            {
                                oneWon += p1count * p2count * PlayOneTurn(newp1pos, newp2pos, newp1score, newp2score);
                            }
                        }
                    }
                }
                return oneWon;
            }
        }
    }
}
