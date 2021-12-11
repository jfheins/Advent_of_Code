using Core;
using Core.Combinatorics;
using MoreLinq.Extensions;
using System.IO;
using System.Linq;

namespace AoC_2021.Days
{
    public class Day_11 : BaseDay
    {
        private FiniteGrid2D<char> _input;

        public Day_11()
        {
            _input = Grid2D.FromFile(InputFilePath);
        }

        public override async ValueTask<string> Solve_1()
        {
            var flashCout = 0;
            for (int i = 0; i < 100; i++)
            {
                foreach (var item in _input)
                {
                    _input[item.pos]++;
                }
                List<(System.Drawing.Point pos, char value)> toFlash = new();
                do
                {
                    toFlash = _input.Where(x => x.value > '9' && x.value != '+').ToList();
                    foreach (var item in toFlash)
                    {
                        _input[item.pos] = '+';
                        flashCout++;
                        var neigh = _input.Get8NeighborsOf(item.pos);
                        foreach (var n in neigh.Where(x => _input[x] != '+'))
                        {
                            _input[n]++;
                        }
                    }
                } while (toFlash.Any());

                foreach (var item in _input.Where(x => x.value == '+'))
                {
                    _input[item.pos] = '0';
                }
            }
            return flashCout.ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var itercount = 0;
            checked { 
                var flashCout = 0;
                for (int i = 0; i < 10000; i++)
                {
                    foreach (var item in _input)
                    {
                        _input[item.pos]++;
                    }
                    List<(System.Drawing.Point pos, char value)> toFlash = new();
                    do
                    {
                        toFlash = _input.Where(x => x.value > '9' && x.value != '+').ToList();
                        foreach (var item in toFlash)
                        {
                            _input[item.pos] = '+';
                            flashCout++;
                            var neigh = _input.Get8NeighborsOf(item.pos);
                            foreach (var n in neigh.Where(x => _input[x] != '+'))
                            {
                                if (_input[n] >= '0' && _input[n]  <= ':')
                                    _input[n]++;
                            }
                        }
                    } while (toFlash.Any());

                    if (_input.All(x => x.value == '+'))
                    {
                        itercount = i;
                        break;
                    }

                    foreach (var item in _input.Where(x => x.value == '+'))
                    {
                        _input[item.pos] = '0';
                    }
                }
            return itercount.ToString();
            }
        }
    }
}
