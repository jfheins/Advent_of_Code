using Core;
using Core.Combinatorics;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            public (int, int, int) Hash { get; }

            public IEnumerable<Point3> Points => new[] { A, B, C };
            public Point3 Sum => Points.Aggregate(Point3.Empty, (a, b) => a+b);

            public (Point3 a, Point3 b) GetVectors()
            {
                var minP = Points.OrderBy(p => p.X).ThenBy(p => p.Y).ThenBy(p => p.Z).ToArray();
                var sideA = minP[1] - minP[0];
                var sideB = minP[2] - minP[0];
                return (sideA, sideB);
            }

            public Triangle(Point3 a, Point3 b, Point3 c)
            {
                A = a; B = b; C = c;
                Hash = new[] { a, b, c, a }.PairwiseWithOverlap()
                .Select(t => t.Item1.ManhattanDistTo(t.Item2))
                .OrderBy(x => x)
                .ToTuple3();
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
                var pair = GetNextScanner(_knownScanners);

                var intersectingTriangles = Overlap(pair.known, pair.next);

                var knownTri = pair.known.Triangles
                    .Where(t => intersectingTriangles.Contains(t.Hash)).ToList();
                var p = knownTri.SelectMany(t => t.Points).ToHashSet();

                var nextTri = knownTri.Select(kt => pair.next.Triangles.First(t => t.Hash == kt.Hash)).ToList();

                // Rotation
                var rot = Matrix4x4.Identity;
                var rotatedTriangles = pair.next.Triangles;
                foreach (var potRot in Rotations())
                {
                    rotatedTriangles = Rotate(pair.next.Triangles, potRot);
                    var matches = rotatedTriangles.Sum(rot => knownTri.Count(k => IsKongruent(rot, k)));
                    if (matches > 160)
                    {
                        Console.WriteLine(matches);
                        rot = potRot;
                        break;
                    }
                }

                // Find a triangle in both collections that is actually congruent
                // Read: not only same side lengths
                Triangle? match = null;
                var knownIdx = -1;
                while (match == null)
                {
                    knownIdx++;
                    match = rotatedTriangles.FirstOrDefault(t => IsKongruent(t, knownTri[knownIdx]));
                }

                var offset3 = match.Sum - knownTri[knownIdx].Sum;
                var offset = new Point3(offset3.X / 3, offset3.Y / 3, offset3.Z / 3);

                var newScannerPos = offset.Inverse();

                var offsetScanner = Scanner.WithOffset(pair.next, rot, newScannerPos);
                offsetScanner.Position = newScannerPos;
                _knownScanners.Add(offsetScanner);

                foreach (var newBeacon in offsetScanner.Scan)
                {
                    beacons.Add(newBeacon);
                }

                Console.WriteLine($"added {offsetScanner.Name} at {newScannerPos}");
            }
            return beacons.Count.ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            var c = new Combinations<Scanner>(_knownScanners, 2);
            return c.Max(pair => pair[0].Position.ManhattanDistTo(pair[1].Position)).ToString();
        }

        protected static IReadOnlyList<Triangle> Transform(IReadOnlyList<Triangle> triangles, Matrix4x4 matrix, Point3 offset)
        {
            return triangles.Select(RotateTriangle).ToList();

            Triangle RotateTriangle(Triangle t) => new(transform(t.A), transform(t.B), transform(t.C));
            Point3 transform(Point3 p)
            {
                var v = new Vector3(p.X, p.Y, p.Z);
                var rotated = Vector3.Transform(v, matrix);
                Debug.Assert(v.LengthSquared() == rotated.LengthSquared());
                return new Point3((int)rotated.X, (int)rotated.Y, (int)rotated.Z).TranslateBy(offset);
            }
        }

        protected static IReadOnlyList<Point3> Transform(IReadOnlyList<Point3> points, Matrix4x4 matrix, Point3 offset)
        {
            return points.Select(transform).ToList();
            Point3 transform(Point3 p)
            {
                var v = new Vector3(p.X, p.Y, p.Z);
                var rotated = Vector3.Transform(v, matrix);
                var p2 = new Point3((int)rotated.X, (int)rotated.Y, (int)rotated.Z).TranslateBy(offset);
                return p2;
            }
        }

        private IReadOnlyList<Triangle> Rotate(IEnumerable<Triangle> triangles, Matrix4x4 matrix)
        {
            return triangles.Select(RotateTriangle).ToList();

            Triangle RotateTriangle(Triangle t) => new(rot(t.A), rot(t.B), rot(t.C));
            Point3 rot(Point3 p)
            { 
                var v = new Vector3(p.X, p.Y, p.Z);
                var rotated = Vector3.Transform(v, matrix);
                Debug.Assert(v.LengthSquared() == rotated.LengthSquared());
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

        private float GetError(ICollection<Point3> kpoints, ICollection<Point3> points)
        {
            var error = 0f;
            foreach (var can in points)
            {
                var closestDist = kpoints.Min(p => p.EuklidDistTo(can));
                error += closestDist;
            }
            return error;
        }

        private Point3 Mean(IEnumerable<Point3> points)
        {
            var set = points.ToHashSet();
            var xm = set.Select(p => p.X).Average();
            var ym = set.Select(p => p.Y).Average();
            var zm = set.Select(p => p.Z).Average();
            return new Point3((int)xm, (int)ym, (int)zm);
        }

        private static IReadOnlyCollection<(int, int, int)> Overlap(
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
                        // rotations matrix
                        // p = face*px + up*py + right*pz

                        if (face.X > 0.8 && right.Y > 0.8)
                        {
                            Console.WriteLine();
                        }

                        yield return new Matrix4x4(
                            face.X, right.X, up.X, 0,
                            face.Y, right.Y, up.Y, 0,
                            face.Z, right.Z, up.Z, 0,
                            0, 0, 0, 1);

                        yield return new Matrix4x4(
                            face.X, right.X, -up.X, 0,
                            face.Y, right.Y, -up.Y, 0,
                            face.Z, right.Z, -up.Z, 0,
                            0, 0, 0, 1);


                        //yield return p =>
                        //{
                        //    var newx = face * p.X;
                        //    var newy = right * p.Y;
                        //    var newz = up * p.Z;
                        //    var newPoint = newx + newy + newz;
                        //    return new Point3((int)newPoint.X, (int)newPoint.Y, (int)newPoint.Z);
                        //};
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
        private bool IsKongruent(Triangle left, Triangle right)
        {
            if (left.Hash != right.Hash)
                return false;
            // Same dimensions
            var (lefta, leftb) = left.GetVectors();
            var (righta, rightb) = right.GetVectors();

            if (lefta.ManhattanDistTo(Point3.Empty) == righta.ManhattanDistTo(Point3.Empty)) ;
            return lefta == righta && leftb == rightb;
        }
    }
}
