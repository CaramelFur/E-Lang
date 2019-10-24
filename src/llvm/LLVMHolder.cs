using LLVMSharp;
using System;
using System.IO;
using System.Collections.Generic;
using E_Lang.scope;

namespace E_Lang.llvm
{
  public class LLVMHolder
  {
    private readonly LLVMModuleRef module;
    private readonly LLVMPassManagerRef passManager;
    private readonly EScope scope = new EScope();
    private readonly Stack<LLVMBuilderRef> builderStack = new Stack<LLVMBuilderRef>();

    private ulong counter = 0;

    public static LLVMHolder Create(string name)
    {
      LLVMModuleRef module = LLVM.ModuleCreateWithName(name);
      LLVMPassManagerRef passManager = LLVMFuncs.GenPassManager(module);

      return new LLVMHolder(passManager, module);
    }

    public LLVMHolder(LLVMPassManagerRef passRef, LLVMModuleRef moduleRef)
    {
      passManager = passRef;
      module = moduleRef;
    }

    public LLVMValueRef CreateMainFunc()
    {
      LLVMValueRef main_func = LLVMFuncs.createMainFunction(module);
      LLVMBasicBlockRef entry = LLVM.AppendBasicBlock(main_func, "entry");
      LLVMBuilderRef builder = LLVM.CreateBuilder();
      LLVM.PositionBuilderAtEnd(builder, entry);

      builderStack.Push(builder);
      return main_func;
    }

    public LLVMBuilderRef GetBuilder()
    {
      return builderStack.Peek();
    }

    public EScope GetScope()
    {
      return scope;
    }

    public string GetNewName()
    {
      return "tmp" + counter++;
    }

    public void Close(LLVMValueRef value)
    {
      LLVMBuilderRef builder = builderStack.Pop();
      LLVM.BuildRet(builder, value);
    }

    public void Verify()
    {
      if (LLVM.VerifyModule(module, LLVMVerifierFailureAction.LLVMPrintMessageAction, out var error) != false)
      {
        Console.WriteLine($"Error: {error}");
      }
    }

    public void Destroy()
    {
      LLVM.DisposeModule(module);
      LLVM.DisposePassManager(passManager);
    }

    public void Dump(string path)
    {
      LLVM.WriteBitcodeToFile(module, path);
    }

    public void Print()
    {
      LLVM.DumpModule(module);
    }


  }
}