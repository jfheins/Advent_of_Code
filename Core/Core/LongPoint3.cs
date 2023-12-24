using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Intrinsics;

namespace Core
{
    public struct LongPoint3 : IEquatable<LongPoint3>, ICanEnumerateNeighbors<LongPoint3>
    {
        public static readonly LongPoint3 Empty = new(0, 0, 0);

        public LongPoint3(long x, long y, long z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public override string ToString() => $"<X: {X}, Y: {Y}, Z: {Z}>";

        public bool IsEmpty => this.Equals(Empty);
        public long X { get; set; }
        public long Y { get; set; }
        public long Z { get; set; }

        public bool Equals(LongPoint3 other) => (X == other.X) && (Y == other.Y) && (Z == other.Z);

        public override int GetHashCode() => HashCode.Combine(X, Y, Z);

        public override bool Equals(object? obj) => (obj is LongPoint3 p) && Equals(p);

        public LongPoint3 TranslateBy(long dx, long dy, long dz) => new(X + dx, Y + dy, Z + dz);
        public LongPoint3 TranslateBy((long dx, long dy, long dz) velocity)
            => new(X + velocity.dx, Y + velocity.dy, Z + velocity.dz);
        public LongPoint3 TranslateBy(Vector3 velocity)
            => new(X + (long)velocity.X, Y + (long)velocity.Y, Z + (long)velocity.Z);

        public LongPoint3 TranslateBy(LongPoint3 offset)
            => TranslateBy(offset.X, offset.Y, offset.Z);

        public static LongPoint3 FromArray(long[] arr, long offset = 0)
        {
            if (arr == null)
                throw new ArgumentNullException(nameof(arr));

            if (arr.Length < offset + 3)
                throw new ArgumentException("Not enough elements in the array!");

            return new LongPoint3(arr[offset], arr[offset + 1], arr[offset + 2]);
        }


        public IEnumerable<LongPoint3> GetNeighborsDiag()
        {
            var deltas = new[] { -1, 0, 1 };
            foreach (var dx in deltas)
                foreach (var dy in deltas)
                    foreach (var dz in deltas)
                        if (dx != 0 || dy != 0 || dz != 0)
                            yield return TranslateBy(dx, dy, dz);
        }

        public static bool operator ==(LongPoint3 left, LongPoint3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LongPoint3 left, LongPoint3 right)
        {
            return !(left == right);
        }

        public static LongPoint3 operator *(LongPoint3 l, long n)
            => new(l.X * n, l.Y * n, l.Z * n);

        public static LongPoint3 operator *(LongPoint3 l, double n)
            => new((long)(l.X * n), (long)(l.Y * n), (long)(l.Z * n));

        public static LongPoint3 operator +(LongPoint3 l, LongPoint3 r)
            => new(l.X + r.X, l.Y + r.Y, l.Z + r.Z);

        public static LongPoint3 operator -(LongPoint3 l, LongPoint3 r)
            => new(l.X - r.X, l.Y - r.Y, l.Z - r.Z);

        public LongPoint3 Inverse() => new(-X, -Y, -Z);

        public Vector3 ToVector() => new(X, Y, Z);

        public Vector256<long> AsVector128() => Vector256.Create(X, Y, Z, 0);

        public static LongPoint3 FromVector128(Vector128<long> v) => new(v.GetElement(0), v.GetElement(1), v.GetElement(2));
    }
}
