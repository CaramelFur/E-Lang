using E_Lang.types;
using E_Lang.llvm;
using LLVMSharp;

namespace E_Lang.variables
{
  public class EVChar : EVariable
  {
    private LLVMValueRef value;

    public EVChar(LLVMHolder holder) : base(holder) { }

    public override LLVMTypeRef GetTypeRef()
    {
      return LLVM.Int16Type();
    }

    public override EVariable Assign(EVariable assign)
    {
      EVChar converted = (EVChar)assign.Convert(GetEType());
      value = converted.Get();
      return this;
    }

    protected override EVariable ConvertInternal(ETypeWord to)
    {
      EVariable newvar = New(to, llvm);
      LLVMValueRef convert;
      switch (to.Get())
      {
        case EType.Double:
          convert = LLVM.BuildUIToFP(llvm.GetBuilder(), Get(), newvar.GetTypeRef(), llvm.GetNewName());
          return newvar.Set(convert);
        case EType.Int:
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
      if (setTo.GetType() == typeof(char))
      {
        char parsedValue = setTo;
        value = LLVM.ConstInt(GetTypeRef(), parsedValue, false);
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
      throw new ELangException("EVChar did not receive a char");
    }
  }
}