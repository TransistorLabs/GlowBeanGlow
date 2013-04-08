using GlowBeanGlow.Api.Interfaces;

namespace GlowBeanGlow.Api.Features
{
    internal class SetFeatureReport : IReportData
    {
        public SetFeatureReport()
        {
            CommandData = new byte[6];
        }

        public SetFeatureCommands Command { get; set; }
        public byte Status { get; set; }
        public byte[] CommandData { get; set; }

        public byte[] GetReportData(byte reportId = 0)
        {
            var bytes = new byte[9];
            bytes[0] = reportId;
            bytes[1] = (byte) Command;
            bytes[2] = Status;
            CommandData.CopyTo(bytes, 3);
            return bytes;
        }
    }
}
