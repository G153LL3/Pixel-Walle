
using System;
using System.Text;
using System.Collections.Generic;
using System.IO; // Necesario para StringWriter
public static class Program
{ 

    private static readonly Interpreter interpreter = new Interpreter();
    private static readonly List<string> errors = new List<string>();

    static bool hadError = false;
    private static bool hadRuntimeError = false;
     private static StringWriter outputCapture; // Para capturar la salida

    public static void ResetState()
    {
        errors.Clear();
        hadError = false;
        hadRuntimeError = false;
        interpreter.Reset();  
       
        // Limpia la captura de salida
        if (outputCapture != null)
        {
            outputCapture.GetStringBuilder().Clear();
        }
    }
    public static void Main(string[] args)
    {
    }
    public static void Initialize()
    {
        outputCapture = new StringWriter();
        Console.SetOut(outputCapture);
    }
    
    // Versión modificada de Run para uso en Godot
    public static string Execute(string source)
    {
         ResetState();  
        
        try
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();
            
            if (errors.Count > 0)
            {
                return FormatErrors();
            }

            Parser parser = new Parser(tokens);
            List<Stmt> statements = parser.Parse();
            
            if (errors.Count > 0)
            {
                return FormatErrors();
            }
            
            interpreter.Interpret(statements);
            
            // Combinar salida capturada + errores de runtime
            string result = outputCapture.ToString();
            if (hadRuntimeError)
            {
                result += "\n[RUNTIME ERROR] Execution aborted";
            }
            
            return result;
        }
        catch (Exception ex)
        {
            return $"[UNHANDLED EXCEPTION] {ex.Message}\n{ex.StackTrace}";
        }
    }
    private static string FormatErrors()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Errors found:");
        foreach (var error in errors)
        {
            sb.AppendLine($"- {error}");
        }
        return sb.ToString();
    }
    public static void Error(int linea, string message)
    {
        errors.Add($"[Line {linea}] {message}");
    }
    public static void RuntimeError(RuntimeError error)
    {
        errors.Add($"[RUNTIME ERROR] {error.Message}\n[Line {error.token.Line}]");
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
