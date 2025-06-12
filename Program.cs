using System;
using System.Text;
using System.Collections.Generic;

public static class Program
{ 

    private static readonly Interpreter interpreter = new Interpreter();
    private static readonly List<string> errors = new List<string>();

    static bool hadError = false;
    private static bool hadRuntimeError = false;

    public static void Main(string[] args)
    {
        StringBuilder inputBuilder = new StringBuilder();

        while (true)
        {
            Console.Write("> ");  
            string line = Console.ReadLine();
             if (string.IsNullOrWhiteSpace(line)) continue;
            // terminar entrada por ahora
            if (line == null || line.Trim().Equals("salir", StringComparison.OrdinalIgnoreCase))
            {
                if (inputBuilder.Length > 0)
                {
                    Run(inputBuilder.ToString());
                    inputBuilder.Clear();  
                }
                break;
            } else {
                inputBuilder.AppendLine(line);
            }
        }
    }
    private static void Run(string source)
    {
        errors.Clear(); // limpiar errores anteriores
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.ScanTokens();
         if (errors.Count > 0)
        {
            Console.WriteLine("\nErrors found:");
            foreach (var error in errors)
            {
                Console.WriteLine($"- {error}");
            }
            return;
        }

        Parser parser = new Parser(tokens);
    
        List<Stmt> statements = parser.Parse();
        if (errors.Count > 0)
        {
            Console.WriteLine("\nErrors found:");
            foreach (var error in errors)
            {
                Console.WriteLine($"- {error}");
            }
            return;
        }
        interpreter.Interpret(statements);
    }
    public static void Error(int linea, string message)
    {   
        errors.Add($"[Line {linea}] {message}");
    }
    public static void RuntimeError(RuntimeError error)
    {
        Console.Error.WriteLine(error.Message + 
            $"\n[Line {error.token.Line}]");
        hadRuntimeError = true;
    }
    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine("[Line " + line + "] Error" + where + ": " + message);
        hadError = true;
    }
    public static void Error(Token token, string message)
    {
        errors.Add($"[Line {token.Line}] {message}");
    }    
}