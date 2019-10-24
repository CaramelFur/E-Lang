using E_Lang.llvm;
using E_Lang.variables;

namespace E_Lang.operations
{
  // The base class of an operation
  public abstract class EOperation
  {
    public override string ToString()
    {
      return "";
    }

    public virtual EVariable Exec(LLVMHolder llvm) { 
      throw new ELangException("Cannot exec an abstract class");
    }
  }

}