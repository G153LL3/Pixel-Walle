using System;
using System.Collections.Generic;

public class Environment
{
    private readonly Dictionary<string, Object> values = new Dictionary<string, Object>();

    public Object Get(Token name) 
    {
        if (values.ContainsKey(name.Lexeme))
        {
            return values[name.Lexeme];
        }
        throw new RuntimeError(name,
        "Undefined variable '" + name.Lexeme + "'.");
    }

    public void Define(string name, Object value) 
    {
        values[name] = value;
    }
}