using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GlowBeanGlow.Compiler
{
    public class Scanner
    {
        private const string IdentifierTokenMatch = @"\w";
        private readonly List<Token> _tokens = new List<Token>();

        private string _input = "";
        public Scanner(string input)
        {
            _input = input;
        }

        public List<Token> GetTokens()
        {
            _input = StripComments(_input);
            var index = 0;
            var currentLine = 1;
            var currentCharacter = 1;
            var currentWord = "";
            while (index < _input.Length)
            {
                var character = _input.Substring(index, 1);
                if (Regex.Match(character, IdentifierTokenMatch).Captures.Count > 0)
                {
                    currentWord += character;
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(currentWord))
                    {
                        var wordToken = new Token
                            {
                                RawValue = currentWord,
                                Type = TokenType.Keyword,
                                LineNumber = currentLine,
                                CharacterNumber = currentCharacter - currentWord.Length
                            };
                        _tokens.Add(wordToken);
                        currentWord = "";
                    }

                    if (!String.IsNullOrWhiteSpace(character))
                    {
                        var charToken = new Token
                            {
                                RawValue = character,
                                LineNumber = currentLine,
                                CharacterNumber = currentCharacter
                            };

                        switch (character)
                        {
                            case "{":
                                charToken.Type = TokenType.ScopeUp;
                                break;

                            case "}":
                                charToken.Type = TokenType.ScopeDown;
                                break;

                            case "@":
                                charToken.Type = TokenType.StartLabel;
                                break;

                            case "(":
                                charToken.Type = TokenType.StartFunction;
                                break;
                                
                            case ":":
                                charToken.Type = TokenType.EndParameterName;
                                break;

                            case ")":
                                charToken.Type = TokenType.EndFunction;
                                break;

                            case ",":
                                charToken.Type = TokenType.EndParameterValue;
                                break;

                            case "[":
                                charToken.Type = TokenType.StartArray;
                                break;

                            case "]":
                                charToken.Type = TokenType.EndArray;
                                break;

                            case "#":
                                charToken.Type = TokenType.StartColor;
                                break;

                            case ";":
                                charToken.Type = TokenType.TerminateStatement;
                                break;

                            default:
                                throw new ApplicationException("Invalid character: " + character);
                        }

                        _tokens.Add(charToken);
                    }
                    else if (character == "\n")
                    {
                        ++currentLine;
                        currentCharacter = 0;
                    }
                }
                ++currentCharacter;
                ++index;
            }

            return _tokens;
        }

        private static string StripComments(string input)
        {
            input = Regex.Replace(input, "//.*$", "", RegexOptions.Multiline);
            input = Regex.Replace(input, @"/\*(.|[\r\n])*?\*/", "", RegexOptions.Multiline);
            return input;
        }

        private static string StripWhitespace(string input)
        {
            input = Regex.Replace(input, @"\s*", "");
            return input;
        }
    }
}
