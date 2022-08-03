using System;
using System.Collections;

namespace PeachInterpreter
{
    static class Lexer
    {
        private static string _lex_buf;

        public static List<Token<TokenType, Enum>> tokenize(List<String> l)
        {
            init();

        }

        private static void init()
        {
            _lex_buf = string.Empty;
        }
    }
}