using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Linq;

namespace Core.Test;

[TestClass]
public class GridTilerTests
{
    [TestMethod]
    public void GetPoints()
    {
        var tiler = new GridTiler([new(7, 7), new(16, 16)],
            new Rectangle(0, 0, 160, 160));

        Assert.AreEqual(1, tiler.GetPointsOnTile(0, 0).Count);
        Assert.AreEqual(0, tiler.GetPointsOnTile(1, 1).Count);
        Assert.AreEqual(1, tiler.GetPointsOnTile(2, 2).Count);
    }
    
    [TestMethod]
    public void GetNeighborhood()
    {
        var tiler = new GridTiler([new(7, 7), new(16, 16)],
            new Rectangle(0, 0, 160, 160));

        Assert.AreEqual(0, tiler.GetNeighborhood(new Point(8, 8), 1).Count());
        Assert.AreEqual(1, tiler.GetNeighborhood(new Point(8, 8), 2).Count());
    }
    
    [TestMethod]
    public void TestDiamond()
    {
        var origin = new Point(77, 77);
        var allPoints = new HashSet<Point>();

        for (var dx = -12; dx < 12; dx++)
        {
            for (var dy = -12; dy < 12; dy++)
                allPoints.Add(origin.MoveBy(dx, dy));
        }
        
        var tiler = new GridTiler(allPoints, new Rectangle(0, 0, 160, 160));

        for (var i = 0; i < 12; i++)
        {
            var expected = i * (i + 1) * 2 + 1;
            Assert.AreEqual(expected, tiler.GetNeighborhood(origin, i).Count());
        }
    }
}