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
        private readonly IList<InstructionContext> _instructionContextData;
        private int _tokenIndex = 0;
        private Stack<string> _loopScopeLabels;

        private Dictionary<string, int> _labels;

        public Parser(IList<Token> tokens)
        {
            _loopScopeLabels = new Stack<string>();
            _labels = new Dictionary<string, int>();
            _tokens = tokens;
            _instructionContextData = new List<InstructionContext>();
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
            return _instructionContextData.Select(p => p.Instruction);
        }

        private void ResolveLabels()
        {
            foreach (var instructionContext in _instructionContextData)
            {
                var jumpTo = instructionContext.Instruction as JumpToInstruction; 
                if (jumpTo != null)
                {
                    jumpTo.JumpTargetIndex = (ushort)_labels[instructionContext.GotoLabel];
                }

                var buttonEvent = instructionContext.Instruction as ButtonEventInstruction;
                if (buttonEvent != null)
                {
                    buttonEvent.JumpTargetIndex = (ushort)_labels[instructionContext.GotoLabel];
                }

                var tempCondition = instructionContext.Instruction as TempConditionInstruction;
                if (tempCondition != null)
                {
                    tempCondition.JumpTargetIndex = (ushort)_labels[instructionContext.GotoLabel];
                }
            }
        }

        private void ProcessLabel()
        {
            
            var token = GetNextToken();
            AssertValid(token, "expected: label" + _tokenIndex, x => x.Type == TokenType.Keyword);
            
            if (_labels.ContainsKey(token.RawValue))
            {
                LogError(token, "duplicate label found");
            }

            // Add label and program index
            _labels.Add(token.RawValue, _instructionContextData.Count);
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
                    if (_loopScopeLabels.Count > 0)
                    {
                        // Pop off the last label in the stack and make a Goto statement for it
                        var label = _loopScopeLabels.Pop();
                        var jumpInstruction = GetNewProgramInstruction(new JumpToInstruction());
                        jumpInstruction.GotoLabel = label;
                        _instructionContextData.Add(jumpInstruction);
                    }
                    else
                    {
                        return false;
                    }
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
                    var tempLabel = Guid.NewGuid().ToString();
                    _labels.Add(tempLabel, _instructionContextData.Count);
                    _loopScopeLabels.Push(tempLabel);
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
                
                case "ifTemp":
                    ProcessTempConditionFunction();
                    break;
                default:
                    LogError(token, "unknown token");
                    break;
            }
        }

        private InstructionContext GetNewProgramInstruction(IInstruction instruction)
        {
            var p = new InstructionContext {Instruction = instruction};
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


            _instructionContextData.Add(GetNewProgramInstruction(setInstruction));
        }

        //ifTemp (
        //    between: 71,
        //    and: 80,
        //    goto: clockwiseShift
        //); 

        private void ProcessTempConditionFunction()
        {
            var tempProgramInstruction = GetNewProgramInstruction(new TempConditionInstruction());

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
                        case "between":
                        case "low":
                            ((TempConditionInstruction)tempProgramInstruction.Instruction).LowTempF = (ushort)ProcessNumber();
                            break;

                        case "and":
                        case "high":
                            ((TempConditionInstruction)tempProgramInstruction.Instruction).HighTempF = (ushort)ProcessNumber();
                            break;

                        case "goto":
                            tempProgramInstruction.GotoLabel = ProcessLabelDefinition(true);
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

            _instructionContextData.Add(tempProgramInstruction);
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

            _instructionContextData.Add(GetNewProgramInstruction(incrementInstruction));
        }

        private void ProcessGotoFunction()
        {
            var jumpInstruction = GetNewProgramInstruction(new JumpToInstruction());
            var nextToken = GetNextToken();
            AssertValid(nextToken, "Expected: (", x => x.Type == TokenType.StartFunction);

            jumpInstruction.GotoLabel = ProcessLabelDefinition(false);

            nextToken = GetNextToken();
            AssertValid(nextToken, "expected: )",
                        x => x.Type == TokenType.EndFunction);

            nextToken = GetNextToken();
            AssertValid(nextToken, "expected: ;",
                        x => x.Type == TokenType.TerminateStatement);

            _instructionContextData.Add(jumpInstruction);
        }

        private void ProcessButtonEventFunction()
        {
            var buttonEventInstruction = new InstructionContext { Instruction = new ButtonEventInstruction() };

            var nextToken = GetNextToken();
            AssertValid(nextToken, "Expected: (", x => x.Type == TokenType.StartFunction);

            buttonEventInstruction.GotoLabel = ProcessLabelDefinition(false);

            nextToken = GetNextToken();
            AssertValid(nextToken, "expected: )",
                        x => x.Type == TokenType.EndFunction);

            nextToken = GetNextToken();
            AssertValid(nextToken, "expected: ;",
                        x => x.Type == TokenType.TerminateStatement);
            
            _instructionContextData.Add(buttonEventInstruction);
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

        private string ProcessLabelDefinition(bool expectParamSeparatorToken)
        {
            if (expectParamSeparatorToken)
            {
                var endParamToken = GetNextToken();
                AssertValid(endParamToken, "Expected: colon (:)",
                            x => x.Type == TokenType.EndParameterName);
            }

            var nextToken = GetNextToken();
            AssertValid(nextToken, "Expected: label definition",
                        x => x.Type == TokenType.Keyword);
            return nextToken.RawValue;
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
