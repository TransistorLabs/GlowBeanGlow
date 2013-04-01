using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlowBeanGlow.Api.DataTypes.Enumerations;

namespace GlowBeanGlow.Api.DataTypes
{
    public interface IInstruction
    {
        InstructionTypes InstructionType { get; }
	    byte[] GetByteArray(byte reportId);
    }
}
