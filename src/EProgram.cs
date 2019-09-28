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

  public class EOperation
  {
    public override string ToString()
    {
      return "ENoOp";
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

  }

  public class EAssignOperation : EOperation
  {
    public EWord variable;

    public ESolvable content;

    public override string ToString()
    {
      return "EAssignOperation{" + variable + " = " + content.ToString() + "}";
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
  }
}
