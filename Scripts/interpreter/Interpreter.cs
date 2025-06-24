using System;
using System.Collections.Generic;

public class Interpreter
{
    private Environment environment = new Environment();
    private readonly Dictionary<string, dynamic> variables = new Dictionary<string, dynamic>();
    private readonly Dictionary<string, int> labels = new Dictionary<string, int>();
    private int currentLine = 0;
    private bool spawned = false;
    private int currentX, currentY; // pos actual
    private string currentColor = "Transparent"; //pincel
    private int currentSize = 1; // pincel
    

    public void Reset()
    {
        spawned = false;
        environment = new Environment();

    }
    public void Interpret(List<Stmt> statements)
    {
        try
        {
            if (statements.Count == 0 || !(statements[0] is Stmt.Spawn))
            {
                throw new RuntimeError(null, "The program must start with a Spawn command");
            }
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

    public object VisitFunctionCallExpr(Expr.FunctionCall expr)
    {
        switch (expr.name.Lexeme)
        {
            case "GetActualX":
                // validar que no tenga argumentos
                if (expr.arguments.Count > 0)
                {
                    throw new RuntimeError(expr.name, "The GetActualX function does not accept arguments");
                }
                return currentX;

            case "GetActualY":
                // validar que no tenga argumentos
                if (expr.arguments.Count > 0)
                {
                    throw new RuntimeError(expr.name, "The GetActualY function does not accept arguments");
                }
                return currentY;

            case "GetCanvasSize":
                // validar que no tenga argumentos
                if (expr.arguments.Count > 0)
                {
                    throw new RuntimeError(expr.name, "The GetActualX function does not accept arguments");
                }
                return Canvas.GridSize;

            case "IsBrushSize":
                // validar que solo tenga 1 argumento
                if (expr.arguments.Count != 1)
                {
                    throw new RuntimeError(expr.name, "IsBrushSize requires 1 argument");
                }
                Object firstarg = Evaluate(expr.arguments[0]);
                IsInt(firstarg);

                int size = (int)firstarg;
                if (size == currentSize) return 1;
                return 0;

            case "IsBrushColor":
                // validar que solo tenga 1 argumento
                if (expr.arguments.Count != 1)
                {
                    throw new RuntimeError(expr.name, "IsBrushColor requires 1 argument");
                }
                Object colorarg = Evaluate(expr.arguments[0]);
                IsString(colorarg);

                string colorName = (string)colorarg;
                if (colorName == currentColor) return 1;
                return 0;

            case "IsCanvasColor":
                if (expr.arguments.Count != 3)
                {
                    throw new RuntimeError(expr.name, "IsCanvasColor requires 3 arguments");
                }
                Object farg = Evaluate(expr.arguments[0]);
                Object sarg = Evaluate(expr.arguments[1]);
                Object targ = Evaluate(expr.arguments[2]);

                IsString(farg);
                IsInt(sarg);
                IsInt(targ);
                
                string color = (string)farg;
                int v = (int)sarg;
                int h = (int)targ;

                int posh = currentX + h;
                int posv = currentY + v;
                IsValidPosition(posh, posv);
                
                if (color == Canvas.GetPixel(posv, posh)) return 1;
                return 0;

            case "GetColorCount":    

                if (expr.arguments.Count != 5)
                {
                    throw new RuntimeError(expr.name, "GetColorCount requires 5 arguments");
                }
                Object colors = Evaluate(expr.arguments[0]);
                Object X1 = Evaluate(expr.arguments[1]);
                Object Y1 = Evaluate(expr.arguments[2]);
                Object X2 = Evaluate(expr.arguments[3]);
                Object Y2 = Evaluate(expr.arguments[4]);

                IsString(colors);
                IsInt(X1);
                IsInt(X2);
                IsInt(Y1);
                IsInt(Y2);

                string color1 = (string)colors;
                int x1 = (int)X1;
                int x2 = (int)X2;
                int y1 = (int)Y1;
                int y2 = (int)Y2;

                IsValidPosition(x1,y1);
                IsValidPosition(x2,y2);

                int startX = Math.Min(x1, x2);
                int endX = Math.Max(x1, x2);
                int startY = Math.Min(y1, y2);
                int endY = Math.Max(y1, y2);

                int count = 0; // contador de pixeles
                for (int y = startY; y <= endY; y++)
                {
                    for (int x = startX; x <= endX; x++)
                    {
                        string colorCan = Canvas.GetPixel(y, x);
                        if (colorCan == color1)
                        {
                            count++;
                        }   
                    }
                }  
                return count;
            default:
                throw new RuntimeError(expr.name, $"Undefined function: {expr.name.Lexeme}");
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
                    throw new RuntimeError(labelStmt.name, $"Duplicate label '{labelStmt.name.Lexeme}'.");
                }
                labels[labelStmt.name.Lexeme] = i;
            }
        }
    }

    public Object VisitSpawnStmt(Stmt.Spawn stmt)
    {
        if (spawned)
        {
            throw new RuntimeError(null, "Spawn can only be used once");
        }
        Object xObj = Evaluate(stmt.X);
        Object yObj = Evaluate(stmt.Y);

        IsInt(xObj);
        IsInt(yObj);

        int x = (int)xObj;
        int y = (int)yObj;

        IsValidPosition(x,y);
        
        currentX = x;
        currentY = y;
        spawned = true; // marcar q ya hay un spawn

        return null;
    }
    
    public Object VisitColorStmt(Stmt.Color stmt)
    {
        string color = stmt.ColorName;

        string[] validColors = {
            "Red", "Blue", "Green", "Yellow",
            "Orange", "Purple", "Black", "White", "Transparent"
        };
        bool isValidColor = false;
        foreach (string validColor in validColors)
        {
            if (color == validColor)
            {
                isValidColor = true;
                break;
            }
        }
        if (!isValidColor)
        {
            throw new RuntimeError(stmt.ColorToken, $"Color '{color}' is not supported");
        }
        currentColor = color; // cambiar color del pincel
        return null;
    }

    public Object VisitSizeStmt(Stmt.Size stmt)
    {
        Object value = Evaluate(stmt.Value);

        IsInt(value);
        int k = (int)value;
        IsPositive(k);

        if (k % 2 == 0)
        {
            k = k - 1;
        }
        currentSize = k;
        return null;
    }

    public Object VisitDrawLineStmt(Stmt.Draw_Line stmt)
    {
        Object dirXObj = Evaluate(stmt.DirX);
        Object dirYObj = Evaluate(stmt.DirY);
        Object distanceObj = Evaluate(stmt.Distance);

        IsInt(dirXObj);
        IsInt(dirYObj);
        IsInt(distanceObj);

        int dirX = (int)dirXObj;
        int dirY = (int)dirYObj;
        int distance = (int)distanceObj;

        IsValidDir(dirX, dirY);
        IsPositive(distance);
        
        int endX = currentX + dirX * distance;
        int endY = currentY + dirY * distance;

        IsValidPosition(endX, endY);

        if (currentColor != "Transparent")
        {
            Walle.DrawLine(currentX, currentY, endX, endY, dirX, dirY, distance, currentColor, currentSize);
        }
 
        currentX = endX;
        currentY = endY;
        return null;
    }

    public object VisitDrawRectangleStmt(Stmt.Draw_Rectangle stmt) 
    {
        Object dirXObj = Evaluate(stmt.DirX);
        Object dirYObj = Evaluate(stmt.DirY);
        Object distanceObj = Evaluate(stmt.Distance);
        Object widthObj = Evaluate(stmt.Width);
        Object heightObj = Evaluate(stmt.Height);

        IsInt(dirXObj);
        IsInt(dirYObj);
        IsInt(distanceObj);
        IsInt(widthObj);
        IsInt(heightObj);

        int dirX = (int)dirXObj;
        int dirY = (int)dirYObj;
        int distance = (int)distanceObj;
        int width = (int)widthObj;
        int height = (int)heightObj;

        IsValidDir(dirX, dirY);
        IsPositive(distance);
        IsPositive(width);
        IsPositive(height);

        // calcular posición del centro del rectángulo
        int centerX = currentX + dirX * distance;
        int centerY = currentY + dirY * distance;

        IsValidPosition(centerX,centerY);
        
        if (currentColor != "Transparent") // dibujar
        {
            // Calcular esquinas del rectángulo
            int left = centerX - width / 2;
            int right = centerX + width / 2;
            int top = centerY - height / 2;
            int bottom = centerY + height / 2;

            Walle.DrawRectangleLines(left, top, right, bottom, currentColor, currentSize);
        }
        // actualizar el walle al centro
        currentX = centerX;
        currentY = centerY;
        return null;
    }

    public Object VisitDrawCircleStmt(Stmt.Draw_Circle stmt)
    {
        Object dirXObj = Evaluate(stmt.DirX);
        Object dirYObj = Evaluate(stmt.DirY);
        Object radiusObj = Evaluate(stmt.Radius);
        
        IsInt(dirXObj);
        IsInt(dirYObj);
        IsInt(radiusObj);
        
        int dirX = (int)dirXObj;
        int dirY = (int)dirYObj;
        int radius = (int)radiusObj;
        
        IsValidDir(dirX, dirY);
        IsPositive(radius);
        
        int centerX = currentX + dirX * radius;
        int centerY = currentY + dirY * radius;

        IsValidPosition(centerX,centerY);
        
        if (currentColor != "Transparent")
        {
            Walle.DrawCircleOutline(centerX, centerY, radius, currentColor, currentSize);
        }
        currentX = centerX;
        currentY = centerY;
        return null;
    }

    public object VisitFillStmt(Stmt.Fill stmt)
    {
        int x = currentX;
        int y = currentY;
        if (currentColor != "Transparent")
        {
            Walle.FloodFill(x, y,currentColor, Canvas.GridSize);
        }
        return null;
    }

    public void IsValidPosition(int x, int y)
    {
        if (x < 0 || x >= Canvas.GridSize || y < 0 || y >= Canvas.GridSize)
        {
            throw new RuntimeError(null, $"Position final outside the canvas");
        }
    }

    public void IsPositive(int x)
    {
        if (x < 0)
        {
            throw new RuntimeError(null, "Value must be positive");
        }
    }

    public void IsInt(Object x)
    {
        if (!(x is int))
        {
            throw new RuntimeError(null, "Parameters must be integers");
        }
    }
    public void IsString (Object x)
    {
        if (!(x is string))
        {
            throw new RuntimeError(null, "Invalid argument");
        }
    }
    
    public void IsValidDir(int x, int y)
    {
        if (Math.Abs(x) > 1 || Math.Abs(y) > 1)
        {
            throw new RuntimeError(null, "Direction values must be -1, 0, or 1");
        }
        if (x == 0 && y == 0)
        {
            //throw new RuntimeError(null, "Invalid direction");
        }
    }

    public Object VisitGoToStmt(Stmt.GoTo stmt)
    {
        bool shouldJump = true; // evaluar condicion
        if (stmt.condition != null)
        {
            Object conditionValue = Evaluate(stmt.condition);
            if (conditionValue is not bool)
            {
                throw new RuntimeError(stmt.label, "The condition must evaluate to a boolean value");
            }
            shouldJump = IsTruthy(conditionValue);
        }
        if (shouldJump)
        {
            if (labels.TryGetValue(stmt.label.Lexeme, out int targetLine))
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
    // devuelve el valor almacenado en el nodo
    public object VisitLabelStmt(Stmt.Label stmt)
    {
        return null;
    }

    public Object VisitLiteralExpr(Expr.Literal expr) 
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

    private Object Evaluate(Expr expr) 
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
