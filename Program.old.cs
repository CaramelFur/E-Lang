using System;
using System.Runtime.InteropServices;
using LLVMSharp;


public class ProgramN
{
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate int Add(int a, int b);

  private static void MainN(string[] args)
  {
    LLVMBool Success = new LLVMBool(0);
    LLVMModuleRef mod = LLVM.ModuleCreateWithName("LLVMSharpIntro");

    LLVMTypeRef[] param_types = { LLVM.Int32Type(), LLVM.Int32Type() };
    LLVMTypeRef ret_type = LLVM.FunctionType(LLVM.Int32Type(), param_types, false);
    LLVMValueRef sum = LLVM.AddFunction(mod, "sum", ret_type);

    LLVMBasicBlockRef entry = LLVM.AppendBasicBlock(sum, "entry");

    LLVMBuilderRef builder = LLVM.CreateBuilder();
    LLVM.PositionBuilderAtEnd(builder, entry);
    LLVMValueRef tmp = LLVM.BuildAdd(builder, LLVM.GetParam(sum, 0), LLVM.GetParam(sum, 1), "tmp");
    LLVM.BuildRet(builder, tmp);

    if (LLVM.VerifyModule(mod, LLVMVerifierFailureAction.LLVMPrintMessageAction, out var error) != Success)
    {
      Console.WriteLine($"Error: {error}");
    }

    LLVM.LinkInMCJIT();

    LLVM.InitializeX86TargetMC();
    LLVM.InitializeX86Target();
    LLVM.InitializeX86TargetInfo();
    LLVM.InitializeX86AsmParser();
    LLVM.InitializeX86AsmPrinter();

    LLVMMCJITCompilerOptions options = new LLVMMCJITCompilerOptions { NoFramePointerElim = 1 };
    LLVM.InitializeMCJITCompilerOptions(options);
    if (LLVM.CreateMCJITCompilerForModule(out var engine, mod, options, out error) != Success)
    {
      Console.WriteLine($"Error: {error}");
    }

    var addMethod = (Add)Marshal.GetDelegateForFunctionPointer(LLVM.GetPointerToGlobal(engine, sum), typeof(Add));
    int result = addMethod(10, 10);

    Console.WriteLine("Result of sum is: " + result);

    if (LLVM.WriteBitcodeToFile(mod, "sum.bc") != 0)
    {
      Console.WriteLine("error writing bitcode to file, skipping");
    }


    LLVM.DumpModule(mod);

    LLVM.DisposeBuilder(builder);
    LLVM.DisposeExecutionEngine(engine);
  }
}

