using System;
using GlowBeanGlow.Api.Instructions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlowBeanGlow.Api.Tests
{
    [TestClass]
    public class IncrementFrameTests
    {
        [TestMethod]
        public void SetFrameHasCorrectInstructionType()
        {
            var instruction = new IncrementFrameInstruction();
            Assert.AreEqual(InstructionTypes.IncrementFrame, instruction.InstructionType);
        }

        [TestMethod]
        public void GetByteArrayReturnsCorrectlyPopulatedArrayForNegativeIncrements()
        {
            var instruction = new IncrementFrameInstruction
            {
                RedIncrement = -1,
                GreenIncrement = -2,
                BlueIncrement = -3,
            };

            var bytes = instruction.GetReportData(0);
            Assert.AreEqual(0xff, bytes[1]);
            Assert.AreEqual(0xfe, bytes[2]);
            Assert.AreEqual(0xfd, bytes[3]);
        }

        [TestMethod]
        public void GetByteArrayReturnsCorrectArrayLength()
        {
            var instruction = new IncrementFrameInstruction
            {
                RedIncrement = 0x7f,
                GreenIncrement = -0x02,
                BlueIncrement = 0x00,
                ColorIncrementDelayMs = 0x0f,
                ColorIncrementCount = 0xff,
                LedShiftDelayMs = 0x0f,
                LedShiftCount = 0x03
            };

            var bytes = instruction.GetReportData(0);
            Assert.AreEqual(9, bytes.Length);
        }
    }
}
