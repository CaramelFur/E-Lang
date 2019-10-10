using System;
using System.Linq;
using System.Collections.Generic;

using E_Lang.variables;
using E_Lang.functions;


namespace E_Lang.scope
{

  // This a scope that holds variables and function, it can also create nested scopes
  public class EScope
  {
    private readonly Dictionary<string, EVariable> variables = new Dictionary<string, EVariable>();
    private readonly Dictionary<string, EFunction> functions = new Dictionary<string, EFunction>();

    private readonly EScope parent;
    private readonly int depth;

    public EScope()
    {
      depth = 0;
    }

    public EScope(EScope parent)
    {
      this.parent = parent;
      depth = this.parent.depth + 1;
    }

    public void Set(string name, EVariable variable)
    {
      try
      {
        variables.Add(name, variable);
      }
      catch 
      {
        throw new ELangException("The variable " + name + " has already been declared");
      }
    }

    public EVariable Get(string name)
    {
      if (!variables.ContainsKey(name))
      {
        if (depth == 0) throw new ELangException("Could not find the variable " + name);
        return parent.Get(name);
      }
      return variables[name];
    }

    public void SetFunction(string name, EFunction program)
    {
      try
      {
        functions.Add(name, program);
      }
      catch 
      {
        throw new ELangException("The function " + name + " has already been declared");
      }
    }

    public EFunction GetFunction(string name)
    {
      if (!functions.ContainsKey(name))
      {
        if (depth == 0) throw new ELangException("Could not find the function " + name);
        return parent.GetFunction(name);
      }
      return functions[name];
    }

    public string[] GetVariables()
    {
      string[] buffer;
      if (depth == 0) buffer = new string[] { };
      else buffer = parent.GetVariables();

      foreach (KeyValuePair<string, EVariable> pair in variables)
      {
        buffer = buffer.Append(pair.Key).ToArray();
      }

      return buffer;
    }

    public string[] GetFunctions()
    {
      string[] buffer;
      if (depth == 0) buffer = new string[] { };
      else buffer = parent.GetFunctions();

      foreach (KeyValuePair<string, EFunction> pair in functions)
      {
        buffer = buffer.Append(pair.Key).ToArray();
      }

      return buffer;
    }

    public EScope GetChild()
    {
      return new EScope(this);
    }
  }

}