using System;
using System.Drawing;

namespace Core;

/// <summary>
/// Helper methods for geometric algorithms
/// </summary>
public static class AlgebraHelpers
{
    /// <summary>
    /// Calculate orientation: cross product of vectors (b-a) and (c-a)
    /// Returns positive if c is left of line ab, negative if right, zero if collinear
    /// </summary>
    public static int Orient(Point a, Point b, Point c)
    {
        var ba = new Point(b.X - a.X, b.Y - a.Y);
        var ca = new Point(c.X - a.X, c.Y - a.Y);
        return Math.Sign(ba.Cross(ca));
    }
    
    /// <summary>
    /// Check if line segments (a,b) and (c,d) properly intersect
    /// Proper intersection means segments cross each other (not just touching at endpoints)
    /// </summary>
    public static bool ProperIntersection(Point a, Point b, Point c, Point d)
    {
        var oa = Orient(c, d, a);
        var ob = Orient(c, d, b);
        var oc = Orient(a, b, c);
        var od = Orient(a, b, d);
        
        // Proper intersection exists iff opposite signs
        return oa * ob < 0 && oc * od < 0;
    }
}

