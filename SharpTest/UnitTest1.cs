using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharp;

namespace SharpTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {

            for (int i = 0; i < 10000; i++) {

                Assert.AreEqual(i, rBase31.rBase31ToNumber(rBase31.NumberTorBase31(i)));

                var x = new rBase31(i); 

                Assert.AreEqual(i, new rBase31(x).NumericValue); 

            }

        }
    }
}
