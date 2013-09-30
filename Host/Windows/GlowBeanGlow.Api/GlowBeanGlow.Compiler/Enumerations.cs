using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlowBeanGlow.Compiler
{
    public enum TokenType
    {
        Keyword,
        ScopeUp,
        ScopeDown,
        StartLabel,
        StartColor,
        StartFunction,
        StartArray,
        EndArray,
        EndParameterName,
        EndParameterValue,
        TerminateStatement,
        EndFunction
    }

    public enum MainKeywords
    {
        Loop,
        Set,
        Increment,
        OnPressEvent,
        GoTo,
        If,
        GetTempF,
        GetTempC
    }

    public enum ParameterKeywords
    {
        Color,
        Duration,
        LedsOn,
        AddRed,
        AddBlue,
        AddGreen,
        IncrementColorDelay,
        IncrementColorCount,
        RotationDirection,
        RotationDelay,
        RotationCount,
        Button,
        Label
    }
}
