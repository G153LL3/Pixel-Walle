using System;
using System.Collections.Generic;

public class Interpreter 
{
    private Environment environment = new Environment();
    private readonly Dictionary<string, dynamic> variables = new Dictionary<string, dynamic>();
    private readonly Dictionary<string, int> labels = new Dictionary<string, int>();
    private int currentLine = 0;

    public void Interpret(List<Stmt> statements)
    {
        try
        {    
            CollectLabels(statements);
            currentLine = 0;
            while (currentLine < statements.Count)
            {
                Execute(statements[currentLine]);
                currentLine++;
            }

        }
        catch (RuntimeError error)
        {
            Console.WriteLine($"Runtime error: {error.Message}");
        }
    }

    private void CollectLabels(List<Stmt> statements)
    {
        labels.Clear();
        for (int i = 0; i < statements.Count; i++)
        {
            if (statements[i] is Stmt.Label labelStmt)
            {
                if (labels.ContainsKey(labelStmt.name.Lexeme))
                {
                    throw new RuntimeError(labelStmt.name, $"Label duplicado '{labelStmt.name.Lexeme}'.");
                }
                labels[labelStmt.name.Lexeme] = i;
            }
        }
    }

    public Object VisitGoToStmt(Stmt.GoTo stmt)
    {
        bool shouldJump = true; // evaluar condicion
        if (stmt.condition != null)
        {
            Object conditionValue = Evaluate(stmt.condition);
            shouldJump = IsTruthy(conditionValue);
        }
        if (shouldJump)
        {
            if(labels.TryGetValue(stmt.label.Lexeme, out int targetLine))
            {
                if (targetLine == currentLine)
                {
                    throw new RuntimeError(stmt.label, "Infinite recursive jump detected");
                }
                currentLine = targetLine; // saltar
            } 
            else
            {
                 throw new RuntimeError(stmt.label, $"Label not defined '{stmt.label.Lexeme}'.");
            }
        }
        return null;
    }
    public object VisitLabelStmt(Stmt.Label stmt)
    {
        // solo registrar la etiqueta
        // labels[stmt.name.Lexeme] = labels.Count; 
        return null;
    }

    public Object VisitLiteralExpr(Expr.Literal expr) // devuelve el valor almacenado en el nodo
    {
        return expr.value;
    }
    public Object VisitUnaryExpr(Expr.Unary expr) 
    {
        Object right = Evaluate(expr.right);
    
        switch (expr.operador.Type)
        {
            case TokenType.NOT:
                return !IsTruthy(right);
            case TokenType.MINUS:
                return -(int)right;
        }
        return null;
    }
    private void CheckNumberOperand(Token operador, Object operand) 
    {
        if (operand is int) return;
        throw new RuntimeError(operador, "Operand must be a number.");
    }
    private void CheckNumberOperands(Token operador, Object left, Object right) 
    {
        if (left is int && right is int) return;
        throw new RuntimeError(operador, "Operands must be numbers.");
    }

    private bool IsTruthy(Object obj)
    {
        if (obj == null) return false;
        if (obj is bool) return (bool)obj;
        return true;
    }
    private bool IsEqual(Object a, Object b)
    {
        if (a == null && b == null) return true;
        if (a == null) return false;
        return a.Equals(b);
    }

    public Object VisitGroupingExpr(Expr.Grouping expr)
    {
        return Evaluate(expr.expression);
    }

    private Object Evaluate(Expr expr) // usa  el patron visitor
    {
        return expr.accept(this);
    }
    private void Execute(Stmt stmt)
    {
        stmt.accept(this);
    }
    public Object VisitExpressionStmt(Stmt.Expression stmt)
    {
        Evaluate(stmt.expression);
        return null;
    }
    public Object VisitVariableExpr(Expr.Variable expr)
    {
        return environment.Get(expr.name); 
    }
    public Object VisitPrintStmt(Stmt.Print stmt)
    {
        Object value = Evaluate(stmt.expression);
        Console.WriteLine(value);
        return null;
    }
    public Object VisitVarStmt(Stmt.Var stmt) 
    {
        Object value = null;
        if (stmt.initializer != null) 
        {
            value = Evaluate(stmt.initializer);
        }
        environment.Define(stmt.name.Lexeme, value);
        return null;
    }
    
    public Object VisitBinaryExpr(Expr.Binary expr)
    {
        Object left = Evaluate(expr.left);
        Object right = Evaluate(expr.right);

        switch (expr.operador.Type)
        {
            case TokenType.AND:
                if (!IsTruthy(left)) return false;
                return IsTruthy(right);
            case TokenType.OR:
                if (IsTruthy(left)) return true;
                return IsTruthy(right);

            case TokenType.NOT_EQUAL:  return !IsEqual(left, right);
            case TokenType.EQUAL_EQUAL: return IsEqual(left, right);

            case TokenType.GREATER:
                CheckNumberOperands(expr.operador, left, right);
                return (int)left > (int)right;
            case TokenType.GREATER_EQUAL:
                CheckNumberOperands(expr.operador, left, right);
                return (int)left >= (int)right;
            case TokenType.LESS:
                CheckNumberOperands(expr.operador, left, right);
                return (int)left < (int)right;
            case TokenType.LESS_EQUAL:
                CheckNumberOperands(expr.operador, left, right);
                return (int)left <= (int)right;
            case TokenType.MINUS:
                CheckNumberOperand(expr.operador, right);
                CheckNumberOperands(expr.operador, left, right);
                return (int)left - (int)right;
            case TokenType.PLUS:
                CheckNumberOperands(expr.operador, left, right);
                return (int)left + (int)right;
            case TokenType.DIV:
                CheckNumberOperands(expr.operador, left, right);
                int a = (int)right;
                if (a == 0)
                {
                    throw new RuntimeError(expr.operador, "Division by zero is not allowed");
                    
                }
                return (int)left / (int)right;
            case TokenType.MULT:
                CheckNumberOperands(expr.operador, left, right);
                return (int)left * (int)right;
            case TokenType.MOD:
                CheckNumberOperands(expr.operador, left, right);
                return (int)left % (int)right;
            case TokenType.POW:
                CheckNumberOperands(expr.operador, left, right);
                return Bpow((int)left, (int)right); // no uso el de c# pq devuelve double
        }
        return null;
    }
    public int Bpow(int b, int e) // exponenciacion binaria (x fin algo de cp)
    {
        if (e == 0) return 1;
        if (e % 2 == 0)
        {
            int t = Bpow(b, e/2);
            return t*t;
        }
        return Bpow(b, e-1) * b;        
    } 
}
