using System.Collections.Generic;
using System;

namespace PeachInterpreter
{
    static class Seperators
    {
        public static readonly Dictionary<string, SeperatorType> Map = new Dictionary<string, SeperatorType>() {
            {";", SeperatorType.semi },
            {",", SeperatorType.comma },
            {"{", SeperatorType.lbrace },
            {"}", SeperatorType.rbrace },
            {"(", SeperatorType.lparen },
            {")", SeperatorType.rparen },
            {"[", SeperatorType.lbrack },
            {"]", SeperatorType.rbrack },
        };
    }

    enum SeperatorType
    {
        semi,
        comma,
        lbrace,
        rbrace,
        lparen,
        rparen,
        lbrack,
        rbrack,
    }
}