using System;
using GlowBeanGlow.Api.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlowBeanGlow.Api.Tests
{
    [TestClass]
    public class DeviceConfigurationTests
    {
        [TestMethod]
        public void DeviceConfigurationPopulatesCorrectlyFromByteArray()
        {
            var byteData = new byte[] { 0x00, 0xff, 0x80, 0x0f, 0x00, 0x00, 0x7f, 0x01, 0x00 };
            var settings = DeviceConfiguration.CreateConfigurationObjectFromBytes(byteData);
            Assert.AreEqual(0xff, settings.OfflineColor.Red);
            Assert.AreEqual(0x80, settings.OfflineColor.Green);
            Assert.AreEqual(0x0f, settings.OfflineColor.Blue);
            Assert.AreEqual(0x017f, settings.MaxInstructions);
        }
    }
}
