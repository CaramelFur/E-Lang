using System;

using E_Lang.llvm;
using LLVMSharp;

namespace E_Lang.variables
{
  public class EVVoid : EVariable
  {
    public EVVoid(LLVMHolder holder) : base(holder, LLVM.VoidType(), "void", true) { }

    protected override EVariable Set(LLVMValueRef value){
      return this;
    }

    protected override LLVMValueRef Get() {
      return LLVM.ConstInt(LLVM.Int1Type(), (ulong) 0, false);
    }
  }
}