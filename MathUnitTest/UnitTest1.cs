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
            Assert.AreEqual("3*7*23*1.1123".Evaulate(), 537.2409);
            Assert.AreEqual("23/56+12/7-12/-3".Evaulate(), 6.125);
        }
        [TestMethod]
        public void TestPower()
        {
            Assert.AreEqual("-1+123^3*2".Evaulate(), 3721733);
            Assert.AreEqual("63*9+123325%3".Evaulate(), 568);
            Assert.AreEqual("10*4!/3".Evaulate(), 80);
        }

    }

}
