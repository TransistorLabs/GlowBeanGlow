using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlowBeanGlow.Api.Features;
using GlowBeanGlow.Api.Interfaces;

namespace GlowBeanGlow.Compiler
{
    public static class HidCodeGenerator
    {
        public static string Generate(IEnumerable<IInstruction> instructions)
        {
            /* 
             sf 00  01 00 01 00 00 00 00 00 
             WR 00  FF FF FF 01 00 10 00 00 
             sf 00  01 00 02 00 00 00 00 00 

             */

            var startCommand = new SetFeatureReport { Command = SetFeatureCommands.ChangeFeatureMode };
            startCommand.CommandData[0] = (byte)FeatureModeOptions.StoreProgramStart;

            var stopCommand = new SetFeatureReport { Command = SetFeatureCommands.ChangeFeatureMode };
            stopCommand.CommandData[0] = (byte)FeatureModeOptions.StoreProgramStop;
                
            var sb = new StringBuilder();
            sb.AppendLine(GenerateFromSetFeatureReport(startCommand));
            foreach (var instruction in instructions)
            {
                sb.AppendLine(GenerateFromInstruction(instruction));
            }
            sb.AppendLine(GenerateFromSetFeatureReport(stopCommand));
            return sb.ToString();
        }

        private static string GenerateFromInstruction(IInstruction instruction)
        {
            var bytes = instruction.GetReportData();
            var sb = new StringBuilder();
            sb.Append("WR ");
            AppendBytesAsString(bytes, sb);
            return sb.ToString();
        }

        private static string GenerateFromSetFeatureReport(SetFeatureReport report)
        {
            var bytes = report.GetReportData();
            var sb = new StringBuilder();
            sb.Append("sf ");
            AppendBytesAsString(bytes, sb);
            return sb.ToString();
        }

        private static void AppendBytesAsString(byte[] bytes, StringBuilder sb)
        {
            sb.Append(bytes[0].ToString("X2") + " ");
            for (var i = 1; i < bytes.Length; i++)
            {
                sb.Append(" " + bytes[i].ToString("X2"));
            }
        }
    }
}
