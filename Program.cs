using System;
using System.Text;
//using System.Array;
using LLVMSharp;

using System.IO;

public class Program
{
  private static void Main(string[] args)
  {
    var parsed = Parser.ParseExpression("1");
    Console.WriteLine(parsed);

    //LLVM.ConstInlineAsm(LLVM.Int32Type(c), "bswap $0", "=r,r", !truu, !truu);


    Console.WriteLine("Building");

    LLVMBool falseb = new LLVMBool(0);

    LLVMModuleRef module = LLVM.ModuleCreateWithName("Main");

    LLVMTypeRef[] param_types = { };
    LLVMTypeRef ret_type = LLVM.FunctionType(LLVM.Int64Type(), param_types, false);

    LLVMValueRef sum = LLVM.AddFunction(module, "main", ret_type);
    LLVMBasicBlockRef entry = LLVM.AppendBasicBlock(sum, "entry");
    LLVMBuilderRef builder = LLVM.CreateBuilder();
    LLVM.PositionBuilderAtEnd(builder, entry);

    LLVMValueRef solved = LLVM.ConstInt(LLVM.Int64Type(), 69, false);//parsed.Solve(builder);
    LLVMValueRef printOp = LLVM.ConstInt(LLVM.Int64Type(), 1, false);

    LLVMValueRef holder = LLVM.BuildAlloca(builder, LLVM.Int64Type(), "holder");
    LLVM.SetAlignment(holder, 4);
    LLVMValueRef store = LLVM.BuildStore(builder, solved, holder);
    LLVM.SetAlignment(store, 4);

    LLVMTypeRef[] asmparams = { LLVM.Int64Type(), LLVM.Int64Type(), LLVM.TypeOf(holder), LLVM.Int64Type() };
    LLVMTypeRef asmfuncthing = LLVM.FunctionType(LLVM.Int64Type(), asmparams, false);
    LLVMValueRef asmthing = LLVM.ConstInlineAsm(asmfuncthing, "syscall", "=r,{rax},{rdi},{rsi},{rdx},~{dirflag},~{fpsr},~{flags}", true, false);

    LLVMValueRef[] argss = { printOp, printOp, holder, printOp };
    LLVMValueRef called = LLVM.BuildCall(builder, asmthing, argss, "thingy");

    LLVM.BuildRet(builder, called);



    if (LLVM.VerifyModule(module, LLVMVerifierFailureAction.LLVMPrintMessageAction, out var error) != falseb)
    {
      Console.WriteLine($"Error: {error}");
    }


    LLVM.DumpModule(module);
    
    LLVM.WriteBitcodeToFile(module, "./out/bitcode.bc");
    


  }

}

