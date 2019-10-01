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
    public EWord name;
    public EOperation[] operations = { };

    public EFunction() { }

    public EFunction(EFunctionOperation operation)
    {
      arguments = operation.arguments;
      type = operation.type;
      operations = operation.operations;
      name = operation.name;
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
      if (args.Length != arguments.Length) throw new Exception("Wrong amount of args for function " + name);

      EScope lowerScope = scope.GetChild();

      for (int i = 0; i < arguments.Length; i++)
      {
        string argname = arguments[i].variable.ToString();
        EType argtype = arguments[i].type;

        EVariable solved = EVariable.New(argtype).Assign(args[i].Solve(scope));
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
