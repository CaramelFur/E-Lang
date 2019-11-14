using E_Lang.types;
using E_Lang.scope;
using E_Lang.operations;
using E_Lang.variables;
using E_Lang.llvm;
using System;

using LLVMSharp;

namespace E_Lang.compiler
{
  public class Compiler
  {
    private readonly LLVMHolder llvm;

    public Compiler(LLVMHolder llvm)
    {
      this.llvm = llvm;
    }

    public LLVMHolder Compile(EProgram program)
    {
      return Compile(program, llvm);
    }

    // Run a program
    public static LLVMHolder Compile(EProgram program, LLVMHolder llvm)
    {
      llvm.CreateMainFunc();

      EVariable solved = SmallCompile(program, llvm);

      llvm.Close(solved.Get());
      return llvm;
    }

    public static EVariable SmallCompile(EProgram program, LLVMHolder llvm)
    {
      EVariable solved = new EVVoid(llvm);
      // Loop over every instruction and return the last value
      EOperation[] operations = program.GetOperations();
      foreach (EOperation operation in operations)
      {
        solved = operation.Exec(llvm);

      }
      return solved;
    }
  }
}