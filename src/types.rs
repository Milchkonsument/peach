use crate::errors::InterpreterError;

pub type InterpreterResult<T> = Result<T, InterpreterError>;
