using System;

using E_Lang.types;
using E_Lang.llvm;
using E_Lang.solvable;
using E_Lang.variables;
using E_Lang.compiler;

using LLVMSharp;


namespace E_Lang.operations
{
  // This operation solves a solvable and if it is true executes its code
  public class EWhileOperation : EOperation
  {
    private readonly ESolvable check;
    private readonly EProgram program;

    public EWhileOperation(ESolvable check, EOperation[] operations)
    {
      this.check = check;
      program = new EProgram(operations);
    }

    public override string ToString()
    {
      return "EWhileOperation{\nwhile: " + check + ";\n" + program + "\n}";
    }

    public override EVariable Exec(LLVMHolder llvm)
    {
      LLVMBuilderRef mainBuilder = llvm.GetBuilder();
      // build 

      LLVMBasicBlockRef endBlock = llvm.CreateNewBlock();

      LLVMBasicBlockRef repeatBlock = llvm.CreateNewBlock();

      LLVMBasicBlockRef compareBlock = llvm.CreateNewBlock();
      LLVMValueRef solved = EVariable.GetRawValueFromVariable(check.Solve(llvm));
      LLVM.BuildCondBr(llvm.GetBuilder(), solved, repeatBlock, endBlock);
      llvm.MoveBackABlock();

      EVariable outVariable = Compiler.SmallCompile(program, llvm);
      LLVM.BuildBr(llvm.GetBuilder(), compareBlock);
      llvm.MoveBackABlock();

      LLVM.MoveBasicBlockAfter(repeatBlock, compareBlock);
      LLVM.MoveBasicBlockAfter(endBlock, repeatBlock);

      LLVM.BuildBr(mainBuilder, compareBlock);


      return outVariable;
    }
  }

}