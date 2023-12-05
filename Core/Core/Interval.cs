using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Core;

public readonly record struct Interval : IEnumerable<int>
{
    public int Start { get; } // Inclusive
    public int End { get; } // Exclusive

    public Interval(int start, int end) : this()
    {
        Debug.Assert(start <= end);
        Start = start;
        End = end;
    }

    public int Length => End - Start;
    public bool IsEmpty => Start == End;

    public IEnumerator<int> GetEnumerator()
    {
        return Enumerable.Range(Start, Length).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static Interval FromInclusiveBounds(int start, int end) => new(start, end + 1);

    /// <summary>
    /// Returns true, if the other interval is a subset of this interval.
    /// </summary>
    /// <param name="other">Other interval</param>
    public bool Contains(Interval other)
        => Start <= other.Start && other.End <= End;

    public bool Contains(int point)
        => Start <= point && point < End;

    /// <summary>
    /// Return true, if this interval has at least one element in common with the other interval.
    /// By definition, return false if one interval is empty.
    /// </summary>
    /// <param name="other">Other interval</param>
    /// <returns></returns>
    public bool OverlapsWith(Interval other) => Start < other.End && other.Start < End;
}