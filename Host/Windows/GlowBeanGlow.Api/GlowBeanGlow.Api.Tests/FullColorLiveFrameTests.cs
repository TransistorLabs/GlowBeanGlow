using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlowBeanGlow.Api.Tests
{
    [TestClass]
    public class FullColorLiveFrameTests
    {
        [TestMethod]
        public void LiveFrameReturnsCorrectReportBytesForPage0()
        {
            var frame = new FullColorLiveFrame();
            frame.Colors.Add(new Display.RgbColor { Red = 0xff, Green = 0xfe, Blue = 0xfd });
            frame.Colors.Add(new Display.RgbColor { Red = 0x0f, Green = 0x0e, Blue = 0x0d });
            var reportBytes = frame.GetReportDataForPage(0);

            Assert.AreEqual(0x00, reportBytes[0]);
            Assert.AreEqual(0xff, reportBytes[1]);
            Assert.AreEqual(0xfe, reportBytes[2]);
            Assert.AreEqual(0xfd, reportBytes[3]);
            Assert.AreEqual(0x0f, reportBytes[4]);
            Assert.AreEqual(0x0e, reportBytes[5]);
            Assert.AreEqual(0x0d, reportBytes[6]);
            Assert.AreEqual(0x00, reportBytes[7]);
            Assert.AreEqual(0x01, reportBytes[8]);
        }

        [TestMethod]
        public void LiveFrameReturnsCorrectReportBytesForPage1()
        {
            var frame = new FullColorLiveFrame();
            frame.Colors.Add(new Display.RgbColor { });
            frame.Colors.Add(new Display.RgbColor { });
            frame.Colors.Add(new Display.RgbColor { Red = 0xff, Green = 0xfe, Blue = 0xfd });
            frame.Colors.Add(new Display.RgbColor { Red = 0x0f, Green = 0x0e, Blue = 0x0d });
            var reportBytes = frame.GetReportDataForPage(1);

            Assert.AreEqual(0x00, reportBytes[0]);
            Assert.AreEqual(0xff, reportBytes[1]);
            Assert.AreEqual(0xfe, reportBytes[2]);
            Assert.AreEqual(0xfd, reportBytes[3]);
            Assert.AreEqual(0x0f, reportBytes[4]);
            Assert.AreEqual(0x0e, reportBytes[5]);
            Assert.AreEqual(0x0d, reportBytes[6]);
            Assert.AreEqual(0x00, reportBytes[7]);
            Assert.AreEqual(0x11, reportBytes[8]);
        }

        [TestMethod]
        public void LiveFrameReturnsCorrectReportBytesForPage5()
        {
            var frame = new FullColorLiveFrame();
            frame.Leds.LedRawBits = (ushort)0xaa55;

            frame.Colors.Add(new Display.RgbColor { });
            frame.Colors.Add(new Display.RgbColor { });
            frame.Colors.Add(new Display.RgbColor { });
            frame.Colors.Add(new Display.RgbColor { });
            frame.Colors.Add(new Display.RgbColor { });
            frame.Colors.Add(new Display.RgbColor { });
            frame.Colors.Add(new Display.RgbColor { });
            frame.Colors.Add(new Display.RgbColor { });
            frame.Colors.Add(new Display.RgbColor { });
            frame.Colors.Add(new Display.RgbColor { });

            frame.Colors.Add(new Display.RgbColor { Red = 0xff, Green = 0xfe, Blue = 0xfd });
            var reportBytes = frame.GetReportDataForPage(5);

            Assert.AreEqual(0x00, reportBytes[0]);
            Assert.AreEqual(0xff, reportBytes[1]);
            Assert.AreEqual(0xfe, reportBytes[2]);
            Assert.AreEqual(0xfd, reportBytes[3]);
            Assert.AreEqual(0x55, reportBytes[4]);
            Assert.AreEqual(0xaa, reportBytes[5]);
            Assert.AreEqual(0x00, reportBytes[6]);
            Assert.AreEqual(0x00, reportBytes[7]);
            Assert.AreEqual(0x52, reportBytes[8]);
        }
    }
}


