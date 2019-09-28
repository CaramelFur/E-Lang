using System;
using System.Linq;

namespace E_Lang.src
{
  public class EProgram
  {
    public EOperation[] operations;

    public override string ToString()
    {
      return operations.Aggregate("Operations:", (prev, next) => prev + "\n" + next.ToString());
    }

  }

  public class EFunction
  {
    public EFunctionArgument[] arguments = { };
    public EType type;
    public EOperation[] operations = { };

    public EFunction() { }

    public EFunction(EFunctionOperation operation)
    {
      this.arguments = operation.arguments;
      this.type = operation.type;
      this.operations = operation.operations;
    }

    public override string ToString()
    {
      string oppString = operations.Aggregate("operations:", (prev, next) => prev + "\n" + next.ToString());
      string argString = "";
      for (int i = 0; i < arguments.Length; i++)
      {
        if (i != 0) argString += ", ";
        argString += arguments[i].ToString();
      }
      return "EFunction{\ntype: " + type + "\narguments: (" + argString + ")\n" + oppString + "\n}";
    }

    public EVariable Exec(EScope scope, ESolvable[] args)
    {
      // TODO: output type casting
      if (args.Length != arguments.Length) throw new Exception("Wrong amount of args");

      EScope lowerScope = scope.GetChild();

      for (int i = 0; i < operations.Length; i++)
      {
        string argname = arguments[i].variable.ToString();
        EType argtype = arguments[i].type;

        EVariable solved = EVariable.New(argtype).Assign(args[i]);
        scope.Set(argname, solved);
      }

      return Interpreter.Run(new EProgram { operations = operations }, lowerScope);
    }
  }

  public class EToken
  {
    public string token;

    public override string ToString()
    {
      return token;
    }
  }

  public class EWord
  {
    public string word;

    public override string ToString()
    {
      return word;
    }
  }

  public class EType
  {
    public string type;

    public override string ToString()
    {
      return type;
    }
  }

  public class ESolvable
  {
    public string contents;

    public override string ToString()
    {
      return contents;
    }

    public string SolveString()
    {
      return contents;
    }

    public int SolveInt()
    {
      try
      {
        int result = Int32.Parse(contents);
        return result;
      }
      catch (FormatException)
      {
        throw new Exception("Could not parse int: " + contents);
      }
    }

    public bool SolveBool()
    {
      if (SolveInt() != 0) return true;
      else return false;
    }
  }

  public class EFunctionArgument
  {
    public EType type;
    public EWord variable;

    public override string ToString()
    {
      return type + ": " + variable;
    }
  }
}
