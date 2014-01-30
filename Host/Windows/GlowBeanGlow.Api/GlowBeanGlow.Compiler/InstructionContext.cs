using GlowBeanGlow.Api.Interfaces;

namespace GlowBeanGlow.Compiler
{
    public class InstructionContext
    {
        public IInstruction Instruction { get; set; }
        public string GotoLabel { get; set; }
    }
}
