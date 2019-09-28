using System;
using System.Linq;
using System.Collections.Generic;

namespace E_Lang.src
{
  public class EScope
  {
    public Dictionary<string, EVariable> variables = new Dictionary<string, EVariable>();
    public Dictionary<string, EFunction> functions = new Dictionary<string, EFunction>();

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
      variables[name] = variable;
    }

    public EVariable Get(string name)
    {
      if (!variables.ContainsKey(name))
      {
        if (depth == 0) throw new Exception();
        return parent.Get(name);
      }
      return variables[name];
    }

    public void SetFunction(string name, EFunction program)
    {
      functions[name] = program;
    }

    public EFunction GetFunction(string name)
    {
      if (!functions.ContainsKey(name))
      {
        if (depth == 0) throw new Exception("Could not find function " + name);
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

  public class EVariable
  {
    public static EVariable New(EType type)
    {
      switch (type.ToString())
      {
        case "int":
          return new EInt();
        case "boolean":
          return new EBoolean();
        case "string":
          return new EString();
        default:
          return new EVariable();
      }
    }

    public virtual EVariable Assign(ESolvable solvable)
    {

      return this;
    }

    public override string ToString()
    {
      return "Void";
    }
  }

  public class EInt : EVariable
  {
    public int value = 0;

    public override EVariable Assign(ESolvable solvable)
    {
      value = solvable.SolveInt();
      return this;
    }

    public override string ToString()
    {
      return value.ToString();
    }
  }

  public class EBoolean : EVariable
  {
    public bool value = false;

    public override EVariable Assign(ESolvable solvable)
    {
      value = solvable.SolveBool();
      return this;
    }

    public override string ToString()
    {
      return value.ToString();
    }
  }

  public class EString : EVariable
  {
    public string value = "";

    public override EVariable Assign(ESolvable solvable)
    {
      value = solvable.SolveString();
      return this;
    }

    public override string ToString()
    {
      return value;
    }
  }
}