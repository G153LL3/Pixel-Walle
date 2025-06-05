using System;
using System.Collections.Generic;

public enum TokenType
{
    // Agrupadores 
    COMMA, LEFT_PAREN, RIGHT_PAREN,
    LEFT_BRACKET, RIGHT_BRACKET,

    // Operadores
    DIV, PLUS, MINUS, MULT, MOD,
    POW,

    // Comparadores 
    LESS_EQUAL, EQUAL_EQUAL, 
    LESS, GREATER_EQUAL, GREATER, NOT,
    NOT_EQUAL, AND, OR,

    // Operador de asignacion
    ASSIGNAMENT,

    // Literales
    NUMBER, IDENTIFIER,
    
    // Plabras clave   
    TRUE, FALSE, GOTO,
    ELSE, IF, PRINT,
    //agregar funciones

    // Fin de archivo
    EOF
}  
public class Token
{
    public readonly TokenType Type;
    public readonly string Lexeme;
    public readonly object Literal;
    public readonly int Line;

    public Token(TokenType type, string lexeme, object literal, int line)
    {
        Type = type;
        Lexeme = lexeme;
        Literal = literal;
        Line = line;
    }

    public override string ToString()
    {
        return $"{Type} {Lexeme} {Literal}";
    }    
}