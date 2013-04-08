using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using GlowBeanGlow.Api.Display;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlowBeanGlow.Api.Tests
{
    [TestClass]
    public class LedStateTests
    {
        [TestMethod]
        public void LedStateIndexerSetsCorrectBit()
        {
            var state = new LedState();
            Assert.AreEqual(0, state.LedRawBits);

            state[0] = true;
            state[7] = true;

            Assert.AreEqual(0x81, state.LedRawBits);
        }

        [TestMethod]
        public void LedStateIndexerClearsCorrectBit()
        {
            var state = new LedState();
            Assert.AreEqual(0, state.LedRawBits);

            state[0] = true;
            state[7] = true;

            Assert.AreEqual(0x81, state.LedRawBits);

            state[0] = false;
            Assert.AreEqual(0x80, state.LedRawBits);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void LedStateErrorsWhenOutOfRangeIndexIsUsed()
        {
            var state = new LedState();
            var outOfRangeIndex = state.Length + 1;

            state[outOfRangeIndex] = true;
        }
    }
}
