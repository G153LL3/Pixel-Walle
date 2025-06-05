using System;
using System.Collections.Generic;

public abstract class Stmt
{
    public abstract object accept(Interpreter interpreter);

    public class Expression : Stmt
    {
        public Expr expression;
        
        public Expression(Expr expression)
        {
            this.expression = expression;
        }
        
        public override object accept(Interpreter interpreter)
        {
            return interpreter.VisitExpressionStmt(this);
        }
    }

    public class Print : Stmt
    {
        public Expr expression;
        
        public Print(Expr expression)
        {
            this.expression = expression;
        }
        
        public override object accept(Interpreter interpreter)
        {
            return interpreter.VisitPrintStmt(this);
        }
    }
}