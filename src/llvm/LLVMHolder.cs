using LLVMSharp;
using System;
using System.Collections.Generic;

namespace E_Lang.llvm
{
  public class LLVMHolder
  {
    private static readonly LLVMBool LLVMBoolFalse = new LLVMBool(0);
    private static readonly LLVMValueRef NullValue = new LLVMValueRef(IntPtr.Zero);

    private readonly LLVMModuleRef module;
    private readonly LLVMBuilderRef builder;
    private readonly LLVMExecutionEngineRef execEngine;
    private readonly LLVMPassManagerRef passManager;
    private readonly Stack<LLVMValueRef> valueStack = new Stack<LLVMValueRef>();

    public LLVMModuleRef Module { get { return module; } }
    public LLVMBuilderRef Builder { get { return builder; } }
    public LLVMExecutionEngineRef ExecutionEngine { get { return execEngine; } }
    public LLVMPassManagerRef PassManager { get { return passManager; } }
    public Stack<LLVMValueRef> ResultStack { get { return valueStack; } }

    public LLVMHolder(LLVMExecutionEngineRef engineRef, LLVMPassManagerRef passRef, LLVMModuleRef moduleRef, LLVMBuilderRef builderRef)
    {
      execEngine = engineRef;
      passManager = passRef;
      module = moduleRef;
      builder = builderRef;
    }


  }
}