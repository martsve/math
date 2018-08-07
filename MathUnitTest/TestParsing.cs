using System;
using math;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathUnitTest
{
    [TestClass]
    public class TestParsing
    {
        [TestMethod]
        public void ParseNumbers()
        {
            Assert.AreEqual("123".Calculate(), 123);
            Assert.AreEqual("123.23".Calculate(), 123.23);
        }

        [TestMethod]
        public void ParseNumbersNegative()
        {
            Assert.AreEqual("-123".Calculate(), -123);
            Assert.AreEqual("-123.23".Calculate(), -123.23);
        }

        [TestMethod]
        public void ParseScientific()
        {
            Assert.AreEqual("123E02".Calculate(), 12300);
            Assert.AreEqual("123.23E+1".Calculate(), 1232.3);
            Assert.AreEqual("123.23E-1".Calculate(), 12.323);
        }

        [TestMethod]
        public void ParseScientificNegative()
        {
            Assert.AreEqual("-123E02".Calculate(), -12300);
            Assert.AreEqual("-123.23E+1".Calculate(), -1232.3);
            Assert.AreEqual("-123.23E-1".Calculate(), -12.323);
        }

        [TestMethod]
        public void ParseZero()
        {
            Assert.AreEqual("0".Calculate(), 0);
            Assert.AreEqual("0.0".Calculate(), 0);
            Assert.AreEqual("00.0000".Calculate(), 0);
            Assert.AreEqual("0E00".Calculate(), 0);
            Assert.AreEqual("-0E00".Calculate(), 0);
            Assert.AreEqual("0.00E00".Calculate(), 0);
            Assert.AreEqual("-0.00E00".Calculate(), 0);
        }

        private bool Approx(double a, double b, int significance = 9)
        {
            return Math.Abs((a-b)/b) < 1 / Math.Pow(10, significance);
        }
    }
}
