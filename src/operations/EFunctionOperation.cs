using E_Lang.types;
using E_Lang.variables;
using E_Lang.llvm;
using E_Lang.functions;

namespace E_Lang.operations
{
  // This operation creates a new function to be executed later
  public class EFunctionOperation : EOperation
  {
    private readonly ETypeNameKey[] arguments = { };
    private readonly ETypeWord type;
    private readonly EWord name;
    private readonly EProgram program;

    public EFunctionOperation(EWord name, ETypeWord type, ETypeNameKey[] arguments, EOperation[] operations)
    {
      this.name = name;
      this.type = type;
      this.arguments = arguments;
      program = new EProgram(operations);
    }

    public ECustomFunction ToEFunction()
    {
      return new ECustomFunction(name, type, program, arguments);
    }

    public override EVariable Exec(LLVMHolder llvm)
    {
      return new EVVoid(llvm);
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
  }

}