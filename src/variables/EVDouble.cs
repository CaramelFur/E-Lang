using E_Lang.types;
using E_Lang.llvm;
using LLVMSharp;
using System;

namespace E_Lang.variables
{
  public class EVDouble : EVariable
  {
    public EVDouble(LLVMHolder holder) :
    base(holder, LLVM.DoubleType(), "double")
    { }

    protected override LLVMValueRef? ParseInternallyFromToThis(LLVMValueRef from, EType type)
    {
      LLVMValueRef convert;
      switch (type.Get())
      {
        case "char":
        case "boolean":
        case "int":
          string name = llvm.GetNewName();
          convert = LLVM.BuildUIToFP(llvm.GetBuilder(), from, GetTypeRef(), name);
          return convert;
      }

      return null;
    }
  }
}