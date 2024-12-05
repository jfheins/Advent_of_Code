using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Core.Test
{
    [TestClass]
    public class LinqTests
    {
        [DataTestMethod]
        [DataRow("AABBCCDD", "AA", "BB", "CC", "DD")]
        [DataRow("AAAABBC", "AAAA", "BB", "C")]
        [DataRow("ABGR", "A", "B", "G", "R")]
        [DataRow("AAAAAAA", "AAAAAAA")]
        [DataRow("1100111", "11", "00", "111")]
        public void RightChunks(string data, params string[] expectedChunks)
        {
            var chunks = data.Chunks().Select(c => string.Concat(c)).ToList();
            CollectionAssert.AreEqual(expectedChunks, chunks, $"string: '{data}'");
        }

        [DataTestMethod]
        [DataRow("AABBCCDD")]
        [DataRow("AAAABBC")]
        [DataRow("ABGR")]
        [DataRow("AAAAAAA")]
        [DataRow("1100111")]
        public void RightRuns(string data)
        {
            var runs = data.Runs().ToList();
            var chunks = data.Chunks().ToList();

            foreach (var (run, chunk) in runs.Zip(chunks))
            {
                Assert.AreEqual(chunk.Length, run.Count);
                Assert.AreEqual(chunk.ToArray()[0], run.Element);
            }
        }

        [TestMethod]
        public void RightChunksNonString()
        {
            var array = new[] { 1, 1, 1, 2, 2, 3, 6, 9, 9, 9, 8, 7, 7, 7, 5, 5, 4, 4, 4, 8, 8, 8, 8, 3, 3, 3, 3, 3, 9 };
            var chunks = array.Chunks();

            var expected = new int[][] {
                [1, 1, 1], [2, 2], [3], [6],
                [9, 9, 9], [8], [7, 7, 7], [5, 5],
                [4, 4, 4], [8, 8, 8, 8], [3, 3, 3, 3, 3], [9]
            };

            foreach (var (exp, result) in expected.Zip(chunks))
                CollectionAssert.AreEqual(exp, result);
        }


        [TestMethod]
        public void RightNumberOfDoubles()
        {
            static bool Check(int num) => num.ToString().Chunks().Any(c => c.Length == 2);

            var range = Enumerable.Range(134564, 450596);
            Assert.AreEqual(166392, range.Count(Check));
        }


        [TestMethod]
        public void StepBy3()
        {
            var array = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            var result = array.StepBy(3).ToArray();
            CollectionAssert.AreEqual(new[] {1, 4, 7} , result);
            result = array.StepBy(3, 1).ToArray();
            CollectionAssert.AreEqual(new[] { 2, 5, 8 }, result);
            result = array.StepBy(4, 3).ToArray();
            CollectionAssert.AreEqual(new[] { 4, 8 }, result);
            result = array.StepBy(1, 7).ToArray();
            CollectionAssert.AreEqual(new[] { 8, 9 }, result);
        }
        
        

        [DataTestMethod]
        [DataRow("AA_BB_CC", "AA", "BB", "CC")]
        [DataRow("__A_B__", "A", "B")]
        [DataRow("A__B__C__D", "A", "B", "C", "D")]
        public void SplitTest(string data, params string[] expected)
        {
            var result1 = data.Split("_", StringSplitOptions.RemoveEmptyEntries);
            var result2 = data.ToCharArray().SplitBy('_');

            CollectionAssert.AreEqual(result1, result2.SelectList(it => new string(it)));
        }
    }
}
