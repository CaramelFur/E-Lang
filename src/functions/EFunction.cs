using E_Lang.llvm;
using E_Lang.solvable;
using LLVMSharp;

namespace E_Lang.functions
{
  public abstract class EFunction
  {
    public virtual LLVMValueRef Exec(LLVMHolder llvm, ESolvable[] args)
    {
      return default(LLVMValueRef);
    }
  }
}