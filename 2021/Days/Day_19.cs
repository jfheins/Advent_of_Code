using Core;
using Core.Combinatorics;
using System.Diagnostics;
using System.Numerics;
using static MoreLinq.Extensions.SplitExtension;

namespace AoC_2021.Days
{
    public class Day_19 : BaseDay
    {
        private List<Scanner> _scanners;
        private List<Scanner> _knownScanners;

        public record Scanner(string Name, Point3[] Scan, IReadOnlyList<Triangle> Triangles)
        {
            public Point3 Position { get; set; }

            internal static Scanner WithOffset(Scanner next, Matrix4x4 rot, Point3 offset)
            {
                var offsetPoints = Transform(next.Scan, rot, offset);
                var combi = new Combinations<Point3>(offsetPoints.ToList(), 3);
                var triangles = combi.Select(c => new Triangle(c[0], c[1], c[2])).ToList();
                return new Scanner(next.Name, offsetPoints.ToArray(), triangles);
            }
        }
        public record Triangle
        {
            public Point3 A { get; init; }
            public Point3 B { get; init; }
            public Point3 C { get; init; }
            public long Hash { get; }

            public Point3[] Points { get; }
            public Point3 Sum => Points.Aggregate(Point3.Empty, (a, b) => a+b);

            public (Point3 a, Point3 b) CalcVectors()
            {
                var sideA = Points[1] - Points[0];
                var sideB = Points[2] - Points[0];
                return (sideA, sideB);
            }

            public Triangle(Point3 a, Point3 b, Point3 c)
            {
                A = a; B = b; C = c;
                var distances = new int[] { a.ManhattanDistTo(b), b.ManhattanDistTo(c), c.ManhattanDistTo(a) };
                Array.Sort(distances);
                Hash = ((long)distances[2]) << 40 | ((long)distances[1] << 20) | ((long)distances[0]);
                Debug.Assert(distances.Max() < (1 << 20));

                Points = new[] { A, B, C }.OrderBy(p => p.X).ThenBy(p => p.Y).ThenBy(p => p.Z).ToArray();
            }
        }

        public Day_19()
        {
            var input = File.ReadAllLines(InputFilePath).Split("").Select(x => x.ToArray()).ToArray();
            _scanners = input.Select(ParseBlock).ToList();
        }

        private Scanner ParseBlock(string[] block)
        {
            var name = block[0];
            var scan = block[1..].Select(line => Point3.FromArray(line.ParseInts(3))).ToArray();
            var combi = new Combinations<Point3>(scan, 3);
            var triangles = combi.Select(c => new Triangle(c[0], c[1], c[2])).ToList();
            return new Scanner(name, scan, triangles);
        }

        public override async ValueTask<string> Solve_1()
        {
            _knownScanners = new List<Scanner> { _scanners[0] };
            var beacons = new HashSet<Point3>(_scanners[0].Scan);

            for (int i = 1; i < _scanners.Count; i++)
            {
                // pick a scanner with big overlap out of the list
                var (known, next) = GetNextScanner(_knownScanners);

                var intersectingTriangles = Overlap(known, next);

                var knownTri = known.Triangles.Where(t => intersectingTriangles.Contains(t.Hash)).ToList();
                var nextTri = knownTri.Select(kt => next.Triangles.First(t => t.Hash == kt.Hash)).ToList();

                // Rotation
                var rotated = Rotations().Select(potRot =>
                {
                    var rotatedTriangles = Rotate(nextTri, potRot);
                    var matches = rotatedTriangles.Zip(knownTri, (rotated, known) => IsKongruent(rotated, known)).Count(x => x);
                    return (isMatch: matches > 200, rotation: potRot, triangles: rotatedTriangles);
                }).First(t => t.isMatch);

                // Find a triangle in both collections that is actually congruent
                // Read: not only same side lengths
                Triangle? match = null;
                var knownIdx = -1;
                while (match == null)
                {
                    knownIdx++;
                    match = rotated.triangles.FirstOrDefault(t => IsKongruent(t, knownTri[knownIdx]));
                }

                var offset3 = match.Sum - knownTri[knownIdx].Sum;
                var offset = new Point3(offset3.X / 3, offset3.Y / 3, offset3.Z / 3);

                var newScannerPos = offset.Inverse();
                
                var offsetScanner = Scanner.WithOffset(next, rotated.rotation, newScannerPos);
                offsetScanner.Position = newScannerPos;
                _knownScanners.Add(offsetScanner);

                foreach (var newBeacon in offsetScanner.Scan)
                {
                    beacons.Add(newBeacon);
                }
            }
            return beacons.Count.ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var c = new Combinations<Scanner>(_knownScanners, 2);
            return c.Max(pair => pair[0].Position.ManhattanDistTo(pair[1].Position)).ToString();
        }

        protected static IReadOnlyList<Point3> Transform(IReadOnlyList<Point3> points, Matrix4x4 matrix, Point3 offset)
        {
            return points.Select(transform).ToList();
            Point3 transform(Point3 p)
            {
                var v = new Vector3(p.X, p.Y, p.Z);
                var rotated = Vector3.Transform(v, matrix);
                return new Point3((int)rotated.X, (int)rotated.Y, (int)rotated.Z).TranslateBy(offset);
            }
        }

        private IReadOnlyList<Triangle> Rotate(IEnumerable<Triangle> triangles, Matrix4x4 matrix)
        {
            return triangles.Select(RotateTriangle).ToList();

            Triangle RotateTriangle(Triangle t) => new(rot(t.A), rot(t.B), rot(t.C));
            Point3 rot(Point3 p)
            {
                var rotated = Vector3.Transform(p.ToVector(), matrix);
                return new Point3((int)rotated.X, (int)rotated.Y, (int)rotated.Z);
            }
        }


        private (Scanner known, Scanner next) GetNextScanner(List<Scanner> knownScanners)
        {
            var maxOverlap = 0;
            var next = _scanners[1];
            var known = knownScanners[0];
            var knownList = knownScanners.Select(s => s.Name).ToHashSet();
            foreach (var potNext in _scanners.Where(s => !knownList.Contains(s.Name)))
            {
                foreach (var knownScanner in knownScanners)
                {
                    var overlap = Overlap(potNext, knownScanner).Count;
                    if (overlap > maxOverlap)
                    {
                        next = potNext;
                        known = knownScanner;
                        maxOverlap = overlap;
                    }
                }
            }

            return (known, next);
        }

        private static IReadOnlySet<long> Overlap(
            Scanner a,
            Scanner b)
        {
            return a.Triangles.Select(t => t.Hash)
                .Intersect(b.Triangles.Select(t => t.Hash)).ToHashSet();
        }

        private static IEnumerable<Matrix4x4> Rotations()
        {
            var facing = new Vector3[]
            {
                new Vector3(1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, 0, 1),
                new Vector3(-1, 0, 0),
                new Vector3(0, -1, 0),
                new Vector3(0, 0, -1),
            };
            foreach (var face in facing)
            {
                foreach (var right in facing)
                {
                    var up = Vector3.Cross(face, right);
                    if (up.LengthSquared() > 0)
                    {
                        yield return new Matrix4x4(
                            face.X, right.X, up.X, 0,
                            face.Y, right.Y, up.Y, 0,
                            face.Z, right.Z, up.Z, 0,
                            0, 0, 0, 1);
                    }
                }
            }
        }

        /// <summary>
        /// Returns if this is the same triangle (correct oriantation) at (maybe) another positin in space
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static bool IsKongruent(Triangle left, Triangle right)
        {
            if (left.Hash != right.Hash)
                return false;
            // Same dimensions
            var (lefta, leftb) = left.CalcVectors();
            var (righta, rightb) = right.CalcVectors();

            return lefta == righta && leftb == rightb;
        }
    }
}
