using Core;

using MatrixDotNet.Extensions.Statistics.TableSetup;

using System.Diagnostics;
using System.Drawing;

namespace AoC_2023.Days;

public sealed partial class Day_22 : BaseDay
{
    private readonly string[] _input;
    private readonly FiniteGrid2D<char> _grid;

    public Day_22()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var bricks = _input.SelectArray(ParseBrick);

        Settle(bricks);
        var dependsOn = new Dictionary<Brick, List<Brick>>();

        for (var i = 0; i < bricks.Length; i++)
        {
            var b = bricks[i];
            var lower = bricks.Where(o => o.MaxZ == b.MinZ-1 && o.X.OverlapsWith(b.X) && o.Y.OverlapsWith(b.Y));
            dependsOn[b] = lower.ToList();
        }

        var possibleDis = bricks.Count(b => dependsOn.Values.All(dependant => dependant.Contains(b) ? dependant.Count > 1 : true));

        return possibleDis.ToString();
    }

    private void Settle(Brick[] bricks)
    {
        Array.Sort(bricks, (a, b) => a.MinZ.CompareTo(b.MinZ));

        bool hasChange = true;
        while (hasChange)
        {
            hasChange = false;
            for (int i = 0; i < bricks.Length; i++)
            {
                var y = bricks[i].Y;
                var minz = 1;
                for (int li = 0; li < bricks.Length && bricks[li].MinZ < bricks[i].MinZ; li++)
                {
                    if (bricks[li].X.OverlapsWith(bricks[i].X) && bricks[li].Y.OverlapsWith(bricks[i].Y))
                    {
                        minz = Math.Max(minz, bricks[li].MaxZ + 1);
                    }
                }
                if (minz != bricks[i].MinZ)
                {
                    bricks[i].MoveTo(minz);
                    hasChange = true;
                }
            }
            Array.Sort(bricks, (a, b) => a.MinZ.CompareTo(b.MinZ));
        }
    }

    private Brick ParseBrick(string arg)
    {
        var n = arg.ParseInts(6);
        return new Brick
        {
            A = new Point3(n[0], n[1], n[2]),
            B = new Point3(n[3], n[4], n[5])
        };
    }
    class Brick()
    {
        public Point3 A { get; set; }
        public Point3 B { get; set; }
        public int MinZ => Math.Min(A.Z, B.Z);
        public int MaxZ => Math.Max(A.Z, B.Z);
        public Interval X => Interval.FromInclusiveBoundsOrInverse(A.X, B.X);
        public Interval Y => Interval.FromInclusiveBoundsOrInverse(A.Y, B.Y);

        public void MoveTo(int newZ)
        {
            var shift = MinZ - newZ;
            A = A.TranslateBy(0, 0, -shift);
            B = B.TranslateBy(0, 0, -shift);
        }
    }

    public override async ValueTask<string> Solve_2()
    {
        var bricks = _input.SelectArray(ParseBrick);

        Settle(bricks);
        var dependsOn = new Dictionary<Brick, List<Brick>>();

        for (var i = 0; i < bricks.Length; i++)
        {
            var b = bricks[i];
            var lower = bricks.Where(o => o.MaxZ == b.MinZ - 1 && o.X.OverlapsWith(b.X) && o.Y.OverlapsWith(b.Y));
            dependsOn[b] = lower.ToList();
        }
        var sum = 0;

        foreach (var b in bricks)
        {
            // How many fall if this is gone
            var falling = new HashSet<Brick>() { b };
            var ls = 1;
            do
            {
                ls = falling.Count;
                var above = dependsOn.Where(kvp => kvp.Value.Any() && kvp.Value.All(falling.Contains))
                    .SelectList(kvp => kvp.Key);
                falling.UnionWith(above);
            } while (falling.Count > ls);
            sum += falling.Count - 1; // count only others
        }

        
        return sum.ToString();
    }
}