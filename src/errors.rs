use std::fmt::Display;

use crate::pos::Position;

#[derive(Debug)]
pub struct InterpreterError(pub Position, pub String, pub String);

impl Display for InterpreterError {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        write!(
            f,
            "[{},{}] at '{}': {}",
            self.0.row + 1,
            self.0.col,
            self.1,
            self.2
        )
    }
}
