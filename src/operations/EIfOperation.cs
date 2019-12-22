using System;

using LLVMSharp;

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

      LLVMValueRef solved = EVariable.GetRawValueFromVariable(check.Solve(llvm));
      LLVMBuilderRef mainBuilder = llvm.GetBuilder();
      // build 
      LLVMBasicBlockRef endBlock = llvm.CreateNewBlock();

      LLVMBasicBlockRef thenBlock = llvm.CreateNewBlock();
      EVariable outVariable = Compiler.SmallCompile(program, llvm);
      LLVM.BuildBr(llvm.GetBuilder(), endBlock);
      llvm.MoveBackABlock();

      LLVM.MoveBasicBlockAfter(endBlock, thenBlock);

      if (elseProgram != null)
      {
        LLVMBasicBlockRef elseBlock = llvm.CreateNewBlock();
        outVariable = Compiler.SmallCompile(elseProgram, llvm);
        LLVM.BuildBr(llvm.GetBuilder(), endBlock);
        llvm.MoveBackABlock();

        LLVM.MoveBasicBlockAfter(endBlock, elseBlock);

        LLVM.BuildCondBr(mainBuilder, solved, thenBlock, elseBlock);
      }
      else
      {
        LLVM.BuildCondBr(mainBuilder, solved, thenBlock, endBlock);
      }

      return outVariable;
    }

    public override string ToString()
    {
      if (elseProgram != null) return "EIfOperation{\ncheck: " + check + ";\n" + program + "\nelse" + elseProgram + "\n}";
      return "EIfOperation{\ncheck: " + check + ";\n" + program + "\n}";
    }
  }

}