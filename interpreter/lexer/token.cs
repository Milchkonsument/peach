using System;
using System.Text;

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

    public class Token<TSub>
    {
        public Token(TokenType type, TSub subtype, string lexeme, CodePosition start)
        {
            this.type = type;
            this.subtype = subtype;
            this.lexeme = lexeme;
            this.position = start;
        }

        public TokenType type;
        public TSub subtype;
        public string lexeme;
        public CodePosition position;

        public override string ToString() => $"{position}\t{type}\t|\t{subtype}\t|\t{lexeme}";
    }

    public struct CodePosition
    {
        public CodePosition(int line, int column)
        {
            this.line = line;
            this.column = column;
        }

        public int line;
        public int column;

        public override string ToString() => $"[{line + 1:D3}, {column + 1:D3}]";
    }
}