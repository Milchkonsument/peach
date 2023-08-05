use crate::{
    errors::InterpreterError,
    pos::{AtPosition, Position},
    token::{Lit, Opr, Sep, Token, TokenBody, LEXEME_TO_KEYWORD},
};

pub struct Lexer<'a> {
    lines: Vec<&'a str>,
    tokens: Vec<Vec<Token>>,
    errors: Vec<InterpreterError>,
    position: Position,
    lexeme_start: Position,
}

impl<'a> Lexer<'a> {
    pub fn new() -> Self {
        Lexer {
            lines: vec![],
            tokens: vec![vec![]],
            errors: vec![],
            position: Position::start(),
            lexeme_start: Position::start(),
        }
    }

    pub fn lex(&mut self, lines: Vec<&'a str>) {
        self.lines = lines;
        self.position.reset();
        self.lexeme_start.reset();

        while self.lines.get(self.position.row).is_some() {
            while let Some(ch) = self.next() {
                match ch {
                    '+' => self.add(TokenBody::Opr(Opr::Add)),
                    '-' => self.add(TokenBody::Opr(Opr::Sub)),
                    '*' => self.add(TokenBody::Opr(Opr::Mul)),
                    '/' => self.add(TokenBody::Opr(Opr::Div)),
                    '(' => self.add(TokenBody::Sep(Sep::LBrace)),
                    ')' => self.add(TokenBody::Sep(Sep::RBrace)),
                    '=' => self.add(TokenBody::Opr(Opr::Ass)),
                    '0'..='9' | '.' => {
                        let mut has_dec = ch == '.';

                        while self.peek().is_some_and(|c| c.is_numeric() || c == '.') {
                            if self.next().is_some_and(|c| c == '.') {
                                match has_dec {
                                    // true => self.err("number already has floating point"),
                                    // handle member access somewhere
                                    true => break,
                                    false => has_dec = true,
                                }
                            }
                        }

                        match self.get_lexeme().chars().last().is_some_and(|c| c == '.') {
                            true => 
                            self.err(String::from("number cannot have trailing decimal point. expected member access or digit")),
                            false => self.add(TokenBody::Lit(if has_dec {
                                Lit::F32((if self.get_lexeme().starts_with('.') {format!("0{}", self.get_lexeme())} else {self.get_lexeme()}).parse().unwrap())
                            } else {
                                Lit::I32(self.get_lexeme().parse().unwrap())
                            })),
                        }
                    }
                    c if c.is_whitespace() => {
                        self.lexeme_start.copy_from(&self.position);
                        continue;
                    }
                    '\'' => {
                        while self.peek().is_some_and(|c| c != '\'') {
                            self.next();
                        }

                        match self.next().is_some_and(|c| c == '\'') {
                            false => self.err("unclosed string literal".to_owned()),
                            true => self.add(TokenBody::Lit(Lit::Str(self.get_lexeme().get(1..self.get_lexeme().len()-1).unwrap().to_owned()))),
                        }
                    },
                    '_' | 'A'..='z' => {
                        while self.peek().is_some_and(|c| matches!(c, '_' | 'A'..='z' | '0'..='9')) {
                            self.next();
                        }
                        
                        match LEXEME_TO_KEYWORD.get(self.get_lexeme().as_str()) {
                            Some(k) => match k {
                                crate::token::Key::Let => match self.peek().is_some_and(|c| c == '!') {
                                    true => {
                                        self.next();
                                        self.add(TokenBody::Key(crate::token::Key::LetMut))
                                    },
                                    false => self.add(TokenBody::Key(k.clone())),
                                },
                                _ => self.add(TokenBody::Key(k.clone()))
                            },
                            None => self.add(TokenBody::Idn),
                        }
                    },
                    c => self.err(format!("unexpected token: {}", c)),
                }
            }
            
            self.tokens.push(vec![]);

            self.position.linebreak();
            self.lexeme_start.linebreak();
        }
    }

    pub fn drain(&mut self) -> (Vec<Vec<Token>>, Vec<InterpreterError>) {
        (
            self.tokens.drain(..).collect::<Vec<_>>(),
            self.errors.drain(..).collect::<Vec<_>>(),
        )
    }

    fn next(&mut self) -> Option<char> {
        let ch = self.peek();
        self.position.next();
        ch
    }

    fn peek(&self) -> Option<char> {
        self.lines.at_position(&self.position)
    }

    fn add(&mut self, body: TokenBody) {
        let lexeme = self.get_lexeme();
        self.lexeme_start.copy_from(&self.position);

        self.tokens.last_mut().unwrap().push(Token {
            lexeme,
            body,
            pos: self.position,
        })
    }

    fn get_lexeme(&self) -> String {
        let lex = String::from(
            self.lines
                .get(self.lexeme_start.row)
                .unwrap()
                .get(self.lexeme_start.col..self.position.col)
                .unwrap_or_default(),
        );
        lex
    }

    fn err(&mut self, err: String) {
        self.errors
            .push(InterpreterError(self.position, self.get_lexeme(), err))
    }
}
