using System.Linq;
using System;

namespace E_Lang.src
{
  public class EOperation
  {
    public override string ToString()
    {
      return "ENoOp";
    }

    public virtual EVariable Exec(EScope scope)
    {
      return new EVariable();
    }
  }

  public class ECreateOperation : EOperation
  {
    public EWord name;
    public EType type;

    public override string ToString()
    {
      return "ECreateOperation{" + type + ": '" + name + "'}";
    }

    public override EVariable Exec(EScope scope)
    {
      EVariable newVar = EVariable.New(type);
      scope.Set(name.ToString(), newVar);
      return newVar;
    }
  }

  public class EAssignOperation : EOperation
  {
    public EWord variable;

    public ESolvable content;

    public override string ToString()
    {
      return "EAssignOperation{" + variable + " = " + content.ToString() + "}";
    }

    public override EVariable Exec(EScope scope)
    {
      string[] vars = scope.GetVariables();
      if (vars.Contains(variable.ToString()))
      {
        EVariable toUpdate = scope.Get(variable.ToString());
        toUpdate.Assign(content);
        return toUpdate;
      }
      else
      {
        throw new Exception("Variable " + variable + " does not exist!");
      }
      
    }
  }

  public class ECheckOperation : EOperation
  {
    public ESolvable check;
    public EOperation[] operations = { };

    public override string ToString()
    {
      string oppString = operations.Aggregate("operations:", (prev, next) => prev + "\n" + next.ToString());
      return "ECheckOperation{\ncheck: " + check + ";\n" + oppString + "\n}";
    }

    public override EVariable Exec(EScope scope)
    {
      return new EVariable();
    }
  }

  public class EFunctionOperation : EOperation
  {
    public EFunctionArgument[] arguments = { };
    public EType type;
    public EWord name;
    public EOperation[] operations = { };

    public override string ToString()
    {
      string oppString = operations.Aggregate("operations:", (prev, next) => prev + "\n" + next.ToString());
      string argString = "";
      for (int i = 0; i < arguments.Length; i++)
      {
        if (i != 0) argString += ", ";
        argString += arguments[i].ToString();
      }
      return "EFunction{\nname: '" + name + "'\ntype: " + type + "\narguments: (" + argString + ")\n" + oppString + "\n}";
    }

    public override EVariable Exec(EScope scope)
    {
      scope.SetFunction(name.ToString(), new EFunction(this));
      return new EVariable();
    }
  }

  public class ECallOperation : EOperation
  {
    public EWord callFunc;
    public ESolvable[] arguments = { };

    public override string ToString()
    {
      string argString = "";
      for (int i = 0; i < arguments.Length; i++)
      {
        if (i != 0) argString += ", ";
        argString += arguments[i].ToString();
      }
      return "ECall{function: " + callFunc + ", arguments: (" + argString + ")}";
    }

    public override EVariable Exec(EScope scope)
    {
      string[] funcs = scope.GetFunctions();
      if (!funcs.Contains(callFunc.ToString())) throw new Exception("Function " + callFunc + " does not exist!");

      EFunction toRun = scope.GetFunction(callFunc.ToString());

      return toRun.Exec(scope, arguments);
    }
  }

  public class EAPrintOperation : EOperation
  {
    public EWord variable;

    public override string ToString()
    {
      return "EAPrint{variable: " + variable + "}";
    }

    public override EVariable Exec(EScope scope)
    {
      Console.WriteLine(scope.Get(variable.ToString()));
      return new EVariable();
    }
  }

}