using GlowBeanGlow.Api.Display;
using GlowBeanGlow.Api.Helpers;
using GlowBeanGlow.Api.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlowBeanGlow.Api
{
    public class FullColorLiveFrame
    {
        private const byte LedFrameTypeFullColorFramePart = 0x01;
        public FullColorLiveFrame()
        {
            Colors = new List<RgbColor>();
            Leds = new LedState();
        }

        public IList<RgbColor> Colors { get; set; }
        public LedState Leds { get; set; }

        public byte[] GetReportDataForPage(int pageNumber, byte reportId = 0)
        {
            var byteArray = new byte[9];
            byteArray[0] = reportId;
            var index = pageNumber * 2;

            switch (pageNumber)
            {
                case 0:
                    byteArray[7] = BitHelpers.GetLowByte(Leds.LedRawBits);
                    break;

                case 1:
                    byteArray[7] = BitHelpers.GetHighByte(Leds.LedRawBits);
                    break;
            }

            byteArray[1] = Colors[index].Red;
            byteArray[2] = Colors[index].Green;
            byteArray[3] = Colors[index].Blue;

            ++index;
            byteArray[4] = Colors[index].Red;
            byteArray[5] = Colors[index].Green;
            byteArray[6] = Colors[index].Blue;

            byte w = LedFrameTypeFullColorFramePart;
            w |= Convert.ToByte(pageNumber << 4);
            byteArray[8] = w;

            return byteArray;
        }

        public void RotateColorsClockwise()
        {
            var lastColor = Colors.Last();
            for (int i = Colors.Count - 1; i > 0; i--)
            {
                Colors[i] = Colors[i - 1];
            }

            Colors[0] = lastColor;
        }
    }
}
