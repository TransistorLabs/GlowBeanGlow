using GlowBeanGlow.Api.Interfaces;

namespace GlowBeanGlow.Api.Instructions
{
    public class IncrementFrameInstruction : IReportData, IInstruction
    {
        public sbyte RedIncrement { get; set; }
        public sbyte GreenIncrement { get; set; }
        public sbyte BlueIncrement { get; set; }
        public byte ColorIncrementDelayMs { get; set; }
        public byte ColorIncrementCount { get; set; }

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
            byteArray[1] = (byte)RedIncrement;
            byteArray[2] = (byte)GreenIncrement;
            byteArray[3] = (byte)BlueIncrement;
            return byteArray;
        }
    }
}
