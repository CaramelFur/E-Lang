using E_Lang.types;
using E_Lang.llvm;
using LLVMSharp;

namespace E_Lang.variables
{
  public class EVPointer : EVariable
  {
    protected EVariable pointTo = null;

    public EVPointer(LLVMHolder holder) : base(holder) { }

    public override LLVMTypeRef GetTypeRef()
    {
      if (pointTo == null) throw new ELangException("Tried to use undefined pointer");
      return LLVM.PointerType(pointTo.GetTypeRef(), 0);
    }

    public override EVariable Assign(EVariable assign)
    {
      if (!(assign is EVPointer))
      {
        throw new ELangException("You cant assign a pointer with a non-pointer");
      }
      EVPointer pntr = (EVPointer)assign;
      pointTo = pntr.pointTo;
      return this;
    }

    public override LLVMValueRef Get()
    {
      if (pointTo == null) throw new ELangException("Tried to use undefined pointer");
      LLVMValueRef pointer = LLVM.BuildAlloca(llvm.GetBuilder(), pointTo.GetTypeRef(), llvm.GetNewName());
      LLVM.BuildStore(llvm.GetBuilder(), pointTo.Get(), pointer);
      return pointer;
    }

    public override EVariable Set(dynamic setTo)
    {
      if (!(setTo is EVariable)) throw new ELangException("Pointer received invalid variable");
      EVariable assign = setTo;

      pointTo = assign;
      return this;
    }
  }
}