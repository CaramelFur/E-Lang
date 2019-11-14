using E_Lang.types;
using E_Lang.llvm;
using LLVMSharp;

namespace E_Lang.variables
{
  public class EVChar : EVariable
  {
    private static LLVMTypeRef type = LLVM.Int16Type();
    private LLVMValueRef value;

    public EVChar(LLVMHolder holder) : base(holder) { }

    public override LLVMTypeRef GetTypeRef()
    {
      return type;
    }

    public override EVariable Assign(EVariable assign)
    {
      EVChar converted = (EVChar)assign.Convert(GetEType());
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
        case "int":
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
      if (setTo.GetType() == typeof(char))
      {
        char parsedValue = setTo;
        value = LLVM.ConstInt(type, parsedValue, false);
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
      throw new ELangException("EVChar did not receive a char");
    }
  }
}