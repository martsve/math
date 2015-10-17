using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathEvaluation;

namespace MathUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestAddition()
        {

            Assert.AreEqual("1+2+3+4+5+6+7+8+9".Evaulate(), 45);
            Assert.AreEqual("1-6+8-4-3-3--2".Evaulate(), -5);
        }
        [TestMethod]
        public void TestMultiply()
        {
            Assert.IsTrue(approx("3*7*23*1.1123".Evaulate(), 537.2409));
            Assert.IsTrue(approx("23/56*12/7*12/-3".Evaulate(), -2.816326531));
        }

        [TestMethod]
        public void TestPower()
        {
            Assert.AreEqual("4^3".Evaulate(), 64);
        }
        [TestMethod]
        public void TestPower2()
        {
            Assert.AreEqual("2^3^4".Evaulate(), 4096);
        }

        [TestMethod]
        public void TestMod()
        {
            Assert.AreEqual("6%3".Evaulate(), 0);
            Assert.AreEqual("123325%3".Evaulate(), 1);
        }
        [TestMethod]
        public void TestFactorial()
        {
            Assert.AreEqual("8!".Evaulate(), 40320);
        }
        [TestMethod]
        public void TestFactorial2()
        {
            Assert.AreEqual("3!!".Evaulate(), 720);
        }

        [TestMethod]
        public void TestFactorial3()
        {
            Assert.IsTrue(approx("4!!".Evaulate(), 6.2044E+23));

        }

        [TestMethod]
        public void TestOrder()
        {
            Assert.AreEqual("-1+123^3*2".Evaulate(), 3721733);
            Assert.AreEqual("63*9+123325%3".Evaulate(), 568);
            Assert.AreEqual("10*4!/3".Evaulate(), 80);
        }

        [TestMethod]
        public void TestPara()
        {
            Assert.AreEqual("1+(2-(-3))".Evaulate(), 6);
        }

        bool approx(double a, double b, int significance = 3)
        {
            return Math.Abs((a-b)/b) < 1 / Math.Pow(10, significance);
        }


    }

}
