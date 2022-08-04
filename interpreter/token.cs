using System;

namespace PeachInterpreter
{
    public enum TokenType
    {
        // keyword
        key,
        // operator
        oper,
        // identifier
        iden,
        // literal
        lit,
        // seperator
        sep,
    }

    public class Token<TSub> where TSub : Enum
    {
        TokenType type;
        TSub subtype;
        string lexeme;
        CodePosition position;
    }

    public struct CodePosition
    {
        public int line;
        public int column;
    }
}