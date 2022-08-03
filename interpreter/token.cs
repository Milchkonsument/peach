using System;

namespace PeachInterpreter
{
    enum TokenType
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

    class Token<T, TSub> where T : TokenType where TSub : Enum
    {
        T type;
        TSub subtype;
        string lexeme;
        CodePosition position;
    }

    struct CodePosition
    {
        int line;
        int column;
    }
}