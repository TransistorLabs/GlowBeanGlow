using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GlowBeanGlow.Compiler
{
    public static class Preprocessor
    {
        private const string TokenSplitter = @"[^-\w]";

        public static string Process(string input)
        {
            input = StripComments(input);

            return input;
        }

        private static string StripComments(string input)
        {
            // Remove Single line "//" style comments
            input = Regex.Replace(input, "//.*$", "", RegexOptions.Multiline);

            // Remove Single line "/* */" style comments
            input = Regex.Replace(input, @"/\*(.)*?\*/", "", RegexOptions.Multiline);

            // Remove Multiple line "/* */" style comments, preserving pre-existing newlines to ensure correct error location reporting
            input = StripMultilineComments(input);

            input = ProcessDefines(input);
            
            return input;
        }

        private static string StripMultilineComments(string input)
        {
            bool commentActive = false;
            var lines = input.Split('\n');
            for (int i = 0; i < lines.Count(); i++)
            {
                // Remove any extraneous \r's as we'll normalize all newlines to Evironment.Newline at the end
                lines[i] = lines[i].Replace("\r", "");

                if (commentActive)
                {
                    if (lines[i].Contains("*/"))
                    {
                        commentActive = false;
                        lines[i] = Regex.Replace(lines[i], @"^(.)*?\*/", "", RegexOptions.Multiline);
                    }
                    else
                    {
                        lines[i] = string.Empty;
                    }
                }
                else
                {
                    if (lines[i].Contains("/*"))
                    {
                        commentActive = true;
                        lines[i] = Regex.Replace(lines[i], @"/\*(.)*?$", "", RegexOptions.Multiline);
                    }
                }
            }
            return string.Join(Environment.NewLine, lines);
        }

        private static string ProcessDefines(string input)
        {
            var defines = new Dictionary<string, string>();
            var lines = input.Split(new string[] {Environment.NewLine}, StringSplitOptions.None);
            for (int i = 0; i < lines.Count(); i++)
            {
                var line = lines[i].Trim();

                // gather and remove #define statements
                if (line.StartsWith("#define"))
                {
                    var tokens = line.Split();
                    if (tokens.Length < 3)
                    {
                        throw new ApplicationException("invalid #define statement on line " + (i + 1));
                    }
                    var key = tokens[1];
                    var replacement = string.Join(" ",tokens.Skip(2).ToArray());
                    defines.Add(key, replacement);
                    lines[i] = ""; // remove the processed #define statement
                }
            }

            input = string.Join(Environment.NewLine, lines);

            // Perform actual definition replacement 
            foreach (var define in defines)
            {
                var pattern = string.Format("({0}){1}({0})", TokenSplitter, define.Key);
                var replacement = string.Format("$1{0}$2", define.Value);
                input = Regex.Replace(input, pattern, replacement, RegexOptions.Multiline);
            }
            
            return input;
        }
    }
}
