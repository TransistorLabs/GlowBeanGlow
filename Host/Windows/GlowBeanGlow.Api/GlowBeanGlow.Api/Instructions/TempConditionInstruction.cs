using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlowBeanGlow.Api.Helpers;
using GlowBeanGlow.Api.Interfaces;

namespace GlowBeanGlow.Api.Instructions
{
    public class TempConditionInstruction : IInstruction
    {
        /// <summary>
        /// The index of the animation frame to jump to
        /// </summary>
        public ushort JumpTargetIndex { get; set; }
        public ushort LowTempF { get; set; }
        public ushort HighTempF { get; set; }

        public byte[] GetReportData(byte reportId = 0)
        {
            var byteArray = new byte[9];
            byteArray[0] = reportId;
            byteArray[1] = BitHelpers.GetLowByte(JumpTargetIndex);
            byteArray[2] = BitHelpers.GetHighByte(JumpTargetIndex);

            byteArray[3] = BitHelpers.GetLowByte(LowTempF);
            byteArray[4] = BitHelpers.GetHighByte(LowTempF);

            byteArray[5] = BitHelpers.GetLowByte(HighTempF);
            byteArray[6] = BitHelpers.GetHighByte(HighTempF);

            byteArray[8] = (byte)InstructionType;
            return byteArray;
        }

        public InstructionTypes InstructionType
        {
            get
            {
                return InstructionTypes.TempCondition;
            }
        }
    }
}
