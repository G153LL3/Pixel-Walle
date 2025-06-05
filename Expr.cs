public abstract class Expr
{ 
    // define estructura del ast

    public abstract Object accept (Interpreter interpreter);

    public class Binary : Expr
    {
        public Expr left;
        public Expr right;
        public Token operador;
        public Binary(Expr left, Token operador, Expr right)
        {
            this.left = left;
            this.operador = operador;
            this.right = right;
        }
        public override Object accept(Interpreter interpreter)
        {
            return interpreter.VisitBinaryExpr(this);
        }
    }
    public class Literal : Expr
    {
        public Object value;
        public Literal(Object value)
        {
            this.value = value;
        }
        public override Object accept(Interpreter interpreter)
        {
            return interpreter.VisitLiteralExpr(this);
        }
    }
    public class Unary : Expr
    {
        public Expr right;
        public Token operador;
        public Unary(Token operador, Expr right)
        {
            this.operador = operador;
            this.right = right;
        }
        public override Object accept(Interpreter interpreter)
        {
            return interpreter.VisitUnaryExpr(this);
        }   
    }
    public class Grouping : Expr
    {
        public Expr expression;
        public Grouping(Expr expression)
        {
            this.expression = expression;
        }
        public override Object accept(Interpreter interpreter)
        {
            return interpreter.VisitGroupingExpr(this);
        }     
    }
}