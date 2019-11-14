using E_Lang.types;
using E_Lang.llvm;
using LLVMSharp;
using System;

namespace E_Lang.variables
{
  public class EVDouble : EVariable
  {
    private static LLVMTypeRef type = LLVM.DoubleType();
    private LLVMValueRef value;

    public EVDouble(LLVMHolder holder) : base(holder) { }

    public override LLVMTypeRef GetTypeRef()
    {
      return type;
    }

    public override EVariable Assign(EVariable assign)
    {
      EVDouble converted = (EVDouble)assign.Convert(GetEType());
      value = converted.Get();
      return this;
    }

    protected override EVariable ConvertInternal(string to)
    {
      EVariable newvar = New(to, llvm);
      LLVMValueRef convert;
      switch (to)
      {
        case "int":
        case "char":
        case "boolean":
          convert = LLVM.BuildFPToUI(llvm.GetBuilder(), Get(), newvar.GetTypeRef(), llvm.GetNewName());
          return newvar.Set(convert);
      }

      return null;
    }

    public override LLVMValueRef Get()
    {
      return value;
    }

    public override EVariable Set(dynamic setTo)
    {
      if (setTo.GetType() == typeof(decimal))
      {
        decimal parsedValue = setTo;
        value = LLVM.ConstReal(type, (double)parsedValue);
        return this;

      }
      else if (setTo.GetType() == typeof(LLVMValueRef))
      {
        LLVMValueRef parsedValue = setTo;
        if (LLVM.TypeOf(parsedValue).Equals(type))
        {
          value = parsedValue;
          return this;
        }
      }
      throw new ELangException("EVDouble did not receive a decimal");
    }
  }
}