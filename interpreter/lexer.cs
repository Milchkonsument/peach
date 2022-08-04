using System;
using System.Collections.Generic;

namespace PeachInterpreter
{
    public class Lexer
    {
        private string _lexeme;
        private CodePosition _pos;

        private List<Token<Enum>> _tokens;

        public List<Token<Enum>> Tokens => _tokens;
        
        public Lexer()
        {
            _lexeme = string.Empty;
            _tokens = new List<Token<Enum>>();
        }

        public static Lexer tokenize(List<String> l)
        {
            Lexer lexer = new Lexer();
            foreach(string line in l)
            {
                lexer._pos.line++;
                lexer._pos.column = 0;
                for(int i = 0; i < line.Length; i++)
                {
                    lexer._pos.column++;

                    Action parse = line[i] switch
                    {
                        _ => throw new NotImplementedException(),
                    };
                }
            }
            return lexer;
        }
    }
}