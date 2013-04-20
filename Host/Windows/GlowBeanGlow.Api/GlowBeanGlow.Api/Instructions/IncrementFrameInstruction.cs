using GlowBeanGlow.Api.Interfaces;

namespace GlowBeanGlow.Api.Instructions
{
    public class IncrementFrameInstruction : IInstruction
    {
        public sbyte RedIncrement { get; set; }
        public sbyte GreenIncrement { get; set; }
        public sbyte BlueIncrement { get; set; }
        public byte ColorIncrementDelayMs { get; set; }
        public byte ColorIncrementCount { get; set; }

        public LedShiftOptions LedShiftType { get; set; }
        public byte LedShiftDelayMs { get; set; }
        public byte LedShiftCount { get; set; }

        public InstructionTypes InstructionType
        {
            get { return InstructionTypes.IncrementFrame; }
        }

        public byte[] GetReportData(byte reportId = 0)
        {
            var byteArray = new byte[9];
            byteArray[0] = reportId;
            byteArray[1] = (byte) RedIncrement;
            byteArray[2] = (byte) GreenIncrement;
            byteArray[3] = (byte) BlueIncrement;
            byteArray[4] = (byte) ColorIncrementDelayMs;
            byteArray[5] = (byte) ColorIncrementCount;
            byteArray[6] = (byte) LedShiftDelayMs;
            byteArray[7] = (byte) LedShiftCount;

            var shiftType = (byte)LedShiftType;
            shiftType <<= 4;
            byteArray[8] = (byte) InstructionType;
            byteArray[8] |= shiftType;
            return byteArray;
        }
    }
}
