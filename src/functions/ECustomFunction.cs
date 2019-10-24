using System;

using E_Lang.types;
using E_Lang.llvm;
using E_Lang.solvable;
using LLVMSharp;


namespace E_Lang.functions
{
  public class ECustomFunction : EFunction
  {
    private readonly ETypeNameKey[] arguments;
    private readonly ETypeWord type;
    private readonly EWord name;
    private readonly EProgram program;

    public ECustomFunction(EWord name, ETypeWord type, EProgram program, ETypeNameKey[] arguments)
    {
      this.name = name;
      this.type = type;
      this.arguments = arguments;
      this.program = program;
    }

    public ECustomFunction(EWord name, ETypeWord type, EProgram program) : this(name, type, program, new ETypeNameKey[] { })
    { }

    public override string ToString()
    {
      string argString = "";
      for (int i = 0; i < arguments.Length; i++)
      {
        if (i != 0) argString += ", ";
        argString += arguments[i].ToString();
      }
      return "EFunction{\ntype: " + type + "\narguments: (" + argString + ")\n" + program + "\n}";
    }

    public override LLVMValueRef Exec(LLVMHolder llvm, ESolvable[] args)
    {
      // TODO: output type casting
      if (args.Length != arguments.Length) throw new ELangException("Wrong amount of args for function " + name);

      /*EScope lowerScope = scope.GetChild();

      for (int i = 0; i < arguments.Length; i++)
      {
        string argname = arguments[i].GetVariable().ToString();
        ETypeWord argtype = arguments[i].GetEType();

        EVariable solved = EVariable.New(argtype).Assign(args[i].Solve(lowerScope));
        lowerScope.Set(argname, solved);
      }

      return Interpreter.Run(program, lowerScope);*/
      return default(LLVMValueRef);
    }
  }
}