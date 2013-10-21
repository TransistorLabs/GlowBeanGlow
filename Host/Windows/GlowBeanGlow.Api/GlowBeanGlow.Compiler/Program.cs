using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GlowBeanGlow.Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("\nPlease supply a path to a file (.gbg) to compile.");
                return;
            }

            Console.WriteLine("\nCompiling " + args[0] + " ...");

            var success = true;
            try
            {
                var fileContents = File.ReadAllText(args[0]);
                fileContents = Preprocessor.Process(fileContents);

                File.WriteAllText(args[0] + ".preprocessed", fileContents);
                var lexer = new Lexer(fileContents);
                var tokens = lexer.GetTokens();

                // output tokens file
                var tokenOutput = string.Join("\n", tokens.Select(
                    x =>
                        x.RawValue
                        + "\t\t\t"
                        + Enum.GetName(typeof(TokenType), x.Type)
                        + "\t\t\t"
                        + x.LineNumber
                        + ","
                        + x.CharacterNumber));
                File.WriteAllText(args[0] + ".tokens", tokenOutput);

                var parser = new Parser(tokens);
                var instructions = parser.Parse();
                var hidCode = HidCodeGenerator.Generate(instructions);

                // output hidcode (SimpleHidWrite syntax)
                File.WriteAllText(args[0] + ".hidcode", hidCode);

                Console.WriteLine("\nDone.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                success = false;
            }

        }
    }
}
