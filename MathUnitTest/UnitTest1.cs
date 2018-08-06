using System;
using math;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestAddition()
        {

            Assert.AreEqual("1+2+3+4+5+6+7+8+9".Calculate(), 45);
            Assert.AreEqual("1-6+8-4-3-3--2".Calculate(), -5);
        }
        [TestMethod]
        public void TestMultiply()
        {
            Assert.IsTrue(Approx("3*7*23*1.1123".Calculate(), 537.2409));
            Assert.IsTrue(Approx("23/56*12/7*12/-3".Calculate(), -2.816326531));
        }

        [TestMethod]
        public void TestPower()
        {
            Assert.AreEqual("4^3".Calculate(), 64);
        }
        [TestMethod]
        public void TestPower2()
        {
            Assert.AreEqual("2^3^4".Calculate(), 4096);
        }

        [TestMethod]
        public void TestMod()
        {
            Assert.AreEqual("6%3".Calculate(), 0);
            Assert.AreEqual("123325%3".Calculate(), 1);
        }
        [TestMethod]
        public void TestFactorial()
        {
            Assert.AreEqual("8!".Calculate(), 40320);
        }
        [TestMethod]
        public void TestFactorial2()
        {
            Assert.AreEqual("3!!".Calculate(), 720);
        }

        [TestMethod]
        public void TestFactorial3()
        {
            Assert.IsTrue(Approx("4!!".Calculate(), 6.2044E+23));

        }

        [TestMethod]
        public void TestOrder()
        {
            Assert.AreEqual("-1+123^3*2".Calculate(), 3721733);
            Assert.AreEqual("63*9+123325%3".Calculate(), 568);
            Assert.AreEqual("10*4!/3".Calculate(), 80);
        }

        [TestMethod]
        public void TestMinusStickingToNumber1()
        {
            Assert.AreEqual("1--1".Calculate(), 2);
            Assert.AreEqual("2-cos(0)".Calculate(), 1);
            Assert.AreEqual("-cos(0)".Calculate(), -1);
            Assert.AreEqual("-abs(-2)^2".Calculate(), -4);
        }

        [TestMethod]
        public void TestPara()
        {
            Assert.AreEqual("1+(2-(-3))".Calculate(), 6);
        }

        private bool Approx(double a, double b, int significance = 3)
        {
            return Math.Abs((a-b)/b) < 1 / Math.Pow(10, significance);
        }
    }
}
