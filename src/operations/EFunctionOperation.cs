using E_Lang.types;
using E_Lang.variables;
using E_Lang.scope;
using E_Lang.functions;

namespace E_Lang.operations
{
  // This operation creates a new function to be executed later
  public class EFunctionOperation : EOperation
  {
    private readonly EFunctionArgument[] arguments = { };
    private readonly EType type;
    private readonly EWord name;
    private readonly EProgram program;

    public EFunctionOperation(EWord name, EType type, EFunctionArgument[] arguments, EOperation[] operations)
    {
      this.name = name;
      this.type = type;
      this.arguments = arguments;
      program = new EProgram(operations);
    }

    public override string ToString()
    {
      string argString = "";
      for (int i = 0; i < arguments.Length; i++)
      {
        if (i != 0) argString += ", ";
        argString += arguments[i].ToString();
      }
      return "EFunction{\nname: '" + name + "'\ntype: " + type + "\narguments: (" + argString + ")\n" + program + "\n}";
    }

    public ECustomFunction ToEFunction()
    {
      return new ECustomFunction(name, type, program, arguments);
    }

    public override EVariable Exec(EScope scope)
    {
      scope.SetFunction(name.ToString(), ToEFunction());
      return new EVVoid();
    }
  }

}