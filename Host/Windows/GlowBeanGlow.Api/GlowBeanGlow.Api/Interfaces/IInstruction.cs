using GlowBeanGlow.Api.Instructions;

namespace GlowBeanGlow.Api.Interfaces
{
    public interface IInstruction : IReportData
    {
        InstructionTypes InstructionType { get; }
    }
}
