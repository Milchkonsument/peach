use std::{env, fs};

use chrono::Local;
use lexer::Lexer;
use parser::Parser;
use pretty_printer::PrettyPrinter;

use crate::interpreter::Interpreter;

mod errors;
mod expression;
mod interpreter;
mod lexer;
mod parser;
mod pos;
mod pretty_printer;
mod token;
mod types;

fn main() {
    let args = env::args().collect::<Vec<_>>();

    let path = args.get(1);

    if path.is_none() {
        println!("usage: peach source.peach");
        return;
    }

    let path = path.unwrap();

    match fs::read_to_string(path) {
        Ok(src) => interpret(src.lines().collect::<Vec<_>>()),
        Err(err) => eprintln!("{err}"),
    }
}

fn interpret(lines: Vec<&str>) {
    for line in &lines {
        println!("{line}")
    }

    println!();

    let start_time = Local::now();

    let mut lexer = Lexer::new();
    let mut parser = Parser::new();
    let pretty_printer = PrettyPrinter::new();
    let interpreter = Interpreter::new();

    lexer.lex(lines);

    let (tokens, errors) = lexer.drain();

    for tok in &tokens {
        for t in tok {
            println!("{:?}", t)
        }
        println!()
    }

    for err in &errors {
        eprintln!("{}", err)
    }

    if errors.is_empty() == false {
        return;
    }

    parser.parse(tokens);

    let (exprs, errors) = parser.drain();

    for err in &errors {
        eprintln!("{}", err)
    }

    if errors.is_empty() == false {
        return;
    }

    for expr in &exprs {
        println!("{}", pretty_printer.print(&expr))
    }

    // for expr in &exprs {
    //     println!("{:?}", interpreter.eval(expr))
    // }

    println!(
        "\n[ took {}ms ]",
        Local::now()
            .signed_duration_since(start_time)
            .num_milliseconds()
    )
}
