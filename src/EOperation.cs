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
      EVariable toUpdate = scope.Get(variable.ToString());
      toUpdate.Assign(content.Solve(scope));
      return toUpdate;
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
    public EWord setVariable;
    public bool alsoSet = false;

    public override string ToString()
    {
      string argString = "";
      for (int i = 0; i < arguments.Length; i++)
      {
        if (i != 0) argString += ", ";
        argString += arguments[i].ToString();
      }
      if (!alsoSet) return "ECall{function: " + callFunc + ", arguments: (" + argString + ")}";
      else return "ECallAndAssign{function: " + callFunc + ", arguments: (" + argString + "), assign: " + setVariable + "}";
    }

    public override EVariable Exec(EScope scope)
    {
      string[] funcs = scope.GetFunctions();
      if (!funcs.Contains(callFunc.ToString())) throw new Exception("Function " + callFunc + " does not exist!");

      EFunction toRun = scope.GetFunction(callFunc.ToString());

      EVariable output = toRun.Exec(scope, arguments);

      if (alsoSet)
      {
        EVariable toUpdate = scope.Get(setVariable.ToString());
        toUpdate.Assign(output.value);
      }

      return output;
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