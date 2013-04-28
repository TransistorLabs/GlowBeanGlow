using System;
using GlowBeanGlow.Api.Instructions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlowBeanGlow.Api.Tests
{
    [TestClass]
    public class JumpToTests
    {
        [TestMethod]
        public void JumpToHasCorrectInstructionType()
        {
            var instruction = new JumpToInstruction();
            Assert.AreEqual(InstructionTypes.JumpTo, instruction.InstructionType);
        }

        [TestMethod]
        public void GetByteArrayReturnsCorrectlyPopulatedArray()
        {
            var instruction = new JumpToInstruction()
            {
                JumpTargetIndex = 0xAA55
            };

            var bytes = instruction.GetReportData(0);
            Assert.AreEqual(0x00, bytes[0]);
            Assert.AreEqual(0x55, bytes[1]);
            Assert.AreEqual(0xAA, bytes[2]);
            Assert.AreEqual(0x00, bytes[3]);
            Assert.AreEqual(0x00, bytes[4]);
            Assert.AreEqual(0x00, bytes[5]);
            Assert.AreEqual(0x00, bytes[6]);
            Assert.AreEqual(0x00, bytes[7]);
            Assert.AreEqual(0x03, bytes[8]);
        }
    }
}
