using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static MoreLinq.Extensions.SplitExtension;

using Core;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Data;

namespace AoC_2020.Days
{
    public class Day_22 : BaseDay
    {
        private readonly LinkedList<int> player1;
        private readonly LinkedList<int> player2;
        private ImmutableList<int> p1list;
        private ImmutableList<int> p2list;

        public Day_22()
        {
            var input = File.ReadAllLines(InputFilePath).Split("").ToArray(); ;
            player1 = new LinkedList<int>(input[0].Skip(1).Select(int.Parse));
            player2 = new LinkedList<int>(input[1].Skip(1).Select(int.Parse));
            p1list = player1.ToImmutableList();
            p2list = player2.ToImmutableList();
        }

        public override string Solve_1()
        {
            while (player1.Count * player2.Count > 0)
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
            var count = deck.Count;
            var score = deck.Select((item, idx) => (count - idx) * (long)item).Sum();
            return score.ToString();
        }

        private (int player, ImmutableList<int> deck) RecurseCombat(ImmutableList<int> deck1, ImmutableList<int> deck2)
        {
            HashSet<(ImmutableList<int>, ImmutableList<int>)> seen = new(new SequenceComparer());
            var drawn = new int[2];
            while (deck1.Any() && deck2.Any())
            {
                if (!seen.Add((deck1, deck2)))
                    return (1, deck1);

                drawn[0] = deck1[0];
                drawn[1] = deck2[0];

                int winner;
                if (deck1.Count > drawn[0] && deck2.Count > drawn[1])
                    winner = RecurseCombat(deck1.GetRange(1, drawn[0]), deck2.GetRange(1, drawn[1])).player;
                else
                    winner = drawn[0] > drawn[1] ? 1 : 2;

                if (winner == 1)
                {
                    deck1 = deck1.RemoveAt(0).AddRange(drawn);
                    deck2 = deck2.RemoveAt(0);
                }
                else if(winner == 2)
                {
                    deck1 = deck1.RemoveAt(0);
                    deck2 = deck2.RemoveAt(0).AddRange(drawn.Reverse());
                }
            }
            return deck1.Any() ? (1, deck1) : (2, deck2);
        }

        private class SequenceComparer : IEqualityComparer<(ImmutableList<int>, ImmutableList<int>)>
        {
            public bool Equals((ImmutableList<int>, ImmutableList<int>) x, (ImmutableList<int>, ImmutableList<int>) y)
            {
                return Enumerable.SequenceEqual(x.Item1, y.Item1) && Enumerable.SequenceEqual(x.Item2, y.Item2);
            }

            public int GetHashCode([DisallowNull] (ImmutableList<int>, ImmutableList<int>) obj)
            {
                var left = obj.Item1.Count > 2 ? obj.Item1[0] + (obj.Item1[1] * 59) + (obj.Item1[^1] * 59 * 59) : 487;
                var right = obj.Item2.Count > 2 ? unchecked((obj.Item2[0] + (obj.Item2[1] * 59) + (obj.Item2[^1] * 59 * 59)) << 20) : 1000037;
                return left ^ right;
            }
        }
    }
}

