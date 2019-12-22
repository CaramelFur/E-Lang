using E_Lang.types;
using E_Lang.llvm;
using LLVMSharp;

namespace E_Lang.variables
{
  public class EVChar : EVariable
  {
    public EVChar(LLVMHolder holder) :
        base(holder, LLVM.Int8Type(), "char")
    { }

    protected override LLVMValueRef? ParseInternallyFromToThis(LLVMValueRef from, EType type)
    {
      LLVMValueRef convert;
      switch (type.Get())
      {
        case "double":
          convert = LLVM.BuildFPToUI(llvm.GetBuilder(), from, GetTypeRef(), llvm.GetNewName());
          return convert;
        case "int":
        case "boolean":
          convert = LLVM.BuildIntCast(llvm.GetBuilder(), from, GetTypeRef(), llvm.GetNewName());
          return convert;
      }

      return null;
    }

  }
}