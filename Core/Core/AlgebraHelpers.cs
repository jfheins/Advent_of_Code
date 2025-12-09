using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Core;

/// <summary>
/// Helper methods for geometric algorithms
/// </summary>
public static class AlgebraHelpers
{
    /// <summary>
    /// Calculate orientation: cross product of vectors (b-a) and (c-a)
    /// Returns +1 if c is left of line ab, -1 if right, 0 if collinear
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
    
    /// <summary>
    /// Check if a point is inside a polygon using the winding number algorithm
    /// The polygon should be closed (first point == last point)
    /// </summary>
    public static bool IsPointInPolygon(Point point, IReadOnlyCollection<Point> polygon)
    {
        var windingNumber = 0;
        var a = polygon.Last();
        
        foreach (var b in polygon)
        {
            if (a.Y <= point.Y)
            {
                if (b.Y > point.Y) // Upward crossing
                {
                    if (Orient(a, b, point) > 0) // Point is left of edge
                        windingNumber++;
                }
            }
            else
            {
                if (b.Y <= point.Y) // Downward crossing
                {
                    if (Orient(a, b, point) < 0) // Point is right of edge
                        windingNumber--;
                }
            }
            a = b;
        }
        
        return windingNumber != 0;
    }
}

