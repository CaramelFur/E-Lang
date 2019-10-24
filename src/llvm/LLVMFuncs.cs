using LLVMSharp;

namespace E_Lang.llvm
{
  class LLVMFuncs
  {

    public static LLVMValueRef createMainFunction(LLVMModuleRef module)
    {
      LLVMTypeRef[] param_types = { };
      LLVMTypeRef main_func_type = LLVM.FunctionType(LLVM.Int32Type(), param_types, false);

      LLVMValueRef main = LLVM.AddFunction(module, "main", main_func_type);
      return main;
    }

    public static LLVMPassManagerRef GenPassManager(LLVMModuleRef module)
    {
      LLVMPassManagerRef passManager = LLVM.CreateFunctionPassManagerForModule(module);
      // Provide basic AliasAnalysis support for GVN.
      LLVM.AddBasicAliasAnalysisPass(passManager);

      // Promote allocas to registers.
      LLVM.AddPromoteMemoryToRegisterPass(passManager);

      // Do simple "peephole" optimizations and bit-twiddling optzns.
      LLVM.AddInstructionCombiningPass(passManager);

      // Reassociate expressions.
      LLVM.AddReassociatePass(passManager);

      // Eliminate Common SubExpressions.
      LLVM.AddGVNPass(passManager);

      LLVM.AddVerifierPass(passManager);

      // Simplify the control flow graph (deleting unreachable blocks, etc).
      LLVM.AddCFGSimplificationPass(passManager);

      LLVM.InitializeFunctionPassManager(passManager);

      return passManager;
    }
  }


}