using System.Collections.Generic;
using System.IO;
using System.Linq;
using static MoreLinq.Extensions.SplitExtension;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace AoC_2020.Days
{
    public class Day_22 : BaseDay
    {
        private readonly LinkedList<int> player1;
        private readonly LinkedList<int> player2;
        private ImmutableArray<int> p1list;
        private ImmutableArray<int> p2list;

        public Day_22()
        {
            var input = File.ReadAllLines(InputFilePath).Split("").ToArray(); ;
            player1 = new LinkedList<int>(input[0].Skip(1).Select(int.Parse));
            player2 = new LinkedList<int>(input[1].Skip(1).Select(int.Parse));
            p1list = player1.ToImmutableArray();
            p2list = player2.ToImmutableArray();
        }

        public override string Solve_1()
        {
            while (player1.Any() && player2.Any())
            {
                var played = (player1.First!.Value, player2.First!.Value);

                player1.RemoveFirst();
                player2.RemoveFirst();

                if (played.Item1 > played.Item2)
                {
                    player1.AddLast(played.Item1);
                    player1.AddLast(played.Item2);
                }
                else
                {
                    player2.AddLast(played.Item2);
                    player2.AddLast(played.Item1);
                }
            }

            var winner = player1.Any() ? player1 : player2;
            var count = winner.Count;
            var score = winner.Select((item, idx) => (count - idx) * (long)item).Sum();

            return score.ToString();
        }

        public override string Solve_2()
        {
            var (_, deck) = RecurseCombat(p1list, p2list);
            var score = deck.Select((item, idx) => (deck.Length - idx) * (long)item).Sum();
            return score.ToString();
        }

        private (int player, ImmutableArray<int> deck) RecurseCombat(ImmutableArray<int> deck1, ImmutableArray<int> deck2, int depth = 1)
        {
            if (depth > 1 && deck1.Max() > deck2.Max())
                return (1, deck1); // Player 1 cannot loose if he has the highest card

            HashSet<(ImmutableArray<int>, ImmutableArray<int>)> seen = new(new SequenceComparer());
            var drawn = new int[2];
            while (deck1.Any() && deck2.Any())
            {
                if (!seen.Add((deck1, deck2)))
                    return (1, deck1);

                drawn[0] = deck1[0];
                drawn[1] = deck2[0];
                deck1 = deck1.RemoveAt(0);
                deck2 = deck2.RemoveAt(0);

                int winner;
                if (deck1.Length >= drawn[0] && deck2.Length >= drawn[1])
                    winner = RecurseCombat(ImmutableArray.CreateRange(deck1.Take(drawn[0])), ImmutableArray.CreateRange(deck2.Take(drawn[1])), depth + 1).player;
                else
                    winner = drawn[0] > drawn[1] ? 1 : 2;

                if (winner == 1)
                {
                    deck1 = deck1.AddRange(drawn);
                }
                else if (winner == 2)
                {
                    deck2 = deck2.AddRange(drawn.Reverse());
                }
            }
            return deck1.Any() ? (1, deck1) : (2, deck2);
        }

        private class SequenceComparer : IEqualityComparer<(ImmutableArray<int>, ImmutableArray<int>)>
        {
            public bool Equals((ImmutableArray<int>, ImmutableArray<int>) x, (ImmutableArray<int>, ImmutableArray<int>) y)
            {
                return Enumerable.SequenceEqual(x.Item1, y.Item1) && Enumerable.SequenceEqual(x.Item2, y.Item2);
            }

            public int GetHashCode([DisallowNull] (ImmutableArray<int>, ImmutableArray<int>) obj)
            {
                var left = obj.Item1.Length > 2 ? obj.Item1[0] + (obj.Item1[1] * 59) + (obj.Item1[^1] * 59 * 59) : 487;
                var right = obj.Item2.Length > 2 ? unchecked((obj.Item2[0] + (obj.Item2[1] * 59) + (obj.Item2[^1] * 59 * 59)) << 20) : 1000037;
                return left ^ right;
            }
        }
    }
}

