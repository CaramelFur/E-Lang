using E_Lang.types;
using E_Lang.llvm;
using LLVMSharp;

namespace E_Lang.variables
{
  public class EVPointer : EVariable
  {
    protected EVariable pointTo = null;
    protected LLVMValueRef pointer;
    protected LLVMTypeRef typeRef;

    public EVPointer(LLVMHolder holder) : base(holder) { }

    public override LLVMTypeRef GetTypeRef()
    {
      if (pointTo == null) throw new ELangException("Tried to use undefined pointer");
      return typeRef;
    }

    public override EVariable Assign(EVariable assign)
    {
      if (!(assign is EVPointer))
      {
        throw new ELangException("You cant assign a pointer with a non-pointer");
      }
      EVPointer pntr = (EVPointer)assign;
      pointTo = pntr.pointTo;
      pointer = pntr.pointer;
      typeRef = pntr.typeRef;
      return this;
    }

    public override LLVMValueRef Get()
    {
      if (pointTo == null) throw new ELangException("Tried to use undefined pointer");
      if (pointer.IsUndef()) IsUndefined();
      return pointer;
    }

    public override EVariable Set(dynamic setTo)
    {
      if (!(setTo is EVariable)) throw new ELangException("Pointer received invalid variable");
      EVariable assign = setTo;

      pointTo = assign;
      typeRef = LLVM.PointerType(assign.GetTypeRef(), 0);

      pointer = LLVM.BuildAlloca(llvm.GetBuilder(), assign.GetTypeRef(), llvm.GetNewName());
      LLVM.BuildStore(llvm.GetBuilder(), assign.Get(), pointer);

      return this;
    }
  }
}