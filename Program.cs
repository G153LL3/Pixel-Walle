using System;
using System.Collections.Generic;

public static class Program
{ 

    private static readonly Interpreter interpreter = new Interpreter();
    static bool hadError = false;
    private static bool hadRuntimeError = false;

    public static void Main(string[] args)
    {
       
        RunPrompt();

    }
    private static void RunPrompt()
    {
        // bucle infinito
        // espera que el usuario escriba codigo
        for (;;)
        {
            Console.WriteLine("> ");
            string line = Console.ReadLine();
            if (line == null) break;
            
            Run(line);
            hadError = false;
        }
    }   
    private static void Run(string source)
    {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.ScanTokens();
        Parser parser = new Parser(tokens);
    
        List<Stmt> statements = parser.Parse();

        // detener si hubo un error de sintaxis
        if (hadError) return;
            interpreter.Interpret(statements);
    }
    public static void Error(int linea, string message)
    {   
        // reporta errores
        Report(linea, "", message);
    }
    public static void RuntimeError(RuntimeError error)
    {
        Console.Error.WriteLine(error.Message + 
            $"\n[line {error.token.Line}]");
        hadRuntimeError = true;
    }
    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine("[línea " + line + "] Error" + where + ": " + message);
        hadError = true;
    }
    public static void Error(Token token, string message)
    {
        if (token.Type == TokenType.EOF)
        {
            Report(token.Line, " at end", message);
        }
        else
        {
            Report(token.Line, $" at '{token.Lexeme}'", message);
        }
    }    
}