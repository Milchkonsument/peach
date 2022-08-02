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
        sep
    }

    class Token<T, TVal> where T : TokenType
    {
        T type;
        TVal value;
        string lexeme;
        CodePosition position;
    }

    struct CodePosition
    {
        int line;
        int column;
    }
}