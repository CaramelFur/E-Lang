using E_Lang.types;
using E_Lang.llvm;
using LLVMSharp;

namespace E_Lang.variables
{
  public class EVPointer : EVariable
  {
    private EVariable pointTo = null;
    private LLVMValueRef pointer;
    private LLVMTypeRef typeRef;

    public static EVPointer Create(LLVMHolder holder, EVariable dest){
      EVPointer ptr = new EVPointer(holder);
      return (EVPointer)ptr.Assign(dest);
    }

    private EVPointer(LLVMHolder holder)
    : base(holder)
    {
    }

    public override LLVMTypeRef GetTypeRef()
    {
      return typeRef;
    }

    public override EVariable Assign(EVariable assign)
    {
      pointTo = assign;
      typeRef = LLVM.PointerType(assign.GetTypeRef(), 0);

      pointer = LLVM.BuildAlloca(llvm.GetBuilder(), assign.GetTypeRef(), llvm.GetNewName());
      LLVM.BuildStore(llvm.GetBuilder(), assign.Get(), pointer);

      return this;
    }

    public override LLVMValueRef Get()
    {
      if (pointer.IsUndef()) IsUndefined();
      return pointer;
    }

    public override EVariable Set(dynamic setTo)
    {
      throw new ELangException("You cannot set a pointer");
    }
  }
}