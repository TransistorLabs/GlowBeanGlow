using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlowBeanGlow.Api.Interfaces;

namespace GlowBeanGlow.Compiler
{
    public class ProgramInstruction
    {
        public IInstruction Instruction { get; set; }
        public string Label { get; set; }
    }
}
