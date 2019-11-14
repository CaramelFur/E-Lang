using E_Lang.types;
using E_Lang.llvm;
using LLVMSharp;
using System;


namespace E_Lang.variables
{
  public class EVInt : EVariable
  {
    private static LLVMTypeRef type =  LLVM.Int32Type();
    private LLVMValueRef value;

    public EVInt(LLVMHolder holder) : base(holder) { }

    public override LLVMTypeRef GetTypeRef()
    {
      return type;
    }

    public override EVariable Assign(EVariable assign)
    {
      EVInt converted = (EVInt)assign.Convert(GetEType());
      value = converted.Get();
      return this;
    }

    protected override EVariable ConvertInternal(string to)
    {
      EVariable newvar = New(to, llvm);
      LLVMValueRef convert;
      switch (to)
      {
        case "double":
          convert = LLVM.BuildUIToFP(llvm.GetBuilder(), Get(), newvar.GetTypeRef(), llvm.GetNewName());
          return newvar.Set(convert);
        case "char":
        case "boolean":
          convert = LLVM.BuildIntCast(llvm.GetBuilder(), Get(), newvar.GetTypeRef(), llvm.GetNewName());
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
      if (setTo.GetType() == typeof(int))
      {
        int parsedValue = setTo;
        value = LLVM.ConstInt(type, (ulong)parsedValue, false);
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
      throw new ELangException("EVInt did not receive an int");
    }

  }
}