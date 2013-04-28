using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlowBeanGlow.Api.Helpers;
using GlowBeanGlow.Api.Interfaces;

namespace GlowBeanGlow.Api.Instructions
{
    public class JumpToInstruction : IInstruction
    {
        /// <summary>
        /// The index of the animation frame to jump to
        /// </summary>
        public ushort JumpTargetIndex { get; set; }

        public byte[] GetReportData(byte reportId = 0)
        {
            var byteArray = new byte[9];
            byteArray[0] = reportId;
            byteArray[1] = BitHelpers.GetLowByte(JumpTargetIndex);
            byteArray[2] = BitHelpers.GetHighByte(JumpTargetIndex);

            byteArray[8] = (byte)InstructionType;
            return byteArray;
        }

        public InstructionTypes InstructionType
        {
            get
            {
                return InstructionTypes.JumpTo;
            }
        }
    }
}
