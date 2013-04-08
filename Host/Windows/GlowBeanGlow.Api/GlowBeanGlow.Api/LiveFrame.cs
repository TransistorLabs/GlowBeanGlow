using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlowBeanGlow.Api.Display;
using GlowBeanGlow.Api.Helpers;
using GlowBeanGlow.Api.Instructions;
using GlowBeanGlow.Api.Interfaces;

namespace GlowBeanGlow.Api
{
    public class LiveFrame : IReportData
    {
        public LiveFrame()
        {
            Leds = new LedState();
            Color = new RgbColor();
        }

        public RgbColor Color { get; set; }
        public LedState Leds { get; set; }

        public byte[] GetReportData(byte reportId = 0)
        {
            var byteArray = new byte[9];
            byteArray[0] = reportId;
            byteArray[1] = Color.Red;
            byteArray[2] = Color.Green;
            byteArray[3] = Color.Blue;
            byteArray[4] = BitHelpers.GetLowByte(Leds.LedRawBits);
            byteArray[5] = BitHelpers.GetHighByte(Leds.LedRawBits);

            return byteArray;
        }
    }
}
