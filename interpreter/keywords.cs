namespace PeachInterpreter
{
    using System;
    using System.Collections.Generic;

    static class Keywords
    {
        public readonly Dictionary<String, KeywordType> List = {
            {"if", KeywordType.@if },
            {"else", KeywordType.@else },
            {"for", KeywordType.@for },
            {"fn", KeywordType.@fn },
            {"return", KeywordType.@return },
            {"int", KeywordType.@int },
            {"float", KeywordType.@float },
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
        @string,
        @struct,
    }
}