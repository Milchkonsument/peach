use crate::{
    errors::InterpreterError,
    expression::Expr,
    pos::{AtPosition, Position},
    token::{Key, Opr, Sep, Token, TokenBody},
};

pub struct Parser {
    tokens: Vec<Vec<Token>>,
    errors: Vec<InterpreterError>,
    exprs: Vec<Expr>,
    position: Position,
}

impl Parser {
    pub fn new() -> Self {
        Parser {
            tokens: vec![vec![]],
            errors: vec![],
            exprs: vec![],
            position: Position::start(),
        }
    }

    pub fn parse(&mut self, tokens: Vec<Vec<Token>>) {
        self.tokens = tokens;
        self.position.reset();

        while self.tokens.get(self.position.row).is_some() {
            if self.tokens.get(self.position.row).unwrap().is_empty() {
                self.position.linebreak();
                continue;
            }

            let exp = self.exp();
            self.exprs.push(exp);

            if self.peek().is_some() {
                self.err(format!(
                    "unexpected token: '{}'",
                    self.peek().unwrap().lexeme
                ));
            }

            self.position.linebreak();
        }
    }

    pub fn exp(&mut self) -> Expr {
        if self.peek().is_none() {
            return self.err("expected expression".to_owned());
        }

        let t = self.next().unwrap();

        let expr = match t.clone().body {
            TokenBody::Lit(_) => self.lit(t),
            TokenBody::Opr(opr) => self.opr(opr, t),
            TokenBody::Sep(sep) => self.sep(sep),
            TokenBody::Key(key) => self.key(key),
            TokenBody::Idn => self.idn(t),
        };

        expr
    }

    fn idn(&mut self, t: Token) -> Expr {
        match self.peek().is_some() {
            true => {
                if let Some(TokenBody::Opr(opr)) = self.peek().and_then(|t| Some(t.body)) {
                    self.next();
                    match opr {
                        Opr::Add => Expr::Add(Box::new(Expr::Lit(t)), Box::new(self.exp())),
                        Opr::Sub => Expr::Sub(Box::new(Expr::Lit(t)), Box::new(self.exp())),
                        Opr::Mul => Expr::Mul(Box::new(Expr::Lit(t)), Box::new(self.exp())),
                        Opr::Div => Expr::Div(Box::new(Expr::Lit(t)), Box::new(self.exp())),
                        Opr::Ass => Expr::Ass(t, Box::new(self.exp())), // _ => self.err("unexpected token".to_owned()),
                    }
                } else {
                    Expr::Lit(t)
                }
            }
            false => Expr::Lit(t),
        }
    }

    fn key(&mut self, k: Key) -> Expr {
        match k {
            Key::Print => match self.peek().is_some() {
                true => Expr::Print(Box::new(self.exp())),
                false => self.err("expected expression to print".to_owned()),
            },
            Key::Let => {
                let iden = self.next();
                let eq = self.next();

                match iden
                    .clone()
                    .is_some_and(|t| matches!(t.body, TokenBody::Idn))
                    && eq.is_some_and(|t| matches!(t.body, TokenBody::Opr(Opr::Ass)))
                {
                    true => Expr::Let(iden.unwrap(), Box::new(self.exp())),
                    false => self.err("expected valid assignment".to_owned()),
                }
            }
            Key::LetMut => {
                let iden = self.next();
                let eq = self.next();

                match iden
                    .clone()
                    .is_some_and(|t| matches!(t.body, TokenBody::Idn))
                    && eq.is_some_and(|t| matches!(t.body, TokenBody::Opr(Opr::Ass)))
                {
                    true => Expr::Let(iden.unwrap(), Box::new(self.exp())),
                    false => self.err("expected valid assignment".to_owned()),
                }
            }
        }
    }

    fn sep(&mut self, sep: Sep) -> Expr {
        match sep {
            Sep::LBrace => {
                let exp = self.exp();
                match self
                    .next()
                    .is_some_and(|t| t.body == TokenBody::Sep(Sep::RBrace))
                {
                    true => Expr::Grp(Box::new(exp)),
                    false => self.err("missing closing brace".to_owned()),
                }
            }
            Sep::RBrace => self.err("missing opening brace".to_owned()),
        }
    }

    fn opr(&mut self, opr: Opr, t: Token) -> Expr {
        match opr {
            Opr::Add | Opr::Sub => Expr::Sign(t, Box::new(self.exp())),
            _ => self.err("unsupported prefix operator".to_owned()),
        }
    }

    fn lit(&mut self, t: Token) -> Expr {
        match self.peek().is_some() {
            true => match self.peek().unwrap().body {
                TokenBody::Opr(opr) => match opr {
                    Opr::Add => {
                        self.next();
                        Expr::Add(Box::new(Expr::Lit(t)), Box::new(self.exp()))
                    }
                    Opr::Sub => {
                        self.next();
                        Expr::Sub(Box::new(Expr::Lit(t)), Box::new(self.exp()))
                    }
                    Opr::Mul => {
                        self.next();
                        Expr::Mul(Box::new(Expr::Lit(t)), Box::new(self.exp()))
                    }
                    Opr::Div => {
                        self.next();
                        Expr::Div(Box::new(Expr::Lit(t)), Box::new(self.exp()))
                    }
                    _ => self.err("cannot assign to literal".to_owned()),
                },
                _ => Expr::Lit(t),
            },
            false => Expr::Lit(t),
        }
    }

    pub fn drain(&mut self) -> (Vec<Expr>, Vec<InterpreterError>) {
        (
            self.exprs.drain(..).collect::<Vec<_>>(),
            self.errors.drain(..).collect::<Vec<_>>(),
        )
    }

    pub fn next(&mut self) -> Option<Token> {
        let token = self.peek();
        self.position.next();
        token
    }

    pub fn peek(&self) -> Option<Token> {
        self.peek_n(1)
    }

    pub fn peek_n(&self, n: usize) -> Option<Token> {
        self.tokens.at_position(&Position {
            row: self.position.row,
            col: self.position.col + n - 1,
        })
    }

    pub fn err(&mut self, err: String) -> Expr {
        self.errors.push(InterpreterError(
            self.position,
            self.peek_n(1)
                .and_then(|t| Some(t.lexeme))
                .unwrap_or(String::from("??? bruh")),
            err,
        ));
        Expr::Err
    }
}
