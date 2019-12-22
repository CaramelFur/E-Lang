using E_Lang.types;
using E_Lang.llvm;
using LLVMSharp;
using System;


namespace E_Lang.variables
{
  public class EVInt : EVariable
  {
    public EVInt(LLVMHolder holder) :
    base(holder, LLVM.Int32Type(), "int")
    { }

    protected override LLVMValueRef? ParseInternallyFromToThis(LLVMValueRef from, EType type)
    {
      LLVMValueRef convert;
      switch (type.Get())
      {
        case "double":
          convert = LLVM.BuildFPToUI(llvm.GetBuilder(), from, GetTypeRef(), llvm.GetNewName());
          return convert;
        case "char":
        case "boolean":
          convert = LLVM.BuildIntCast(llvm.GetBuilder(), from, GetTypeRef(), llvm.GetNewName());
          return convert;
      }

      return null;
    }

    public EVInt InsertRaw(int value)
    {
      LLVMValueRef newintval = LLVM.ConstInt(GetTypeRef(), (ulong)value, false);
      this.Set(newintval);
      return this;
    }

  }
}