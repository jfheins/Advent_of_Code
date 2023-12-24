using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Core;

public readonly record struct LongInterval : IEnumerable<long>
{
    public long Start { get; } // Inclusive
    public long End { get; } // Exclusive

    public LongInterval(long start, long end)
    {
        Debug.Assert(start < end);
        Start = start;
        End = end;
    }

    public static LongInterval FromStartAndLength(long start, long length) => new(start, start + length);
    public static LongInterval FromInclusiveEnd(long start, long end) => new(start, end + 1);
    public static LongInterval? Create(long start, long end) => start < end ? new LongInterval(start, end) : null;

    public long Length => End - Start;
    public bool IsEmpty => Start == End;

    /// <summary>
    /// Returns true, if the other interval is a subset of this interval.
    /// </summary>
    /// <param name="other">Other interval</param>
    public bool Contains(LongInterval other) => Start <= other.Start && other.End <= End;

    public bool Contains(long point) => Start <= point && point < End;
    public bool Contains(double point) => Start <= point && point < End;


    /// <summary>
    /// Return true, if this interval has at least one element in common with the other interval.
    /// By definition, return false if one interval is empty.
    /// </summary>
    /// <param name="other">Other interval</param>
    /// <returns></returns>
    public bool OverlapsWith(LongInterval other) => Start < other.End && other.Start < End;

    public (LongInterval? prefix, LongInterval? intersection, LongInterval? suffix) Intersect(LongInterval other)
    {
        var intersection = Create(Math.Max(Start, other.Start), Math.Min(End, other.End));
        if (intersection is null)
            return (null, null, null);
        var prefix = Create(Start, intersection.Value.Start);
        var suffix = Create(intersection.Value.End, End);
        return (prefix, intersection, suffix);
    }

    public static explicit operator LongInterval(Interval it) => new(it.Start, it.End);

    public IEnumerator<long> GetEnumerator()
    {
        var current = Start;
        while (current < End)
            yield return current++;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}