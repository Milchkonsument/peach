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

        public override string ToString() => $"Error at ({position.line + 1}, {position.column + 1}) near '{lexeme_at}' : {message}";
    }

    public class Lexer
    {
        private string lexeme => currentLine.Substring(_lexemeStart, _pos.column - _lexemeStart);

        private CodePosition _pos;

        private int _lexemeStart = 0;

        private List<String> _buffer;

        private List<Token<object>> _tokens;

        public List<Token<object>> Tokens => _tokens;

        public List<Error> error = new List<Error>();

        public bool had_error => error.Count != 0;

        public Lexer()
        {
            _tokens = new List<Token<object>>();
        }

        public static Lexer tokenize(List<String> l)
        {
            Lexer lexer = new Lexer();
            lexer._buffer = l;

            foreach (string line in l)
            {
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

                char c = current();
                if (c == '\"')
                {
                    process_string();
                }

                else if (c == '\'')
                {
                    process_char();
                }

                else if (char.IsDigit(c) || c == '.')
                {
                    process_num();
                }

                else if (c == '_' || char.IsLetter(c))
                {
                    process_iden_or_keyword();
                }

                else
                {
                    process_other();
                }
            }
        }

        private void process_other()
        {
            char c = next();
            // process operators
            switch (c)
            {
                case ('*'):
                    add_token(TokenType.oper, OperatorType.star);
                    return;
                case ('/'):
                    add_token(TokenType.oper, OperatorType.slash);
                    return;
                case ('+'):
                    add_token(TokenType.oper, current_maybe_next('+') ? OperatorType.plpl : OperatorType.plus);
                    return;
                case ('-'):
                    add_token(TokenType.oper, current_maybe_next('-') ? OperatorType.mnmn : OperatorType.minus);
                    return;
                case ('%'):
                    add_token(TokenType.oper, OperatorType.slash);
                    return;
                case ('='):
                    add_token(TokenType.oper, current_maybe_next('=') ? OperatorType.eqeq : OperatorType.eq);
                    return;
                case ('!'):
                    add_token(TokenType.oper, current_maybe_next('=') ? OperatorType.neq : OperatorType.not);
                    return;
            }

            // process seperators
            if (is_seperator(c))
            {
                add_token(TokenType.sep, Seperators.Map[c.ToString()]);
                return;
            }

            throw_error($"Unhandled token '{lexeme}'.");
        }

        private void process_string()
        {
            next(); // consume the first double quotes

            while (!at_end())
            {
                char c = next();

                if (c == '"' && peek(-2) != '\\')
                {
                    // maybe_next();
                    add_token(TokenType.lit, LiteralType.@string);
                    return;
                }
            }

            throw_error("Encountered non-terminated string literal.");
        }

        private void process_char()
        {
            next();
            char c = next();

            // char has escaped symbol
            if ((c == '\\' && peek(2) == '\''))
            {
                next();
                next();
                add_token(TokenType.lit, LiteralType.@char);
                return;
            }

            // char has normal symbol
            if (peek(1) == '\'')
            {
                next();
                add_token(TokenType.lit, LiteralType.@char);
                return;
            }

            throw_error("Overfull or unterminated char literal.");
        }

        private void process_num()
        {
            bool hasFloatingPoint = false;

            while (!current_terminating())
            {
                char c = next();

                if (!char.IsNumber(c) && c != '.')
                {
                    throw_error("Unexpected character in number literal.");
                    return;
                }

                if (c == '.')
                {
                    if (hasFloatingPoint)
                    {
                        throw_error("Number already has a floating point.");
                        return;
                    }

                    hasFloatingPoint = true;
                }
            }

            if (current() == '.')
            {
                throw_error("Number does not have a decimal place after floating point.");
                return;
            }

            add_token(TokenType.lit, hasFloatingPoint ? LiteralType.@double : LiteralType.@int);
        }

        private void process_iden_or_keyword()
        {
            while (!current_terminating() && (char.IsLetterOrDigit(current()) || current() == '_'))
            {
                next();
            }

            if (is_keyword(lexeme))
            {
                add_token(TokenType.key, Keywords.Map[lexeme]);
            }
            else
            {
                add_token<object>(TokenType.iden, null);
            }
        }

        string currentLine => _buffer[_pos.line];

        // returns whether the current char is a char that would terminate the current lexeme
        bool current_terminating() => at_end() || char.IsWhiteSpace(current()) || is_seperator(current()) || is_operator(current());

        char peek() => peek(1);

        char peek(int n) => _pos.column + n > currentLine.Length - 1 ? '\0' : currentLine[_pos.column + n];

        // returns the current char under pointer
        char current() => peek(0);

        // consumes and returns current()
        char next()
        {
            // Console.Write($"{currentLine[_pos.column]}");
            return currentLine[_pos.column++];
        }

        void maybe_next() => _pos.column += at_end() ? 0 : 1;

        bool at_end() => current() == '\0';

        // peeks the current character and consumes if it matches
        bool current_maybe_next(char c) => current() == c ? ++_pos.column == _pos.column : false;

        private bool is_keyword(string lexeme) => Keywords.Map.ContainsKey(lexeme);

        private bool is_operator(string lexeme) => Operators.Map.ContainsKey(lexeme);
        private bool is_operator(char c) => is_operator(c.ToString());

        private bool is_seperator(string lexeme) => Seperators.Map.ContainsKey(lexeme);
        private bool is_seperator(char c) => is_seperator(c.ToString());

        private void add_token<TSub>(TokenType type, TSub subtype)
        {
            _tokens.Add(new Token<object>(type, subtype, lexeme, new CodePosition(_pos.line, _lexemeStart)));
        }

        private void throw_error(String msg) => error.Add(new Error(msg, new CodePosition(_pos.line, _lexemeStart), Tokens.Count == 0 ? "" : Tokens.Last().lexeme));

        private void log(String msg) => Console.WriteLine(msg);
    }
}