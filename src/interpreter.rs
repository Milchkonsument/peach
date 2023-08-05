use crate::{expression::Expr, token::Token};

pub struct Interpreter {}

impl Interpreter {
    pub fn new() -> Self {
        Interpreter {}
    }

    pub fn eval(&self, expr: &Expr) -> Option<f64> {
        match expr {
            Expr::Lit(t) => self.eval_lit(t),
            Expr::Sign(t, e) => self.eval_sign(t, e),
            Expr::Sub(e0, e1) => self.eval_sub(e0, e1),
            Expr::Add(e0, e1) => self.eval_add(e0, e1),
            Expr::Div(e0, e1) => self.eval_div(e0, e1),
            Expr::Mul(e0, e1) => self.eval_mul(e0, e1),
            Expr::Grp(e) => self.eval_grp(e),
            Expr::Ass(e0, e1) => todo!(),
            Expr::Let(e0, e1) => todo!(),
            Expr::LetMut(t, e) => todo!(),
            Expr::Print(e) => todo!(),
            Expr::Idn(t) => todo!(),
            Expr::Err => None,
        }
    }

    fn eval_lit(&self, t: &Token) -> Option<f64> {
        match t.body.clone() {
            crate::token::TokenBody::Lit(lit) => match lit {
                crate::token::Lit::I32(i) => Some(i as f64),
                crate::token::Lit::F32(f) => Some(f as f64),
                crate::token::Lit::Str(_) => None,
            },
            _ => None,
        }
    }

    fn eval_sign(&self, t: &Token, e: &Expr) -> Option<f64> {
        match t.body.clone() {
            crate::token::TokenBody::Opr(opr) => match opr {
                crate::token::Opr::Add => self.eval(e).and_then(|f| Some(f)),
                crate::token::Opr::Sub => self.eval(e).and_then(|f| Some(-f)),
                _ => None,
            },
            _ => None,
        }
    }

    fn eval_sub(&self, e0: &Expr, e1: &Expr) -> Option<f64> {
        self.eval_bin_op(e0, e1, |f0, f1| f0 - f1)
    }

    fn eval_add(&self, e0: &Expr, e1: &Expr) -> Option<f64> {
        self.eval_bin_op(e0, e1, |f0, f1| f0 + f1)
    }

    fn eval_div(&self, e0: &Expr, e1: &Expr) -> Option<f64> {
        self.eval_bin_op(e0, e1, |f0, f1| f0 / f1)
    }

    fn eval_mul(&self, e0: &Expr, e1: &Expr) -> Option<f64> {
        self.eval_bin_op(e0, e1, |f0, f1| f0 * f1)
    }

    fn eval_grp(&self, e: &Expr) -> Option<f64> {
        self.eval(e)
    }

    fn eval_err(&self) -> Option<f64> {
        None
    }

    fn eval_bin_op(&self, e0: &Expr, e1: &Expr, op: fn(f64, f64) -> f64) -> Option<f64> {
        let eval0 = self.eval(e0);
        let eval1 = self.eval(e1);

        if eval0.is_none() || eval1.is_none() {
            None
        } else {
            Some(op(eval0.unwrap(), eval1.unwrap()))
        }
    }
}
