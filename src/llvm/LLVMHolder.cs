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
    private readonly Stack<LLVMValueRef> valueStack = new Stack<LLVMValueRef>();

    public Stack<LLVMValueRef> ResultStack { get { return valueStack; } }

    public LLVMHolder(LLVMModuleRef moduleRef, LLVMBuilderRef builderRef)
    {
      module = moduleRef;
      builder = builderRef;
    }
  }
}