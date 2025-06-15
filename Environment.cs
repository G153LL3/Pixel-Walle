using System;
using System.Collections.Generic;

public class Environment
{
    private readonly Dictionary<string, Object> values = new Dictionary<string, Object>();
    private readonly HashSet<string> declared = new HashSet<string>();  // Nuevo: registro de declaraciones
    public readonly Environment Enclosing;

    public Environment(Environment enclosing = null)
    {
        Enclosing = enclosing;
    }
    public void Declare(string name)
    {
        declared.Add(name);
        values[name] = null;  // Opcional: inicializar como null
    }

    // Verificar si una variable está declarada
    public bool IsDeclared(string name)
    {
        if (declared.Contains(name)) return true;
        return Enclosing?.IsDeclared(name) ?? false;
    }

    public Object Get(Token name)
    {
        if (values.TryGetValue(name.Lexeme, out object value))
        {
            return value;
        }

        if (Enclosing != null) return Enclosing.Get(name);

        throw new RuntimeError(name, $"Undeclared variable'{name.Lexeme}'");
    }
    public void Define(string name, Object value) 
    {
        values[name] = value;
    }
}