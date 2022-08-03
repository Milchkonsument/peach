using System.Collections.Generic;

namespace PeachInterpreter
{
    static class Seperators
    {
        public readonly Dictionary<String, SeperatorType> Map = new Dictionary<String, SeperatorType>() {
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