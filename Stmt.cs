using System;
using System.Collections.Generic;

public abstract class Stmt
{
    public abstract Object accept(Interpreter interpreter);

    public class Spawn : Stmt  
    {
        public Expr X { get; }
        public Expr Y { get; }
        public Spawn(Expr x, Expr y)
        {
            X = x;
            Y = y;
        }

        public override Object accept(Interpreter interpreter)
        {
            return interpreter.VisitSpawnStmt(this);
        }
    }
    public class Color : Stmt
    {
        public Token ColorToken { get; }
        public string ColorName { get; }

        public Color(Token colorToken)
        {
            ColorToken = colorToken;
            ColorName = colorToken.Lexeme;
        }

        public override Object accept(Interpreter interpreter)
        {
            return interpreter.VisitColorStmt(this);
        }   
    }
    public class Size : Stmt
    {
        public Expr Value { get; }

        public Size(Expr value)
        {
            Value = value;
        }

        public override Object accept(Interpreter interpreter)
        {
            return interpreter.VisitSizeStmt(this);
        }
    }
    public class Draw_Line : Stmt
    {
        public Expr DirX { get; }
        public Expr DirY { get; }
        public Expr Distance { get; }

        public Draw_Line(Expr dirX, Expr dirY, Expr distance)
        {
            DirX = dirX;
            DirY = dirY;
            Distance = distance;
        }

        public override Object accept(Interpreter interpreter)
        {   
            return interpreter.VisitDrawLineStmt(this);
        }
    }
    public class Draw_Rectangle : Stmt
    {
        public Expr DirX { get; }
        public Expr DirY { get; }
        public Expr Distance { get; }
        public Expr Width { get; }
        public Expr Height { get; }

        public Draw_Rectangle(Expr dirX, Expr dirY, Expr distance, Expr width, Expr height)
        {
            DirX = dirX;
            DirY = dirY;
            Distance = distance;
            Width = width;
            Height = height;
        }
        public override Object accept(Interpreter interpreter)
        {
            return interpreter.VisitDrawRectangleStmt(this);
        }
    }

    public class Draw_Circle : Stmt
    {
        public Expr DirX { get; }
        public Expr DirY { get; }
        public Expr Radius { get; }

        public Draw_Circle(Expr dirX, Expr dirY, Expr radius)
        {
            DirX = dirX;
            DirY = dirY;
            Radius = radius;
        }

        public override Object accept(Interpreter interpreter)
        {
            return interpreter.VisitDrawCircleStmt(this);
        }
    }

     public class Fill : Stmt
    {
        public Fill()
        {
            // no necesita parámetros
        }

        public override Object accept(Interpreter interpreter)
        {
            return interpreter.VisitFillStmt(this);
        }
    }

    public class Label : Stmt
    {
        public Token name;

        public Label(Token name)
        {
            this.name = name;
        }

        public override Object accept(Interpreter interpreter)
        {
            return interpreter.VisitLabelStmt(this);
        }
    }
    
    public class GoTo : Stmt
    {
        public Token label;
        public Expr condition;

        public GoTo(Token label, Expr condition)
        {
            this.label = label;
            this.condition = condition;
        }

        public override Object accept(Interpreter interpreter)
        {
            return interpreter.VisitGoToStmt(this);
        }
    }
    
    public class Expression : Stmt
    {
        public Expr expression;
        
        public Expression(Expr expression)
        {
            this.expression = expression;
        }
        
        public override Object accept(Interpreter interpreter)
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
        
        public override Object accept(Interpreter interpreter)
        {
            return interpreter.VisitPrintStmt(this);
        }
    }
}