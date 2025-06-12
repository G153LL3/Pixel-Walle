using System;
using System.Collections.Generic;

public class Parser // convierte los tokens en un ast usando analisis descendente recurivo
{
    public List<Stmt> Parse()
    {
        List<Stmt> statements = new List<Stmt>();
        while (!IsAtEnd())
        {
            try
            {
                statements.Add(Declaration());
                ConsumeNewline();
            
                // consumir todos los saltos consecutivos
                while (Match(TokenType.NEWLINE)) { }
            }
            catch (ParseError) 
            {
                Synchronize();
            }
        }
        return statements;
    }
   
    // Orden de precedencia 
    // 1. Primary (literales, grupos)
    // 2. Unary (-, !)
    // 3. Exponent (**)
    // 4. Factor (*, /, %)
    // 5. Term (+, -)
    // 6. Comparison(<, <=, >, >=)
    // 7. Equality (==, !=)
    // 8. Logic (&& ||)
    private bool hadErrorInCurrentDeclaration = false;
    private readonly List<Token> tokens; // tokens generados por lexer
    private int current = 0; // pos del token actual


    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    private Expr Expression()
    {
        return LogicOr();
    }
    
    private Stmt Declaration() 
    {
        if (Check(TokenType.IDENTIFIER) && CheckNext(TokenType.ARROW))
        {
            return VarDeclaration();
        }
        else
        {
            return Statement();
        }
    }

    private Stmt Statement()
    {
        if (Match(TokenType.GOTO)) return GoToStatement();
        if (Match(TokenType.PRINT)) return PrintStatement();
        if (Check(TokenType.IDENTIFIER)) return LabelStatement();
            
        throw Error(Peek(), "Expect print statement, variable declaration, or label"); // arreglar nombre del error        
        return null; 
    }
    private Stmt GoToStatement()
    {
        Token gotoToken = Previous(); 
        Consume(TokenType.LEFT_BRACKET, "Expect '[' after 'GoTo'");
        Token label = Consume(TokenType.IDENTIFIER, "Expect label name after 'GoTo['"); // verificar que label exista
        Consume(TokenType.RIGHT_BRACKET, "Expect ']' after label name");
        Consume(TokenType.LEFT_PAREN, "Expect '(' after ']'");
        Expr condition = Expression();
        Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition");
        return new Stmt.GoTo(label, condition);
    }

    private Stmt LabelStatement()
    {
        Token labelToken = Advance();
        return new Stmt.Label(labelToken);
    }
    private Stmt PrintStatement() 
    {
        Expr value = Expression();
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
        return new Stmt.Var(name, initializer);
    }

    private Stmt ExpressionStatement() 
    {
        Expr expr = Expression();       
        return new Stmt.Expression(expr);
    }

    private Expr LogicOr()
    {
        Expr expr = LogicAnd();
    
        while (Match(TokenType.OR))
        {
            Token operador = Previous();
            Expr right = LogicAnd();
            expr = new Expr.Binary(expr, operador, right);
        }
    
        return expr;
    }

    private Expr LogicAnd()
    {
        Expr expr = Equality();
    
        while (Match(TokenType.AND))
        {
            Token operador = Previous();
            Expr right = Equality();
            expr = new Expr.Binary(expr, operador, right);
        }
    
        return expr;
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
        return new Expr.Literal(null);
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
    private void ConsumeNewline()
    {
        if (Check(TokenType.EOF)) return;
    
        // si ya hay un salto de línea o viene EOF, está bien
        if (Check(TokenType.NEWLINE) || Check(TokenType.EOF)) return;
    
        throw Error(Peek(), "Expected newline after statement");
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
                case TokenType.PRINT:
                case TokenType.GOTO:
                    return;
            }
            Advance();
        }
    }
}
internal class ParseError : Exception { }