using E_Lang.types;
using E_Lang.llvm;
using LLVMSharp;
using System;

namespace E_Lang.variables
{
  public class EVDouble : EVariable
  {
    private LLVMValueRef value;

    public EVDouble(LLVMHolder holder) : base(holder) { }

    public override LLVMTypeRef GetTypeRef()
    {
      return LLVM.DoubleType();
    }

    public override EVariable Assign(EVariable assign)
    {
      EVDouble converted = (EVDouble)assign.Convert(GetEType());
      value = converted.Get();
      return this;
    }

    protected override EVariable ConvertInternal(ETypeWord to)
    {
      EVariable newvar = New(to, llvm);
      LLVMValueRef convert;
      switch (to.Get())
      {
        case EType.Int:
        case EType.Char:
        case EType.Boolean:
          convert = LLVM.BuildFPToUI(llvm.GetBuilder(), Get(), newvar.GetTypeRef(), llvm.GetNewName());
          return newvar.Set(convert);
      }

      return null;
    }

    public override LLVMValueRef Get()
    {
      if (value.IsUndef()) IsUndefined();
      return value;
    }

    public override EVariable Set(dynamic setTo)
    {
      if (setTo.GetType() == typeof(decimal))
      {
        decimal parsedValue = setTo;
        value = LLVM.ConstReal(GetTypeRef(), (double)parsedValue);
        return this;

      }
      else if (setTo.GetType() == typeof(LLVMValueRef))
      {
        LLVMValueRef parsedValue = setTo;
        if (LLVM.TypeOf(parsedValue).Equals(GetTypeRef()))
        {
          value = parsedValue;
          return this;
        }
      }
      throw new ELangException("EVDouble did not receive a decimal");
    }
  }
}