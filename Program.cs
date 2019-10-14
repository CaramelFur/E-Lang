using System;
using LLVMSharp;


public class Program
{
  private static void Maine(string[] args)
  {
    var parsed = Parser.ParseExpression("2 + 4 -1");
    Console.WriteLine(parsed);

    LLVMBool Success = new LLVMBool(0);

    LLVMModuleRef module = LLVM.ModuleCreateWithName("Main");

    LLVMTypeRef[] param_types = { };
    LLVMTypeRef ret_type = LLVM.FunctionType(LLVM.Int32Type(), param_types, false);
    LLVMValueRef sum = LLVM.AddFunction(module, "main", ret_type);

    LLVMBasicBlockRef entry = LLVM.AppendBasicBlock(sum, "entry");
    LLVMBuilderRef builder = LLVM.CreateBuilder();
    LLVM.PositionBuilderAtEnd(builder, entry);

    LLVMValueRef solved = parsed.Solve(builder);

    LLVM.BuildRet(builder, solved);

    if (LLVM.VerifyModule(module, LLVMVerifierFailureAction.LLVMPrintMessageAction, out var error) != Success)
    {
      Console.WriteLine($"Error: {error}");
    }

    LLVM.DumpModule(module);

  }
}

