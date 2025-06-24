using System;
using System.Collections.Generic;


public class Scanner
{
    private readonly string source; // codigo 
    private readonly List<Token> tokens = new List<Token>(); // tokens encontrados
    private int start = 0; // inicio del token actual
    private int current = 0; // pos actual deltoken
    private int line = 1; // linea actual

    private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>
    {
        // palabras clave del lenguaje
        {"GoTo", TokenType.GOTO},
        {"true", TokenType.TRUE},
        {"false", TokenType.FALSE},
        {"print", TokenType.PRINT},
        
        // instrucciones
        {"Spawn", TokenType.SPAWN},
        {"Size", TokenType.SIZE},
        {"Color", TokenType.COLOR},
        {"DrawLine", TokenType.DRAW_LINE},
        {"DrawCircle", TokenType.DRAW_CIRCLE},
        {"DrawRectangle", TokenType.DRAW_RECTANGLE},
        {"Fill", TokenType.FILL},
    };


    public Scanner(string source)
    {
        this.source = source;
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd()) 
        {
            start = current;
            ScanToken(); // procesa
        }
        // agregar un token de salto virtual al final 
        if (tokens.Count > 0 && tokens[^1].Type != TokenType.NEWLINE)
        {
            tokens.Add(new Token(TokenType.NEWLINE, "", null, line));
        }
        tokens.Add(new Token(TokenType.EOF, "", null, line)); // fin 
        return tokens;
    }

    private void ScanToken() // analiza cada caracter y le asigna un token 
    {
        char c = Advance();
        switch (c)
        {
            case '(': AddToken(TokenType.LEFT_PAREN); break;
            case ')': AddToken(TokenType.RIGHT_PAREN); break;
            case '[': AddToken(TokenType.LEFT_BRACKET); break;
            case ']': AddToken(TokenType.RIGHT_BRACKET); break;
            case ',': AddToken(TokenType.COMMA); break;
            case '-': AddToken(TokenType.MINUS); break;
            case '+': AddToken(TokenType.PLUS); break;
            case '/': AddToken(TokenType.DIV); break;
            case '%': AddToken(TokenType.MOD); break;

            case '!':
                AddToken(Match('=') ? TokenType.NOT_EQUAL: TokenType.NOT);
                break;
            case '*': 
                AddToken(Match('*') ? TokenType.POW : TokenType.MULT); 
                break;
            case '<':   
                AddToken(Match('=') ? TokenType.LESS_EQUAL : Match('_') ? TokenType.ARROW : TokenType.LESS);
                break;
            case '>':
                AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                break;
            case '=':
                if (Match('='))
                {
                    AddToken(TokenType.EQUAL_EQUAL);
                } else {
                    Program.Error(line, "Unexpected character.");
                }
                break;
            case '&':
                if (Match('&'))
                {
                    AddToken(TokenType.AND);
                } else {
                    Program.Error(line, "Unexpected character.");
                }
                break;
            case '|':
                if (Match('|'))
                {
                    AddToken(TokenType.OR);
                } else {
                    Program.Error(line, "Unexpected character.");
                }
                break;
            case ' ':
            case '\r':
            case '\t':
            // ignora espacios en blanco
                break;
            case '\n':
                if (tokens.Count > 0 && tokens[^1].Type != TokenType.NEWLINE)
                {
                   AddToken(TokenType.NEWLINE);
                }
                line++;
                break;    
            default:

                if (IsDigit(c)) 
                {
                    Number();
                } else if (IsAlpha(c)) {
                    Identifier();
                } else {
                    Program.Error(line, "Unexpected character.");
                    if (!IsAtEnd())
                    {
                        Advance();
                    }
                }
                break;
        }
    }

    private void Identifier() // procesa identificadores y palabras clave
    {
        while (IsAlphaNumeric(Peek())) Advance(); 
        
        string text = source.Substring(start, current - start);
        
        if (text[0] != '_') // las etiquetas solo pueden empezar con letras
        {               
            TokenType type = keywords.ContainsKey(text) ? keywords[text] : TokenType.IDENTIFIER; // verificar si es palabra clave o identificador
            AddToken(type);
        } else {
            Program.Error(line, "Unexpected character.");
        }
    }

    private bool IsAlpha(char c) 
    {
        return (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z'
        || c == '_' || c == 'ñ' || c == 'Ñ');

        // no se si quitar las mayusculas
    }

    private bool IsAlphaNumeric(char c)
    {
        return(IsAlpha(c) || IsDigit(c));
    }

    private void Number() // verifica si el string es un num
    {
        while (IsDigit(Peek())) Advance();
    
            AddToken(TokenType.NUMBER,
            int.Parse(source.Substring(start, current - start)));
    }
   
    private bool Match(char expected) // mira si el caracter coincide con el esperado
    {
        if (IsAtEnd()) return false;
        if (source[current] != expected) return false;
        current++;
        return true;
    }

    private char Peek() // retorna el char del string a analizar
    {   
        if (IsAtEnd()) return '\0';
        return source[current];
    }

    private bool IsDigit(char c) // true si es un num
    {
        return c >= '0' && c <= '9';
    }

    private bool IsAtEnd() // verifica que estemos en pos valida de string
    {
        return current >= source.Length;
    }

    private char Advance() // consume el siguiente caracter
    {
        if (IsAtEnd()) return '\0';
        current++;
        char c = source[current - 1];
        return c;
    }

    private void AddToken(TokenType type)
    {
        AddToken(type, null);
    }

    private void AddToken(TokenType type, object literal) //crea el token
    {
        string text = source.Substring(start, current - start);
        tokens.Add(new Token(type, text, literal, line));
    }
}