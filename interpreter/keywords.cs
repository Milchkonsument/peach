using System;
using System.Collections.Generic;

namespace PeachInterpreter
{
    static class Keywords
    {
        public static readonly Dictionary<string, KeywordType> Map = new Dictionary<string, KeywordType>() {
            {"if", KeywordType.@if },
            {"else", KeywordType.@else },
            {"for", KeywordType.@for },
            {"fn", KeywordType.@fn },
            {"return", KeywordType.@return },
            {"int", KeywordType.@int },
            {"float", KeywordType.@float },
            {"double", KeywordType.@double },
            {"string", KeywordType.@string },
            {"struct", KeywordType.@struct },
        };
    }

    enum KeywordType
    {
        @if,
        @else,
        @for,
        @fn,
        @return,
        @int,
        @float,
        @double,
        @string,
        @struct,
    }
}