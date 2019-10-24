using E_Lang.types;
using E_Lang.llvm;
using LLVMSharp;
using System;


namespace E_Lang.variables
{
  public class EVInt : EVariable
  {
    private LLVMValueRef value;

    public EVInt(LLVMHolder holder) : base(holder) { }

    public override LLVMTypeRef GetTypeRef()
    {
      return LLVM.Int32Type();
    }

    public override EVariable Assign(EVariable assign)
    {
      EVInt converted = (EVInt)assign.Convert(GetEType());
      value = converted.Get();
      return this;
    }

    protected override EVariable ConvertInternal(ETypeWord to)
    {
      EVariable newvar = EVariable.New(to, llvm);
      LLVMValueRef convert;
      switch (to.Get())
      {
        case EType.Double:
          convert = LLVM.BuildUIToFP(llvm.GetBuilder(), Get(), newvar.GetTypeRef(), llvm.GetNewName());
          return newvar.Set(convert);
        case EType.Char:
        case EType.Boolean:
          convert = LLVM.BuildIntCast(llvm.GetBuilder(), Get(), newvar.GetTypeRef(), llvm.GetNewName());
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
      if (setTo.GetType() == typeof(int))
      {
        int parsedValue = setTo;
        value = LLVM.ConstInt(GetTypeRef(), (ulong)parsedValue, false);
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
      throw new ELangException("EVInt did not receive an int");
    }

  }
}