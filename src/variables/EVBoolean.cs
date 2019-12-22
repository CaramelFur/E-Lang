using LLVMSharp;
using E_Lang.llvm;
using E_Lang.types;

namespace E_Lang.variables
{
  public class EVBoolean : EVariable
  {
    public EVBoolean(LLVMHolder holder) :
    base(holder, LLVM.Int1Type(), "boolean")
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
        case "int":
          convert = LLVM.BuildIntCast(llvm.GetBuilder(), from, GetTypeRef(), llvm.GetNewName());
          return convert;
      }

      return null;
    }

    public EVBoolean InsertRaw(bool value)
    {
      LLVMValueRef newintval = LLVM.ConstInt(GetTypeRef(), (ulong)(value ? 1 : 0), false);
      this.Set(newintval);
      return this;
    }
  }
}