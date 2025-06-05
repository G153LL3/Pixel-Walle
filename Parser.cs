using System;
using System.Collections.Generic;

public class Parser // convierte los tokens en un ast usando analisis descendente recurivo
{
    public Expr? Parse()
    {
        try
        {
            return Expression();
        }
        catch (ParseError)
        {
            return null;
        }
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
    {
        Advance();

        while (!IsAtEnd())
        {
            switch (Peek().Type)
            {
                case TokenType.GOTO:
                
                /*
                case TokenType.If:
                case TokenType.Else:
                case TokenType.While:
                case TokenType.Print:
                */
                    return;
            }

            Advance();
        }
    }
    private Expr Expression()
    {
        return Equality();
    }
}
