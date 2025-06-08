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
    public class Var : Stmt
    {
        public Token name;
        public Expr initializer; 
    
        public Var(Token name, Expr initializer)
        {
            this.name = name;
            this.initializer = initializer;
        }
    
        public override Object accept(Interpreter interpreter)
        {
            return interpreter.VisitVarStmt(this);
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