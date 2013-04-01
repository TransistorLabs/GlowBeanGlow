using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlowBeanGlow.Api.Helpers
{
    public static class BitHelpers
    {
        public static byte GetHighByte(ushort value)
        {
            var s = (ushort) (value >> 8);
            return GetLowByte(s);
        }

        public static byte GetLowByte(ushort value)
        {
            return (byte)(value & 0xff);            
        }
    }
}
