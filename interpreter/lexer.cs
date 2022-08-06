using System;
using System.Collections.Generic;
using System.Linq;

namespace PeachInterpreter
{
    public struct Error
    {
        public Error(string message, CodePosition position, string lexeme_at)
        {
            this.message = message;
            this.position = position;
            this.lexeme_at = lexeme_at;
        }

        string message;
        CodePosition position;
        string lexeme_at;

        public override string ToString() => $"Error near '{lexeme_at}' : {message} (line {position.line}, column {position.column})";
    }

    public class Lexer
    {
        private string lexeme => currentLine.Substring(_lexemeStart, _pos.column - _lexemeStart + 1);

        private CodePosition _pos;

        private int _lexemeStart = 0;

        private List<String> _buffer;

        private List<Token<object>> _tokens;

        public List<Token<object>> Tokens => _tokens;

        public List<Error> error = new List<Error>();

        public bool had_error => error.Count == 0;

        public Lexer()
        {
            _tokens = new List<Token<object>>();
        }

        public static Lexer tokenize(List<String> l)
        {
            Lexer lexer = new Lexer();
            lexer._buffer = l;

            foreach (string line in l.Select((s) => s.Trim()))
            {
                if (line.Length == 0)
                    continue;

                lexer.tokenize_line();
                lexer._pos.line++;
            }
            return lexer;
        }

        private void tokenize_line()
        {
            _pos.column = 0;


            while (!at_end())
            {
                while (char.IsWhiteSpace(current())) next();

                _lexemeStart = _pos.column;

                // Action process = current() switch
                // {
                //     '\"' => processString,
                //     '\'' => processChar,
                //     char.IsDigit => processNum,
                //     '_' or char.IsLetter => process_iden,
                //     _ => process_other,
                // };

                // process.Invoke();

                char c = current();
                if (c == '\"')
                    process_string();
                else if (c == '\'')
                    process_char();
                else if (char.IsDigit(c))
                    process_num();
                else if (c == '_' || char.IsLetter(c))
                    process_iden();
                else
                    process_other();
            }
        }

        private void process_other()
        {

            while (!at_end()) next();
        }

        private void process_string()
        {
            while (!at_end())
            {
                char c = next();

                if (peek(-1) != '\\' && c == '"')
                {
                    maybe_next();
                    _tokens.Add(new Token<object>(TokenType.lit, LiteralType.@string, lexeme, new CodePosition(_pos.line, _lexemeStart)));
                    return;
                }
            }

            throw_error("Encountered non-terminated string literal.");
        }

        private void throw_error(String msg) => error.Add(new Error(msg, new CodePosition(_pos.line, _lexemeStart), Tokens.Count == 0 ? "" : Tokens.Last().lexeme));

        private void process_char()
        {
            while (!at_end()) next();
        }

        private void process_num()
        {
            while (!at_end()) next();
        }

        private void process_iden()
        {
            while (!at_end()) next();
        }

        string currentLine => _buffer[_pos.line];

        char peek() => currentLine[_pos.column + 1];

        char peek(int n) => currentLine[_pos.column + n];

        char current() => peek(0);

        char next() => currentLine[++_pos.column];

        void maybe_next() => _pos.column += at_end() ? 0 : 1;

        bool at_end() => _pos.column >= currentLine.Length - 1;

        bool peek_match(char c) => peek() == c ? _pos.column++ == _pos.column : false;

        private bool is_keyword(String lexeme) => Keywords.Map.ContainsKey(lexeme);

        private bool is_operator(String lexeme) => Operators.Map.ContainsKey(lexeme);

        private bool is_seperator(String lexeme) => Seperators.Map.ContainsKey(lexeme);
    }
}