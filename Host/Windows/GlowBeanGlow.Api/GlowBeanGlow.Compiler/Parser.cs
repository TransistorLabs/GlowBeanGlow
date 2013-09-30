using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlowBeanGlow.Api.Display;
using GlowBeanGlow.Api.Instructions;
using GlowBeanGlow.Api.Interfaces;

namespace GlowBeanGlow.Compiler
{
    public class Parser
    {
        private readonly IList<Token> _tokens;
        private readonly IList<ProgramInstruction> _instructions;
        private int _tokenIndex = 0;
        private int _currentInstructionIndex = 9;
        private int _currentLoopInstructionIndex = 0;
        private string _currentLabel = "";

        public Parser(IList<Token> tokens)
        {
            _tokens = tokens;
            _instructions = new List<ProgramInstruction>();
        }

        public IEnumerable<IInstruction> Parse()
        {
            if (_tokens.Count == 0)
            {
                throw new ApplicationException("No tokens found.");
            }

            if (_tokens[0].RawValue != "loop" && _tokens[1].Type != TokenType.ScopeUp)
            {
                throw new ApplicationException("Main loop not found.\n\tAll programs must begin with: loop {");
            }

            _tokenIndex += 1;

            while (_tokenIndex < _tokens.Count - 1)
            {
                ProcessNextToken();
            }

            ResolveLabels();
            return _instructions.Select(p => p.Instruction);
        }

        private void ResolveLabels()
        {
            foreach (var instruction in _instructions)
            {
                var jumpTo = instruction.Instruction as JumpToInstruction; 
                if (jumpTo != null)
                {
                    jumpTo.JumpTargetIndex =
                        (ushort)_instructions.IndexOf(_instructions.FirstOrDefault(i => i.Label == instruction.Label));
                }

                var buttonEvent = instruction.Instruction as ButtonEventInstruction;
                if (buttonEvent != null)
                {
                    buttonEvent.JumpTargetIndex =
                        (ushort)_instructions.IndexOf(_instructions.FirstOrDefault(i => i.Label == instruction.Label));
                }
            }
        }

        private void ProcessLabel()
        {
            
            var token = GetNextToken();
            AssertValid(token, "expected: label" + _tokenIndex, x => x.Type == TokenType.Keyword);
            
            if (!string.IsNullOrWhiteSpace(_currentLabel))
            {
                LogError(token, "a label is already defined for this location");
            }

            _currentLabel = token.RawValue;
        }
        
        private bool ProcessNextToken()
        {
            var token = GetNextToken();
            AssertValid(token, "Token not found; parsing failed at token index: " + _tokenIndex, x => true);
            switch (token.Type)
            {
                case TokenType.Keyword:
                    CheckKeyword();
                    break;

                case TokenType.StartLabel:
                    ProcessLabel();
                    break;

                case TokenType.ScopeDown:
                    return false;
                    break;
            }
            return true;
        }

        private void CheckKeyword()
        {
            var token = GetCurrentToken();
            switch (token.RawValue)
            {
                case "loop":
                    var scopeUp = GetNextToken();
                    AssertValid(scopeUp, "Expected: {", x => x.Type == TokenType.ScopeUp);
                    //save index for scope exit
                    _currentLoopInstructionIndex = _currentInstructionIndex;
                    break;

                case "set":
                    ProcessSetFunction();
                    break;

                case "increment":
                    ProcessIncrementFunction();
                    break;

                case "goto":
                    ProcessGotoFunction();
                    break;

                case "onUserButtonPress":
                    ProcessButtonEventFunction();
                    break;

                default:
                    LogError(token, "unknown token");
                    break;
            }
        }

        private ProgramInstruction GetNewProgramInstruction(IInstruction instruction)
        {
            var p = new ProgramInstruction {Instruction = instruction};
            if (!string.IsNullOrWhiteSpace(_currentLabel))
            {
                p.Label = _currentLabel;
                _currentLabel = "";
            }
            return p;
        }

        private void ProcessSetFunction()
        {
            var setInstruction = new SetFrameInstruction();

            var nextToken = GetNextToken();
            AssertValid(nextToken, "Expected: (", x => x.Type == TokenType.StartFunction);
            while (nextToken.Type != TokenType.TerminateStatement)
            {
                nextToken = GetNextToken();
                AssertValid(nextToken, "Expected: parameter keyword or ) or ;",
                            x =>
                            x.Type == TokenType.Keyword || x.Type == TokenType.EndFunction || x.Type == TokenType.TerminateStatement);
                if(nextToken.Type == TokenType.Keyword)
                {
                    switch (nextToken.RawValue)
                    {
                        case "color":
                            setInstruction.Color = ProcessColor();
                            break;

                        case "duration":
                            setInstruction.MillisecondsHold = (ushort)ProcessNumber();
                            break;

                        case "ledsOn":
                            setInstruction.Leds = ProcessLedStateArray();
                            break;
                        default:
                            LogError(nextToken, "unknown parameter: " + nextToken.RawValue);
                            break;
                    }

                    nextToken = GetNextToken();
                    AssertValid(nextToken, "expected: , or )",
                                x => x.Type == TokenType.EndParameterValue || x.Type == TokenType.EndFunction);
                }
            }


            _instructions.Add(GetNewProgramInstruction(setInstruction));
        }

        private void ProcessIncrementFunction()
        {
            var incrementInstruction = new IncrementFrameInstruction();

            var nextToken = GetNextToken();
            AssertValid(nextToken, "Expected: (", x => x.Type == TokenType.StartFunction);
            while (nextToken.Type != TokenType.TerminateStatement)
            {
                nextToken = GetNextToken();
                AssertValid(nextToken, "Expected: parameter keyword or ) or ;",
                            x =>
                            x.Type == TokenType.Keyword || x.Type == TokenType.EndFunction || x.Type == TokenType.TerminateStatement);
                if (nextToken.Type == TokenType.Keyword)
                {
                    switch (nextToken.RawValue)
                    {
                        case "addRed":
                            incrementInstruction.RedIncrement = (sbyte)ProcessNumber();
                            break;

                        case "addBlue":
                            incrementInstruction.BlueIncrement = (sbyte)ProcessNumber();
                            break;

                        case "addGreen":
                            incrementInstruction.GreenIncrement = (sbyte)ProcessNumber();
                            break;

                        case "incrementColorDelay":
                            incrementInstruction.ColorIncrementDelayMs = (byte)ProcessNumber();
                            break;

                        case "incrementColorCount":
                            incrementInstruction.ColorIncrementCount = (byte)ProcessNumber();
                            break;

                        case "rotationDirection":
                            incrementInstruction.LedShiftType = ProcessShiftType();
                            break;

                        case "rotationDelay":
                            incrementInstruction.LedShiftDelayMs = (byte) ProcessNumber();
                            break;

                        case "rotationCount":
                            incrementInstruction.LedShiftCount = (byte) ProcessNumber();
                            break;
                        
                        default:
                            LogError(nextToken, "unknown parameter: " + nextToken.RawValue);
                            break;
                    }

                    nextToken = GetNextToken();
                    AssertValid(nextToken, "expected: , or )",
                                x => x.Type == TokenType.EndParameterValue || x.Type == TokenType.EndFunction);
                }
            }

            _instructions.Add(GetNewProgramInstruction(incrementInstruction));
        }

        private void ProcessGotoFunction()
        {
            var jumpInstruction = GetNewProgramInstruction(new JumpToInstruction());
            var nextToken = GetNextToken();
            AssertValid(nextToken, "Expected: (", x => x.Type == TokenType.StartFunction);
            
            nextToken = GetNextToken();
            AssertValid(nextToken, "Expected: label definition",
                        x => x.Type == TokenType.Keyword);
            if (nextToken.Type == TokenType.Keyword)
            {
                jumpInstruction.Label = nextToken.RawValue;
            }

            nextToken = GetNextToken();
            AssertValid(nextToken, "expected: )",
                        x => x.Type == TokenType.EndFunction);

            nextToken = GetNextToken();
            AssertValid(nextToken, "expected: ;",
                        x => x.Type == TokenType.TerminateStatement);

            _instructions.Add(jumpInstruction);
        }

        private void ProcessButtonEventFunction()
        {
            var buttonEventInstruction = new ProgramInstruction { Instruction = new ButtonEventInstruction() };

            var nextToken = GetNextToken();
            AssertValid(nextToken, "Expected: (", x => x.Type == TokenType.StartFunction);
            
            nextToken = GetNextToken();
            AssertValid(nextToken, "Expected: label definition",
                        x => x.Type == TokenType.Keyword);
            if (nextToken.Type == TokenType.Keyword)
            {
                buttonEventInstruction.Label = nextToken.RawValue;
            }

            nextToken = GetNextToken();
            AssertValid(nextToken, "expected: )",
                        x => x.Type == TokenType.EndFunction);

            nextToken = GetNextToken();
            AssertValid(nextToken, "expected: ;",
                        x => x.Type == TokenType.TerminateStatement);
            
            _instructions.Add(buttonEventInstruction);
        }

        private void AssertParameterSeparator()
        {
            var parmSeparator = GetNextToken();
            AssertValid(parmSeparator, "expected: :", x => x.Type == TokenType.EndParameterName);
        }

        private RgbColor ProcessColor()
        {
            AssertParameterSeparator();

            var startColor = GetNextToken();
            AssertValid(startColor, "expected: #", x => x.Type == TokenType.StartColor);

            var colorValue = GetNextToken();
            AssertValid(colorValue, "color value keyword expected", x => x.Type == TokenType.Keyword);

            var color = ColorTranslator.FromHtml("#" + colorValue.RawValue);
            return new RgbColor {Red = color.R, Green = color.G, Blue = color.B};

        }

        private int ProcessNumber()
        {
            AssertParameterSeparator();

            int number = 0;
            var numberToken = GetNextToken();
            AssertValid(numberToken, "expected: a numeric value",
                        x => x.Type == TokenType.Keyword && int.TryParse(x.RawValue, out number));

            return number;
        }

        private LedShiftOptions ProcessShiftType()
        {
            AssertParameterSeparator();

            int number = 0;
            var keyword = GetNextToken();
            AssertValid(keyword, "expected: CLOCKWISE or COUNTERCLOCKWISE",
                        x =>
                        x.Type == TokenType.Keyword && (x.RawValue == "CLOCKWISE" || x.RawValue == "COUNTERCLOCKWISE"));

            return keyword.RawValue == "CLOCKWISE" 
                ? LedShiftOptions.ShiftLedRight 
                : LedShiftOptions.ShiftLedLeft;
        }

        private LedState ProcessLedStateArray()
        {
            var ledState = new LedState();
            AssertParameterSeparator();

            var arryStartToken = GetNextToken();
            AssertValid(arryStartToken, "expected: [\n\tNote: this can also be a array constant",
                        x => x.Type == TokenType.StartArray);

            Token nextToken;
            do
            {
                nextToken = GetNextToken();
                AssertValid(nextToken, "expected: numeric value, comma (,) or ]",
                            (x => x.Type == TokenType.Keyword || x.Type == TokenType.EndArray || x.Type == TokenType.EndParameterValue));

                if (nextToken.Type == TokenType.Keyword)
                {
                    int value = 0;
                    if (int.TryParse(nextToken.RawValue, out value))
                    {
                        ledState[value] = true;
                    }
                }
            } while (nextToken.Type != TokenType.EndArray);

            return ledState;
        }
        
        private Token GetCurrentToken()
        {
            return GetTokenByIndex(_tokenIndex);
        }

        private Token GetNextToken()
        {
            ++_tokenIndex;
            return GetCurrentToken();
        }

        private Token GetPreviousToken()
        {
            return GetTokenByIndex(_tokenIndex - 1);
        }

        private Token GetTokenByIndex(int index)
        {
            if (index < _tokens.Count)
            {
                return _tokens[index];
            }

            return null;
        }

        private void AssertValid(Token token, string errorMessage, Func<Token, bool> isValid)
        {
            if (token == null)
            {
                throw new ApplicationException(errorMessage);
            }

            if (!isValid(token))
            {
                LogError(token, errorMessage);
            }
        }

        private void LogError(Token token, string errorMessage)
        {
            throw new ApplicationException(string.Format("{0}\n\tToken: {1}\n\tLine Number: {2}\n\tCharacter: {3}", errorMessage, token.RawValue, token.LineNumber, token.CharacterNumber));
        }
    }
}
