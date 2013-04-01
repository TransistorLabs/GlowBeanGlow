using GlowBeanGlow.Api.DataTypes.Enumerations;
using GlowBeanGlow.Api.Helpers;

namespace GlowBeanGlow.Api.DataTypes
{
	public class SetFrameInstruction : IInstruction
	{
		public SetFrameInstruction()
		{
			Leds = new LedState();
		}

		public byte Red { get; set; }
		public byte Green { get; set; }
		public byte Blue { get; set; }
		public LedState Leds { get; set; }
		public ushort MillisecondsHold { get; set; }

	    public InstructionTypes InstructionType
	    {
	        get { return InstructionTypes.SetFrame; }
	    }

	    public byte[] GetByteArray(byte reportId)
        {
            var byteArray = new byte[9];
			byteArray[0] = reportId;
			byteArray[1] = Red;
	        byteArray[2] = Green;
	        byteArray[3] = Blue;
            byteArray[4] = BitHelpers.GetLowByte(Leds.LedRawBits);
			byteArray[5] = BitHelpers.GetHighByte(Leds.LedRawBits);
			byteArray[6] = BitHelpers.GetHighByte(MillisecondsHold);
            byteArray[7] = BitHelpers.GetLowByte(MillisecondsHold);
	        byteArray[8] = (byte)InstructionType;
            return byteArray;
        }
	}
}
