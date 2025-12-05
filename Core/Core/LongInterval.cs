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
    
    public static LongInterval ParseInclusive(ReadOnlySpan<char> str)
    {
        var indexOfDash = str.IndexOf('-');
        var start = str.Slice(0, indexOfDash);
        var end = str.Slice(indexOfDash + 1);
        return FromInclusiveEnd(long.Parse(start), long.Parse(end));
    }

    public long Length => End - Start;
    public bool IsEmpty => Start == End;

    /// <summary>
    /// Returns true, if the other interval is a subset of this interval.
    /// </summary>
    /// <param name="other">Other interval</param>
    public bool Contains(LongInterval other) => Start <= other.Start && other.End <= End;

    public bool Contains(long point) => Start <= point && point < End;
    public bool Contains(double point) => Start <= point && point < End;
    public long Last() => Length > 0 ? End - 1 : throw new InvalidOperationException("Interval is empty");


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

    /// <summary>
    /// Returns the union of this interval with the other interval.
    /// Throws an exception if the intervals do not overlap.
    /// </summary>
    public LongInterval Union(LongInterval other)
    {
        if (!OverlapsWith(other))
            throw new InvalidOperationException("Intervals do not overlap");
        return new LongInterval(Math.Min(Start, other.Start), Math.Max(End, other.End));
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