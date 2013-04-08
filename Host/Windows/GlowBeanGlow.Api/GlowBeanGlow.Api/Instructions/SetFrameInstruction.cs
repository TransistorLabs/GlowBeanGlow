using GlowBeanGlow.Api.Display;
using GlowBeanGlow.Api.Helpers;
using GlowBeanGlow.Api.Interfaces;

namespace GlowBeanGlow.Api.Instructions
{
    public class SetFrameInstruction : IReportData, IInstruction
    {
        public SetFrameInstruction()
        {
            Leds = new LedState();
            Color = new RgbColor();
        }

        public RgbColor Color { get; set; }
        public LedState Leds { get; set; }
        public ushort MillisecondsHold { get; set; }

        public InstructionTypes InstructionType
        {
            get { return InstructionTypes.SetFrame; }
        }

        public byte[] GetReportData(byte reportId = 0)
        {
            var byteArray = new byte[9];
            byteArray[0] = reportId;
            byteArray[1] = Color.Red;
            byteArray[2] = Color.Green;
            byteArray[3] = Color.Blue;
            byteArray[4] = BitHelpers.GetLowByte(Leds.LedRawBits);
            byteArray[5] = BitHelpers.GetHighByte(Leds.LedRawBits);
            byteArray[6] = BitHelpers.GetHighByte(MillisecondsHold);
            byteArray[7] = BitHelpers.GetLowByte(MillisecondsHold);
            byteArray[8] = (byte)InstructionType;
            return byteArray;
        }
    }
}
