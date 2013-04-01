using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using GlowBeanGlow.Api.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlowBeanGlow.Api.Tests
{
    [TestClass]
    public class HelperTests
    {
        [TestMethod]
        public void GetHighByteReturnsCorrectValue()
        {
            var highByte = BitHelpers.GetHighByte(0xaa55);
            Assert.AreEqual(0xaa, highByte);
        }

        [TestMethod]
        public void GetLowByteReturnsCorrectValue()
        {
            var lowByte = BitHelpers.GetLowByte(0xaa55);
            Assert.AreEqual(0x55, lowByte);
        }
    }
}
