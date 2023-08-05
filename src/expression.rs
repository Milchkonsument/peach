use crate::token::Token;

type BExpr = Box<Expr>;

#[derive(Debug, Clone)]
pub enum Expr {
    Lit(Token),
    Sign(Token, BExpr),
    Sub(BExpr, BExpr),
    Add(BExpr, BExpr),
    Div(BExpr, BExpr),
    Mul(BExpr, BExpr),
    Ass(Token, BExpr),
    Let(Token, BExpr),
    LetMut(Token, BExpr),
    Grp(BExpr),
    Print(BExpr),
    Idn(Token),
    Err,
}
