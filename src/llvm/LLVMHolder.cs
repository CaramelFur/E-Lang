using LLVMSharp;
using System;
using System.IO;
using System.Collections.Generic;
using E_Lang.scope;
using E_Lang.variables;
using E_Lang.types;

namespace E_Lang.llvm
{
  public class LLVMHolder
  {
    private readonly LLVMModuleRef module;
    private readonly LLVMPassManagerRef passManager;
    private readonly EScope scope = new EScope();

    private readonly Stack<LLVMValueRef> functionStack = new Stack<LLVMValueRef>();
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

      GetScope().Set("putchar",
        new EVFunction(this)
        .Set(
          new EVFunctionDefinition(
            LLVM.AddFunction(
              module,
              "putchar",
              LLVM.FunctionType(
                LLVM.Int32Type(),
                new LLVMTypeRef[] {
                  LLVM.Int32Type()
                },
                false
              )
            ),
            new EType[] {
              new EType("int")
            },
            new EType("int")
          )

        )
      );

      GetScope().Set("getchar",
        new EVFunction(this)
        .Set(
          new EVFunctionDefinition(
            LLVM.AddFunction(
              module,
              "getchar",
              LLVM.FunctionType(
                LLVM.Int32Type(),
                new LLVMTypeRef[] {},
                false
              )
            ),
            new EType[] {},
            new EType("int")
          )

        )
      );
    }

    public LLVMValueRef CreateMainFunc()
    {
      LLVMValueRef main_func = LLVMFuncs.createMainFunction(module);

      functionStack.Push(main_func);

      LLVMBasicBlockRef entry = LLVM.AppendBasicBlock(main_func, GetNewName());
      LLVMBuilderRef builder = LLVM.CreateBuilder();
      LLVM.PositionBuilderAtEnd(builder, entry);

      builderStack.Push(builder);
      return main_func;
    }

    public LLVMBasicBlockRef CreateNewBlock()
    {
      LLVMValueRef func = functionStack.Peek();
      LLVMBasicBlockRef newBlock = LLVM.AppendBasicBlock(func, GetNewName());
      LLVMBuilderRef builder = LLVM.CreateBuilder();
      LLVM.PositionBuilderAtEnd(builder, newBlock);

      builderStack.Push(builder);

      return newBlock;
    }

    public void MoveBackABlock(){
      builderStack.Pop();
    }


    // ========== NOT VERY IMPORTANT
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