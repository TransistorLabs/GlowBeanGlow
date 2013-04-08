using GlowBeanGlow.Api.Instructions;

namespace GlowBeanGlow.Api.Interfaces
{
    public interface IInstruction
    {
        InstructionTypes InstructionType { get; }
    }
}
