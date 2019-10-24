using System;

using E_Lang.types;
using E_Lang.variables;
using E_Lang.llvm;
using E_Lang.solvable;
using E_Lang.compiler;

namespace E_Lang.operations
{
  // This operation solves a solvable and if it is true executes its code
  public class EIfOperation : EOperation
  {
    private readonly ESolvable check;
    private readonly EProgram program;
    private readonly EProgram elseProgram = null;

    public EIfOperation(ESolvable check, EOperation[] operations)
    {
      this.check = check;
      program = new EProgram(operations);
    }

    public EIfOperation(ESolvable check, EOperation[] operations, EOperation[] elseOperations) :
    this(check, operations)
    {
      elseProgram = new EProgram(elseOperations);
    }

    public override EVariable Exec(LLVMHolder llvm)
    {
      return new EVVoid(llvm);
    }

    public override string ToString()
    {
      return "EIfOperation{\ncheck: " + check + ";\n" + program + "\n}";
    }
  }

}