using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using GlowBeanGlow.Api.Instructions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlowBeanGlow.Api.Tests
{
    [TestClass]
    public class SetFrameTests
    {
        [TestMethod]
        public void SetFrameHasCorrectInstructionType()
        {
            var instruction = new SetFrameInstruction();
            Assert.AreEqual(InstructionTypes.SetFrame, instruction.InstructionType);
        }

        [TestMethod]
        public void GetByteArrayReturnsCorrectlyPopulatedArray()
        {
            var instruction = new SetFrameInstruction
                {
                    Color = { Red = 0xff, Green = 0x00, Blue = 0x10 },
                    Leds = { LedRawBits = 0xaa55 },
                    MillisecondsHold = 0xbb66,
                };
            var bytes = instruction.GetReportData(0);
            Assert.AreEqual(0x00, bytes[0]);
            Assert.AreEqual(0xff, bytes[1]);
            Assert.AreEqual(0x00, bytes[2]);
            Assert.AreEqual(0x10, bytes[3]);
            Assert.AreEqual(0x55, bytes[4]);
            Assert.AreEqual(0xaa, bytes[5]);
            Assert.AreEqual(0xbb, bytes[6]);
            Assert.AreEqual(0x66, bytes[7]);
            Assert.AreEqual(0x00, bytes[8]);
        }

        [TestMethod]
        public void GetByteArrayReturnsCorrectArrayLength()
        {
            var instruction = new SetFrameInstruction
            {
                Color = { Red = 0xff, Green = 0x00, Blue = 0x10 },
                Leds = { LedRawBits = 0xaa55 },
                MillisecondsHold = 0xbb66,
            };
            var bytes = instruction.GetReportData(0);
            Assert.AreEqual(9, bytes.Length);
        }
    }
}
