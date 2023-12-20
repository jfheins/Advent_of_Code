using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core;

public class LoopDetector(int windowSize = 100)
{
    /// <summary>
    /// Maximum loop length that can be found
    /// </summary>
    public int WindowSize { get; } = windowSize;

    public int LastIteration { get; private set; }
    public bool HasLoop { get; private set; }
    public int LoopSize { get; private set; }

    public int MinLoopCheckSize { get; init; } = 1;

    private readonly long[] PastValues = new long[windowSize * 2];

    public void Feed(int iteration, long value)
    {
        Array.Copy(PastValues, 0, PastValues, 1, PastValues.Length - 1);
        PastValues[0] = value;
        LastIteration = iteration;
        if (iteration > windowSize)
            DetectLoop();
    }

    public void DetectLoop()
    {
        for (var loopSize = MinLoopCheckSize; loopSize <= WindowSize; loopSize++)
        {
            var checkCount = (PastValues.Length / loopSize) - 1;
            ReadOnlySpan<long> reference = PastValues.AsSpan(0, loopSize);
            var allEqual = true;
            for (var i = 1; i <= checkCount; i++)
            {
                var checkSpan = PastValues.AsSpan(i * loopSize, loopSize);
                allEqual = allEqual && reference.SequenceEqual(checkSpan);
                if (!allEqual)
                    break;
            }

            if (allEqual)
            {
                HasLoop = true;
                LoopSize = loopSize;
                break;
            }
        }
    }

    public long Extrapolate(int targetIter)
    {
        Debug.Assert(HasLoop);
        var remainder = (targetIter - LastIteration) % LoopSize;
        return PastValues[LoopSize - remainder];
    }
}