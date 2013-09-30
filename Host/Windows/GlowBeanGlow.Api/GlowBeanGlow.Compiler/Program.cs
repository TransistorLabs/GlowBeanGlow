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
                Console.WriteLine("\nPlease supply a file path (.gbg) to compile as an argument.");
                return;
            }

            Console.WriteLine("\nCompiling " + args[0] + " ...");

            var success = true;
            try
            {
                var fileContents = File.ReadAllText(args[0]);
                var scanner = new Scanner(fileContents);
                var tokens = scanner.GetTokens();

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
