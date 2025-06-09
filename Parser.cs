using System;
using System.Collections.Generic;

public class Parser // convierte los tokens en un ast usando analisis descendente recurivo
{
    public List<Stmt> Parse()
    {
        List<Stmt> statements = new List<Stmt>();
        while (!IsAtEnd())
        {
            statements.Add(Declaration());
        }
        return statements;
    }

    private class ParseError : Exception
    {
        public ParseError() : base() { }
    
        public ParseError(string message) : base(message) { }

        public ParseError(string message, Exception inner) : base(message, inner) { }
    }
   
    // Orden de precedencia 
    // 1. Primary (literales, grupos)
    // 2. Unary (-, !)
    // 3. Exponent (**)
    // 4. Factor (*, /, %)
    // 5. Term (+, -)
    // 6. Comparison(<, <=, >, >=)
    // 7. Equality (==, !=)
    
    private readonly List<Token> tokens; // tokens generados por lexer
    private int current = 0; // pos del token actual

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }
    private Expr Expression()
    {
        return Equality();
    }
    private Stmt Declaration() 
    {
        try 
        {
            if (Check(TokenType.IDENTIFIER) && CheckNext(TokenType.ARROW)) return VarDeclaration();
            return Statement();
        } 
        catch (ParseError error) 
        {
            Synchronize();
            return null;
        }
    }
    private Stmt Statement()
    {
        if (Match(TokenType.PRINT)) return PrintStatement();
        return ExpressionStatement();
    }
    private Stmt PrintStatement() 
    {
        Expr value = Expression();
        // consumir salto de linea
        return new Stmt.Print(value);
    }
    private Stmt VarDeclaration() 
    {
        Token name = Consume(TokenType.IDENTIFIER, "Expect variable name.");
        Expr initializer = null;
    
        if (Match(TokenType.ARROW)) 
        {
            initializer = Expression();
        }
        // consumir salto de linea
        return new Stmt.Var(name, initializer);
    }
    private Stmt ExpressionStatement() 
    {
        Expr expr = Expression();
        // consumir salto de linea
        return new Stmt.Expression(expr);
    }
    private Expr Equality()
    {
        Expr expr = Comparison();
        while (Match(TokenType.NOT_EQUAL, TokenType.EQUAL_EQUAL))
        {
            Token operador = Previous();
            Expr right = Comparison();
            expr = new Expr.Binary(expr, operador, right);
        }
        return expr;
    }
    private Expr Comparison()
    {
        Expr expr = Term();
        while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
        {
            Token operador = Previous();
            Expr right = Term();
            expr = new Expr.Binary(expr, operador, right);
        }
        return expr;
    }
    private Expr Term()
    {
        Expr expr = Factor();
        while (Match(TokenType.MINUS, TokenType.PLUS))
        {
            Token operador = Previous();
            Expr right = Factor();
            expr = new Expr.Binary(expr, operador, right);
        }
        return expr;
    }
    private Expr Factor()
    {
        Expr expr = Exponent();
        while (Match(TokenType.DIV, TokenType.MULT, TokenType.MOD))
        {
            Token operador = Previous();
            Expr right = Exponent();
            expr = new Expr.Binary(expr, operador, right);
        }
        return expr;
    }
    private Expr Exponent() 
    {
        Expr expr = Unary();
        if (Match(TokenType.POW)) 
        {
            Token operador = Previous();
            Expr right = Exponent(); 
            expr = new Expr.Binary(expr, operador, right);
        }
        return expr;
    }
    
    private Expr Unary()
    {
        if (Match(TokenType.NOT, TokenType.MINUS))
        {
            Token operador = Previous();
            Expr right = Unary();
            return new Expr.Unary(operador, right);
        }
        return Primary();
    }
    private Expr Primary()
    {   
        if (Match(TokenType.FALSE)) return new Expr.Literal(false);
        if (Match(TokenType.TRUE)) return new Expr.Literal(true);
    
        if (Match(TokenType.NUMBER))
        {
            return new Expr.Literal(Previous().Literal);
        }   
        if (Match(TokenType.IDENTIFIER))
        {
            return new Expr.Variable(Previous());
        }
        if (Match(TokenType.LEFT_PAREN))
        {
            Expr expr = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
            return new Expr.Grouping(expr);
        }
        throw Error(Peek(), "Expect expression.");
    }
    private bool Match(params TokenType[] types)
    {
        foreach (TokenType type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }
        return false;
    }
    private Token Consume(TokenType type, string message) 
    {
        if (Check(type)) return Advance();
        throw Error(Peek(), message);
    }
    private bool Check(TokenType type)
    {   
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }
    private bool CheckNext(TokenType type) // mira elsiguiente token
    {
        if (IsAtEnd() || current + 1 >= tokens.Count) return false;
        return tokens[current + 1].Type == type;
    }

    private Token Advance()
    {
        if (!IsAtEnd()) current++;
        return Previous();
    }

    private bool IsAtEnd()
    {
        return Peek().Type == TokenType.EOF;
    }

    private Token Peek()
    {
        return tokens[current];
    }

    private Token Previous()
    {
        return tokens[current - 1];
    }
    private ParseError Error(Token token, string message) 
    {
        Program.Error(token, message);
        return new ParseError();
    }
    private void Synchronize() // busca ptos de resincronizacion
    {                          // evita que los errores en cascada afecten 
        Advance();

        while (!IsAtEnd())
        {
            switch (Peek().Type)
            {
                case TokenType.GOTO:
                case TokenType.PRINT:
                /*
                case TokenType.If:
                case TokenType.Else:
                case TokenType.While:
                
                */
                    return;
            }

            Advance();
        }
    }
}
