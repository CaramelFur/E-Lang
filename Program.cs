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

    LLVMValueRef printAsm = LLVM.ConstInlineAsm(LLVM.Int32Type(), ConvToSbyte("bswap $0"), ConvToSbyte("=r,r"), 0, 0);
    //LLVM.asm
    Console.WriteLine("Building");
    LLVM.DumpValue(printAsm);


    Console.WriteLine("Building");

    /*LLVMBool falseb = new LLVMBool(0);

    LLVMModuleRef module = LLVM.ModuleCreateWithName("Main");


    LLVMTypeRef[] param_types = { };
    LLVMTypeRef ret_type = LLVM.FunctionType(LLVM.DoubleType(), param_types, false);

    LLVMValueRef sum = LLVM.AddFunction(module, "main", ret_type);
    LLVMBasicBlockRef entry = LLVM.AppendBasicBlock(sum, "entry");
    LLVMBuilderRef builder = LLVM.CreateBuilder();
    LLVM.PositionBuilderAtEnd(builder, entry);

    LLVMValueRef solved = parsed.Solve(builder);

    LLVMValueRef toPrint = LLVM.BuildIntCast(builder, solved, LLVM.Int64Type(), "tempprint");
    LLVMValueRef printOp = LLVM.BuildAdd(builder, LLVM.ConstInt(LLVM.Int64Type(), 1, falseb), LLVM.ConstInt(LLVM.Int64Type(), 0, falseb), "printop");



    LLVMValueRef[] argss = { printOp, toPrint };
    //LLVMValueRef called = LLVM.BuildCall(builder, printAsm, argss, "thingy");

    LLVM.BuildRet(builder, solved);



    if (LLVM.VerifyModule(module, LLVMVerifierFailureAction.LLVMPrintMessageAction, out var error) != falseb)
    {
      Console.WriteLine($"Error: {error}");
    }


    LLVM.DumpModule(module);
    LLVM.WriteBitcodeToFile(module, "./bitcode.bc");*/

  }

  public static sbyte* ConvToSbyte(string str)
  {
    byte[] bytes = Encoding.ASCII.GetBytes(str);
    fixed (byte* p = bytes)
    {
      return (sbyte*)p;
    }
  }

}

