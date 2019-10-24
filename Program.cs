using System;
using System.Text;
//using System.Array;
using LLVMSharp;
using System.IO;

public unsafe class Program
{
  private static void Main(string[] args)
  {
    //var parsed = Parser.ParseExpression("2 + 4 -1");
    //Console.WriteLine(parsed);

    Console.WriteLine("hello");

    LLVMBool falseb = new LLVMBool(0);

    LLVMModuleRef module = LLVM.ModuleCreateWithName("Main");

    LLVMTypeRef charpointer = LLVM.PointerType(LLVM.Int8Type(), 0);
    LLVMTypeRef[] putsparams = { charpointer };
    LLVMTypeRef putstype = LLVM.FunctionType(LLVM.Int64Type(), putsparams, false);

    LLVMValueRef putsfunc = LLVM.AddFunction(module, "printf", putstype);

    //LLVM.AddAttributeAtIndex(putsfunc, 0, LLVMSharp.LLVM.attribute)


    LLVMTypeRef[] param_types = { };
    LLVMTypeRef ret_type = LLVM.FunctionType(LLVM.Int8Type(), param_types, false);

    LLVMValueRef sum = LLVM.AddFunction(module, "main", ret_type);
    LLVMBasicBlockRef entry = LLVM.AppendBasicBlock(sum, "entry");
    LLVMBuilderRef builder = LLVM.CreateBuilder();
    LLVM.PositionBuilderAtEnd(builder, entry);


    LLVMValueRef ep = LLVM.ConstInt(LLVM.Int8Type(), 69, false);
    LLVMValueRef tp = LLVM.ConstInt(LLVM.Int8Type(), 0, false);

    LLVMValueRef arr = LLVM.ConstArray(LLVM.Int8Type(), new LLVMValueRef[] { ep, tp });
    LLVMValueRef pointer = LLVM.BuildArrayAlloca(builder, LLVM.Int8Type(), arr, "tjiehf");
    
    //LLVMValueRef pointer = LLVM.BuildAlloca(builder, LLVM.ArrayType(LLVM.Int8Type(), 2), "charp");
    LLVM.BuildStore(builder, arr, pointer);

    LLVM.BuildCall(builder, putsfunc, new LLVMValueRef[] { pointer }, "result");

    LLVMValueRef ret = LLVM.ConstInt(LLVM.Int8Type(), 0, false);
    LLVM.BuildRet(builder, ret);

    if (LLVM.VerifyModule(module, LLVMVerifierFailureAction.LLVMPrintMessageAction, out var error) != falseb)
    {
      Console.WriteLine($"Error: {error}");
    }


    LLVM.DumpModule(module);
    LLVM.WriteBitcodeToFile(module, "./bitcode.bc");

  }

}

