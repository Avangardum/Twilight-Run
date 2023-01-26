using System.Numerics;
using NUnit.Framework;
using Avangardum.TwilightRun.Models;
// ReSharper disable InconsistentNaming

namespace Avangardum.TwilightRun.Tests
{
    public class AabbUtilTests
    {
        private static readonly object[] DoAabbsIntersectByPositionAndSize_Cases =
        {
            new object[] { new Vector2(-10, -10), new Vector2(1, 1), new Vector2(10, 10), new Vector2(1, 1), false },
            new object[] { new Vector2(-10, -10), new Vector2(30, 30), new Vector2(10, 10), new Vector2(10, 10), true },
            new object[] { new Vector2(-10, -10), new Vector2(40, 40), new Vector2(10, 10), new Vector2(20, 20), true },
            new object[] { new Vector2(-5, 0), new Vector2(2, 1), new Vector2(5, 0), new Vector2(1, 2), false },
            new object[] { new Vector2(-0.5f, 0), new Vector2(2, 1), new Vector2(0.5f, 0), new Vector2(1, 2), true },
            new object[] { new Vector2(0, 0), new Vector2(3, 5), new Vector2(0, 0), new Vector2(1, 1), true }
        };

        [TestCaseSource(nameof(DoAabbsIntersectByPositionAndSize_Cases))]
        public void DoAabbsIntersect(Vector2 aabb1Position, Vector2 aabb1Size,
            Vector2 aabb2Position, Vector2 aabb2Size, bool expectedResult)
        {
            var result = AabbUtil.DoAabbsIntersect(aabb1Position, aabb1Size, aabb2Position, aabb2Size);
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        
        [TestCaseSource(nameof(DoAabbsIntersectByPositionAndSize_Cases))]
        public void DoAabbsIntersectCommutative(Vector2 aabb1Position, Vector2 aabb1Size,
            Vector2 aabb2Position, Vector2 aabb2Size, bool expectedResult)
        {
            var result = AabbUtil.DoAabbsIntersect(aabb2Position, aabb2Size, aabb1Position, aabb1Size);
            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}