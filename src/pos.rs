use crate::token::Token;

#[derive(Debug, Clone, Copy)]
pub struct Position {
    pub row: usize,
    pub col: usize,
}

impl Position {
    pub fn start() -> Self {
        Position { row: 0, col: 0 }
    }

    pub fn linebreak(&mut self) -> usize {
        self.row += 1;
        self.col = 0;
        self.row - 1
    }

    pub fn next(&mut self) -> usize {
        self.col += 1;
        self.col
    }

    pub fn reset(&mut self) {
        self.col = 0;
        self.row = 0;
    }

    pub fn copy_from(&mut self, other: &Position) {
        self.col = other.col;
        self.row = other.row;
    }
}

pub trait AtPosition<T> {
    fn at_position(&self, position: &Position) -> Option<T>;
}

impl AtPosition<char> for Vec<&str> {
    fn at_position(&self, position: &Position) -> Option<char> {
        self.get(position.row)
            .and_then(|v| v.chars().nth(position.col))
    }
}

impl AtPosition<Token> for Vec<Vec<Token>> {
    fn at_position(&self, position: &Position) -> Option<Token> {
        self.get(position.row)
            .and_then(|v| v.get(position.col))
            .and_then(|t| Some(t.clone()))
    }
}
