use crate::expression::Expr;

pub struct PrettyPrinter {}

impl PrettyPrinter {
    pub fn new() -> Self {
        PrettyPrinter {}
    }

    pub fn print(&self, expr: &Expr) -> String {
        match expr {
            Expr::Lit(t) => format!("{}", t.lexeme),
            Expr::Sign(t, e) => format!("{}({})", t.lexeme, &self.print(e.as_ref())),
            Expr::Sub(e0, e1) => format!(
                "-({} {})",
                &self.print(e0.as_ref()),
                &self.print(e1.as_ref())
            ),
            Expr::Add(e0, e1) => format!(
                "+({} {})",
                &self.print(e0.as_ref()),
                &self.print(e1.as_ref())
            ),
            Expr::Div(e0, e1) => format!(
                "/({} {})",
                &self.print(e0.as_ref()),
                &self.print(e1.as_ref())
            ),
            Expr::Mul(e0, e1) => format!(
                "*({} {})",
                &self.print(e0.as_ref()),
                &self.print(e1.as_ref())
            ),
            Expr::Grp(e) => format!("({})", &self.print(e.as_ref())),
            Expr::Ass(t, e) => format!("=({} {})", t.lexeme, self.print(e)),
            Expr::Let(t, e) => format!("l=({} {})", t.lexeme, self.print(e)),
            Expr::LetMut(t, e) => format!("l!=({} {})", t.lexeme, self.print(e)),
            Expr::Print(e) => format!("print({})", self.print(e)),
            Expr::Idn(id) => format!("{}", id.lexeme),
            Expr::Err => "[?]".to_owned(),
        }
    }
}
