use std::collections::HashMap;

use once_cell::sync::Lazy;

use crate::pos::Position;

#[derive(Debug, Clone)]
pub struct Token {
    pub lexeme: String,
    pub body: TokenBody,
    pub pos: Position,
}

#[derive(Debug, Clone, PartialEq)]
pub enum TokenBody {
    Lit(Lit),
    Opr(Opr),
    Sep(Sep),
    Key(Key),
    Idn,
}

#[derive(Debug, Clone, PartialEq)]
pub enum Lit {
    I32(i32),
    F32(f32),
    Str(String),
}

#[derive(Debug, Clone, PartialEq)]
pub enum Opr {
    Add,
    Sub,
    Mul,
    Div,
    Ass,
}

#[derive(Debug, Clone, PartialEq)]
pub enum Sep {
    LBrace,
    RBrace,
}

#[derive(Debug, Clone, PartialEq)]
pub enum Key {
    Print,
    Let,
    LetMut,
}

pub const LEXEME_TO_KEYWORD: Lazy<HashMap<&'static str, Key>> = Lazy::new(|| {
    HashMap::from_iter(vec![
        ("let", Key::Let),
        ("let!", Key::LetMut),
        ("print", Key::Print),
    ])
});
