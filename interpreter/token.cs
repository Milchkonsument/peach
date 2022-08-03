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

    class Token<TSub> where TSub : Enum
    {
        TokenType type;
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